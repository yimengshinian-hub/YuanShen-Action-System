using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    [Serializable]
    public class SlopeData 
    {
        [field: SerializeField][field: Range(0f, 1f)] public float StepHeightPercentage { get; private set; } = 0.25f;//步长高度
        [field: SerializeField][field: Range(0f, 5f)] public float FloatRayDistance { get; private set; } = 2f;//射线离地的最大距离 (光线距离)
        [field: SerializeField][field: Range(0f, 50f)] public float StepReachForce { get; private set; } = 25f;
    }
}
