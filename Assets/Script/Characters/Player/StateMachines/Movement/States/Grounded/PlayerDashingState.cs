using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMoveSystem
{
    public class PlayerDashingState : PlayerGroundedState
    {
        //冲刺状态
        private PlayerDashData dashData;
        private float startTime;
        private int consecutiveDashesUsed;//判断完成了多少次连续冲刺
        private bool shouldKeepRotating;
        public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            dashData = movementData.DashData;
        }
        #region IState Methods
        public override void Enter()
        {
            //两种情况，一种是从待机转到该状态，需要添加一个力，另一种是从移动状态转到该状态，只需要更改速度调节器（Speed Modifier）即可
            stateMachine.ReusableData.MovementSpeedModifier = dashData.SpeedModifier;//这是对于移动状态转换过来的
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.DashParameterHash);
            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;
            //下面的方法是相对于待机状态转换过来的
            stateMachine.ReusableData.RotationData = dashData.RotationData;
            Dash();
            //结束
            //冲刺时自动旋转
            shouldKeepRotating = stateMachine.ReusableData.MovementInput != Vector2.zero;//没有按下任何键时该值为false
            //结束
            //更新冲刺的状态，是否禁用冲刺按键，还是连续冲刺次数加一
            UpdateConsecutiveDashes();
            //结束
            startTime = Time.time;  
            
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.DashParameterHash);
            SetBaseRotationData();
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (!shouldKeepRotating)
            {
                return;
            }
            RotateTowardsTargetRotation();
        }
        public override void OnAnimationTransitionEvent()
        {
            if(stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.HardStoppingState);
                return;
            }
            stateMachine.ChangeState(stateMachine.SprintingState);
        }


        #endregion

        #region Main Methods
        private void Dash()
        {
            Vector3 dashDirection = stateMachine.Player.transform.forward;//旋转方向
            dashDirection.y = 0f;//竖直旋转设为0,只用水平旋转
            UpdateTargetRotation(dashDirection,false);//改变目标旋转方向（在冲刺时改变目标移动方向，以防突然松开按键也能完成转向）
            if (stateMachine.ReusableData.MovementInput != Vector2.zero)//如果有输入(即是移动状态)
            {
                UpdateTargetRotation(GetMovementInputDirection());
                dashDirection = GetTargetRotationDirection(stateMachine.ReusableData.CurrentTargetRotation.y);
            }
            //将在Rigidbody中修改Velocity来设置力,这是为了做到冲刺的同时移动
            stateMachine.Player.Rigidbody.velocity = dashDirection * GetMovementSpeed(false);//方向 * 速度（为速度设置方向）
        }
        private void UpdateConsecutiveDashes()
        {
            if (!IsConsecutive())
            {
                //在不是连续冲刺时重置次数
                consecutiveDashesUsed = 0;
            }
            //在是连续冲刺时使冲刺次数加1
            consecutiveDashesUsed++;
            //现在需要检测已经冲刺的次数是否等于极限冲刺次数
            if(consecutiveDashesUsed == dashData.ConsecutiveDashesLimitAmount)
            {
                //重置冲刺次数
                consecutiveDashesUsed = 0;
                stateMachine.Player.Input.disableActionFor(stateMachine.Player.Input.PlayerActions.Dash,dashData.DashLimitReachedCooldown);
            }
        }

        private bool IsConsecutive()
        {
            //该方法用于判断是否是连续冲刺（若当前时间小于上一个冲刺结束时间，即又冲刺了（连续冲刺））
            return Time.time < startTime+dashData.TimeToBeConsideredConsecutive;//当前游戏时间和上一个冲刺结束时间相比较
        }
        #endregion

        #region Resuable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();
            stateMachine.Player.Input.PlayerActions.Movement.performed += OnMovementPerformed;
        }

      

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();
            //注，performed是相当于一个函数指针，可以储存函数，当start或者该键按下时调用里面存储的函数
            stateMachine.Player.Input.PlayerActions.Movement.performed -= OnMovementPerformed;
        }
        #endregion

        #region Input Methods
        protected override void OnDashStarted(InputAction.CallbackContext context)
        {
        }
        private void OnMovementPerformed(InputAction.CallbackContext context)
        {
            shouldKeepRotating = true;
        }
        #endregion
    }
}
