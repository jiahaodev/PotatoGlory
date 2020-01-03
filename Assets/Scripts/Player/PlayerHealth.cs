/****************************************************
    文件：PlayerHealth.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2020/1/2 22:35:28
	功能：血量控制脚本
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour
{
    [Tooltip("角色的最大生命值")]
    public float MaxHP = 100f;
    [Tooltip("角色受伤后的免伤时间")]
    public float FreeDamagePeriod = 0.35f;
    [Tooltip("角色的受伤音效")]
    public AudioClip[] OuchClips;
    [Tooltip("血量条")]
    public SpriteRenderer HealthSprite;

    //角色当前血量
    private float m_CurrentHP;
    //上一次受到伤害的时间
    private float m_LastFreeDamageTime;
    //血量条初始长度
    private Vector3 m_InitHealthScale;

    private Rigidbody2D m_Rigidbody2D;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        m_CurrentHP = MaxHP;
        m_LastFreeDamageTime = 0f;
        m_InitHealthScale = HealthSprite.transform.localScale;
    }


    //受伤函数
    public void TakeDamege(Transform enemy,float hurtForce, float damage)
    {
        //判断此时时是否处于免伤状态
        if (Time.time <= m_LastFreeDamageTime + FreeDamagePeriod)
        {
            return;
        }

        m_LastFreeDamageTime = Time.time;

        //给角色加上后退的力，制造击退效果
        Vector3 hurtVector = transform.position - enemy.position + Vector3.up * 5f;
        m_Rigidbody2D.AddForce(hurtVector.normalized * hurtForce);

        //更新角色的生命值
        m_CurrentHP -= damage;

        //更新生命条
        UpdateHealthBar();

        //随机播放音频
        if (OuchClips != null && OuchClips.Length > 0)
        {
            int i = UnityEngine.Random.Range(0, OuchClips.Length);
            AudioSource.PlayClipAtPoint(OuchClips[i], transform.position);
        }
        else
        {
            Debug.LogWarning("请设置OuchClips");
        }

        //角色死亡
        if (m_CurrentHP <= 0f)
        {
            Death();
        }

    }

    //更新生命条
    private void UpdateHealthBar()
    {
        if (HealthSprite != null)
        {
            //更新血量条颜色
            HealthSprite.color = Color.Lerp(Color.green,Color.red,1-m_CurrentHP*0.01f);
            //更新血量条长度
            HealthSprite.transform.localScale = Vector3.Scale(m_InitHealthScale,new Vector3(m_CurrentHP*0.01f,1,1));
        }
        else
        {
            Debug.LogError("请设置HealthSprite");
        }
    }

    //死亡
    private void Death()
    {
        // 将碰撞体设置为Trigger，避免和其他物体产生碰撞效果
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (Collider2D c in cols)
        {
            c.isTrigger = true;
        }

        //禁用脚本(控制脚本，不再运动)
        GetComponent<PlayerController>().enabled = false;

        //播放死亡动画
        GetComponent<Animator>().SetTrigger("Death");
    }
}