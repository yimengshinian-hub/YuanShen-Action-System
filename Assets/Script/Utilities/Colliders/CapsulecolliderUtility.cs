using System;
using UnityEngine;

namespace YuanshenMoveSystem
{
    [Serializable]
    public class CapsulecolliderUtility
    {
        //在此脚本中，每次在Inspector中更新从0 -> 1的滑块时，我们基本上就会重新计算我们的ColliderHeight
        //0是0%，1是100%相对于原来碰撞器的高度
        //下面为每一种数据类型都创建一个对象（实例化）
        public CapsuleColliderData CapsuleColliderData { get; private set; }
        [field:SerializeField] public DefaultColliderData DefaultColliderData { get; private set; }
        [field: SerializeField] public SlopeData SlopeData { get; private set; }
        //结束
      
        public void Initialize(GameObject gameObject)//初始化我们的胶囊碰撞器数据
        {
            if (CapsuleColliderData != null)//已经初始化完成
            {
                return;
            }
            CapsuleColliderData = new CapsuleColliderData();//这里是没有序列化需要这样做，如果序列化了直接拖就行(这里是没有序列化
            CapsuleColliderData.Initialize(gameObject);
            OnInitialize();
        }
        protected virtual void OnInitialize()
        {

        }

        //下面是计算我们的ColliderDimensions所需的方法
        public void CalculateCapsuleColliderDimensions()//每次更新Inspector值时，我们都会调用此方法(需要重新计算高度，半径和中心)
        {
            SetCapsuleColliderRadius(DefaultColliderData.Radius);
            SetCapsuleColliderHeight(DefaultColliderData.Height * (1f - SlopeData.StepHeightPercentage));//高度乘以步高百分比（1-是因为这里的步长百分比是我们要删除的)
            RecalculateCapsuleColliderCenter();
            float halfColliderHeight = CapsuleColliderData.Collider.height / 2f;
            if (halfColliderHeight < CapsuleColliderData.Collider.radius)//如果高度小于半径的2倍，即中间区域已经被压缩没了，这时候如果不做处理碰撞器会变成一个球，并不会上升，应该将半径逐渐减小
            {
                SetCapsuleColliderRadius(halfColliderHeight);//将半径变为高度的一半
            }
            CapsuleColliderData.UpdataColliderData();//将其更新到胶囊碰撞器上
        }

      

        public void SetCapsuleColliderRadius(float radius)
        {
            CapsuleColliderData.Collider.radius = radius;
        }
        public void SetCapsuleColliderHeight(float hegiht)
        {
            CapsuleColliderData.Collider.height = hegiht;
        }
        public void RecalculateCapsuleColliderCenter()
        {
            float colliderHeightDifference = DefaultColliderData.Height - CapsuleColliderData.Collider.height;//默认高度减去当前高度
            Vector3 newColliderCenter = new Vector3(0f, DefaultColliderData.CenterY + (colliderHeightDifference / 2f), 0f);
            CapsuleColliderData.Collider.center = newColliderCenter;    
        }
    }
}
