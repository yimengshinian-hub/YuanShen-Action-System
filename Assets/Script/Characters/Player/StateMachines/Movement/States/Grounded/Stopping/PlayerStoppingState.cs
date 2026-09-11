using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMoveSystem
{
    public class PlayerStoppingState : PlayerGroundedState
    {
        public PlayerStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {

        }
        #region IState Methods
        public override void Enter()
        {
            stateMachine.ReusableData.MovementSpeedModifier = 0f;
            SetBaseCameraRecenteringData();
            base.Enter();
           
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            //以下是实现自动转向(每当我们进入停止状态，即使没有移动按下方向键也可以向该方向旋转
            RotateTowardsTargetRotation();
            //结束
            if (!IsMovingHorizontally())
            {
                //如果没有在移动：
                return;
            }
            DecelerateHorizontally();//在移动减速
        }
        public override void OnAnimationTransitionEvent()
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
        }

        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();
            stateMachine.Player.Input.PlayerActions.Movement.started += OnmovementStarted;
        }

       
        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();
            stateMachine.Player.Input.PlayerActions.Movement.started -= OnmovementStarted;
        }
        #endregion

        #region Input Methods
        protected void OnmovementStarted(InputAction.CallbackContext context)
        {
            OnMove();
        }

        #endregion
    }
}
