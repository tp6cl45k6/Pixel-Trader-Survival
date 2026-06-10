using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 確保有引入 UI 命名空間

public class LiveMarketController : MonoBehaviour
{
    public static LiveMarketController Instance { get; private set; }

    [Header("UI 綁定")]
    public UILineGraph lineGraph;
    public Dropdown stockDropdown; // 新增：畫面的下拉選單組件

    [Header("可交易股票庫 (拖入 ScriptableObjects)")]
    public List<StockData> availableStocks = new List<StockData>();

    [Header("市場設定")]
    public float updateInterval = 1f;  // 每幾秒更新一次

    // 核心資料庫：個別儲存每檔股票的當前價格、走勢紀錄以及今日開盤的基準價（昨收價）
    private Dictionary<StockData, List<float>> stockHistories = new Dictionary<StockData, List<float>>();
    private Dictionary<StockData, float> currentPrices = new Dictionary<StockData, float>();
    private Dictionary<StockData, float> dailyReferencePrices = new Dictionary<StockData, float>();

    private StockData currentSelectedStock;
    private float timer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }

    void Start()
    {
        InitializeMarket();
        SetupDropdown();
        
        // 預設選取第一檔股票開盤
        if (availableStocks.Count > 0)
        {
            SelectStock(availableStocks[0]);
        }
    }

    // 初始化所有股票的開盤資料
    private void InitializeMarket()
    {
        stockHistories.Clear();
        currentPrices.Clear();
        dailyReferencePrices.Clear();

        foreach (var stock in availableStocks)
        {
            currentPrices[stock] = stock.basePrice;
            dailyReferencePrices[stock] = stock.basePrice; // 第一天的基準價就是初始價
            stockHistories[stock] = new List<float> { stock.basePrice };
        }
    }

    // 將 ScriptableObject 的股票名稱自動塞入 Dropdown 選項中
    private void SetupDropdown()
    {
        if (stockDropdown == null) return;

        // 首字母改為大寫 C
        stockDropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (var stock in availableStocks)
        {
            options.Add($"{stock.stockCode} {stock.stockName}");
        }

        stockDropdown.AddOptions(options);
        // 監聽下拉選單點擊事件
        stockDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    void Update()
    {
        // 時間攔截：非營業時間股市凍結
        if (GameTimeManager.Instance != null && !GameTimeManager.Instance.isMarketOpen) return;

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            GenerateNewTickForAllStocks();
            timer = 0f;
        }
    }

    // 🔥 關鍵優化：在背景同時推演「所有股票」的實時價格，並套用漲跌停限制
    public void GenerateNewTickForAllStocks()
    {
        foreach (var stock in availableStocks)
        {
            float currentPrice = currentPrices[stock];
            // 讀取每檔股票自定義的波動度
            float volatility = Random.Range(-stock.volatility, stock.volatility);
            float rawPrice = currentPrice * (1 + volatility);
            
            float newPrice = ApplyTickSize(rawPrice);

            // 🔥【新增】台股無情 ±10% 限制
            if (dailyReferencePrices.TryGetValue(stock, out float refPrice))
            {
                // 計算並套用升降單位四捨五入後的極限價格
                float maxLimit = ApplyTickSize(refPrice * 1.1f); // 漲停板
                float minLimit = ApplyTickSize(refPrice * 0.9f); // 跌停板
                
                // 強制把價格鎖在台灣法規區間內
                newPrice = Mathf.Clamp(newPrice, minLimit, maxLimit);
            }

            currentPrices[stock] = newPrice;
            stockHistories[stock].Add(newPrice);

            // 超過最大顯示點數就移除歷史第 0 筆
            if (stockHistories[stock].Count > lineGraph.maxVisiblePoints)
            {
                stockHistories[stock].RemoveAt(0);
            }

            // 通知交割戶計算每檔股票最新的未實現損益
            if (PlayerPortfolio.Instance != null)
            {
                PlayerPortfolio.Instance.UpdateUnrealizedProfit(stock, newPrice);
            }
        }

        // 全市場同步跳動完後，刷新目前玩家正在看的圖表
        RefreshGraph();
    }

    // 切換目前畫面上正在觀察的股票
    public void SelectStock(StockData stock)
    {
        currentSelectedStock = stock;
        RefreshGraph();
        
        // 切換股票時，同步刷一下庫存文字顯示
        if (PlayerPortfolio.Instance != null)
        {
            PlayerPortfolio.Instance.UpdateUI();
        }
    }

    private void RefreshGraph()
    {
        if (currentSelectedStock == null || lineGraph == null) return;

        List<float> history = stockHistories[currentSelectedStock];
        
        // 取得今日平盤價 (昨日收盤價)
        float basePrice = dailyReferencePrices.TryGetValue(currentSelectedStock, out float openPrice) ? openPrice : currentSelectedStock.basePrice;
        
        // 向 Portfolio 索取這檔股票的自定義均價成本
        float avgCost = PlayerPortfolio.Instance != null ? PlayerPortfolio.Instance.GetAverageCost(currentSelectedStock) : 0f;

        lineGraph.ShowGraph(history, basePrice, avgCost);
    }

    private void OnDropdownChanged(int index)
    {
        if (index >= 0 && index < availableStocks.Count)
        {
            SelectStock(availableStocks[index]);
        }
    }

    // 換日重置所有股票盤勢
    public void ResetMarketForNewDay()
    {
        timer = 0f;
        foreach (var stock in availableStocks)
        {
            float lastClosePrice = currentPrices[stock];
            
            // 昨天的收盤價，變成今天計算漲跌停的基準價！
            dailyReferencePrices[stock] = lastClosePrice; 
            
            stockHistories[stock].Clear();
            stockHistories[stock].Add(lastClosePrice); // 昨收變今開
        }
        RefreshGraph();
        Debug.Log("📈 [股市] 全市場換日成功，已刷新今日漲跌停基準價。");
    }

    private float ApplyTickSize(float price)
    {
        if (price >= 1000f) return Mathf.Round(price / 5f) * 5f;
        if (price >= 500f)  return Mathf.Round(price / 1f) * 1f;     
        if (price >= 100f)  return Mathf.Round(price / 0.5f) * 0.5f; 
        if (price >= 50f)   return Mathf.Round(price / 0.1f) * 0.1f;
        if (price >= 10f)   return Mathf.Round(price / 0.05f) * 0.05f;
        return Mathf.Round(price / 0.01f) * 0.01f;
    }

    public float GetCurrentPrice(StockData stock) 
    {
        if (currentPrices.TryGetValue(stock, out float price)) return price;
        return stock.basePrice;
    }

    public StockData GetCurrentSelectedStock()
    {
        return currentSelectedStock;
    }
}