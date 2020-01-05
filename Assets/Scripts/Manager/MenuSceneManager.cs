/****************************************************
    文件：MenuSceneManager.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2020/01/05 21:23
	功能：场景菜单管理
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneManager : MonoBehaviour 
{
    // 加载SinglePlayerGameScene场景开始游戏
    public void StartGame() {
        SceneManager.LoadScene("SinglePlayerGameScene");
    }

    //退出应用
    public void ExitGame() {
        Application.Quit();
    }
}