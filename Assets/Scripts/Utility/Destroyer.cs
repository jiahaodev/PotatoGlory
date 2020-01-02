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
    //销毁自身
    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}