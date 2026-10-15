using System.Collections;
using System.Diagnostics;
using System;
using UnityEngine;

namespace YuanshenMoveSystem
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }//单例
        public event Action<int> OnDeath;           // 参数：死亡次数,死亡
        public event Action<string> OnCheckpoint;   // 参数：检查点名,到达检查点
        public event Action<float, int> OnComplete; // 参数：通关时间, 死亡次数,到达终点
        //可配置引用
        [field: Header("References")]
        [field: SerializeField] public Player Player { get; private set; }
        [field: SerializeField] public Transform DefaultSpawn { get; private set; }//默认出生点
        //可配置参数
        [field: Header("Settings")]
        [field: SerializeField]
        [field: Range(0f, 2f)]
        public float RespawnDelay { get; private set; } = 0.3f;//重生的延迟时间，默认是0.3s
        //运行时状态，只有LevelManager自己知道
        private Transform currentCheckpoint;//当前生效的检查点，初始时候为DefaultSpawn
        private int deathCount;//死亡次数
        private float levelStartTime;//关卡开始时间，通关时用 Time.time - levelStartTime 算出用时
        private bool levelCompleted;//是否已经通关

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            //上面是单例保护
            if (DefaultSpawn != null)
                currentCheckpoint = DefaultSpawn;
        }

        private void Start()
        {
            levelStartTime = Time.time;
        }

        public void SetCheckpoint(Transform checkpoint)//设置检查点，Checkpoint 触发器命中玩家时调用
        {
            currentCheckpoint = checkpoint;
            OnCheckpoint?.Invoke(checkpoint.name);//触发事件到达检查点
            UnityEngine.Debug.Log($"[Level] Checkpoint updated: {checkpoint.name}");
        }

        public void OnPlayerDeath()
        {
            if (levelCompleted) return;//如果玩家已经通关，这里就不管了，也不复活了

            deathCount++;//加死亡次数
            OnDeath?.Invoke(deathCount);//触发死亡事件
            UnityEngine.Debug.Log($"[Level] Player died. Deaths: {deathCount}");
            StartCoroutine(RespawnRoutine());//启动重生协程
        }

        private IEnumerator RespawnRoutine()//重生协程,用协程是为了不中断主线程，让之前的动画可以正常播放0.3s,给玩家自己死了的视觉反馈
        {
            yield return new WaitForSeconds(RespawnDelay);//延迟

            if (currentCheckpoint == null)//检测一下重生点是不是被设置了
            {
                UnityEngine.Debug.LogError("[Level] No checkpoint set!");
                yield break;
            }

            //强制重置状态机
            Player.MovementStateMachine.ForceReset();

            //位移到检查点
            Player.transform.SetPositionAndRotation(currentCheckpoint.position, currentCheckpoint.rotation);
        }

        public void OnLevelComplete()//通关，Goal 触发器命中玩家时调用
        {
            if (levelCompleted) return;//防止重复触发
            levelCompleted = true;

            float time = Time.time - levelStartTime;//计算通关时间
            OnComplete?.Invoke(time, deathCount);//触发通关事件
            UnityEngine.Debug.Log($"[Level] Complete! Time: {time:F2}s, Deaths: {deathCount}");
        }
    }
}
