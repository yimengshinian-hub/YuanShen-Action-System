using UnityEngine;
using System;
using System.Collections.Generic;

namespace YuanshenMoveSystem
{
    [Serializable]//这个标签为了使其序列化，让其可在Insp...面板中可见
    public class PlayerWalkData
    {
        [field: SerializeField][field: Range(0f, 1f)] public float SpeedModifier { get; private set; } = 0.225f;
        [field: SerializeField] public List<PlayerCameraRecenteringData> BackwardsCameraRecenteringData { get; private set; }

    }
}
