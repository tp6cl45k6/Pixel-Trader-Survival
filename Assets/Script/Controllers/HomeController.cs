using UnityEngine;
using UnityEngine.UI;

public class HomeController : MonoBehaviour
{
    [Header("首頁打工按鈕 UI 綁定")]
    public Text txtHomeJobName; // 拖入首頁按鈕上用來顯示工作名稱 the Text

    [Header("防偷懶 / 邏輯鎖定按鈕")]
    public Button btnStockApp;    // 股市 App 按鈕
    public Button btnShopApp;     // 商店/買東西按鈕
    public Button btnSleep;       // 回家睡覺按鈕

    [Header("鎖定狀態提示 (可選)")]
    public Text txtStockBtnLabel; // 股市按鈕的文字 (例如用來顯示 "主管在盯")

    private void Update()
    {
        // 1. 實時同步首頁的職缺名稱與薪水顯示
        UpdateHomeJobText();

        // 2. 🔥【核心修正】：每幀主動檢查上班狀態，解決沒點工作頁面按鈕不消失的 BUG
        HandleGlobalWorkRestrictions();
    }

    // 負責更新首頁打工文字
    private void UpdateHomeJobText()
    {
        if (CareerManager.Instance != null && CareerManager.Instance.currentJob != null && txtHomeJobName != null)
        {
            var job = CareerManager.Instance.currentJob;
            if (job.jobId == "unemployed")
            {
                txtHomeJobName.text = "尋找打工";
            }
            else
            {
                txtHomeJobName.text = $"{job.jobTitle}\n(${job.dailySalary}/日)";
            }
        }
    }

    // 🔥 每幀主動控管大廳按鈕的可點擊狀態
    private void HandleGlobalWorkRestrictions()
    {
        if (CareerManager.Instance == null) return;

        // 直接向 CareerManager 詢問目前「是不是在上班時間內」
        bool currentlyOnDuty = CareerManager.Instance.isOnDuty;

        if (currentlyOnDuty)
        {
            // 🚫 只要在上班時間內，商店和睡覺按鈕「一律實時強制鎖死」！
            if (btnShopApp != null && btnShopApp.interactable) btnShopApp.interactable = false;
            if (btnSleep != null && btnSleep.interactable)     btnSleep.interactable = false;

            // 🚫 檢查上班摸魚時間是不是用光了
            if (CareerManager.Instance.currentMoyuTimeLeft <= 0f)
            {
                if (btnStockApp != null && btnStockApp.interactable)
                {
                    btnStockApp.interactable = false;
                    if (txtStockBtnLabel != null) txtStockBtnLabel.text = "股市 (主管在盯)";
                }
            }
            else
            {
                // 如果上班時間內還有摸魚額度，股市按鈕保持可以點擊
                if (btnStockApp != null && !btnStockApp.interactable)
                {
                    btnStockApp.interactable = true;
                    if (txtStockBtnLabel != null) txtStockBtnLabel.text = "股市看盤 (摸魚中)";
                }
            }
        }
        else
        {
            // 🔓 下班時間、或是失業狀態，無條件全部解鎖，恢復自由！
            if (btnShopApp != null && !btnShopApp.interactable) btnShopApp.interactable = true;
            if (btnSleep != null && !btnSleep.interactable)     btnSleep.interactable = true;
            if (btnStockApp != null && !btnStockApp.interactable) btnStockApp.interactable = true;
            
            if (txtStockBtnLabel != null && txtStockBtnLabel.text != "股市看盤")
            {
                txtStockBtnLabel.text = "股市看盤";
            }
        }
    }
}
