using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMoveSystem
{
    public class PlayerRunningState : PlayerMovingState
    {
        private PlayerSprintData sprintData;
        private float startTime;//过渡开始的时间
        public PlayerRunningState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            sprintData = movementData.SprintData;
        }

        #region Istate Methods
        public override void Enter()
        {
            stateMachine.ReusableData.MovementSpeedModifier = movementData.RunData.SpeedModifier;
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.RunParameterHash);
            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.MediumForce;
            startTime = Time.time;
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.RunParameterHash);
        }
        public override void Update()
        {
            base.Update();
            if (!stateMachine.ReusableData.ShouldWalk)//当ShouldWalk为false时我们应当处于Run状态
            {
                return;
            }
            //当ShouldWalk为true时我们应该处于Walk,但是确在Run,说明进入过Sprint状态
            if(Time.time<startTime + sprintData.RunToWalkTime)
            {
                return;
            }
            StopRunning();
        }
        #endregion

        #region Main Methods
        private void StopRunning()
        {
            if(stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.IdlingState);
                return;
            }
            stateMachine.ChangeState(stateMachine.WalkingState);
        }

        #endregion

        #region Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.MediumStoppingState);//把原来的那个转换到待机状态的换掉，改为转换为停止状态
            base.OnMovementCanceled(context);
        }
        protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            base.OnWalkToggleStarted(context);
            stateMachine.ChangeState(stateMachine.WalkingState);//跑步的过渡
        }
   
        #endregion

    }
}
