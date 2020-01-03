/****************************************************
    文件：Destroyer.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2020/1/3 0:57:16
	功能：销毁游戏对象
*****************************************************/

using UnityEngine;


public class Destroyer : MonoBehaviour
{

    [Tooltip("是否在Awake时执行销毁操作")]
    public bool DestroyOnAwake = false;
    [Tooltip("销毁延迟时间")]
    public float AwakeDestroyDelay = 0f;

    private void Awake()
    {
        if (DestroyOnAwake)
        {
            Destroy(this.gameObject,AwakeDestroyDelay);
        }
    }

    //销毁自身
    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}