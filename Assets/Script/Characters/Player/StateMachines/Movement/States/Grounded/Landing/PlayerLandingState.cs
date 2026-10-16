using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMoveSystem
{
    public class PlayerLandingState : PlayerGroundedState
    {
        public PlayerLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }
        #region IState Methods
        public override void Enter()
        {
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.LandingParameterHash);
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.LandingParameterHash);
        }
        protected override void OnRollStarted(InputAction.CallbackContext context)
        {
            // 留空，落地家族不能翻滚（轻落/重落/翻滚中都不能再触发）
        }
        #endregion
    }
}
