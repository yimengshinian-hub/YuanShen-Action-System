using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

namespace YuanshenMoveSystem
{
    public class PlayerAirborneState : PlayerMovementState
    {
        public PlayerAirborneState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }
        #region IState Methods
        public override void Enter()
        {
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.AirborneParameterHash);
            ResetSprintState();
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.AirborneParameterHash);
        }
        #endregion

        #region Resuable Methods
        protected override void AddInputActionsCallbacks()//Ìí¼Ó»Øµ÷
        {
            base.AddInputActionsCallbacks();
            stateMachine.Player.Input.PlayerActions.Jump.started += OnJumpStarted;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            stateMachine.Player.Input.PlayerActions.Jump.started -= OnJumpStarted;
            base.RemoveInputActionsCallbacks();
        }
        protected override void OnContactWithGround(Collider collider)
        {
            stateMachine.ChangeState(stateMachine.LightLandingState);
        }
        protected virtual void ResetSprintState()
        {
            stateMachine.ReusableData.ShouldSprint = false;
        }
        #endregion
        protected virtual void OnJumpStarted(InputAction.CallbackContext context)
        {
            if (stateMachine.ReusableData.CoyoteTimer > 0)
            {
                stateMachine.ReusableData.CoyoteTimer = 0;
                stateMachine.ChangeState(stateMachine.JumpingState);
            }
        }
        #region Input Methods

        #endregion
    }
}
