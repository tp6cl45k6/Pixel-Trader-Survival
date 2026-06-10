using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPortfolio : MonoBehaviour
{
    public static PlayerPortfolio Instance { get; private set; }

    // 用來儲存單一標的持股狀況的內嵌資料結構
    [System.Serializable]
    public class StockPosition
    {
        public int sharesOwned = 0;
        public float averageCost = 0f;
    }

    [Header("資產狀況")]
    public float cash = 100000f;       // 初始本金 10 萬
    
    // 核心資料庫：每檔 StockData 對應一個獨立的庫存與成本
    private Dictionary<StockData, StockPosition> portfolio = new Dictionary<StockData, StockPosition>();

    [Header("UI 綁定")]
    public Text cashText;              
    public Text sharesText;            
    public Text profitLossText;        

    [Header("特效設定")]
    public GameObject floatingTextPrefab;  
    public Transform buyButtonTransform;   
    public Transform sellButtonTransform;  

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }

    private void Start()
    {
        UpdateUI(); 
    }

    // 安全取得某一檔股票持股狀態的方法
    private StockPosition GetPosition(StockData stock)
    {
        if (!portfolio.ContainsKey(stock))
        {
            portfolio[stock] = new StockPosition();
        }
        return portfolio[stock];
    }

    public float GetAverageCost(StockData stock) => GetPosition(stock).averageCost;
    public int GetSharesOwned(StockData stock) => GetPosition(stock).sharesOwned;

    // 市價買進 1000 股 (一張)
    public void BuyStock()
    {
        // 詢問市場目前畫面上選中的是哪一檔股票
        StockData currentStock = LiveMarketController.Instance.GetCurrentSelectedStock();
        if (currentStock == null) return;

        float currentPrice = LiveMarketController.Instance.GetCurrentPrice(currentStock);
        float totalCost = currentPrice * 1000f;

        if (cash >= totalCost)
        {
            StockPosition pos = GetPosition(currentStock);
            float oldTotalCost = pos.sharesOwned * pos.averageCost;
            pos.sharesOwned += 1000;
            pos.averageCost = (oldTotalCost + totalCost) / pos.sharesOwned;

            cash -= totalCost;
            Debug.Log($"[買進成功] 買入 {currentStock.stockName}, 價格: {currentPrice:0.0}, 現金: {cash:0}");
            UpdateUI();

            ShowFloatingText($"-{totalCost:0}", Color.green, buyButtonTransform);
        }
        else
        {
            Debug.LogWarning("[餘額不足] 買不起啦！");
        }
    }

    public void SellStock()
    {
        StockData currentStock = LiveMarketController.Instance.GetCurrentSelectedStock();
        if (currentStock == null) return;

        float currentPrice = LiveMarketController.Instance.GetCurrentPrice(currentStock);
        StockPosition pos = GetPosition(currentStock);

        if (pos.sharesOwned >= 1000)
        {
            float totalRevenue = currentPrice * 1000f;
            
            pos.sharesOwned -= 1000;
            if (pos.sharesOwned == 0) pos.averageCost = 0f;

            cash += totalRevenue;
            Debug.Log($"[賣出成功] 賣出 {currentStock.stockName}, 價格: {currentPrice:0.0}, 現金: {cash:0}");
            UpdateUI();

            ShowFloatingText($"+{totalRevenue:0}", Color.red, sellButtonTransform);
        }
        else
        {
            Debug.LogWarning("[庫存不足] 你沒有這檔股票可以賣了！");
        }
    }

    // 外部市場引擎跳動時，會持續推播最新價格進來
    public void UpdateUnrealizedProfit(StockData stock, float currentPrice)
    {
        // 只有當前正在觀看的股票，才去刷新 UI 面板上的損益文字
        StockData currentStock = LiveMarketController.Instance.GetCurrentSelectedStock();
        if (stock != currentStock) return;

        StockPosition pos = GetPosition(stock);
        if (pos.sharesOwned == 0)
        {
            if (profitLossText != null) profitLossText.text = "未實現損益: $0";
            return;
        }

        float unrealizedProfit = (currentPrice - pos.averageCost) * pos.sharesOwned;
        if (profitLossText != null)
        {
            profitLossText.text = $"未實現損益: ${unrealizedProfit:0}";
            profitLossText.color = unrealizedProfit >= 0 ? Color.red : Color.green; 
        }
    }

    public void UpdateUI()
    {
        if (cashText != null) cashText.text = $"交割戶餘額: ${cash:0}";
        
        StockData currentStock = LiveMarketController.Instance != null ? LiveMarketController.Instance.GetCurrentSelectedStock() : null;
        if (currentStock != null && sharesText != null)
        {
            StockPosition pos = GetPosition(currentStock);
            sharesText.text = $"{currentStock.stockName} 持有: {pos.sharesOwned / 1000} 張 (均價: {pos.averageCost:0.0})";
        }
    }

    private void ShowFloatingText(string message, Color color, Transform targetTransform)
    {
        if (floatingTextPrefab == null || targetTransform == null) return;
        GameObject floatingObj = Instantiate(floatingTextPrefab, targetTransform.position, Quaternion.identity, targetTransform.parent);
        Text floatingText = floatingObj.GetComponent<Text>();
        if (floatingText != null)
        {
            floatingText.text = message;
            floatingText.color = color;
        }
    }
}