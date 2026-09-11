using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    public class CapsuleColliderData 
    {
       //该脚本会存储我们的胶囊碰撞器的参考（用于计算的副本），还会存储它的本地空间中心
        public CapsuleCollider Collider {  get; private set; }
        public Vector3 ColliderCenterInLocalSpace { get; private set; }
        public Vector3 ColliderVerticalExtends {  get; private set; }//获得当前胶囊体的一半高度，主要用于和中心点一起得到胶囊体的底部
        public void Initialize(GameObject gameObject)//这是因为我们不会通过Inspector设置我们的引用，而是通过我们传入的游戏对象
        {
            if (Collider != null)
            {
                return;//这意味着已经初始化了
            }
            Collider = gameObject.GetComponent<CapsuleCollider>();//获取对象身上的胶囊碰撞器
            UpdataColliderData();
        }

        public void UpdataColliderData()
        {
            ColliderCenterInLocalSpace = Collider.center;
            ColliderVerticalExtends = new Vector3(0f, Collider.bounds.extents.y, 0f);//中间会返回胶囊体大小的一半
        }
    }
}
