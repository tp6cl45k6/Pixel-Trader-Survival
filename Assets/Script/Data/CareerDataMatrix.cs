using System.Collections.Generic;
using UnityEngine;

// 1. 角色永久核心能力值
[System.Serializable]
public class PlayerCapabilities
{
    public float professional = 10f;  
    public float communication = 10f; 
    public float charisma = 10f;      
    public int totalDaysWorked = 0;   
}

// 2. 單一職位資料結構
[System.Serializable]
public class JobPosition
{
    public string jobId;             
    public string jobTitle;          
    public float dailySalary;        
    public int tier;                 

    [Header("面試能力門檻")]
    public float reqProfessional;    
    public float reqCommunication;   
    public float reqCharisma;        

    [Header("工作與摸魚時程 (24小時制)")]
    public int startHour = 9;        
    public int endHour = 17;         
    public float totalMoyuMinutes;   

    [Header("每日任務與消耗")]
    public string kpiDescription;    
    public float energyCostPerDay;   
    public float stressGainPerDay;   

    public string companyName
    {
        get
        {
            if (jobId == null) return "未知公司";
            if (jobId.StartsWith("tech")) return "科技公司";
            if (jobId.StartsWith("it")) return "資訊運維公司";
            if (jobId.StartsWith("srv")) return "服務業門市";
            if (jobId.StartsWith("food")) return "餐飲業店面";
            return "公司";
        }
    }
}

// 3. 產業線包裝結構
[System.Serializable]
public class IndustryTrack
{
    public string industryId;        
    public string industryName;      
    public List<JobPosition> ranks = new List<JobPosition>();  
}
