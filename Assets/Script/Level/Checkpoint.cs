using System.Diagnostics;
using UnityEngine;

namespace YuanshenMoveSystem
{
    //检查点触发器
    public class Checkpoint : MonoBehaviour
    {
        [field: Header("Settings")]
        [field: SerializeField] public Transform SpawnPoint { get; private set; }//复活点的位置，在Inspector中配置,不配的话就是用检查点自身的位置

        private bool activated;

        private void OnTriggerEnter(Collider other)
        {
            // 只响应玩家 + 只触发一次
            if (activated || !other.CompareTag("Player")) return;

            activated = true;

            // 如果 SpawnPoint 没拖（Inspector 为空），用自己位置
            Transform spawnPos = SpawnPoint != null ? SpawnPoint : transform;

            LevelManager.Instance.SetCheckpoint(spawnPos);

            // 可选：视觉反馈（缩放、颜色变化等）
            // 暂时只打印
            UnityEngine.Debug.Log($"[Checkpoint] Activated: {name}");
        }
    }
}