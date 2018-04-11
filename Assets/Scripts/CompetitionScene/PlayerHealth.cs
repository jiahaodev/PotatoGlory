﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

namespace AsanCai.Competition {
    public class PlayerHealth : PunBehaviour {

        [Tooltip("角色的生命值")]
        public int maxHP = 100;
        [Tooltip("角色受到伤害后的免伤时间")]
        public float invincibleTime = 2f;
        [Tooltip("角色受到伤害时受到的力，制造击退效果")]
        public float hurtForce = 100f;
        //[Tooltip("角色被炸弹炸到时受到的冲击力")]
        //public float bombForce = 100f;
        [Tooltip("受伤害时减少的血量")]
        public int damageAmount = 10;
        [Tooltip("受伤音效")]
        public AudioClip[] ouchClips;
        [Tooltip("玩家头上的血量条")]
        public SpriteRenderer healthBar;

        //玩家所属队伍
        [HideInInspector]
        public int player;
        //玩家当前的血量
        [HideInInspector]
        public int currentHP;
        //玩家是否存活
        [HideInInspector]
        public bool isAlive;
        //玩家对象是否无敌
        [HideInInspector]
        public bool invincible;   


        private PlayerController playCtrl;

        private Vector3 healthScale;
        private Animator anim;
        //用于检测当前是否处于免伤状态
        private float timer;
        //用于设置自定义属性
        private ExitGames.Client.Photon.Hashtable costomProperties;


        private void Awake() {
            playCtrl = GetComponent<PlayerController>();
            anim = GetComponent<Animator>();

            //初始化必需的值
            currentHP = maxHP;
            isAlive = true;
            invincible = false;
        }

        private void Start() {
            timer = 0;
            healthScale = healthBar.transform.localScale;

            if (!photonView.isMine) {
                return;
            }

            //使用RPC，更新其他客户端中该玩家对象当前血量
            photonView.RPC("UpdateHP", PhotonTargets.Others, currentHP, PhotonNetwork.player);	
            
            if(PhotonNetwork.player.CustomProperties["Player"].ToString() == "Player1") {
                player = 1;
            } else {
                player = 2;
            }
            //使用RPC，设置其他客户端中该玩家对象的队伍
            photonView.RPC("SetPlayer", PhotonTargets.Others, player);		
        }

        private void Update() {
            //不是本地玩家对象，结束函数执行
            if (!photonView.isMine)     
                return;

            if (invincible) {
                //当前是无敌状态，累加玩家对象的无敌时间
                timer += Time.deltaTime;
            } else {
                //当前不是无敌状态，重置计时器
                timer = 0;
            }

            //超过无敌时间，退出无敌状态
            if (timer > invincibleTime) {
                photonView.RPC("SetInvincible", PhotonTargets.All, false);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            //假如撞到怪物
            if (collision.gameObject.tag == "Enemy") {
                //调用受伤函数
                TakeDamage(damageAmount, collision.transform.position, false);
            }
        }

        #region 公用函数
        //受伤函数
        public void Hurt(int damage, Vector3 enemyPos, bool bombed = false) {
            photonView.RPC("TakeDamage", PhotonTargets.All, damage, enemyPos, bombed);

            UpdateHealthDisplay();
        }

        //更新玩家头上的血量条
        private  void UpdateHealthDisplay() {
            //把玩家头上的生命条的颜色逐渐变红
            healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - currentHP * 0.01f);
            //缩短玩家头上的生命条
            healthBar.transform.localScale = new Vector3(healthScale.x * currentHP * 0.01f, 1, 1);
        }

        //死亡函数
        private void Death() {

            if (photonView.isMine) {
                //播放死亡动画
                anim.SetTrigger("Die");
            }

            //角色死亡
            Collider2D[] cols = GetComponents<Collider2D>();
            foreach (Collider2D c in cols) {
                c.isTrigger = true;
            }

            //把sortingLayer改为UI，下落的时候可以看到
            SpriteRenderer[] spr = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer s in spr) {
                s.sortingLayerName = "UI";
            }

            //禁用脚本
            GetComponent<PlayerController>().enabled = false;
            GetComponent<PlayerShoot>().enabled = false;
        }
        #endregion

        #region RPC函数
        [PunRPC]
        //受伤函数
        private void TakeDamage(int damage, Vector3 hitPos, bool bombed = false) {
            //玩家死亡或者无敌，不执行扣血函数
            if (!isAlive || invincible)
                return;

            //让角色进入无敌状态
            photonView.RPC("SetInvincible", PhotonTargets.All, true);
            //角色不能跳跃
            playCtrl.jump = false;


            //如果不是被炸弹炸到，需要制造受伤效果
            if (!bombed) {
                //给角色加上后退的力，制造击退效果
                Vector3 hurtVector = transform.position - hitPos + Vector3.up * 5f;
                GetComponent<Rigidbody2D>().AddForce(hurtVector * hurtForce);
            } else {
                //给角色加上后退的力，制造击退效果
                Vector3 hurtVector = transform.position - hitPos;
                GetComponent<Rigidbody2D>().AddForce(hurtVector * hurtForce);
            }

            //随机播放音频
            int i = Random.Range(0, ouchClips.Length);
            AudioSource.PlayClipAtPoint(ouchClips[i], transform.position);

            //只有主客户端有权限更新血量值
            if (PhotonNetwork.isMasterClient) {
                //更新角色的生命值
                currentHP -= damage;
                //更新所有客户端，该玩家对象的生命值
                photonView.RPC("UpdateHP", PhotonTargets.All, currentHP, PhotonNetwork.player);
            }
        }


        [PunRPC]
        public void UpdateHP(int newHP, PhotonPlayer p) {
            currentHP = newHP;

            if (currentHP <= 0) {
                isAlive = false;

                Death();
            }
            //设置玩家当前的存活属性
            costomProperties = new ExitGames.Client.Photon.Hashtable { { "isAlive", isAlive } };
            p.SetCustomProperties(costomProperties);

            //更新玩家存活状态
            GameManager.gm.UpdateAliveState();
        }

        //RPC函数，设置玩家的无敌状态
        [PunRPC]
        void SetInvincible(bool isInvincible) {
            invincible = isInvincible;
        }

        //RPC函数，设置玩家队伍
        [PunRPC]
        void SetPlayer(int newpalyer) {
            player = newpalyer;
        }
        #endregion

        
    }
}
