using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    [Serializable]
    public class PlayerDashData 
    {
        [field: SerializeField][field: Range(1f, 3f)] public float SpeedModifier { get; private set; } = 2f;//速度调节器
        [field: SerializeField] public PlayerRotationData RotationData { get; private set; }
        [field: SerializeField][field: Range(0f, 2f)] public float TimeToBeConsideredConsecutive { get; private set; } = 1f;//冲刺需要的时间
        [field: SerializeField][field: Range(1, 10)] public int ConsecutiveDashesLimitAmount { get; private set; } = 2;//极限冲刺次数
        [field: SerializeField][field: Range(0f, 5f)] public float DashLimitReachedCooldown { get; private set; } = 1.75f;//禁用冲刺按键时间
        [field: SerializeField][field: Range(0.1f, 3f)] public float DashDuration { get; private set; } = 0.4f;//冲刺持续时间，超过这个时间强制退出（兜底动画事件丢失）
    }
}
