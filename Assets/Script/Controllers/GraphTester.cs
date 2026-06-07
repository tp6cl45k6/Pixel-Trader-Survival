using System.Collections.Generic;
using UnityEngine;

public class GraphTester : MonoBehaviour
{
    public UILineGraph lineGraph;
    
    void Start()
    {
        // 假設這支股票昨天收盤價是 850 元
        float openPrice = 850f; 
        
        // 假資料區間現在落在 850 上下，視覺上會有非常明顯的波動！
        List<float> mockPrices = new List<float>() { 850, 865, 840, 870, 900, 885, 920 };
        
        // 傳入價格清單與平盤價
        lineGraph.ShowGraph(mockPrices, openPrice);
    }
}