using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    public class PlayerLightLandingState : PlayerLandingState
    {
        public PlayerLightLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }
        #region IState Methods
        public override void Enter()
        {
            stateMachine.ReusableData.MovementSpeedModifier = 0f;//轻着陆状态不允许移动(这里不在LandingState设置是因为滚动是要移动的
            base.Enter();
            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StationaryForce;//在轻着陆时将跳跃力设置为固定力
            ResetVelocity();//着陆后重置速度

        }
        public override void Update()
        {
            base.Update();
            if (stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                return;
            }
            OnMove();
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (!IsMovingHorizontally())//如果不在移动不处理
            {
                return;
            }
            ResetVelocity();
        }
        public override void OnAnimationTransitionEvent()
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
        }
        #endregion
    }
}
