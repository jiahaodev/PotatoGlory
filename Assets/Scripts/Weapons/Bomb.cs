/****************************************************
    文件：Bomb.cs
    作者：JiahaoWu
    邮箱: jiahaodev@163.ccom
    日期：2020/01/03 11:35       
    功能：炸弹脚本
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Bomb : MonoBehaviour
{
    [Tooltip("炸弹产生的伤害值")]
    public float DamageAmount = 50f;
    [Tooltip("爆炸半径")]
    public float BombRadius = 10f;
    [Tooltip("爆炸时产生的冲击力")]
    public float BombForce = 800f;
    [Tooltip("炸弹爆炸时的音效")]
    public AudioClip BoomClip;
    [Tooltip("引信燃烧的时间")]
    public float FuseTime = 1.5f;
    [Tooltip("燃烧引信的音效")]
    public AudioClip FuseClip;
    [Tooltip("炸弹爆炸时的特效")]
    public GameObject BombExplosion;

    private LayerMask m_LayerMask;
    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.playOnAwake = false;
    }

    private void Start()
    {
        //设置LyaerMask检测Enemy和Player这两个Layer
        m_LayerMask = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Player");

        if (transform.root == transform)
        {
            //如果不是附着在其他物体上，就开始执行燃烧引信的协程
            StartCoroutine(BombDetonation());
        }

    }

    //燃烧引信的协程
    private IEnumerator BombDetonation()
    {
        //设置燃烧引信的音效并播放
        if (FuseClip != null)
        {
            m_AudioSource.clip = FuseClip;
            m_AudioSource.Play();
        }
        else
        {
            Debug.LogWarning("请设置FuseClip");
        }

        //等待FuseTime时间
        yield return new WaitForSeconds(FuseTime);

        //等待FuseTime时间之后，执行爆炸函数
        Explode();
    }


    //爆炸函数
    private void Explode()
    {
        //获取一定范围内的所有Layer为Enemy或者Player物体
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, BombRadius, m_LayerMask);

        //造成伤害
        foreach (Collider2D obj in objects)
        {
            if (obj.tag == "Enemy")
            {
                obj.GetComponent<Enemy>().TakeDamage(this.transform, BombForce, DamageAmount);
                continue;
            }
            if (obj.tag == "Player")
            {
                obj.GetComponent<PlayerHealth>().TakeDamage(this.transform, BombForce, DamageAmount);
                continue;
            }
        }

        //实例化爆炸特效
        if (BombExplosion != null)
        {
            Instantiate(BombExplosion, this.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("请设置BombExplosion");
        }

        //播放爆炸音效
        if (BoomClip != null)
        {
            AudioSource.PlayClipAtPoint(BoomClip, transform.position);
        }
        else
        {
            Debug.LogWarning("请设置BoomClip");
        }

        //销毁
        Destroy(gameObject);
    }
}
