using UnityEngine;
using System;
using System.Collections.Generic;

namespace YuanshenMoveSystem
{
    [Serializable]
    public class PlayerGroundedData 
    {
        //将在这个脚本中保留玩家的基本速度
        [field:SerializeField] [field:Range(0f,25f)] public float BaseSpeed { get; private set; } = 5f;//基本速度数据
        [field:SerializeField][field: Range(0f, 5f)] public float GroundToFallRayDistance { get; private set; } = 1f;//坠落需要距离地面的距离
        [field:SerializeField] public List<PlayerCameraRecenteringData> SidewaysCameraRecenteringData { get; private set; }
        [field:SerializeField] public List<PlayerCameraRecenteringData> BackwardsCameraRecenteringData { get; private set; }
        [field:SerializeField] public AnimationCurve SlopeSpeedAngles { get; private set; }//坡速度角
        [field:SerializeField] public PlayerRotationData BaseRotationData { get; private set; }//旋转数据
        [field: SerializeField] public PlayeridleData IdleData { get; private set; }//待机数据
        [field:SerializeField] public PlayerWalkData WalkData { get; private set; }//步行数据
        [field:SerializeField] public PlayerRunData RunData { get; private set; }//跑步数据
        [field:SerializeField] public PlayerDashData DashData { get; private set; }//冲刺数据
        [field: SerializeField] public PlayerSprintData SprintData { get; private set; }//疾跑数据
        [field:SerializeField] public  PlayerStopData StopData{ get; private set; }//停止数据
        [field:SerializeField] public PlayerRollData RollData { get; private set; }//滚动数据
    }
}
