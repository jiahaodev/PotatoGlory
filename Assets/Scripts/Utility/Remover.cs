/****************************************************
    文件：Remover.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2020/1/4 16:24:55
	功能：溅起水花  移除怪物
*****************************************************/

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Remover : MonoBehaviour 
{
    [Tooltip("浪花预设")]
    public GameObject SplashPrefab;

    private BoxCollider2D m_Trigger;

    private void Awake()
    {
        m_Trigger = GetComponent<BoxCollider2D>();
        m_Trigger.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 实例化水花对象，水花对象会自动播放声音和动画
        Instantiate(SplashPrefab,collision.transform.position, collision.transform.rotation);

        // 销毁掉下去的物体
        Destroy(collision.gameObject);
    }
}