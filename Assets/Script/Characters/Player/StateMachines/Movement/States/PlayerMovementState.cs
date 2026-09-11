using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMoveSystem
{
    public class PlayerMovementState : IState
    {
        protected PlayerGroundedData movementData;//现在需要的数据都放在了这里面
        protected PlayerMovementStateMachine stateMachine;
       protected PlayerAirborneData airborneData;
        public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
        {
            stateMachine = playerMovementStateMachine;
            movementData = stateMachine.Player.Data.GroundedData;
            airborneData = stateMachine.Player.Data.AirborneData;
            SetBaseCameraRecenteringData();
            InitializeData();
        }
        private void InitializeData()
        {
            SetBaseRotationData();

        }

        #region IState Methods
        public virtual void Enter()
        {
            Debug.Log("State: "+GetType().Name);
            AddInputActionsCallbacks();
        }

       

        public virtual void Exit()
        {
            RemoveInputActionsCallbacks();
        }

     
        public virtual void HandleInput()//接收新输入系统的返回值
        {
            ReadMovementInput();
        }

        

        public virtual void PhysicsUpdate()
        {
            Move();
        }

     

        public virtual void Update()
        {

        }
        public virtual void OnAnimationEnterEvent()
        {
          
        }

        public virtual void OnAnimationExitEvent()
        {
           
        }

        public virtual void OnAnimationTransitionEvent()
        {
           
        }
        public virtual void OnTriggerEnter(Collider collider)
        {
            if (stateMachine.Player.LayerData.IsGroundLayer(collider.gameObject.layer))
            {
                OnContactWithGround(collider);
                return;
            }
        }
        public void OnTriggerExit(Collider collider)
        {
            if (stateMachine.Player.LayerData.IsGroundLayer(collider.gameObject.layer))
            {
                OnContactWithGroundExited(collider);
                return;
            }
        }

       

        #endregion
        #region Main Methons
        private void ReadMovementInput()//这是接收新输入系统返回值的实际实现
        {
            stateMachine.ReusableData.MovementInput = stateMachine.Player.Input.PlayerActions.Movement.ReadValue<Vector2>();
        }
        private void Move()
        {
            if(stateMachine.ReusableData.MovementInput == Vector2.zero|| stateMachine.ReusableData.MovementSpeedModifier==0f)//第二个是速度调节器，跳跃等的速度调节器为0，不算移动
            {
                return;
            }
            Vector3 movementDirection = GetMovementInputDirection();
            float targetRotationYAngle = Rotate(movementDirection);//转向的角度
            Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);//转向的方向
            float movementSpeed = GetMovementSpeed();
            Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();//获得当前水平方向的速度
            stateMachine.Player.Rigidbody.AddForce(movementSpeed*targetRotationDirection-currentPlayerHorizontalVelocity,ForceMode.VelocityChange);//第二个参数是改模式，将力变为瞬时力，不受时间影响
        }

      

        private float Rotate(Vector3 direction)//这里简化了，详细方法在可重用里面
        {
            float directionAngle = UpdateTargetRotation(direction);
            RotateTowardsTargetRotation();
            return directionAngle;
        }


        private float GetDirectionAngle(Vector3 direction)
        {
            //Atan2传的两个值分别为y,x，相当于z,x,但是不能按这个顺序填，应该按反的顺序填才能得到正确的角度，即x,z（因为unity中坐标系和数学坐标系0度开始轴不一样)
            float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;//这里和Input返回值的顺序一样即可获得对应的弧度（例：（0,1）代表向前，即x=0,z=1）//后面乘一个值是为了把弧度转换为角度
            //以下是将负角转换为正角（因为上面返回值为-180,180)
            if (directionAngle < 0f)
            {
                directionAngle += 360f;//这里得到的是玩家键盘输入的在坐标系里的角度
            }
            return directionAngle;
        }
        private float AddCameraRotationToAngle(Vector3 direction)
        {
            float directionAngle = GetDirectionAngle(direction);
            directionAngle += stateMachine.Player.MainCameraTransform.eulerAngles.y;//相机的y轴为水平轴旋转，相当于获得世界坐标系中的相机角度（但可能超过360度)
            if (directionAngle > 360f)
            {
                directionAngle -= 360f;
            }
            return directionAngle;
        } 
        private void UpdateTargetRotationData(float targetAngle)
        {
             stateMachine.ReusableData.CurrentTargetRotation.y = targetAngle;
             stateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0f;//重置经过的时间

        }


        #endregion
        #region Reusable Methods(可重用方法)
        protected void StartAnimation(int animationHash)
        {
            stateMachine.Player.Animator.SetBool(animationHash, true);
        }
        protected void StopAnimation(int animationHash)
        {
            stateMachine.Player.Animator.SetBool(animationHash, false);
        }
        protected void SetBaseCameraRecenteringData()
        {
            stateMachine.ReusableData.BackwardsCameraRecenteringData = movementData.BackwardsCameraRecenteringData;
            stateMachine.ReusableData.SidewaysCameraRecenteringData = movementData.SidewaysCameraRecenteringData;
        }

        protected void SetBaseRotationData()
        {
            stateMachine.ReusableData.RotationData = movementData.BaseRotationData;
            stateMachine.ReusableData.TimeToReachTargetRotation = stateMachine.ReusableData.RotationData.TargetRotationReachTime;//发送轮转需要的时间
        }
        protected Vector3 GetMovementInputDirection()
        {
            return new Vector3(stateMachine.ReusableData.MovementInput.x, 0, stateMachine.ReusableData.MovementInput.y);//因为我们input输入器定的是Vector只返回两个值，分别叫x,y并不是三维坐标系里的x,y
        }
        protected float GetMovementSpeed(bool shouldConsiderSlopes = true)
        {
            float movementSpeed = movementData.BaseSpeed * stateMachine.ReusableData.MovementSpeedModifier;
            if (shouldConsiderSlopes)//如果在斜坡上，则将速度乘斜坡速度修改器
            {
                movementSpeed *= stateMachine.ReusableData.MovementOnSlopesSpeedModifier;
            }
            return movementSpeed;
        }
        protected Vector3 GetPlayerHorizontalVelocity()
        {
            Vector3 playerHorizontalVelocity = stateMachine.Player.Rigidbody.velocity;//这个得到的是碰撞器中的那个速度变量
            playerHorizontalVelocity.y = 0f;//将竖直方向的速度置为0，防止他起飞
            return playerHorizontalVelocity;
        }
        protected Vector3 GetPlayerVerticalVelocity()//获得玩家的竖直速度
        {
            return new Vector3(0f,stateMachine.Player.Rigidbody.velocity.y, 0f);
        }
        protected void RotateTowardsTargetRotation()//该方法使角色平滑旋转
        {
            float currentYAngle = stateMachine.Player.Rigidbody.rotation.eulerAngles.y;//获取当前玩家旋转角度
            if(currentYAngle ==  stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                return;
            }
            float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle,  stateMachine.ReusableData.CurrentTargetRotation.y, ref  stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y,  stateMachine.ReusableData.TimeToReachTargetRotation.y- stateMachine.ReusableData.DampedTargetRotationPassedTime.y);
             stateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;
            Quaternion targetRotation = Quaternion.Euler(0f,smoothedYAngle, 0f);
            stateMachine.Player.Rigidbody.MoveRotation(targetRotation);

        }
        protected float UpdateTargetRotation(Vector3 direction,bool shouldConsiderCameraRotation = true)
        {
            float directionAngle = GetDirectionAngle(direction);
            if (shouldConsiderCameraRotation)
            {
                //下面二者角度相加达到真正的转身，即向右转身后再按d会向着世界坐标系的后走，即向着转身后的右走
                directionAngle = AddCameraRotationToAngle(direction);
                //现在有了最终的方向角directionAnale,以下是旋转玩家的具体操作
            }
            if (directionAngle !=  stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                UpdateTargetRotationData(directionAngle);
            }
            return directionAngle;
        }
        protected Vector3 GetTargetRotationDirection(float targetAngle)//得到想要去的方向
        {
            return Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;//forward表示我们希望可以从z轴开始旋转，这是玩家前进的轴
        }
        protected void ResetVelocity()//重新设置速度(对碰撞器中的速度变量进行设置，确保是瞬时变化)
        {
            stateMachine.Player.Rigidbody.velocity = Vector3.zero;
        }
        protected void ResetVerticalVelocity()//使垂直速度设置为0
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();//这里返回了一个y值为0的向量，其他都不变
            stateMachine.Player.Rigidbody.velocity = playerHorizontalVelocity;
        }
        protected virtual void AddInputActionsCallbacks()//添加回调（每次进入一个状态都会进行一次回调）
        {
            stateMachine.Player.Input.PlayerActions.WalkToggle.started += OnWalkToggleStarted;
            stateMachine.Player.Input.PlayerActions.Look.started += OnMouseMovementStarted;//鼠标移动时的回调
            stateMachine.Player.Input.PlayerActions.Movement.performed += OnMovementPerformed;//移动时发生的回调
            stateMachine.Player.Input.PlayerActions.Movement.canceled += OnMovementCanceled;//当移动取消时的回调

        }

        protected virtual void RemoveInputActionsCallbacks()//删除回调
        {
            stateMachine.Player.Input.PlayerActions.WalkToggle.started -= OnWalkToggleStarted;
            stateMachine.Player.Input.PlayerActions.Look.started -= OnMouseMovementStarted;
            stateMachine.Player.Input.PlayerActions.Movement.performed -= OnMovementPerformed;
            stateMachine.Player.Input.PlayerActions.Movement.canceled -= OnMovementCanceled;
        }
        protected void DecelerateHorizontally()
        {
            //水平轴上减速
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();//获得水平速度
            stateMachine.Player.Rigidbody.AddForce(-playerHorizontalVelocity * stateMachine.ReusableData.MovementDecelerationForce, ForceMode.Acceleration);//获得一个与速度方向相反的力，并且与时间有关，与质量无关


        }
        protected void DecelerateVertically()
        {
            //水平轴上减速
            Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();//获得竖直速度
            stateMachine.Player.Rigidbody.AddForce(-playerVerticalVelocity * stateMachine.ReusableData.MovementDecelerationForce, ForceMode.Acceleration);//获得一个与速度方向相反的力，并且与时间有关，与质量无关

        }
        protected bool IsMovingHorizontally(float minimumMagnitude = 0.1f)
        {
            //判断是否在水平移动(参数是为了与玩家水平速度大小比较来判断玩家是否正在移动的)
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();//该点距离原点的距离就是速度大小
            Vector2 playerHorizontalMovement = new Vector2(playerHorizontalVelocity.x, playerHorizontalVelocity.z);//该点距离原点的距离就是速度大小
            return (playerHorizontalMovement.magnitude > minimumMagnitude);
        }
        protected bool IsMovingUp(float minimumVelocity = 0.1f)//是否在往上移动
        {
            return GetPlayerVerticalVelocity().y > minimumVelocity;
        }
        protected bool IsMovingDown(float minimumVelocity = 0.1f)//是否在往下移动
        {
            return GetPlayerVerticalVelocity().y < -minimumVelocity;
        }
        protected virtual void OnContactWithGround(Collider collider)
        {
        }
        protected virtual void OnContactWithGroundExited(Collider collider)
        {
        }
        protected void UpdateCameraRecenteringState(Vector2 movementInput)//根据相机角度和运动输入来禁用或启用水平居中
        {
            if (movementInput == Vector2.zero)//停止的已经在移动结束的回调中有处理了
            {
                return;
            }
            //向前移动时禁用水平居中
            if (movementInput == Vector2.up)
            {
                DisableCameraRecentering();
                return;
            }
            //向后移动时查看迭代表，看其是否在范围内，不在范围内禁用水平居中
            float cameraVerticalAngle = stateMachine.Player.MainCameraTransform.eulerAngles.x;
            if (cameraVerticalAngle >= 270f)
            {
                cameraVerticalAngle -= 360f;
            }
            cameraVerticalAngle = Mathf.Abs(cameraVerticalAngle);
            if (movementInput == Vector2.down)
            {
                SetCameraRecenteringState(cameraVerticalAngle, stateMachine.ReusableData.BackwardsCameraRecenteringData);
                return;
            }
            SetCameraRecenteringState(cameraVerticalAngle, stateMachine.ReusableData.SidewaysCameraRecenteringData);
        }
        protected void EnableCameraRecentering(float waitTime = -1f,float recenteringTime = -1f)//启用虚拟相机的水平居中
        {
            float movementSpeed = GetMovementSpeed();
            if (movementSpeed == 0f)
            {
                movementSpeed = movementData.BaseSpeed;//避免除以0的情况，在EnableRecentering方法中
            }
            stateMachine.Player.CameraUtility.EnableRecentering(waitTime, recenteringTime,movementData.BaseSpeed,movementSpeed);
        }
        protected void DisableCameraRecentering()//禁用虚拟相机的水平居中
        {
            stateMachine.Player.CameraUtility.DisableRecentering();
        }
        protected void SetCameraRecenteringState(float cameraVerticalAngle, List<PlayerCameraRecenteringData> cameraRecenteringData)
        {
            foreach (PlayerCameraRecenteringData recenteringData in cameraRecenteringData)//想成auto遍历就行了
            {
                if (!recenteringData.IsWithinRange(cameraVerticalAngle))
                {
                    continue;
                }
                EnableCameraRecentering(recenteringData.WaitTime, recenteringData.RecenteringTime);
                return;
            }
            DisableCameraRecentering();
        }

        #endregion
        #region Input Methods
        protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)//行走刚开始时的操作
        {
             stateMachine.ReusableData.ShouldWalk = !stateMachine.ReusableData.ShouldWalk;
        }

        protected virtual void OnMovementCanceled(InputAction.CallbackContext context)//移动输入结束即停止后的操作
        {
            DisableCameraRecentering();
        }
        private void OnMovementPerformed(InputAction.CallbackContext context)//Performed会在每次按键时候调用,而started则只会在第一次按键时调用
        {
            UpdateCameraRecenteringState(context.ReadValue<Vector2>());
        }

        private void OnMouseMovementStarted(InputAction.CallbackContext context)
        {
            UpdateCameraRecenteringState(stateMachine.ReusableData.MovementInput);
        }
        #endregion
    }
}
