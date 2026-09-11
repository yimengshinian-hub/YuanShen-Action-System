using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMoveSystem
{
    public class PlayerHardLandingState : PlayerLandingState
    {
        public PlayerHardLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }
        #region IState Methods
        public override void Enter()
        {
            stateMachine.ReusableData.MovementSpeedModifier = 0f;//硬着陆不允许移动
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.HardLandParameterHash);
            stateMachine.Player.Input.PlayerActions.Movement.Disable();//禁用移动键
            ResetVelocity();//重置速度

        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.HardLandParameterHash);
            stateMachine.Player.Input.PlayerActions.Movement.Enable();//退出该状态时启用移动键
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
        public override void OnAnimationExitEvent()
        {
            stateMachine.Player.Input.PlayerActions.Movement.Enable();//在动画的某一帧启用移动键
        }
        public override void OnAnimationTransitionEvent()
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
        }
        #endregion

        #region Resuable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();
            stateMachine.Player.Input.PlayerActions.Movement.started += OnMovementStarted;//某个动作开始时候要做的事
        }

      

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();
            stateMachine.Player.Input.PlayerActions.Movement.started -= OnMovementStarted;
        }
        protected override void OnMove()
        {
            if (stateMachine.ReusableData.ShouldWalk)//硬着陆转换不了步行状态
            {
                return;
            }
            stateMachine.ChangeState(stateMachine.RunningState);//可以转换为跑步状态
        }
        #endregion

        #region Input Methods
        protected override void OnJumpStarted(InputAction.CallbackContext context)
        {
            //留空意味着不会过渡到跳跃状态
        }
        private void OnMovementStarted(InputAction.CallbackContext context)
        {
            OnMove();
        }
        #endregion
    }
}
