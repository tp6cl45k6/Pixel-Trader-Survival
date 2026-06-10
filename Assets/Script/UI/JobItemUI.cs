using UnityEngine;
using UnityEngine.UI;

public class JobItemUI : MonoBehaviour
{
    public JobData jobData; // 這裡可以拖入你的超商大夜班資料
    public Text buttonText;

    private void Start()
    {
        if (jobData != null && buttonText != null)
        {
            buttonText.text = $"{jobData.jobName} \n(賺 ${jobData.salary} / 耗體 {jobData.energyCost})";
        }
    }

    public void OnButtonClicked()
    {
        PlayerStatsManager.Instance.DoJob(jobData);
    }
}
