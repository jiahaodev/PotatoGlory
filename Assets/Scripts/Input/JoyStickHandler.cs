/****************************************************
    文件：JoyStickHandler.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2020/01/05 22:40
	功能：操纵杆控制
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoyStickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    public enum AxisOption
    {
        //使用哪个轴
        Both,      //使用两个轴
        Horizontal,//使用水平轴
        Vertical,  //使用竖直轴
    }

    [Tooltip("虚拟摇杆的最大活动范围")]
    public float Range = 100;
    [Tooltip("是否根据屏幕的尺寸对虚拟摇杆的最大获得范围进行缩放")]
    public bool ScaleRange = true;
    [Tooltip("使用哪个轴")]
    public AxisOption AxisToUse = AxisOption.Both;
    [Tooltip("水平轴的名称")]
    public string HorizontalAxisName = "Horizontal";
    [Tooltip("数值轴的名称")]
    public string VerticalAxisName = "Vertical";

    Vector3 m_StartPos;
    bool m_UseHorizontalAxis;
    bool m_UseVerticalAxis;
    VirtualAxis m_HorizontalVirtualAxis;
    VirtualAxis m_VerticalVirtualAxis;


    private void Awake()
    {
        //确保虚拟摇杆运动的位置一样
        if (ScaleRange)
        {
            CanvasScaler scaler = transform.root.GetComponent<CanvasScaler>();

            float scaleX = Screen.width / scaler.referenceResolution.x;
            float scaleY = Screen.height / scaler.referenceResolution.y;

            if (scaleX > scaleY)
            {
                Range *= scaleY;
            }
            else
            {
                Range *= scaleX;
            }
        }

        m_UseHorizontalAxis = (AxisToUse == AxisOption.Both || AxisToUse == AxisOption.Horizontal);
        m_UseVerticalAxis = (AxisToUse == AxisOption.Both || AxisToUse == AxisOption.Vertical);

        //启用时对轴进行初始化
        if (m_UseHorizontalAxis)
        {
            m_HorizontalVirtualAxis = new VirtualAxis(HorizontalAxisName);
        }

        if (m_UseVerticalAxis)
        {
            m_VerticalVirtualAxis = new VirtualAxis(VerticalAxisName);
        }
    }

    private void OnEnable()
    {
        //启用时对轴进行注册
        if (m_UseHorizontalAxis)
        {
            InputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
        }
        if (m_UseVerticalAxis)
        {
            InputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
        }
    }

    private void Start()
    {
        m_StartPos = transform.position;
    }

    private void OnDisable()
    {
        // 禁用时取消轴注册
        if (m_UseHorizontalAxis)
        {
            InputManager.UnRegisterVirtualAxis(m_HorizontalVirtualAxis);
        }

        if (m_UseVerticalAxis)
        {
            InputManager.UnRegisterVirtualAxis(m_VerticalVirtualAxis);
        }
    }

    //更新摇杆的位置和轴的值
    private void UpdateVirtualAxes(Vector3 delta)
    {
        transform.position = new Vector3(
            m_StartPos.x + delta.x,
            m_StartPos.y + delta.y,
            m_StartPos.z + delta.z
            );

        delta /= Range;

        // 这里需要除以Range而不是归一化
        delta /= Range;

        if (m_UseHorizontalAxis)
        {
            m_HorizontalVirtualAxis.Update(delta.x);
        }

        if (m_UseVerticalAxis)
        {
            m_VerticalVirtualAxis.Update(delta.y);
        }

    }

    #region 接口函数
    //拖拽虚拟摇杆
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPos = Vector3.zero;

        //更新水平轴的位置和值
        if (m_UseHorizontalAxis)
        {
            float delta = eventData.position.x - m_StartPos.x;
            newPos.x = delta;
        }

        //更新竖直轴的位置和值
        if (m_UseVerticalAxis)
        {
            float delta = eventData.position.y - m_StartPos.y;
            newPos.y = delta;
        }

        //确保运动范围为半径为Range的圆
        if (newPos.magnitude > Range)
        {
            newPos = newPos.normalized * Range;
        }

        UpdateVirtualAxes(newPos);
    }

    //点击虚拟摇杆
    public void OnPointerDown(PointerEventData eventData)
    {

    }

    //松开虚拟摇杆
    public void OnPointerUp(PointerEventData eventData)
    {
        UpdateVirtualAxes(Vector3.zero);
    }



    #endregion

}