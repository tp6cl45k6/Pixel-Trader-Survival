using UnityEngine;

public class UIManager : MonoBehaviour
{
    // 設定為單例模式，讓未來如果有其他腳本想呼叫介面切換也很方便
    public static UIManager Instance { get; private set; }

    [Header("視窗面板綁定")]
    public GameObject homePanel;   // 主大廳 (全螢幕)
    public GameObject stockPanel;  // 股市 App (全螢幕)
    public GameObject shopPanel;   // 商城 App (彈出視窗)

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }

    private void Start()
    {
        // 遊戲一開始，強制初始化：只顯示主大廳，其他都關閉
        ReturnToHome();
    }

    // --- 視窗控制邏輯 ---

    /// <summary>
    /// 返回主大廳 (關閉所有全螢幕 App)
    /// </summary>
    public void ReturnToHome()
    {
        homePanel.SetActive(true);
        stockPanel.SetActive(false);
        shopPanel.SetActive(false);
        Debug.Log("📱 [系統] 返回桌面主畫面");
    }

    /// <summary>
    /// 開啟股市 App (會蓋掉主大廳)
    /// </summary>
    public void OpenStockApp()
    {
        homePanel.SetActive(false);
        stockPanel.SetActive(true);
        Debug.Log("📈 [系統] 開啟像素看盤軟體");
    }

    /// <summary>
    /// 開啟商城 App (彈出視窗，不關閉底下的畫面)
    /// </summary>
    public void OpenShopApp()
    {
        shopPanel.SetActive(true);
        Debug.Log("🛒 [系統] 開啟紓壓商城");
    }

    /// <summary>
    /// 關閉商城 App (隱藏彈出視窗)
    /// </summary>
    public void CloseShopApp()
    {
        shopPanel.SetActive(false);
        Debug.Log("🛒 [系統] 關閉紓壓商城");
    }
}
