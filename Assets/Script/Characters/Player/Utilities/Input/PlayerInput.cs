using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMoveSystem
{
    public class PlayerInput : MonoBehaviour
    {
      public PlayerInputActions InputActions { get; private set; }
      public PlayerInputActions.PlayerActions PlayerActions { get; private set; }
        private void Awake()
        {
            InputActions = new PlayerInputActions();
            PlayerActions = InputActions.Player;
        }
        private void OnEnable()
        {
            InputActions.Enable();
        }
        private void OnDisable()
        {
            InputActions.Disable();
        }
        public void disableActionFor(InputAction action,float seconds)
        {
            //在该方法中我们将禁用一个输入持续几秒钟（连续两次冲刺后短暂的不能冲刺）
            StartCoroutine(DisableAction(action, seconds));
        }
        private IEnumerator DisableAction(InputAction action,float seconds)//协程
        {
            action.Disable();
            yield return new WaitForSeconds(seconds);
            action.Enable();
        }
    }
}
