using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    [Serializable]
    public class PlayerLayerData
    {
        [field:SerializeField] public LayerMask GroundLayer {  get;private set; }
        public bool ContainsLayer(LayerMask layerMask,int layer)//是否包含某一图层
        {
            return (1 << layer & layerMask) != 0;//1<<layer是为了将layer代表的数字转换为机器码才能和layerMask比较，例如layer表示第6层环境，则1向左移6位到第7位
            //即0000 0000 0000 0000 0000 0000 0100 0000与
            //  0000 0000 0000 0000 0000 0000 0100 0000相比较,若一样则不为0，代表找到了该层

        }
        public bool IsGroundLayer(int layer)//判断是否有地面层
        {
            return ContainsLayer(GroundLayer,layer);
        }
            
    }
}
