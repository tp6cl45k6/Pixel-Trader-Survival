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

    [Header("打工設定")]
    public float workCashReward = 600f; // 打工一次賺 600 元
    public float workEnergyCost = 40f;  // 消耗體力
    public float workStressGain = 20f;  // 增加壓力
    public float workHourCost = 4f;     // 每次打工消耗 4 小時遊戲時間

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
        // 只有在有持股且市場營業時，才結算看盤壓力
        if (PlayerPortfolio.Instance == null || LiveMarketController.Instance == null || GameTimeManager.Instance == null) return;
        if (!GameTimeManager.Instance.isMarketOpen) return;

        float currentPrice = LiveMarketController.Instance.GetCurrentPrice();
        float unrealizedProfit = (currentPrice - PlayerPortfolio.Instance.averageCost) * PlayerPortfolio.Instance.sharesOwned;
        
        // 如果目前有持股，且未實現損益為負 (跌破成本線)
        if (PlayerPortfolio.Instance.sharesOwned > 0 && unrealizedProfit < 0)
        {
            // 賠錢時，壓力每秒上升 (數字越高死越快)
            stress += 1f * Time.deltaTime;
            stress = Mathf.Clamp(stress, 0f, 100f);
            UpdateUI();

            if (stress >= 100f)
            {
                Debug.LogWarning("⚠️ 壓力破表！玩家精神瀕臨崩潰！");
            }
        }
    }

    // --- 玩家行為：去超商打工 ---
    public void GoToWork()
    {
        if (energy >= workEnergyCost)
        {
            // 扣除體力、增加壓力
            energy -= workEnergyCost;
            stress += workStressGain;
            stress = Mathf.Clamp(stress, 0f, 100f);
            
            // 推進遊戲時間
            if (GameTimeManager.Instance != null)
            {
                GameTimeManager.Instance.SkipTime(workHourCost);
            }

            // 將辛苦賺來的工資匯入交割戶
            if (PlayerPortfolio.Instance != null)
            {
                PlayerPortfolio.Instance.cash += workCashReward;
                PlayerPortfolio.Instance.UpdateUI();
            }

            Debug.Log($"🏪 [血汗打工] 耗時 {workHourCost} 小時，賺取 ${workCashReward}！消耗體力 {workEnergyCost}，壓力增加 {workStressGain}。");
            UpdateUI();
        }
        else
        {
            Debug.LogWarning("❌ [過勞警告] 體力不足，無法打工！請先休息。");
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
}
