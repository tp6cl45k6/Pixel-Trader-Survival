using UnityEngine;
using UnityEngine.UI;

public class CareerUIController : MonoBehaviour
{
    [Header("右側佈告欄設定")]
    public Transform jobBoardContent; 
    public GameObject jobItemPrefab;  

    [Header("左側當前狀態 UI 綁定")]
    public Text txtCurrentJobTitle;       
    public Text txtCurrentSalary;         
    public Text txtKpiDescription;        
    public Text txtMoyuTimerLabel;        
    public Slider sldMoyuTimer;           
    public Text txtWorkTimeRange;         
    
    // 🔥【今日新增】綁定藍色考績條
    public Slider sldPerformance;           // 顯示當前職位考績進度條
    public Text txtPerformanceLabel;        // 顯示考績數字 (例如: 考績 35/100)

    private void Start()
    {
        PopulateJobBoard();
        UpdateStaticJobUI();
    }

    private void Update()
    {
        if (CareerManager.Instance != null && CareerManager.Instance.currentJob != null)
        {
            JobPosition currentJob = CareerManager.Instance.currentJob;

            // 1. 更新摸魚計時器與進度條... (這段維持昨天的邏輯)
            if (txtMoyuTimerLabel != null)
            {
                if (currentJob.jobId == "unemployed")
                    txtMoyuTimerLabel.text = "全職看盤中 (時間無限)";
                else
                    txtMoyuTimerLabel.text = CareerManager.Instance.isOnDuty 
                        ? $"上班中！摸魚剩餘: {Mathf.CeilToInt(CareerManager.Instance.currentMoyuTimeLeft)} 分鐘"
                        : $"非上班時間 (今日摸魚額度: {currentJob.totalMoyuMinutes} 分)";
            }

            if (sldMoyuTimer != null)
            {
                if (currentJob.jobId == "unemployed") sldMoyuTimer.value = 1f; 
                else sldMoyuTimer.value = CareerManager.Instance.currentMoyuTimeLeft / currentJob.totalMoyuMinutes;
            }

            // 🔥【今日新增】2. 更新考績進度條 Slider
            if (sldPerformance != null)
            {
                if (currentJob.jobId == "unemployed")
                {
                    sldPerformance.value = 0f; 
                    if (txtPerformanceLabel != null) txtPerformanceLabel.text = "失業中 (無考績)";
                }
                else
                {
                    // 把 0~100 的分數轉換成 0~1 的比例給 Slider
                    sldPerformance.value = CareerManager.Instance.currentPerformance / 100f;
                    if (txtPerformanceLabel != null) txtPerformanceLabel.text = $"考績達成率: {CareerManager.Instance.currentPerformance:0} / 100";
                }
            }
        }
    }

    public void PopulateJobBoard()
    {
        if (CareerManager.Instance == null || jobBoardContent == null || jobItemPrefab == null) return;
        foreach (Transform child in jobBoardContent) Destroy(child.gameObject);

        foreach (var track in CareerManager.Instance.industryDatabase)
        {
            foreach (var job in track.ranks)
            {
                GameObject btnObj = Instantiate(jobItemPrefab, jobBoardContent);
                JobItemUI uiScript = btnObj.GetComponent<JobItemUI>();
                if (uiScript != null) uiScript.Setup(job);
            }
        }
    }

    public void UpdateStaticJobUI()
    {
        if (CareerManager.Instance == null || CareerManager.Instance.currentJob == null) return;
        JobPosition job = CareerManager.Instance.currentJob;

        if (txtCurrentJobTitle != null) txtCurrentJobTitle.text = $"目前職位: {job.jobTitle}";
        if (txtCurrentSalary != null)   txtCurrentSalary.text = $"每日薪資: ${job.dailySalary}";
        if (txtKpiDescription != null)  txtKpiDescription.text = $"【今日工作 KPI】\n{job.kpiDescription}";
        
        if (txtWorkTimeRange != null)
        {
            if (job.jobId == "unemployed") txtWorkTimeRange.text = "上班時間：無限制";
            else txtWorkTimeRange.text = $"上班時間：{job.startHour:00}:00 - {job.endHour:00}:00";
        }
    }

    // 🔥【今日新增】綁定給「認真工作」按鈕點擊用
    public void OnBtnWorkHardClicked()
    {
        if (CareerManager.Instance != null)
        {
            CareerManager.Instance.WorkHard();
        }
    }
}
