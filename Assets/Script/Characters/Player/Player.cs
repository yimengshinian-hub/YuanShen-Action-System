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
        private PlayerMovementStateMachine movementStateMachine;
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
            movementStateMachine = new PlayerMovementStateMachine(this);
        }
        private void OnValidate()//当脚本被加载、或在 Inspector 里修改了该脚本的任何值时调用。
        {
            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimensions();//设置我们的碰撞器数据
        }
        private void Start()
        {
            movementStateMachine.ChangeState(movementStateMachine.IdlingState);
        }
        private void OnTriggerEnter(Collider collider)
        {
            movementStateMachine.OnTriggerEnter(collider);
        }
        private void OnTriggerExit(Collider other)
        {
            movementStateMachine.OnTriggerExit(other);
        }
        private void Update()
        {
            movementStateMachine.HandleInput();//读取玩家输入
            movementStateMachine.Update();//更新状态机的非物理逻辑（比如动画、计时器）
        }
        private void FixedUpdate()//以固定的时间间隔调用，默认 0.02 秒一次
        {
            movementStateMachine.PhysicsUpdate();//执行物理移动
        }
        public void OnMovemetStateAnimationEnterEvent()
        {
            movementStateMachine.OnAnimationEnterEvent();
        }
        public void OnMovemetStateAnimationExitEvent()
        {
            movementStateMachine.OnAnimationExitEvent();
        }
        public void OnMovemetStateAnimationTransitionEvent()
        {
            movementStateMachine.OnAnimationTransitionEvent();
        }
    }
}
