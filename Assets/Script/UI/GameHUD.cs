using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace YuanshenMoveSystem
{
    //游戏主 HUD，挂在 Canvas 上。
    //订阅 LevelManager 的 4 个事件来显示对应 UI。
    public class GameHUD : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject deathPanel;       //死亡提示面板
        [SerializeField] private GameObject checkpointPanel;  //存档点提示面板
        [SerializeField] private GameObject completePanel;     //通关结算面板

        [Header("Panel Canvas Groups")]
        [SerializeField] private CanvasGroup deathGroup;
        [SerializeField] private CanvasGroup checkpointGroup;
        [SerializeField] private CanvasGroup completeGroup;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI deathCountText;   //死亡面板上的次数文本
        [SerializeField] private TextMeshProUGUI checkpointText;    //存档点面板上的名字文本
        [SerializeField] private TextMeshProUGUI completeTimeText;  //通关时间文本
        [SerializeField] private TextMeshProUGUI completeDeathText;  //通关死亡次数文本

        [Header("Settings")]
        [SerializeField] private float checkpointDisplayTime = 2f; //存档点提示显示时长

        [Header("Animation Settings")]
        [SerializeField] private float fadeInDuration = 0.3f;//淡入时长
        [SerializeField] private float fadeOutDuration = 0.3f;//谈出时长

        private void Start()
        {
            // 订阅事件
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.OnDeath += HandleDeath;
                LevelManager.Instance.OnCheckpoint += HandleCheckpoint;
                LevelManager.Instance.OnComplete += HandleComplete;
            }

            // 隐藏面板
            if (deathPanel != null && deathGroup != null)
            {
                deathGroup.alpha = 0f;
                deathPanel.SetActive(false);
            }
            if (checkpointPanel != null && checkpointGroup != null)
            {
                checkpointGroup.alpha = 0f;
                checkpointPanel.SetActive(false);
            }
            if (completePanel != null && completeGroup != null)
            {
                completeGroup.alpha = 0f;
                completePanel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.OnDeath -= HandleDeath;
                LevelManager.Instance.OnCheckpoint -= HandleCheckpoint;
                LevelManager.Instance.OnComplete -= HandleComplete;
            }
        }

        private void HandleDeath(int deaths)
        {
            if (deathPanel != null && deathGroup != null)
            {
                if (deathCountText != null)
                {
                    deathCountText.text = $"Deaths: {deaths}";
                }
                //淡入淡出的效果
                StartCoroutine(DeathSequence());
            }
        }


        private void HandleCheckpoint(string checkpointName)
        {
            if (checkpointPanel != null && checkpointGroup != null)
            {
                if (checkpointText != null)
                {
                    checkpointText.text = $"Checkpoint: {checkpointName}";

                }
                StartCoroutine(CheckpointSequence());
            }
        }

        private void HandleComplete(float time, int deaths)
        {
            if (completePanel != null && completeGroup != null)
            {
                if (completeTimeText != null)
                {
                    int minutes = (int)(time / 60f);
                    float seconds = time % 60f;
                    completeTimeText.text = $"Time: {minutes}:{seconds:00.00}";
                }
                if (completeDeathText != null)
                {
                    completeDeathText.text = $"Deaths: {deaths}";

                }
                // 通关面板只淡入不淡出
                StartCoroutine(FadeIn(completePanel, completeGroup));
            }
        }


        private IEnumerator HidePanelAfterDelay(GameObject panel, float delay)//这个协程是用来关闭UI的
        {
            yield return new WaitForSeconds(delay);
            if (panel != null)
                panel.SetActive(false);
        }
        //淡入
        private IEnumerator FadeIn(GameObject panel, CanvasGroup group)
        {
            panel.SetActive(true);
            group.alpha = 0f;
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                group.alpha = elapsed / fadeInDuration;
                yield return null;
            }
            group.alpha = 1f;
        }
        //淡出
        private IEnumerator FadeOut(GameObject panel, CanvasGroup group)
        {
            group.alpha = 1f;
            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                group.alpha = 1f - (elapsed / fadeOutDuration);
                yield return null;
            }
            group.alpha = 0f;
            panel.SetActive(false);
        }
        //淡入后淡出,中间间隔checkpointDisplayTime
        private IEnumerator CheckpointSequence()
        {
            yield return FadeIn(checkpointPanel, checkpointGroup);
            yield return new WaitForSeconds(checkpointDisplayTime);
            yield return FadeOut(checkpointPanel, checkpointGroup);
        }
        //死亡淡入淡出，中间和RespawnDelay对齐
        private IEnumerator DeathSequence()
        {
            yield return FadeIn(deathPanel, deathGroup);
            yield return new WaitForSeconds(0.3f);   // 和 RespawnDelay 对齐
            yield return FadeOut(deathPanel, deathGroup);
        }
    }
}
