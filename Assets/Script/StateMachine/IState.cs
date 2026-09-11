
using UnityEngine;

namespace YuanshenMoveSystem
{
    public interface IState
    {
        public void Enter();//当状态由其他状态转换到当前状态时调用
        public void Exit();//当状态转换到上一状态时调用
        public void HandleInput();//允许我们运行任何关于读取输入的逻辑
        public void Update();//允许我们运行任何非物理相关的逻辑
        public void PhysicsUpdate();//允许我们运行任何物理相关的逻辑
        public void OnAnimationEnterEvent();//动画开始时发生的事件
        public void OnAnimationExitEvent();//动画离开(结束)时发生的事件
        public void OnAnimationTransitionEvent();//当动画进行到某一帧时转换到其他状态
        public void OnTriggerEnter(Collider collider);//角色下的触发器触发时开始事件
        public void OnTriggerExit(Collider collider);//角色下的触发器离开时发生事件，即未触发时发生事件（当前只有地面触发器，因此又可以说为在空中时发生的事件）
    }
}
