using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace YuanshenMoveSystem
{
    public abstract class StateMachine 
    {
        protected IState currentState;
        public IState CurrentState => currentState;
        public event Action<IState, IState> OnStateChanged;//旧状态，新状态
        public void ChangeState(IState newState)
        {
            IState previousState = currentState;//存储旧状态
            currentState?.Exit();//加问号是C#的一种新运算符，currentState为null时不会调用该方法//当前状态离开
            currentState = newState;//改为新状态
            currentState.Enter();//改为新状态后的一系列操作
            OnStateChanged?.Invoke(previousState, newState);//事件传入两种状态
        }
        public void HandleInput()
        {
            currentState?.HandleInput();
        }
        public void Update()
        {
            currentState?.Update();
        }
        public void PhysicsUpdate()
        {
            currentState?.PhysicsUpdate();
        }
        public void OnAnimationEnterEvent()
        {
            currentState?.OnAnimationEnterEvent();
        }
        public void OnAnimationExitEvent()
        {
            currentState?.OnAnimationExitEvent();
        }
        public void OnAnimationTransitionEvent()
        {
            currentState?.OnAnimationTransitionEvent();
        }
        public void OnTriggerEnter(Collider collider)
        {
            currentState?.OnTriggerEnter(collider);
        }
        public void OnTriggerExit(Collider collider)
        {
            currentState?.OnTriggerExit(collider);
        }
    }
}
