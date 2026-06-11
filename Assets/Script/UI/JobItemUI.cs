using UnityEngine;
using UnityEngine.UI;

public class JobItemUI : MonoBehaviour
{
    [Header("UI 綁定元件")]
    public Text txtTitle;
    public Text txtSalary;
    public Text txtAcceptanceRate;

    private JobPosition myJobData;

    // 接收資料並更新文字
    public void Setup(JobPosition jobData)
    {
        myJobData = jobData;
        
        if (txtTitle != null) txtTitle.text = jobData.jobTitle;
        if (txtSalary != null) txtSalary.text = $"日薪: ${jobData.dailySalary}";
        
        // 【修改這裡】：向 CareerManager 詢問這份工作的錄取率
        if (txtAcceptanceRate != null && CareerManager.Instance != null) 
        {
            float rate = CareerManager.Instance.CalculateAcceptanceRate(jobData);
            txtAcceptanceRate.text = $"錄取率: {rate:0}%";
            
            // 貼心小設計：機率低於 50% 顯示紅色，高於 50% 顯示綠色
            txtAcceptanceRate.color = rate >= 50f ? Color.green : Color.red;
        }
    }

    // 當玩家點擊這個職缺的應徵按鈕時觸發
    public void OnApplyClicked()
    {
        if (CareerManager.Instance != null)
        {
            CareerManager.Instance.ApplyForJob(myJobData);

            // 【新增】：面試完後，通知畫面的總管更新固定文字與重新計算所有列表的錄取率
            CareerUIController uiController = FindObjectOfType<CareerUIController>();
            if (uiController != null)
            {
                uiController.UpdateStaticJobUI(); // 重新整理左側卡片
                uiController.PopulateJobBoard();  // 重新整理右側列表 (因為有了工作經驗，別的工作錄取率可能會變)
            }
        }
    }
}
