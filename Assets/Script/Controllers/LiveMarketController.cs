using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiveMarketController : MonoBehaviour
{
    public static LiveMarketController Instance { get; private set; }

    [Header("UI 綁定")]
    public UILineGraph lineGraph;
    public Dropdown stockDropdown;

    // 現在直接觀察實體股票 (Stock)，而不是 JSON 資料設定 (StockData)
    private Stock currentSelectedStock;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }

    // 由 MarketManager 生成完股票後，主動呼叫此方法建立 UI
    public void PopulateStockListUI()
    {
        if (MarketManager.Instance == null || MarketManager.Instance.activeStocks.Count == 0) return;

        SetupDropdown();

        // 預設選取第一檔股票
        SelectStock(MarketManager.Instance.activeStocks[0]);
    }

    private void SetupDropdown()
    {
        if (stockDropdown == null) return;

        stockDropdown.ClearOptions();
        List<string> options = new List<string>();

        // 從 MarketManager 抓取正在跳動的實體股票名單
        foreach (var stock in MarketManager.Instance.activeStocks)
        {
            options.Add($"{stock.id} {stock.name}");
        }

        stockDropdown.AddOptions(options);
        
        // 重新綁定事件
        stockDropdown.onValueChanged.RemoveAllListeners(); 
        stockDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    public void SelectStock(Stock stock)
    {
        currentSelectedStock = stock;
        RefreshGraph();

        if (PlayerPortfolio.Instance != null)
        {
            PlayerPortfolio.Instance.UpdateUI();
        }
    }

    // 當 MarketManager 時間一到，就會呼叫這裡重繪圖表
    public void RefreshGraph()
    {
        if (currentSelectedStock == null || lineGraph == null) return;

        // 直接向股票實體拿取歷史軌跡與平盤價
        List<float> history = currentSelectedStock.priceHistory;
        float basePrice = currentSelectedStock.dailyReferencePrice; 
        
        // 向交割戶拿這檔股票的平均成本 (注意：PlayerPortfolio 是用 baseData 認人的)
        float avgCost = PlayerPortfolio.Instance != null ? PlayerPortfolio.Instance.GetAverageCost(currentSelectedStock.baseData) : 0f;

        lineGraph.ShowGraph(history, basePrice, avgCost);
    }

    private void OnDropdownChanged(int index)
    {
        if (MarketManager.Instance != null && index >= 0 && index < MarketManager.Instance.activeStocks.Count)
        {
            SelectStock(MarketManager.Instance.activeStocks[index]);
        }
    }

    public Stock GetCurrentSelectedStock()
    {
        return currentSelectedStock;
    }
}