using UnityEngine;
using UnityEngine.UI;

public class PlayerPortfolio : MonoBehaviour
{
    public static PlayerPortfolio Instance { get; private set; }

    [Header("資產狀況")]
    public float cash = 100000f;       // 初始本金 10 萬
    public int sharesOwned = 0;        // 目前持有股數 (假設初期只能買賣一檔股票)
    public float averageCost = 0f;     // 平均成本

    [Header("UI 綁定 (可選，等下再拉)")]
    public Text cashText;              // 顯示現金
    public Text sharesText;            // 顯示持股
    public Text profitLossText;        // 顯示未實現損益

    [Header("特效設定")]
    public GameObject floatingTextPrefab;  // 漂浮文字的預製體
    public Transform buyButtonTransform;   // 買進按鈕的位置
    public Transform sellButtonTransform;  // 賣出按鈕的位置

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }

    private void Start()
    {
        // 遊戲一開局就強制更新一次 UI，讓十萬本金顯示在畫面上
        UpdateUI(); 
    }

    // 市價買進 1000 股 (一張)
    public void BuyStock()
    {
        // 直接向市場中心請求最新價格
        float currentPrice = LiveMarketController.Instance.GetCurrentPrice();
        float totalCost = currentPrice * 1000f;

        if (cash >= totalCost)
        {
            float oldTotalCost = sharesOwned * averageCost;
            sharesOwned += 1000;
            averageCost = (oldTotalCost + totalCost) / sharesOwned;

            cash -= totalCost;
            Debug.Log($"[買進成功] 買入價格: {currentPrice:0.0}, 總花費: {totalCost:0}, 剩餘現金: {cash:0}");
            UpdateUI();

            // 觸發特效：扣錢顯示綠色
            ShowFloatingText($"-{totalCost:0}", Color.green, buyButtonTransform);
        }
        else
        {
            Debug.LogWarning("[餘額不足] 買不起啦！");
        }
    }

    public void SellStock()
    {
        float currentPrice = LiveMarketController.Instance.GetCurrentPrice();

        if (sharesOwned >= 1000)
        {
            float totalRevenue = currentPrice * 1000f;
            float profit = (currentPrice - averageCost) * 1000f;
            
            sharesOwned -= 1000;
            if (sharesOwned == 0) averageCost = 0f;

            cash += totalRevenue;
            Debug.Log($"[賣出成功] 賣出價格: {currentPrice:0.0}, 本次獲利: {profit:0}, 現金: {cash:0}");
            UpdateUI();

            // 觸發特效：賺錢顯示紅色
            ShowFloatingText($"+{totalRevenue:0}", Color.red, sellButtonTransform);
        }
        else
        {
            Debug.LogWarning("[庫存不足] 你沒有股票可以賣了！");
        }
    }

    // 每次股價跳動或交易時呼叫，更新介面數字
    public void UpdateUnrealizedProfit(float currentPrice)
    {
        if (sharesOwned == 0) return;

        float unrealizedProfit = (currentPrice - averageCost) * sharesOwned;
        if (profitLossText != null)
        {
            profitLossText.text = $"未實現損益: {unrealizedProfit:0}";
            profitLossText.color = unrealizedProfit >= 0 ? Color.red : Color.green; // 台灣股市：紅賺綠賠
        }
    }

    public void UpdateUI()
    {
        if (cashText != null) cashText.text = $"交割戶餘額: ${cash:0}";
        if (sharesText != null) sharesText.text = $"持有股數: {sharesOwned} (均價: {averageCost:0.0})";
    }

    // 生成漂浮文字的核心方法
    private void ShowFloatingText(string message, Color color, Transform targetTransform)
    {
        if (floatingTextPrefab == null || targetTransform == null) return;

        // 在按鈕的位置生成文字，並將父物件設定為 Canvas (這樣 UI 才看得到)
        GameObject floatingObj = Instantiate(floatingTextPrefab, targetTransform.position, Quaternion.identity, targetTransform.parent);
        
        Text floatingText = floatingObj.GetComponent<Text>();
        if (floatingText != null)
        {
            floatingText.text = message;
            floatingText.color = color;
            floatingText.fontSize = 24;
            floatingText.fontStyle = FontStyle.Bold;
        }
    }
}