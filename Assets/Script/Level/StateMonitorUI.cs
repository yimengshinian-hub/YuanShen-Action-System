using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuanshenMoveSystem
{
    //常态显示当前状态和状态转换的UI脚本
    public class StateMonitorUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI stateNameText;//当前状态名
        [SerializeField] private TextMeshProUGUI stateTimeText;//状态内计时
        [SerializeField] private TextMeshProUGUI stateLogText;//最近 N 次转换
        [SerializeField] private int logLineCount = 6;//日志保留几行

        private Player player;
        private float stateEnterTime;// 进当前状态的时间戳
        private readonly Queue<string> logLines = new();// 日志队列

        private void Start()
        {
            player = FindObjectOfType<Player>();
            player.MovementStateMachine.OnStateChanged += HandleStateChanged;
            HandleStateChanged(null, player.MovementStateMachine.CurrentState);// 初始态也要显示
        }

        private void OnDestroy()
        {
            if (player != null && player.MovementStateMachine != null)
            {
                player.MovementStateMachine.OnStateChanged -= HandleStateChanged;
            }
        }

        private void HandleStateChanged(IState previous, IState current)
        {
            stateEnterTime = Time.time;
            stateNameText.text = Prettify(current.GetType().Name);
            //生成日志行
            logLines.Enqueue(previous == null
                ? "→ " + Prettify(current.GetType().Name)
                : $"{Prettify(previous.GetType().Name)} → {Prettify(current.GetType().Name)}");
            while (logLines.Count > logLineCount) logLines.Dequeue();//超出保留数量就删旧的
            stateLogText.text = string.Join("\n", logLines);//更新日志显示
        }

        private void Update()
        {
            stateTimeText.text = $"{(Time.time - stateEnterTime):F1}s";
            // 按 R 清空日志
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                logLines.Clear();
                stateLogText.text = "";
                UnityEngine.Debug.Log("[StateMonitorUI] Log cleared.");
            }
        }

        private static string Prettify(string raw)// "PlayerSprintingState" → "Sprinting",简化一下
            => raw.Replace("Player", "").Replace("State", "").Replace("idling", "Idling");
    }
}