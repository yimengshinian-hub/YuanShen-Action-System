using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    [Serializable]
    public class PlayerRollData 
    {
        [field: SerializeField][field: Range(0f, 3f)] public float SpeedModifier { get; private set; } = 1f;
        [field: SerializeField][field: Range(0.1f, 3f)] public float RollDuration { get; private set; } = 1.2f;//防止状态卡住出不去
    }
}
