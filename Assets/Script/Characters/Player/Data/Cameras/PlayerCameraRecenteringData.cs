using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    [Serializable]
    public class PlayerCameraRecenteringData 
    {
        [field:SerializeField][field:Range(0f,360f)] public float MinimumAngle {  get;private set; }
        [field:SerializeField][field:Range(0f,360f)] public float MaximumAngle { get; private set; }//相机的角度范围
        [field:SerializeField][field: Range(-1f,20f)] public float WaitTime { get; private set; }//等待时间
        [field:SerializeField][field: Range(-1f,20f)] public float RecenteringTime { get; private set; }//重新居中的时间
        public bool IsWithinRange(float angle)//判断是否在范围内
        {
            return angle >= MinimumAngle && angle <= MaximumAngle;
        }
    }
}
