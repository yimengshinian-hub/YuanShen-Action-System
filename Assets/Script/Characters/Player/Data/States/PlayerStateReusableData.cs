using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    public class PlayerStateReusableData
    {
        //这里是可重用数据
        public Vector2 MovementInput {  get; set; }//用来接收新输入系统的返回值
        public float MovementSpeedModifier { get; set; } = 1f;//速度修改器
        public float MovementOnSlopesSpeedModifier { get; set; } = 1f;//坡速度修改器
        public float MovementDecelerationForce { get; set; } = 1f;//停止的平均速度
        public List<PlayerCameraRecenteringData> SidewaysCameraRecenteringData { get; set; }//侧（横）向数据
        public List<PlayerCameraRecenteringData> BackwardsCameraRecenteringData { get; set; }//后向数据
        public bool ShouldWalk {  get; set; }//是否应该步行
        public bool ShouldSprint { get; set; }//是否应该疾跑
        //以下是Mathf.SmoothDampAngle需要用到的变量,该方法用于实现平滑转动
        private Vector3 currentTargetRotation;//目标角度
        private Vector3 timeToReachTargetRotation;//到达目标角度需要的时间
        private Vector3 dampedTargetRotationCurrentVelocity;//到达目标的速度
        private Vector3 dampedTargetRotationPassedTime;//经过的时间
        //结束
        public ref Vector3 CurrentTargetRotation
        {
            get
            {
                return ref currentTargetRotation;
            }

        }
        public ref Vector3 TimeToReachTargetRotation
        {
            get
            {
                return ref timeToReachTargetRotation;
            }

        }
        public ref Vector3 DampedTargetRotationCurrentVelocity
        {
            get
            {
                return ref dampedTargetRotationCurrentVelocity;
            }

        }
        public ref Vector3 DampedTargetRotationPassedTime
        {
            get
            {
                return ref dampedTargetRotationPassedTime;
            }

        }
        public Vector3 CurrentJumpForce {  get; set; }

        public PlayerRotationData RotationData { get;  set; }
    }
}
