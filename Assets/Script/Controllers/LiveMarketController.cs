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

    void Start()
    {
        // 初始化第一筆價格 (開盤價)
        currentPrice = basePrice;
        priceHistory.Add(currentPrice);
        
        // 畫出第一筆
        lineGraph.ShowGraph(priceHistory, basePrice);
    }

    void Update()
    {
        // 計時器：每 1 秒觸發一次
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            GenerateNewTick();
            timer = 0f;
        }
    }

    private void GenerateNewTick()
    {
        // 模擬股價跳動：隨機上下震盪 1% (真實感來源：基於上一次的價格跳動)
        float volatility = Random.Range(-0.01f, 0.01f);
        currentPrice = currentPrice * (1 + volatility);
        
        // 將新價格加入歷史清單
        priceHistory.Add(currentPrice);

        // 【滑動視窗核心邏輯】：如果超過 60 筆，就踢掉最舊的第 0 筆
        if (priceHistory.Count > lineGraph.maxVisiblePoints)
        {
            priceHistory.RemoveAt(0);
        }

        // 呼叫圖表重新繪製
        lineGraph.ShowGraph(priceHistory, basePrice);
    }
}