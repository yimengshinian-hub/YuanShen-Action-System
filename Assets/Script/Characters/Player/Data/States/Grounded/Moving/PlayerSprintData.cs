using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    [Serializable]
    public class PlayerSprintData
    {
        [field: SerializeField][field: Range(1f, 3f)] public float SpeedModifier { get; private set; } = 1.7f;
        [field: SerializeField][field: Range(1f, 5f)] public float SprintToRunTime { get; private set; } = 1f;//过渡需要的时间
        [field: SerializeField][field: Range(1f, 2f)] public float RunToWalkTime { get; private set; } = 0.5f;//过渡需要的时间
    }
}
