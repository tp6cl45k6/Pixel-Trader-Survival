using System.Collections.Generic;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    // 單例模式 (Singleton) 方便其他腳本隨時呼叫 MarketManager.Instance
    public static MarketManager Instance { get; private set; }

    // 使用 Dictionary 來儲存市場上的所有股票，方便用代碼 (Symbol) 快速查找
    private Dictionary<string, Stock> marketStocks = new Dictionary<string, Stock>();

    [Header("股市跳動設定")]
    [Tooltip("每隔幾秒鐘跳動一次價格")]
    public float tickInterval = 2.0f; 
    private float timer = 0f;

    private void Awake()
    {
        // 單例模式防呆機制
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // 切換場景時不銷毀
        
        InitializeMarket();
    }

    private void InitializeMarket()
    {
        // 初始化 2026 年市場標的與合理開盤價
        marketStocks.Add("2330", new Stock("2330", "護國神山", 860.0f));
        marketStocks.Add("00919", new Stock("00919", "群英高股息", 26.5f)); 
        marketStocks.Add("00999A", new Stock("00999A", "未來趨勢主動型", 15.2f));

        Debug.Log("股市引擎初始化完成。");
    }

    private void Update()
    {
        // 計時器：達到 tickInterval 指定的時間就執行一次價格跳動
        timer += Time.deltaTime;
        if (timer >= tickInterval)
        {
            GenerateMarketTicks();
            timer = 0f;
        }
    }

    private void GenerateMarketTicks()
    {
        foreach (var stock in marketStocks.Values)
        {
            // 模擬股價波動：這裡先用簡單的隨機漫步 (Random Walk)
            // 實際開發後期可以加入趨勢演算法或新聞事件的權重
            float volatility = UnityEngine.Random.Range(-0.015f, 0.015f); // 上下 1.5% 震幅
            float newPrice = stock.CurrentPrice * (1 + volatility);
            
            // 更新股票模型內的價格
            stock.UpdatePrice(newPrice);
            
            // 將跳動結果印在 Console 以便我們初期測試
            Debug.Log($"[Ticker] {stock.Name} ({stock.Symbol}): {stock.CurrentPrice}");
        }
    }

    // 提供給外部腳本（如玩家下單、AI 腳本）取得特定股票物件的方法
    public Stock GetStock(string symbol)
    {
        if (marketStocks.TryGetValue(symbol, out Stock targetStock))
        {
            return targetStock;
        }
        Debug.LogWarning($"市場上找不到代碼為 {symbol} 的股票！");
        return null;
    }
}