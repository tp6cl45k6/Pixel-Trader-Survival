using System.Collections.Generic;
using UnityEngine;

// 1. 單檔股票的基礎設定資料
[System.Serializable]
public class StockData
{
    public string stockId;          // 股票代號 (例: "2330")
    public string stockName;        // 股票名稱 (例: "護國神山")
    public string category;         // 分類 (例: "半導體", "高股息ETF", "航運")
    
    [Header("數值設定")]
    public float basePrice;         // 遊戲開局初始股價
    public float volatility;        // 波動度 (影響每次跳動的幅度，例如 0.02 代表 2% 波動)
    public float trendBias;         // 趨勢偏好 (大於0易漲，小於0易跌，0為純隨機)
    
    [TextArea]
    public string description;      // 股票描述
}

// 2. JSON 專用根目錄包裝器
[System.Serializable]
public class StockDatabaseRoot
{
    public List<StockData> stocks = new List<StockData>();
}
