using UnityEngine;
using UnityEngine.UI;

public class JobItemUI : MonoBehaviour
{
    [Header("UI 綁定元件")]
    public Text txtTitle;
    public Text txtSalary;
    public Text txtAcceptanceRate;
    public Button btnApply; 

    private JobPosition myJobData;

    public void Setup(JobPosition jobData)
    {
        myJobData = jobData;
        
        if (txtTitle != null) txtTitle.text = jobData.jobTitle;
        if (txtSalary != null) txtSalary.text = $"日薪: ${jobData.dailySalary}";
        
        // 注意：這裡移除了按鈕開關的邏輯，因為全部交給 Update() 實時控管了！
    }

    // 🔥【新增】每幀檢查時間、壓力、面試次數，動態鎖定按鈕！
    private void Update()
    {
        if (myJobData == null || CareerManager.Instance == null || GameTimeManager.Instance == null) return;

        // 1. 如果根本還沒解鎖
        if (!myJobData.isUnlocked)
        {
            if (btnApply != null) btnApply.interactable = false;
            if (txtAcceptanceRate != null)
            {
                txtAcceptanceRate.text = "🔒 考績達100分解鎖";
                txtAcceptanceRate.color = Color.gray;
            }
            return;
        }

        // 2. 實時取得當前狀態
        float hour = GameTimeManager.Instance.currentHour;
        bool isTimeValid = (hour >= 10f && hour < 15f);
        bool isStressValid = (CareerManager.Instance.currentStress <= CareerManager.Instance.maxStressForInterview);
        bool hasNotInterviewed = !CareerManager.Instance.interviewedJobsToday.Contains(myJobData.jobId);

        // 三個條件都滿足才能點擊
        bool canInterview = isTimeValid && isStressValid && hasNotInterviewed;
        if (btnApply != null) btnApply.interactable = canInterview;

        // 3. 根據無法點擊的原因，實時替換文字提示
        if (txtAcceptanceRate != null)
        {
            if (!isTimeValid)
            {
                txtAcceptanceRate.text = "🔒 限 10:00-15:00";
                txtAcceptanceRate.color = Color.gray;
            }
            else if (!isStressValid)
            {
                txtAcceptanceRate.text = "🔒 壓力過大(需休息)";
                txtAcceptanceRate.color = Color.red; // 壓力過大用紅色警告
            }
            else if (!hasNotInterviewed)
            {
                txtAcceptanceRate.text = "🔒 今日已面試";
                txtAcceptanceRate.color = Color.gray;
            }
            else
            {
                // 條件全部通過，顯示正常的錄取機率
                float rate = CareerManager.Instance.CalculateAcceptanceRate(myJobData);
                txtAcceptanceRate.text = $"錄取率: {rate:0}%";
                txtAcceptanceRate.color = rate >= 50f ? Color.green : Color.red;
            }
        }
    }

    public void OnApplyClicked()
    {
        if (CareerManager.Instance != null && myJobData.isUnlocked) 
        {
            CareerManager.Instance.ApplyForJob(myJobData);

            CareerUIController uiController = FindObjectOfType<CareerUIController>();
            if (uiController != null)
            {
                uiController.UpdateStaticJobUI(); 
                // 不強制 Populate，因為 Update() 會自動把剛剛按過的按鈕反灰
            }
        }
    }
}
