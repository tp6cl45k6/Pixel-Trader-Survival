using UnityEngine;
using UnityEngine.UI;

public class GameTimeManager : MonoBehaviour
{
    // 單例模式，方便其他系統來問「現在幾點了？」
    public static GameTimeManager Instance { get; private set; }

    [Header("時間設定")]
    public float timeScale = 10f;       // 時間流逝速度：現實 1 秒 = 遊戲 10 分鐘
    public float currentHour = 8f;      // 遊戲起始時間：早上 8 點
    public float currentMinute = 30f;   // 遊戲起始時間：30 分

    [Header("UI 綁定")]
    public Text timeText;               // 綁定頂部狀態列的 08:30 文字

    // 股市是否營業的開關
    public bool isMarketOpen { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }

    private void Update()
    {
        // 1. 遊戲時間流逝計算
        currentMinute += Time.deltaTime * timeScale;
        if (currentMinute >= 60f)
        {
            currentMinute -= 60f;
            currentHour += 1f;
            
            // 簡單的跨日重置 (未來可以擴充「結算日」機制)
            if (currentHour >= 24f) currentHour = 0f; 
        }

        // 2. 更新時間 UI (強制顯示為 00:00 的兩位數格式)
        if (timeText != null)
        {
            timeText.text = $"{Mathf.FloorToInt(currentHour):00}:{Mathf.FloorToInt(currentMinute):00}";
        }

        // 3. 每幀檢查是否觸發開/收盤
        CheckMarketStatus();
    }

    private void CheckMarketStatus()
    {
        // 將時間轉換為「今天經過的總分鐘數」，方便做數學判斷
        float totalMinutes = currentHour * 60f + currentMinute;

        // 台股營業時間：09:00 (540分) ~ 13:30 (810分)
        if (totalMinutes >= 540f && totalMinutes < 810f)
        {
            if (!isMarketOpen)
            {
                isMarketOpen = true;
                Debug.Log("🔔【09:00】鐘聲響起，台股開盤，開始交易！");
            }
        }
        else
        {
            if (isMarketOpen)
            {
                isMarketOpen = false;
                Debug.Log("🔕【13:30】鐘聲響起，台股收盤，停止交易！");
            }
        }
    }

    // --- 新增：快轉時間系統 ---
    public void SkipTime(float hoursToSkip)
    {
        float totalMinutesToSkip = hoursToSkip * 60f;
        
        // 算出這段時間應該要有幾次「跳動」(依照 timeScale)
        int stepCount = Mathf.FloorToInt(totalMinutesToSkip / timeScale);

        // 在一瞬間 (一幀) 內模擬完這幾小時的推演
        for (int i = 0; i < stepCount; i++)
        {
            // 1. 推進時間
            currentMinute += timeScale;
            if (currentMinute >= 60f)
            {
                currentMinute -= 60f;
                currentHour += 1f;
                if (currentHour >= 24f) currentHour = 0f;
            }

            // 2. 檢查這一瞬間股市開了沒
            CheckMarketStatus();

            // 3. 如果開盤了，強迫股市瞬間生出一筆資料補上圖表！
            if (isMarketOpen && LiveMarketController.Instance != null)
            {
                LiveMarketController.Instance.GenerateNewTickForAllStocks();
            }
        }

        // 快轉完畢，強制更新一次 UI
        if (timeText != null)
        {
            timeText.text = $"{Mathf.FloorToInt(currentHour):00}:{Mathf.FloorToInt(currentMinute):00}";
        }
    }

    // --- 新增：直接跳到隔天早上 08:30 ---
    public void AdvanceToNextDay()
    {
        // 直接將時間重置為 08:30
        currentHour = 8f;
        currentMinute = 30f;
        
        // 強制關閉市場 (避免有任何跨日狀態殘留)
        isMarketOpen = false;

        // --- 新增這段：通知股市清空昨天的走勢圖 ---
        if (LiveMarketController.Instance != null)
        {
            LiveMarketController.Instance.ResetMarketForNewDay();
        }
        // ----------------------------------------

        // 更新時間 UI
        if (timeText != null)
        {
            timeText.text = $"{Mathf.FloorToInt(currentHour):00}:{Mathf.FloorToInt(currentMinute):00}";
        }

        Debug.Log("🌅 新的一天開始了！時間來到 08:30。");
    }
}

