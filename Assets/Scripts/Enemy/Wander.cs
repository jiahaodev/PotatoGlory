/****************************************************
	文件：Wander.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2020/01/02 0:11   	
	功能：怪物巡逻脚本
*****************************************************/

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Wander : MonoBehaviour
{
    [Tooltip("是否朝向右边")]
    [SerializeField]
    public bool FacingRight = true;

    [Tooltip("怪物水平移动的速度")]
    [SerializeField]
    private float MoveSpeed = 2f;

    //用于设置怪物对象的物理属性
    private Rigidbody2D m_Rigidbody;
    //用于保存当前的水平移动速度
    private float m_CurrentMoveSpeed;

    //获取组件引用
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    //设置字段的出初始值
    private void Start()
    {
        if (FacingRight)
        {
            m_CurrentMoveSpeed = MoveSpeed;
        }
        else
        {
            m_CurrentMoveSpeed = -MoveSpeed;
        }
    }

    //执行和物理相关的代码
    private void FixedUpdate()
    {
        m_Rigidbody.velocity = new Vector2(m_CurrentMoveSpeed, m_Rigidbody.velocity.y);
    }

    //在Wander.cs脚本被禁用时调用
    private void OnDisable()
    {
        //设置水平方向上的速度为0
        m_Rigidbody.velocity = new Vector2(0f,m_Rigidbody.velocity.y);
    }


    //转向函数
    public void Flip()
    {
        FacingRight = !FacingRight;

        m_CurrentMoveSpeed *= -1;

        this.transform.localScale = Vector3.Scale(new Vector3(-1, 1, 1), this.transform.localScale);
    }
}