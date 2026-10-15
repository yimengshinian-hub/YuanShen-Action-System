using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    public class PlayerMovementStateMachine : StateMachine
    {
        public Player Player {  get; }
        public PlayerStateReusableData ReusableData { get; }
        public PlayeridlingState IdlingState { get; }
        public PlayerDashingState DashingState { get; }
        public PlayerWalkingState WalkingState { get; }
        public PlayerRunningState RunningState { get; }
        public PlayerSprintingState SprintingState { get; }
        public PlayerLightStoppingState LightStoppingState { get; }
        public PlayerHardStoppingState HardStoppingState { get; }
        public PlayerMediumStoppingState MediumStoppingState { get; }
        public PlayerJumpingState JumpingState { get; }
        public PlayerFallingState FallingState { get; }
        public PlayerLightLandingState LightLandingState { get; }
        public PlayerHardLandingState HardLandingState { get; }
        public PlayerRollingState RollingState { get; }

        public PlayerMovementStateMachine(Player player)
        {
            Player = player;
            ReusableData = new PlayerStateReusableData();
            IdlingState = new PlayeridlingState(this);
            DashingState = new PlayerDashingState(this);
            WalkingState = new PlayerWalkingState(this);
            RunningState = new PlayerRunningState(this);
            SprintingState = new PlayerSprintingState(this);
            LightStoppingState = new PlayerLightStoppingState(this);
            MediumStoppingState = new PlayerMediumStoppingState(this);
            HardStoppingState = new PlayerHardStoppingState(this);
            JumpingState = new PlayerJumpingState(this);
            FallingState = new PlayerFallingState(this);
            LightLandingState = new PlayerLightLandingState(this);
            HardLandingState = new PlayerHardLandingState(this);
            RollingState = new PlayerRollingState(this);
        }
        public void ForceReset()//死亡后的状态重置
        {
            //当前状态 Exit（注销回调、关动画）
            currentState?.Exit();

            //黑板清理
            ReusableData.MovementInput = Vector2.zero;
            ReusableData.ShouldSprint = false;
            ReusableData.ShouldWalk = false;
            ReusableData.CoyoteTimer = 0;

            //清 Rigidbody 速度
            Player.Rigidbody.velocity = Vector3.zero;

            //玩家输入重新启用（防 HardLanding 的 Disable 残留）
            Player.Input.PlayerActions.Enable();

            //进 Idling（走正常 Enter 流程）
            ChangeState(IdlingState);
        }
    }
}
