using UnityEngine;

namespace YuanshenMoveSystem
{
    //死亡触发器
    public class KillZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))//判断是不是玩家
            {
                LevelManager.Instance.OnPlayerDeath();
            }
        }
    }
}