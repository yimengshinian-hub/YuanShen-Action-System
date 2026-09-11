using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    [Serializable]
    public class PlayerFallData 
    {
        [field: SerializeField][field: Range(1f, 5f)] public float FallSpeedLimit { get; private set; } = 15f;//下降速度限制器
        [field: SerializeField][field: Range(0f, 100f)] public float MinimumDistanceToBeConsideredHardFall { get; private set; } = 3f;//最低的高的下降高度

    }
}
