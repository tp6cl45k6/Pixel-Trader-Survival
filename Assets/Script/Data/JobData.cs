using UnityEngine;

[CreateAssetMenu(fileName = "NewJob", menuName = "PixelTrader/Job Data")]
public class JobData : ScriptableObject
{
    public string jobName = "超商大夜班";
    public float salary = 600f;        // 賺多少錢
    public float energyCost = 40f;     // 扣多少體力
    public float stressGain = 20f;     // 增加多少壓力
    public float hoursTaken = 4f;      // 消耗幾小時
}
