using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuanshenMoveSystem
{
    public class PlayerAnimationEventTrigger : MonoBehaviour
    {
        private Player player;
        private void Awake()
        {
            player = transform.parent.GetComponent<Player>();
        }
        public void TriggerOnMovementStateAnimationEnterEvent()
        {
            if (IsInAnimationTransition())
            {
                return;
            }
            player.OnMovemetStateAnimationEnterEvent();
        }
        public void TriggerOnMovementStateAnimationExitEvent()
        {
            if (IsInAnimationTransition())
            {
                return;
            }
            player.OnMovemetStateAnimationExitEvent();
        }
        public void TriggerOnMovementStateAnimationTransitionEvent()
        {
            if (IsInAnimationTransition())
            {
                return;
            }
            player.OnMovemetStateAnimationTransitionEvent();
        }
        private bool IsInAnimationTransition(int layerIndex = 0)
        {
            return player.Animator.IsInTransition(layerIndex);
        }
    }
}
