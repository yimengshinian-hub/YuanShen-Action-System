using UnityEngine;

namespace YuanshenMoveSystem
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        [field: Header("References")]//在Insp...面板上加的注释
        [field: SerializeField] public PlayerSO Data { get; private set; } //这是数据
        [field: Header("Collisions")]
        [field: SerializeField] public PlayerCapsuleColliderUtility ColliderUtility{ get; private set; }//这是用于处理胶囊碰撞器的脚本实例
        [field:SerializeField] public PlayerLayerData LayerData { get; private set; }
        [field:Header("Cameras")]
        [field:SerializeField] public PlayerCameraUtility CameraUtility { get; private set; }
        [field:Header("Animations")]
        [field:SerializeField] public PlayerAnimationData AnimationData { get; private set; }
        public Animator Animator { get; private set; }
        public Rigidbody Rigidbody {  get; private set; }
        public Transform MainCameraTransform { get; private set; }
        public PlayerInput Input {  get; private set; }
        public PlayerMovementStateMachine MovementStateMachine { get; private set; }
        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody> ();
            Animator = GetComponentInChildren<Animator> ();
            Input = GetComponent<PlayerInput>();
            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimensions();//设置我们的碰撞器数据
            CameraUtility.Initialize();//初始化虚拟相机
            AnimationData.initialize();
            MainCameraTransform = Camera.main.transform;//获取主摄像机的位置信息
            MovementStateMachine = new PlayerMovementStateMachine(this);
        }
#if UNITY_EDITOR
        private void OnValidate()//当脚本被加载、或在 Inspector 里修改了该脚本的任何值时调用。
        {
            if (ColliderUtility == null) return;
            if (ColliderUtility.CapsuleColliderData == null) return;
            if (ColliderUtility.TriggerColliderData == null) return;
            if (ColliderUtility.TriggerColliderData.GroundCheckCollider == null) return;

            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimensions();
        }
#endif
        private void Start()
        {
            MovementStateMachine.ChangeState(MovementStateMachine.IdlingState);
        }
        private void OnTriggerEnter(Collider collider)
        {
            MovementStateMachine.OnTriggerEnter(collider);
        }
        private void OnTriggerExit(Collider other)
        {
            MovementStateMachine.OnTriggerExit(other);
        }
        private void Update()
        {
            MovementStateMachine.HandleInput();//读取玩家输入
            UpdateTimers();//更新土狼计时器
            MovementStateMachine.Update();//更新状态机的非物理逻辑（比如动画、计时器）
            //测试代码，测试重置状态是否可行
            /*if (UnityEngine.Input.GetKeyDown(KeyCode.K))
            {
                MovementStateMachine.ForceReset();
            }*/
        }
        private void FixedUpdate()//以固定的时间间隔调用，默认 0.02 秒一次
        {
            MovementStateMachine.PhysicsUpdate();//执行物理移动
        }
        public void OnMovemetStateAnimationEnterEvent()
        {
            MovementStateMachine.OnAnimationEnterEvent();
        }
        public void OnMovemetStateAnimationExitEvent()
        {
            MovementStateMachine.OnAnimationExitEvent();
        }
        public void OnMovemetStateAnimationTransitionEvent()
        {
            MovementStateMachine.OnAnimationTransitionEvent();
        }
        #region Timer Methons
        private void UpdateTimers()
        {
            var data = MovementStateMachine.ReusableData;
            if (data.CoyoteTimer > 0)
            {
                data.CoyoteTimer -= Time.deltaTime;
            }      
            //测试用
           /* if (data.CoyoteTimer > 0)
            {
                Debug.Log($"Coyote: {data.CoyoteTimer:F3}");
            } */
        }
        #endregion
    }
}
