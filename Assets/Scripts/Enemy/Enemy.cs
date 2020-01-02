/****************************************************
    文件：Enemy.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2020/1/2 0:32:36
	功能：怪物统一脚本
*****************************************************/

using UnityEngine;

[RequireComponent(typeof(Wander))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour 
{
    [Tooltip("障碍物检测点")]
    [SerializeField]
    public Transform FrontCheck;
    [Tooltip("怪物的血量")]
    public float MaxHP = 10f;
    [Tooltip("怪物受伤时用来展示的图片")]
    public Sprite DamagedSprite;
    [Tooltip("怪物死亡时用来展示的图片")]
    public Sprite DeadSprite;
    [Tooltip("怪物死亡时用来展示DeadSprite")]
    public SpriteRenderer BodySpriteRenderer;

    [Tooltip("怪物死亡时的音效")]
    public AudioClip[] DeathClips;

    private Wander m_Wander;
    private Rigidbody2D m_Rigidbody2D;

    private LayerMask m_LayerMask;
    private float m_CurrentHP;
    private bool m_Hurt;
    private bool m_Dead;

    private void Awake()
    {
        m_Wander = GetComponent<Wander>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        m_LayerMask = LayerMask.GetMask("Obstacle");
        m_CurrentHP = MaxHP;
        m_Hurt = false;
        m_Dead = false;
    }

    private void Update()
    {
        if (m_Dead)
        {
            return;
        }

        Collider2D[] frontHits = Physics2D.OverlapPointAll(FrontCheck.position, m_LayerMask);

        if (frontHits.Length > 0)
        {
            m_Wander.Flip();
        }
    }
}