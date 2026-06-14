using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    public static MarketManager Instance;

    [Header("資料庫 (從 JSON 載入)")]
    public List<StockData> stockDatabase = new List<StockData>();

    [Header("遊戲內運作的股票 (Runtime)")]
    public List<Stock> activeStocks = new List<Stock>();

    [Header("市場更新設定")]
    public float updateInterval = 2.0f; // 每幾秒更新一次股價
    private float timer = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 1. 讀取或生成 JSON
        InitializeStockDatabase();
        
        // 2. 把 JSON 資料轉換成遊戲裡可以跳動的實體股票
        GenerateRuntimeStocks();
    }

    private void Update()
    {
        // 只要不是暫停狀態，且股市正在營業，股價就會隨時間跳動
        if (GameTimeManager.Instance != null && GameTimeManager.Instance.timeScale > 0 && GameTimeManager.Instance.isMarketOpen)
        {
            timer += Time.deltaTime;
            if (timer >= updateInterval)
            {
                timer = 0f;
                UpdateAllStocks();
            }
        }
    }

    // 📖 讀取與生成 JSON 資料庫
    public void InitializeStockDatabase()
    {
        string folderPath = Application.streamingAssetsPath;
        string filePath = Path.Combine(folderPath, "StockDatabase.json");

        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            StockDatabaseRoot dbRoot = JsonUtility.FromJson<StockDatabaseRoot>(jsonContent);
            stockDatabase = dbRoot.stocks;
            Debug.Log($"📈 [股市系統] 成功從 JSON 讀取了 {stockDatabase.Count} 檔股票設定！");
        }
        else
        {
            Debug.Log("📈 [股市系統] 找不到股票 JSON，開始自動產生預設檔案...");

            stockDatabase = new List<StockData>
            {
                new StockData { stockId = "2330", stockName = "護國神山 (TSMC)", category = "半導體", basePrice = 850.0f, volatility = 0.015f, trendBias = 0.002f, description = "台股的心臟，外資的提款機。" },
                new StockData { stockId = "0056", stockName = "元老高股息 ETF", category = "ETF", basePrice = 38.5f, volatility = 0.005f, trendBias = 0.001f, description = "散戶最愛，穩定配息，波動極小。" },
                new StockData { stockId = "00919", stockName = "群益精選高息", category = "ETF", basePrice = 25.0f, volatility = 0.008f, trendBias = 0.0015f, description = "近期熱門的高股息新星。" },
                new StockData { stockId = "2603", stockName = "長榮海運", category = "航運", basePrice = 200.0f, volatility = 0.045f, trendBias = -0.001f, description = "航海王！波動巨大，一天天堂一天地獄。" },
                new StockData { stockId = "2454", stockName = "發哥 (聯發科)", category = "IC設計", basePrice = 1150.0f, volatility = 0.025f, trendBias = 0.001f, description = "台灣 IC 設計龍頭，跳動一檔就是幾千塊上下。" }
            };

            StockDatabaseRoot dbRoot = new StockDatabaseRoot { stocks = this.stockDatabase };
            string jsonOutput = JsonUtility.ToJson(dbRoot, true);

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            File.WriteAllText(filePath, jsonOutput);
            Debug.Log($"💾 [股市系統] 股票 JSON 已自動匯出至: {filePath}");
        }
    }

    // 🔨 把 JSON 資料實體化為 Runtime Stock
    private void GenerateRuntimeStocks()
    {
        activeStocks.Clear();
        foreach (var data in stockDatabase)
        {
            Stock newStock = new Stock(data);
            activeStocks.Add(newStock);
        }
        Debug.Log($"📊 [股市系統] 已成功啟動 {activeStocks.Count} 檔股票！");

        // 🔥 通知看盤 App UI：「資料好了，請建立下拉選單！」
        if (LiveMarketController.Instance != null)
        {
            LiveMarketController.Instance.PopulateStockListUI(); 
        }
    }

    // 🔄 觸發所有股票更新價格
    public void UpdateAllStocks()
    {
        foreach (var stock in activeStocks)
        {
            stock.UpdatePrice(); // 讓股票自己去跳動

            // 通知交割戶計算最新的未實現損益 (傳入 baseData 給它辨識)
            if (PlayerPortfolio.Instance != null)
            {
                PlayerPortfolio.Instance.UpdateUnrealizedProfit(stock.baseData, stock.currentPrice);
            }
        }

        // 🔥 通知畫面更新折線圖
        if (LiveMarketController.Instance != null)
        {
            LiveMarketController.Instance.RefreshGraph();
        }
    }

    // 換日重置所有股票盤勢
    public void ResetMarketForNewDay()
    {
        foreach (var stock in activeStocks)
        {
            stock.ResetForNewDay();
        }
        // 通知 UI 刷新
        if (LiveMarketController.Instance != null)
        {
            LiveMarketController.Instance.RefreshGraph();
        }
        Debug.Log("📈 [股市] 全市場換日成功，已刷新今日漲跌停基準價。");
    }
}