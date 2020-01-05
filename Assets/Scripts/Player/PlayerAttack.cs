/****************************************************
    文件：PlayerAttack.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2020/1/3 1:11:19
	功能：玩家攻击脚本
*****************************************************/

using System;
using UnityEngine;

// [RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
public class PlayerAttack : MonoBehaviour
{
    [Tooltip("导弹Prefab")]
    public Missile MissilePrefab;
    [Tooltip("导弹发射点")]
    public Transform ShootingPoint;
    [Tooltip("发射导弹的音效")]
    public AudioClip ShootEffect;
    [Tooltip("炸弹Prefab")]
    public Rigidbody2D BombPrefab;
    [Tooltip("炸弹的初始数量")]
    public int InitBombNumber = 4;
    [Tooltip("使用火箭筒抛射炸弹的力")]
    public float ProjectileBombForce = 1000f;

    private Animator m_Animator;
    private PlayerController m_PlayerCtrl;

    private void Awake()
    {
        // m_Animator = GetComponent<Animator>();
        m_PlayerCtrl = GetComponent<PlayerController>();

        // 检查关键属性是否赋值
        if (MissilePrefab == null)
        {
            Debug.LogError("请设置MissilePrefab");
        }

        if (ShootingPoint == null)
        {
            Debug.LogError("请设置ShootingPoint");
        }

        if (BombPrefab == null)
        {
            Debug.LogError("请设置BombPrefab");
        }
    }


    private void Update()
    {
#if UNITY_STANDALONE       //PC端使用Input来获取输入
        if (Input.GetButtonDown("Fire1")) //左键
        {
            // 发射导弹
            Fire();
        }

        if (Input.GetButtonDown("Fire2"))//右键
        {
            // 放置炸弹
            LayBomb();
        }

        if (Input.GetButtonDown("Fire3"))//中键
        {
            // 抛射炸弹
            ProjectileBomb();
        }
#elif UNITY_IOS || UNITY_ANDROID    //移动端使用InputManager来获取输入
        if (InputManager.GetButtonDown("Fire1")) {
            // 发射导弹
            Fire();
        }

        if (InputManager.GetButtonDown("Fire2")) {
            // 放置炸弹
            LayBomb();
        }

        if (InputManager.GetButtonDown("Fire3")) {
            // 抛射炸弹
            ProjectileBomb();
        }        
#endif
    }

    //发射导弹
    private void Fire()
    {
        //播放射击动画
        // m_Animator.SetTrigger("Shoot");
        //播放射击音效
        AudioSource.PlayClipAtPoint(ShootEffect, ShootingPoint.position);

        // 创建导弹
        Missile instance = Instantiate(MissilePrefab, ShootingPoint.position, Quaternion.identity) as Missile;

        // 如果角色跟导弹的朝向不一致，就翻转导弹
        if (m_PlayerCtrl.FacingRight ^ instance.FacingRight)
        {
            instance.Flip();
        }
    }

    // 放置炸弹
    private void LayBomb()
    {
        // 判断当前是否至少有一颗炸弹可以释放
        if (GameStateManager.Instance.BombManagerInstance.ReleaseBomb(1) == false)
        {
            return;
        }

        // 放置炸弹
        Instantiate(BombPrefab, this.transform.position, Quaternion.identity);

    }

    //抛射炸弹
    private void ProjectileBomb()
    {
        // 判断当前是否至少有一颗炸弹可以释放
        if (GameStateManager.Instance.BombManagerInstance.ReleaseBomb(1) == false)
        {
            return;
        }

        //通过刚体进行抛射
        Rigidbody2D body = Instantiate(BombPrefab, ShootingPoint.position, Quaternion.identity) as Rigidbody2D;
        if (m_PlayerCtrl.FacingRight)
        {
            body.AddForce(Vector2.right * ProjectileBombForce);
        }
        else
        {
            body.AddForce(Vector2.left * ProjectileBombForce);
        }

    }

}