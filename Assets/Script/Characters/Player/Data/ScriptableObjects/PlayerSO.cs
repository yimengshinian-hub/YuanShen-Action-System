using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    [CreateAssetMenu(fileName ="Player",menuName ="Custom/Characters/Player")]//这里创建脚本可编辑对象
    public class PlayerSO : ScriptableObject
    {
        //该类用于保存游戏的大部分数据
       [field:SerializeField] public PlayerGroundedData GroundedData { get; private set; }
       [field: SerializeField] public PlayerAirborneData AirborneData { get; private set; }
    }
}
