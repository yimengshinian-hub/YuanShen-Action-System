using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] [Range(0f,10f)] private float defaultDistance = 6f;
        [SerializeField] [Range(0f,10f)] private float minimumDistance = 1f;
        [SerializeField] [Range(0f,10f)] private float maximumDistance = 6f;
        [SerializeField] [Range(0f,10f)] private float smoothing = 4f;
        [SerializeField] [Range(0f,10f)] private float zoomSensitivity = 1f;
        private CinemachineFramingTransposer framingTransposer;
        private CinemachineInputProvider inputProvider;
        private float currentTargetDistance;
        private void Awake()
        {
            framingTransposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
            inputProvider = GetComponent<CinemachineInputProvider>();
            currentTargetDistance = defaultDistance;
        }
        private void Update()
        {
            Zoom();
        }

        private void Zoom()//控制相机缩放（距离)
        {
            float zoomValue = inputProvider.GetAxisValue(2) * zoomSensitivity;//这里的2是指的是cinemachineInputProvider中的z
            currentTargetDistance =Mathf.Clamp(currentTargetDistance + zoomValue,minimumDistance,maximumDistance);//确保这个距离不超过设置的最大最小值
            float currentDistace = framingTransposer.m_CameraDistance;//获取虚拟相机中当前的距离参数
            if(currentDistace == currentTargetDistance)//相机距离到达我们要设置的距离，就返回
            {
                return;
            }
            float lerpedZoomValue =Mathf.Lerp(currentDistace,currentTargetDistance,smoothing*Time.deltaTime);//以一个平滑的速度靠近目标距离
            framingTransposer.m_CameraDistance = lerpedZoomValue;  //改变相机距离
        }
    }
}
