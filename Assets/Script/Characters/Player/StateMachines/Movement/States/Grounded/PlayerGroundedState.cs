using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMoveSystem
{
    public class PlayerGroundedState : PlayerMovementState//接地状态脚本
    {
        private SlopeData slopeData;
      
        public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            slopeData = stateMachine.Player.ColliderUtility.SlopeData;//防止之后每次输入一大行了
        }
        #region Istate Methods
        public override void Enter()
        {
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.GroundedParameterHash);
            UpdateShouldSprintState();
            UpdateCameraRecenteringState(stateMachine.ReusableData.MovementInput);
        }

        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.GroundedParameterHash);
        }

        public override void PhysicsUpdate()
        {
           
            base.PhysicsUpdate();
            Float();
        }


        #endregion

        #region Main Methods
        private void UpdateShouldSprintState()
        {
            if (!stateMachine.ReusableData.ShouldSprint)//是否在疾跑，如果在疾跑该方法不处理
            {
                return;
            }
            if (stateMachine.ReusableData.MovementInput != Vector2.zero) //如果有移动该方法也不处理
            {
                return;
            }
            stateMachine.ReusableData.ShouldSprint = false;//如果以上都不满足,不应该继续疾跑状态(即每次接地时，没有按下移动键，本身也不是疾跑状态,应该把疾跑关掉)
        }
        private void Float()
        {
            //该方法用于解决胶囊碰撞器高度修改后角色下陷或上陷问题，方法是从胶囊碰撞器中心点发射一道射线，知道碰撞器中心点离地的高度，和中心点相较于身体最低点相比，来判断应该给身体施加一个正力还是负力
            Vector3 capsuleColliderInWorldSpace = stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;//发射射线的原点
            Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderInWorldSpace, Vector3.down);//从碰撞器原点向下的射线
            if(Physics.Raycast(downwardsRayFromCapsuleCenter,out RaycastHit hit,slopeData.FloatRayDistance,stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))//中间第二个变量是了解距离的变量,加out只是为了限制其只能作为返回值
            {
                //第五个参数会忽略具有环境层但属于触发器碰撞器的对象（因为我们不想将触发器作为我们行走的地面）
                float groundAngle = Vector3.Angle(hit.normal,-downwardsRayFromCapsuleCenter.direction);
                float slopeSpeedModifier = SetSlopeSpeedModfierOnAngle(groundAngle);
                if(slopeSpeedModifier == 0f)//这样可以使我们不会漂浮在角度太高的地面上
                {
                    return;
                }
                
                float distanceToFloatingPoint = stateMachine.Player.ColliderUtility.CapsuleColliderData.ColliderCenterInLocalSpace.y * stateMachine.Player.transform.localScale.y - hit.distance;//剩余距离(中心点距离地面的距离-中心点距离身体最低点的距离)
                if (distanceToFloatingPoint == 0f)
                {
                    return;
                }
                float amountToLift = distanceToFloatingPoint * slopeData.StepReachForce - GetPlayerVerticalVelocity().y;
                //现在需要将浮动力转换为Vector3的力
                Vector3 liftForce = new Vector3(0f,amountToLift,0f);//只在竖直方向上加力
                stateMachine.Player.Rigidbody.AddForce(liftForce,ForceMode.VelocityChange);//这个添加的是竖直力
            }
        }

        private float SetSlopeSpeedModfierOnAngle(float angle)
        {
            float slopeSpeedModifier = movementData.SlopeSpeedAngles.Evaluate(angle);//这里是获取曲线修改器的值
            if (stateMachine.ReusableData.MovementOnSlopesSpeedModifier != slopeSpeedModifier)
            {
                stateMachine.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;
                UpdateCameraRecenteringState(stateMachine.ReusableData.MovementInput);
            }
            return slopeSpeedModifier;
        }
        private bool IsThereGroundUnderneath()
        {
            BoxCollider groundCheckCollider = stateMachine.Player.ColliderUtility.TriggerColliderData.GroundCheckCollider;
            Vector3 groundColliderCenterInWordsSpace = groundCheckCollider.bounds.center;//地面碰撞器的中心点
            Collider[] overlappedGroundColliders = Physics.OverlapBox(groundColliderCenterInWordsSpace,stateMachine.Player.ColliderUtility.TriggerColliderData.GroundCheckColliderExtents,groundCheckCollider.transform.rotation,stateMachine.Player.LayerData.GroundLayer,QueryTriggerInteraction.Ignore);
            return overlappedGroundColliders.Length > 0;//意味着在玩家下方找到了地面
        }
        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()//添加回调
        {
            base.AddInputActionsCallbacks();
            stateMachine.Player.Input.PlayerActions.Dash.started += OnDashStarted;
            stateMachine.Player.Input.PlayerActions.Jump.started += OnJumpStarted;
        }

        
        protected override void RemoveInputActionsCallbacks()//移除回调
        {
            base.RemoveInputActionsCallbacks();
            stateMachine.Player.Input.PlayerActions.Dash.started -= OnDashStarted;
            stateMachine.Player.Input.PlayerActions.Jump.started -= OnJumpStarted;
        }
        protected virtual void OnMove()
        {
            if (stateMachine.ReusableData.ShouldSprint)
            {
                stateMachine.ChangeState(stateMachine.SprintingState);
                return;
            }
            if ( stateMachine.ReusableData.ShouldWalk)
            {
                stateMachine.ChangeState(stateMachine.WalkingState);
                return;
            }
            stateMachine.ChangeState(stateMachine.RunningState);//如果不是步行，则应转换为跑步状态
        }
        protected override void OnContactWithGroundExited(Collider collider)
        {
            base.OnContactWithGroundExited(collider);
            if (IsThereGroundUnderneath())
            {
                return;//不应该坠落
            }
            Vector3 capsuleColliderCenterInWorldSpace = stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;//获取胶囊体的中心点
            Ray downwardsFormCapsuleBottom = new Ray(capsuleColliderCenterInWorldSpace - stateMachine.Player.ColliderUtility.CapsuleColliderData.ColliderVerticalExtends,Vector3.down);//获得一个从胶囊体底部向下的射线
            if(!Physics.Raycast(downwardsFormCapsuleBottom,out _, movementData.GroundToFallRayDistance,stateMachine.Player.LayerData.GroundLayer,QueryTriggerInteraction.Ignore))
            {
                OnFall();//离开地面后转换为坠落状态(看离地距离,距离太低不算坠落状态)
            }
            
        }

       

        protected virtual void OnFall()
        {
            stateMachine.ChangeState(stateMachine.FallingState);
        }
        #endregion
        #region Input Methods

        protected virtual void OnDashStarted(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.DashingState);
        }
        protected virtual void OnJumpStarted(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.JumpingState);
        }

        #endregion
    }
}
