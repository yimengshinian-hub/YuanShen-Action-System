using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMoveSystem
{
    public class PlayerJumpingState : PlayerAirborneState
    {
        private PlayerJumpData jumpData;
        private bool shouldKeepRotating;
        private bool canStartFalling;
        public PlayerJumpingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            jumpData = airborneData.JumpData;
        }
        #region IState Methods
        public override void Enter()
        {
            base.Enter();
            //stateMachine.ReusableData.MovementOnSlopesSpeedModifier = 0f;//使跳跃过程中不能移动(注释了之后就能朝着前进方向跳越了，不然只能原地蹦)
            stateMachine.ReusableData.RotationData = jumpData.RotationData;
            stateMachine.ReusableData.MovementDecelerationForce = jumpData.DecelerationForce;
            shouldKeepRotating = stateMachine.ReusableData.MovementInput != Vector2.zero;//有输入会继续旋转
            Jump();
        }
        public override void Exit()
        {
            base.Exit();
            SetBaseRotationData();
            canStartFalling = false;
        }
        public override void Update()
        {
            base.Update();
            if(!canStartFalling && IsMovingUp(0f))
            {
                canStartFalling = true;
            }
            if(!canStartFalling || GetPlayerVerticalVelocity().y > 0)//大于0代表没有在下落，还在跳跃过程中
            {
                return;
            }
            stateMachine.ChangeState(stateMachine.FallingState);
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (shouldKeepRotating)
            {
                RotateTowardsTargetRotation();
            }
            if (IsMovingUp())//为使玩家跳跃不虚浮，选择在其上升过程中添加力
            {
                DecelerateVertically();//向竖直方向添加一个向下的力
            }
        }

        #endregion

        #region Reusable Methods
        protected override void ResetSprintState()//除了跳跃状态之外的每个空降状态都会重置shouldSprint属性
        {
            
        }
        #endregion

        #region Main Methods
        private void Jump()
        {
            Vector3 jumpForce = stateMachine.ReusableData.CurrentJumpForce;//跳跃力需要根据倾斜角度和跳跃方向进行更改
            Vector3 jumpDirection = stateMachine.Player.transform.forward;//获取当前玩家面朝向(即是要跳跃方向)
            if (shouldKeepRotating)//如果按下了移动键
            {
                UpdateTargetRotation(GetMovementInputDirection());//会将旋转调整为相对于相机和输入
                jumpDirection = GetTargetRotationDirection(stateMachine.ReusableData.CurrentTargetRotation.y);
            }
            jumpForce.x *= jumpDirection.x;
            jumpForce.z *= jumpDirection.z;
            Vector3 capsuleColliderCenterInWordldSpace = stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;
            Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWordldSpace,Vector3.down);//创建一个从中心点向下的射线
            if(Physics.Raycast(downwardsRayFromCapsuleCenter,out RaycastHit hit,jumpData.JumpToGroundRayDistance,stateMachine.Player.LayerData.GroundLayer,QueryTriggerInteraction.Ignore))
            {
                float groundAngle = Vector3.Angle(hit.normal,-downwardsRayFromCapsuleCenter.direction);
                if (IsMovingUp())
                {
                    float forceModifier = jumpData.JumpForceModifierOnSlopeUpwards.Evaluate(groundAngle);
                    jumpForce.x *= forceModifier;
                    jumpForce.z *= forceModifier;
                }
                if (IsMovingDown())
                {
                    float forceModifier = jumpData.JumpForceModifierOnSlopeDownwards.Evaluate(groundAngle);
                    jumpForce.y *= forceModifier;
                }
            }
            ResetVelocity();//重置速度，防止原本的速度影响到跳跃
            stateMachine.Player.Rigidbody.AddForce(jumpForce,ForceMode.VelocityChange);
        }
        #endregion

        #region Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            
        }
        #endregion
    }
}
