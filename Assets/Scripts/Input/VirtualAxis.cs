/****************************************************
    文件：VirtualAxis.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2020/01/05 21:40
	功能：虚拟轴
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualAxis
{ 
    public string Name { get; private set; }

    private float m_Value;

    //构造函数
    public VirtualAxis(string name) {
        this.Name = name;
    }

    //更新当前的值
    public void Update(float value)
    {
        m_Value = value;
    }

    //返回当前的值
    public float GetValue() {
        return m_Value;
    }

    //返回初始 值
    public float GetValueRaw() {
        return m_Value;
    }

}