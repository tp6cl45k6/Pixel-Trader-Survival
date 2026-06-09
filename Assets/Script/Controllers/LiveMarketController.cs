using System.Collections.Generic;
using UnityEngine;

public class LiveMarketController : MonoBehaviour
{
    public UILineGraph lineGraph;
    
    [Header("市場設定")]
    public float basePrice = 850f;     // 平盤價
    public float updateInterval = 1f;  // 每幾秒更新一次 (設定為 1 秒)
    
    private List<float> priceHistory = new List<float>();
    private float timer = 0f;
    private float currentPrice;
    public static LiveMarketController Instance { get; private set; }

// 在 Start 上面加入 Awake
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }
    void Start()
    {
        // 初始化第一筆價格 (開盤價)
        currentPrice = basePrice;
        priceHistory.Add(currentPrice);
        
        // 畫出第一筆
        lineGraph.ShowGraph(priceHistory, basePrice, PlayerPortfolio.Instance != null ? PlayerPortfolio.Instance.averageCost : 0f);
    }

    void Update()
    {
        // 如果時間管理器存在，且現在不是營業時間，就直接暫停跳動！
        if (GameTimeManager.Instance != null && !GameTimeManager.Instance.isMarketOpen)
        {
            return; 
        }

        // 計時器：每 1 秒觸發一次
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            GenerateNewTick();
            timer = 0f;
        }
    }

    public void GenerateNewTick()
    {
        // 1. 先計算出原始的隨機價格
        float volatility = Random.Range(-0.01f, 0.01f);
        float rawPrice = currentPrice * (1 + volatility);
        
        // 2. 套用台灣股市真實的「升降單位」規則進行四捨五入
        currentPrice = ApplyTickSize(rawPrice);
        
        // 將新價格加入歷史清單
        priceHistory.Add(currentPrice);

        // 滑動視窗核心邏輯：如果超過 60 筆，就踢掉最舊的第 0 筆
        if (priceHistory.Count > lineGraph.maxVisiblePoints)
        {
            priceHistory.RemoveAt(0);
        }

        // 呼叫圖表重新繪製
        lineGraph.ShowGraph(priceHistory, basePrice, PlayerPortfolio.Instance != null ? PlayerPortfolio.Instance.averageCost : 0f);
        
        // 更新交割戶損益
        if (PlayerPortfolio.Instance != null)
        {
            PlayerPortfolio.Instance.UpdateUnrealizedProfit(currentPrice);
        }
    }

    // --- 新增：台股真實升降單位過濾器 ---
    private float ApplyTickSize(float price)
    {
        if (price >= 1000f) return Mathf.Round(price / 5f) * 5f;
        if (price >= 500f)  return Mathf.Round(price / 1f) * 1f;     // 500~999: 跳 1 元
        if (price >= 100f)  return Mathf.Round(price / 0.5f) * 0.5f; // 100~499: 跳 0.5 元
        if (price >= 50f)   return Mathf.Round(price / 0.1f) * 0.1f;
        if (price >= 10f)   return Mathf.Round(price / 0.05f) * 0.05f;
        
        return Mathf.Round(price / 0.01f) * 0.01f;
    }
    // 提供一個公開方法讓外部取得最新價格
    public float GetCurrentPrice() 
    {
        return currentPrice;
    }
}