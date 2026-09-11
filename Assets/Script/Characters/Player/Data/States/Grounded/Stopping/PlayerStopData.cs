using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace YuanshenMoveSystem
{
    [Serializable]
    public class PlayerStopData 
    {
        //该脚本保留3个停止的力
        [field: SerializeField][field: Range(0f, 15f)] public float LightDecelerationForce { get; private set; } = 5f;
        [field: SerializeField][field: Range(0f, 15f)] public float MediumDecelerationForce { get; private set; } = 6.5f;
        [field: SerializeField][field: Range(0f, 15f)] public float HardDecelerationForce { get; private set; } = 5f;
    }
}
