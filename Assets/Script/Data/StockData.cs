using UnityEngine;

// 這個標籤讓你在 Unity 專案區按右鍵可以自動產生這個檔案
[CreateAssetMenu(fileName = "NewStock", menuName = "PixelTrader/Stock Data")]
public class StockData : ScriptableObject
{
    public string stockCode = "00919";       // 股票代號
    public string stockName = "群益台灣精選高息"; // 股票名稱
    public float basePrice = 25.0f;          // 初始開盤價
    public float volatility = 0.005f;        // 波動度 (大盤股波動大，ETF波動小)
}
