using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMoveSystem
{
    public class PlayerRollingState : PlayerLandingState
    {
        private PlayerRollData rollData;
        private float startTime;
        public PlayerRollingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            rollData = movementData.RollData;
        }
        #region IState Methods
        public override void Enter()
        {
            stateMachine.ReusableData.MovementSpeedModifier = rollData.SpeedModifier;
            base.Enter();
            StartAnimation(stateMachine.Player.AnimationData.RollParameterHash);
            stateMachine.ReusableData.ShouldSprint = false;//一旦进入滚动状态就不应该再继续冲刺了
            startTime = Time.time;
            Roll();
        }
        public override void Exit()
        {
            base.Exit();
            StopAnimation(stateMachine.Player.AnimationData.RollParameterHash);
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if(stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                return;//确保仅在不调用Move方法时进行旋转
            }
            RotateTowardsTargetRotation();
        }
        public override void Update()
        {
            base.Update();

            if (Time.time >= startTime + rollData.RollDuration)
            {
                OnAnimationTransitionEvent();  // 复用同一出口和冲刺一样
            }
        }
        public override void OnAnimationTransitionEvent()
        {
            if (stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.MediumStoppingState);//如果在最后一帧没有输入则转为中停止状态
                return;
            }
            OnMove();//因为上面的ShouldSprint改为了false，因此这里的OnMove不会转到冲刺
        }
        #endregion

        #region Main Methods
        private void Roll()
        {
            if (stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                return;// 有输入方向，交给物理帧的 Move() 处理（相机相对方向）
            }
            Vector3 rollDirection = stateMachine.Player.transform.forward;
            rollDirection.y = 0f;
            stateMachine.Player.Rigidbody.velocity = rollDirection * GetMovementSpeed(false);
        }
        #endregion

        #region Input Methods
        protected override void OnJumpStarted(InputAction.CallbackContext context)
        {
            //将其留空，不希望能跳跃
        }

        #endregion
    }
}
