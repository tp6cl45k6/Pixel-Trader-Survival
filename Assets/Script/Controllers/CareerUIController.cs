using UnityEngine;
using UnityEngine.UI;

public class CareerUIController : MonoBehaviour
{
    [Header("右側佈告欄設定")]
    public Transform jobBoardContent; 
    public GameObject jobItemPrefab;  

    [Header("左側當前狀態 UI 綁定")]
    public Text txtCurrentJobTitle;       // 顯示目前職稱
    public Text txtCurrentSalary;         // 顯示目前日薪
    public Text txtKpiDescription;        // 顯示中央的每日任務
    public Text txtMoyuTimerLabel;        // 顯示剩餘摸魚時間文字 (例如: 摸魚剩餘: 45分)
    public Slider sldMoyuTimer;           // 顯示摸魚時間進度條
    public Text txtWorkTimeRange;         // 【新增】顯示上班時間範圍的文字框 (例如：09:00 - 17:00)

    private void Start()
    {
        PopulateJobBoard();
        UpdateStaticJobUI();
    }

    private void Update()
    {
        // 每幀即時刷新摸魚計時器與進度條
        if (CareerManager.Instance != null && CareerManager.Instance.currentJob != null)
        {
            JobPosition currentJob = CareerManager.Instance.currentJob;

            // 1. 更新計時器文字
            if (txtMoyuTimerLabel != null)
            {
                if (currentJob.jobId == "unemployed")
                {
                    txtMoyuTimerLabel.text = "全職看盤中 (時間無限)";
                }
                else
                {
                    txtMoyuTimerLabel.text = CareerManager.Instance.isOnDuty 
                        ? $"上班中！摸魚剩餘: {Mathf.CeilToInt(CareerManager.Instance.currentMoyuTimeLeft)} 分鐘"
                        : $"非上班時間 (今日摸魚額度: {currentJob.totalMoyuMinutes} 分)";
                }
            }

            // 2. 更新進度條 Slider
            if (sldMoyuTimer != null)
            {
                if (currentJob.jobId == "unemployed")
                {
                    sldMoyuTimer.value = 1f; // 失業時進度條永遠滿格
                }
                else
                {
                    // 計算剩餘時間百分比
                    sldMoyuTimer.value = CareerManager.Instance.currentMoyuTimeLeft / currentJob.totalMoyuMinutes;
                }
            }
        }
    }

    // 重新繪製所有的職缺按鈕 (當玩家能力提升、錄取率改變時可以呼叫此方法刷新列表)
    public void PopulateJobBoard()
    {
        if (CareerManager.Instance == null || jobBoardContent == null || jobItemPrefab == null) return;

        // 先清空舊的按鈕
        foreach (Transform child in jobBoardContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var track in CareerManager.Instance.industryDatabase)
        {
            foreach (var job in track.ranks)
            {
                GameObject btnObj = Instantiate(jobItemPrefab, jobBoardContent);
                JobItemUI uiScript = btnObj.GetComponent<JobItemUI>();
                if (uiScript != null)
                {
                    uiScript.Setup(job);
                }
            }
        }
    }

    // 當面試成功換工作時，呼叫此方法更新左側與中央的固定文字
    public void UpdateStaticJobUI()
    {
        if (CareerManager.Instance == null || CareerManager.Instance.currentJob == null) return;

        JobPosition job = CareerManager.Instance.currentJob;

        if (txtCurrentJobTitle != null) txtCurrentJobTitle.text = $"目前職位: {job.jobTitle}";
        if (txtCurrentSalary != null)   txtCurrentSalary.text = $"每日薪資: ${job.dailySalary}";
        if (txtKpiDescription != null)  txtKpiDescription.text = $"【今日工作 KPI】\n{job.kpiDescription}";
        
        // 【新增】動態顯示該工作的上班時間區間
        if (txtWorkTimeRange != null)
        {
            if (job.jobId == "unemployed")
            {
                txtWorkTimeRange.text = "上班時間：無限制";
            }
            else
            {
                // 格式化成 09:00 的樣子
                txtWorkTimeRange.text = $"上班時間：{job.startHour:00}:00 - {job.endHour:00}:00";
            }
        }
    }
}
