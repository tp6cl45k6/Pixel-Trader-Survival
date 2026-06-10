using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }

    [Header("生存數值 (0~100)")]
    public float energy = 100f;   // 體力
    public float stress = 0f;     // 壓力
    public float satiety = 100f;  // 飽食度

    [Header("UI 綁定 (TopBarPanel)")]
    public Text energyText;
    public Text stressText;
    public Text satietyText;

    [Header("生活開銷")]
    public float dailyExpense = 1500f;  // 每天睡醒要扣除的房租、水電、飯錢

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        HandlePassiveStatChanges();
    }

    // 處理隨時間變化的數值 (例如：看盤賠錢會增加壓力)
    private void HandlePassiveStatChanges()
    {
        // 確保所有管理器都存在，且市場正在營業
        if (PlayerPortfolio.Instance == null || LiveMarketController.Instance == null || GameTimeManager.Instance == null) return;
        if (!GameTimeManager.Instance.isMarketOpen) return;

        // 【修正】：獲取目前玩家畫面上正在觀看的股票
        StockData currentStock = LiveMarketController.Instance.GetCurrentSelectedStock();
        if (currentStock == null) return;

        // 【修正】：帶入 currentStock 參數向各系統查詢對應數值
        float currentPrice = LiveMarketController.Instance.GetCurrentPrice(currentStock);
        int sharesOwned = PlayerPortfolio.Instance.GetSharesOwned(currentStock);
        float averageCost = PlayerPortfolio.Instance.GetAverageCost(currentStock);

        float unrealizedProfit = (currentPrice - averageCost) * sharesOwned;
        
        // 如果目前這檔股票有持股，且處於賠錢狀態
        if (sharesOwned > 0 && unrealizedProfit < 0)
        {
            // 賠錢時，壓力每秒上升
            stress += 1f * Time.deltaTime;
            stress = Mathf.Clamp(stress, 0f, 100f);
            UpdateUI();

            if (stress >= 100f)
            {
                Debug.LogWarning("⚠️ 壓力破表！玩家精神瀕臨崩潰！");
            }
        }
    }

    // --- 修改：接收 JobData 的打工方法 ---
    public void DoJob(JobData jobData)
    {
        if (energy >= jobData.energyCost)
        {
            energy -= jobData.energyCost;
            stress += jobData.stressGain;
            stress = Mathf.Clamp(stress, 0f, 100f);
            
            if (GameTimeManager.Instance != null)
            {
                GameTimeManager.Instance.SkipTime(jobData.hoursTaken);
            }

            if (PlayerPortfolio.Instance != null)
            {
                PlayerPortfolio.Instance.cash += jobData.salary;
                PlayerPortfolio.Instance.UpdateUI();
            }

            Debug.Log($"🏪 [打工] 執行【{jobData.jobName}】！耗時 {jobData.hoursTaken} 小時，賺取 ${jobData.salary}。");
            UpdateUI();
        }
        else
        {
            Debug.LogWarning($"❌ [過勞警告] 體力不足以進行【{jobData.jobName}】！");
        }
    }

    // 更新頂部狀態列的介面
    public void UpdateUI()
    {
        if (energyText != null) energyText.text = $"體力: {energy:0}";
        if (stressText != null) 
        {
            stressText.text = $"壓力: {stress:0}";
            // 壓力越高，字體顏色會從白色漸變成紅色
            stressText.color = Color.Lerp(Color.white, Color.red, stress / 100f);
        }
        if (satietyText != null) satietyText.text = $"飽食度: {satiety:0}";
    }

    // --- 新增：上床睡覺 (換日與扣除生活費) ---
    public void GoToSleep()
    {
        // 1. 推進時間到隔天
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.AdvanceToNextDay();
        }

        // 2. 恢復體力與稍微降低壓力 (睡覺也是能紓壓的！)
        energy = 100f;
        stress -= 10f; // 睡一覺壓力少 10
        stress = Mathf.Clamp(stress, 0f, 100f);

        // 3. 殘酷結算：扣除生活基本開銷
        if (PlayerPortfolio.Instance != null)
        {
            PlayerPortfolio.Instance.cash -= dailyExpense;
            PlayerPortfolio.Instance.UpdateUI();

            Debug.Log($"🛏️ [換日結算] 睡了一覺，體力恢復！扣除每日生活費 ${dailyExpense}。");

            // 檢查是否破產
            if (PlayerPortfolio.Instance.cash < 0)
            {
                Debug.LogError("💀 [Game Over] 交割戶餘額為負，繳不出房租，流落街頭！");
                // TODO: 未來可以在這裡跳出「破產結算畫面」並暫停遊戲
            }
        }

        UpdateUI();
    }
}
