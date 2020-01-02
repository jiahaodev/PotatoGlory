/****************************************************
    文件：PlayerAttack.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2020/1/3 1:11:19
	功能：玩家攻击脚本
*****************************************************/

using System;
using UnityEngine;


[RequireComponent(typeof(PlayerController))]
public class PlayerAttack : MonoBehaviour
{
    [Tooltip("导弹Prefab")]
    public Missile MissilePrefab;
    [Tooltip("导弹发射点")]
    public Transform ShootingPoint;
    [Tooltip("发射导弹的音效")]
    public AudioClip ShootEffect;

    private Animator m_Animator;
    private PlayerController m_PlayerCtrl;

    private void Awake()
    {
        // m_Animator = GetComponent<Animator>();
        m_PlayerCtrl = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Fire();
        }
    }

    //发射导弹
    private void Fire()
    {
        //播放射击动画
        // m_Animator.SetTrigger("Shoot");
        //播放射击音效
        AudioSource.PlayClipAtPoint(ShootEffect, ShootingPoint.position);

        if (ShootingPoint != null)
        {
            // 创建导弹
            Missile instance = Instantiate(MissilePrefab, ShootingPoint.position, Quaternion.identity) as Missile;

            // 如果角色跟导弹的朝向不一致，就翻转导弹
            if (m_PlayerCtrl.FacingRight ^ instance.FacingRight)
            {
                instance.Flip();
            }
        }
        else
        {
            Debug.LogError("请设置ShootingPoint");
            return;
        }

    }
}