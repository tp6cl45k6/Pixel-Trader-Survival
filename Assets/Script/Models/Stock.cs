using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stock
{
    public string id;
    public string name;
    public float currentPrice;
    public float dailyReferencePrice; // 昨收價 (用來算 10% 漲跌停)
    public List<float> priceHistory = new List<float>();

    public StockData baseData; // 來自 JSON 的基礎設定

    public Stock(StockData data)
    {
        this.baseData = data;
        this.id = data.stockId;
        this.name = data.stockName;
        
        this.currentPrice = data.basePrice;
        this.dailyReferencePrice = data.basePrice; 
        
        this.priceHistory.Add(this.currentPrice);
    }

    // 核心：自己計算股價跳動與漲跌停限制
    public void UpdatePrice()
    {
        float volatility = Random.Range(-baseData.volatility, baseData.volatility);
        float priceChangePercent = volatility + baseData.trendBias;

        float rawPrice = currentPrice * (1f + priceChangePercent);
        float newPrice = ApplyTickSize(rawPrice);

        // 台股 ±10% 限制
        float maxLimit = ApplyTickSize(dailyReferencePrice * 1.1f);
        float minLimit = ApplyTickSize(dailyReferencePrice * 0.9f);
        
        // 把股價硬限制在區間內
        currentPrice = Mathf.Clamp(newPrice, minLimit, maxLimit);

        priceHistory.Add(currentPrice);

        // 限制圖表點數，避免吃光記憶體 (例如只留 100 根)
        if (priceHistory.Count > 100)
        {
            priceHistory.RemoveAt(0);
        }
    }

    // 換日重置
    public void ResetForNewDay()
    {
        dailyReferencePrice = currentPrice; // 昨收變今開
        priceHistory.Clear();
        priceHistory.Add(currentPrice);
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
}