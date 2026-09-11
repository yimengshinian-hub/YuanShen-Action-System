using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    public class PlayerFallingState : PlayerAirborneState
    {
        private PlayerFallData fallData;
        private Vector3 playerPositionOnEnter;
        public PlayerFallingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            fallData = airborneData.FallData;
        }
        #region IState Methods
        public override void Enter()
        {
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.FallParameterHash);
            playerPositionOnEnter = stateMachine.Player.transform.position;//保留刚进入坠落状态时的位置
            stateMachine.ReusableData.MovementSpeedModifier = 0f;//坠落中不允许移动
            ResetVerticalVelocity();
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.FallParameterHash);
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            LimitVerticalVelocity();
        }
        #endregion

        #region Resuable Methods
        protected override void ResetSprintState()
        {
        }
        protected override void OnContactWithGround(Collider collider)
        {
            float fallDistance = playerPositionOnEnter.y - stateMachine.Player.transform.position.y;//坠落的高度
            if(fallDistance < fallData.MinimumDistanceToBeConsideredHardFall)
            {
                stateMachine.ChangeState(stateMachine.LightLandingState);
                return;
            }
            if (stateMachine.ReusableData.ShouldWalk && !stateMachine.ReusableData.ShouldSprint || stateMachine.ReusableData.MovementInput == Vector2.zero)//如果高度够高且没有移动(打开了应该步行，且不能冲刺)
            {
                stateMachine.ChangeState(stateMachine.HardLandingState);
                return;
            }
            stateMachine.ChangeState(stateMachine.RollingState);
        }
        #endregion

        #region Main Methods
        private void LimitVerticalVelocity()
        {
            Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();
            if (playerVerticalVelocity.y >= -fallData.FallSpeedLimit)//未超过限制进入if
            {
                return;
            }
            Vector3 limitVelocity = new Vector3(0f,-fallData.FallSpeedLimit - playerVerticalVelocity.y,0f);
            stateMachine.Player.Rigidbody.AddForce(limitVelocity,ForceMode.VelocityChange);
        }
        #endregion
    }
}
