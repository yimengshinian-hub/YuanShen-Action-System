using UnityEngine;

namespace YuanshenMoveSystem
{
    //通关触发器
    public class Goal : MonoBehaviour
    {
        private bool triggered;//还是防止重复触发

        private void OnTriggerEnter(Collider other)
        {
            if (triggered || !other.CompareTag("Player")) return;

            triggered = true;
            LevelManager.Instance.OnLevelComplete();
        }
    }
}