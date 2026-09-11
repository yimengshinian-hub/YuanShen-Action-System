using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace YuanshenMoveSystem
{
    public class PlayeridlingState : PlayerGroundedState
    {
        private PlayeridleData idleData;
        public PlayeridlingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            idleData = movementData.IdleData;
        }
        #region IState Methods
        public override void Enter()
        {
            stateMachine.ReusableData.MovementOnSlopesSpeedModifier = 0f;
            stateMachine.ReusableData.BackwardsCameraRecenteringData = idleData.BackwardsCameraRecenteringData;
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.IdleParameterHash);
            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StationaryForce;
            ResetVelocity();
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.IdleParameterHash);
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

        #endregion
    }
}
