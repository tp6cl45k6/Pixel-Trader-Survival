using System;

public class Stock
{
    public string Symbol { get; private set; }
    public string Name { get; private set; }
    public float CurrentPrice { get; private set; }
    public float OpenPrice { get; private set; }
    
    // 定義價格更新的事件委派，訂閱者（如 UI 或自動化腳本）會監聽這個事件
    public event Action<string, float> OnPriceUpdated;

    public Stock(string symbol, string name, float startPrice)
    {
        Symbol = symbol;
        Name = name;
        CurrentPrice = startPrice;
        OpenPrice = startPrice;
    }

    // 接收來自 MarketManager 的價格跳動，並廣播給所有訂閱者
    public void UpdatePrice(float newPrice)
    {
        CurrentPrice = (float)Math.Round(newPrice, 2); // 確保價格符合真實市場的小數位數
        
        // 觸發廣播：如果有人訂閱，就通知他們最新價格
        OnPriceUpdated?.Invoke(Symbol, CurrentPrice);
    }
}