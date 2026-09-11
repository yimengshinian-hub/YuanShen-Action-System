using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMoveSystem
{
    public class PlayerSprintingState : PlayerMovingState
    {
        //疾跑状态(左shift按住到达一定时间可触发)
        private PlayerSprintData sprintData;
        private bool keepSprinting;//判断是否到达一定时间
        private float startTime;//何时进入中刺状态
        private bool shouldResetSprintState;//判断是否应该继续冲刺（比如跳越后）
        public PlayerSprintingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            sprintData = movementData.SprintData;
        }
        #region IState Methods
        public override void Enter()
        {
            stateMachine.ReusableData.MovementSpeedModifier = sprintData.SpeedModifier;
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.SprintParameterHash);
            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;
            shouldResetSprintState = true;
            startTime = Time.time;
        }
        public override void Update()
        {
            base.Update();
            if (keepSprinting)
            {
                return;
            }
            if (Time.time < startTime + sprintData.SprintToRunTime) //意味着还没有经过足够的时间过渡到跑步状态
            {
                return;
            }
            StopSprinting();
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.SprintParameterHash);
            if (shouldResetSprintState)
            {
                keepSprinting = false;
                stateMachine.ReusableData.ShouldSprint = false;
            }
            keepSprinting = false;
        }

        #endregion

        #region Main Methods
        private void StopSprinting()
        {
            if(stateMachine.ReusableData.MovementInput == Vector2.zero)//突然没有任何输入,转换到待机状态
            {
                stateMachine.ChangeState(stateMachine.IdlingState);
                return;
            }
            stateMachine.ChangeState(stateMachine.RunningState);
        }

        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();
            stateMachine.Player.Input.PlayerActions.Sprint.performed += OnSprintPerformed;
        }

        

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();
            stateMachine.Player.Input.PlayerActions.Sprint.performed -= OnSprintPerformed;
        }
        protected override void OnFall()
        {
            shouldResetSprintState = false;
            base.OnFall();
        }
        #endregion
        #region Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.HardStoppingState);//把原来的那个转换到待机状态的换掉，改为转换为停止状态
            base.OnMovementCanceled(context);
        }
        protected override void OnJumpStarted(InputAction.CallbackContext context)
        {
            shouldResetSprintState = false;
            base.OnJumpStarted(context);
        }
        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            keepSprinting = true;
            stateMachine.ReusableData.ShouldSprint = true;
        }
        
        #endregion
    }
}
