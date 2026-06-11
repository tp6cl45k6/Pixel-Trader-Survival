using System.Collections.Generic;
using UnityEngine;

public class CareerManager : MonoBehaviour
{
    public static CareerManager Instance { get; private set; }

    [Header("玩家長期能力值")]
    public PlayerCapabilities playerCaps = new PlayerCapabilities();

    [Header("當前職涯狀態")]
    public JobPosition currentJob = null;
    public float currentMoyuTimeLeft = 0f;
    public bool isOnDuty = false;

    [Header("產業資料庫 (自動生成)")]
    public List<IndustryTrack> industryDatabase = new List<IndustryTrack>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
        
        InitializeJobDatabase();
    }

    private void InitializeJobDatabase()
    {
        // === 1. 科技業 (高專業、高薪、極少摸魚) ===
        IndustryTrack techTrack = new IndustryTrack { industryId = "tech", industryName = "科技業" };
        techTrack.ranks.Add(new JobPosition { jobId = "tech_1", jobTitle = "助理工程師", dailySalary = 1800, tier = 1, reqProfessional = 15, reqCommunication = 10, reqCharisma = 0, startHour = 9, endHour = 18, totalMoyuMinutes = 45, kpiDescription = "負責修小 Bug、查資料、寫寫簡單的測試。", energyCostPerDay = 30, stressGainPerDay = 15 });
        techTrack.ranks.Add(new JobPosition { jobId = "tech_2", jobTitle = "軟體工程師", dailySalary = 2600, tier = 2, reqProfessional = 30, reqCommunication = 15, reqCharisma = 0, startHour = 9, endHour = 18, totalMoyuMinutes = 35, kpiDescription = "獨立開發模組、串接外部 API、維護舊系統。", energyCostPerDay = 35, stressGainPerDay = 20 });
        techTrack.ranks.Add(new JobPosition { jobId = "tech_3", jobTitle = "資深工程師", dailySalary = 3800, tier = 3, reqProfessional = 45, reqCommunication = 25, reqCharisma = 10, startHour = 9, endHour = 18, totalMoyuMinutes = 25, kpiDescription = "規劃系統架構、微調資料庫效能、 Code Review。", energyCostPerDay = 40, stressGainPerDay = 25 });
        techTrack.ranks.Add(new JobPosition { jobId = "tech_4", jobTitle = "技術主導 (Tech Lead)", dailySalary = 5200, tier = 4, reqProfessional = 60, reqCommunication = 35, reqCharisma = 20, startHour = 9, endHour = 18, totalMoyuMinutes = 20, kpiDescription = "帶領開發小組、排除阻礙、配置 CI/CD 自動化。", energyCostPerDay = 45, stressGainPerDay = 30 });
        techTrack.ranks.Add(new JobPosition { jobId = "tech_5", jobTitle = "研發部經理", dailySalary = 6800, tier = 5, reqProfessional = 70, reqCommunication = 55, reqCharisma = 35, startHour = 9, endHour = 18, totalMoyuMinutes = 15, kpiDescription = "控管專案進度、評估團隊 KPI、與產品經理扯皮。", energyCostPerDay = 50, stressGainPerDay = 35 });
        techTrack.ranks.Add(new JobPosition { jobId = "tech_6", jobTitle = "技術總監 (CTO)", dailySalary = 9500, tier = 6, reqProfessional = 85, reqCommunication = 75, reqCharisma = 55, startHour = 9, endHour = 18, totalMoyuMinutes = 10, kpiDescription = "決定公司未來技術走向、無限的跨部門戰略會議。", energyCostPerDay = 55, stressGainPerDay = 40 });

        // === 2. 資訊運維業 (專業溝通並重、大把時間看盤) ===
        IndustryTrack itOpsTrack = new IndustryTrack { industryId = "it_ops", industryName = "資訊運維業" };
        itOpsTrack.ranks.Add(new JobPosition { jobId = "it_1", jobTitle = "電腦維運專員", dailySalary = 1600, tier = 1, reqProfessional = 10, reqCommunication = 15, reqCharisma = 0, startHour = 8, endHour = 17, totalMoyuMinutes = 90, kpiDescription = "檢查伺服器燈號、接聽報修電話、基本硬體重啟。", energyCostPerDay = 20, stressGainPerDay = 10 });
        itOpsTrack.ranks.Add(new JobPosition { jobId = "it_2", jobTitle = "系統工程師", dailySalary = 2400, tier = 2, reqProfessional = 25, reqCommunication = 20, reqCharisma = 0, startHour = 9, endHour = 18, totalMoyuMinutes = 60, kpiDescription = "管理 AD 伺服器驗證、帳號同步、權限控管。", energyCostPerDay = 25, stressGainPerDay = 15 });
        itOpsTrack.ranks.Add(new JobPosition { jobId = "it_3", jobTitle = "網路安全專員", dailySalary = 3500, tier = 3, reqProfessional = 40, reqCommunication = 30, reqCharisma = 10, startHour = 9, endHour = 18, totalMoyuMinutes = 45, kpiDescription = "執行資安掃描 (Mend/Snyk)、評估漏洞、處理 Proxy。", energyCostPerDay = 30, stressGainPerDay = 20 });
        itOpsTrack.ranks.Add(new JobPosition { jobId = "it_4", jobTitle = "資深運維工程師", dailySalary = 4800, tier = 4, reqProfessional = 55, reqCommunication = 40, reqCharisma = 20, startHour = 9, endHour = 18, totalMoyuMinutes = 35, kpiDescription = "升級核心系統版本、排除重大機房障礙。", energyCostPerDay = 35, stressGainPerDay = 25 });
        itOpsTrack.ranks.Add(new JobPosition { jobId = "it_5", jobTitle = "資訊部門主管", dailySalary = 6200, tier = 5, reqProfessional = 65, reqCommunication = 55, reqCharisma = 40, startHour = 9, endHour = 18, totalMoyuMinutes = 25, kpiDescription = "規劃年度 IT 預算、主導基礎設施升級、外包稽核。", energyCostPerDay = 40, stressGainPerDay = 30 });
        itOpsTrack.ranks.Add(new JobPosition { jobId = "it_6", jobTitle = "資訊長 (CIO)", dailySalary = 8800, tier = 6, reqProfessional = 75, reqCommunication = 70, reqCharisma = 60, startHour = 9, endHour = 18, totalMoyuMinutes = 15, kpiDescription = "簽核重大資訊合約、應付董事會資安大稽核。", energyCostPerDay = 45, stressGainPerDay = 35 });

        // === 3. 服務業 (高溝通魅力、高階主管很自由) ===
        IndustryTrack serviceTrack = new IndustryTrack { industryId = "service", industryName = "服務業" };
        serviceTrack.ranks.Add(new JobPosition { jobId = "srv_1", jobTitle = "兼職門市人員", dailySalary = 1300, tier = 1, reqProfessional = 0, reqCommunication = 15, reqCharisma = 5, startHour = 10, endHour = 18, totalMoyuMinutes = 60, kpiDescription = "商品上架、店鋪清潔、收銀結帳。", energyCostPerDay = 35, stressGainPerDay = 20 });
        serviceTrack.ranks.Add(new JobPosition { jobId = "srv_2", jobTitle = "正職門市專員", dailySalary = 1800, tier = 2, reqProfessional = 5, reqCommunication = 25, reqCharisma = 15, startHour = 10, endHour = 19, totalMoyuMinutes = 50, kpiDescription = "處理退換貨、協助新進員工、每日業績交班。", energyCostPerDay = 35, stressGainPerDay = 25 });
        serviceTrack.ranks.Add(new JobPosition { jobId = "srv_3", jobTitle = "副店長", dailySalary = 2400, tier = 3, reqProfessional = 15, reqCommunication = 40, reqCharisma = 25, startHour = 10, endHour = 19, totalMoyuMinutes = 40, kpiDescription = "店員排班、進貨盤點、處理現場突發客訴。", energyCostPerDay = 40, stressGainPerDay = 30 });
        serviceTrack.ranks.Add(new JobPosition { jobId = "srv_4", jobTitle = "門市店長", dailySalary = 3200, tier = 4, reqProfessional = 25, reqCommunication = 55, reqCharisma = 35, startHour = 10, endHour = 19, totalMoyuMinutes = 30, kpiDescription = "扛整間店的單月業績、向總部提報損益。", energyCostPerDay = 45, stressGainPerDay = 35 });
        serviceTrack.ranks.Add(new JobPosition { jobId = "srv_5", jobTitle = "區域督導", dailySalary = 4800, tier = 5, reqProfessional = 35, reqCommunication = 65, reqCharisma = 50, startHour = 9, endHour = 18, totalMoyuMinutes = 45, kpiDescription = "巡視旗下 5 間分店、抽查陳列、指導店長。", energyCostPerDay = 30, stressGainPerDay = 40 });
        serviceTrack.ranks.Add(new JobPosition { jobId = "srv_6", jobTitle = "營業部區域經理", dailySalary = 6800, tier = 6, reqProfessional = 45, reqCommunication = 75, reqCharisma = 65, startHour = 9, endHour = 18, totalMoyuMinutes = 40, kpiDescription = "掌管整個縣市的通路、制定區域大型行銷戰略。", energyCostPerDay = 35, stressGainPerDay = 45 });

        // === 4. 餐飲業 (高勞動、中等薪資) ===
        IndustryTrack foodTrack = new IndustryTrack { industryId = "food", industryName = "餐飲業" };
        foodTrack.ranks.Add(new JobPosition { jobId = "food_1", jobTitle = "餐飲外場助理", dailySalary = 1400, tier = 1, reqProfessional = 0, reqCommunication = 10, reqCharisma = 10, startHour = 11, endHour = 19, totalMoyuMinutes = 30, kpiDescription = "收桌、洗碗、瘋狂送餐、被奧客洗臉。", energyCostPerDay = 50, stressGainPerDay = 30 });
        foodTrack.ranks.Add(new JobPosition { jobId = "food_2", jobTitle = "餐飲專員", dailySalary = 1900, tier = 2, reqProfessional = 10, reqCommunication = 25, reqCharisma = 20, startHour = 11, endHour = 20, totalMoyuMinutes = 25, kpiDescription = "負責點餐收銀、內場基礎備料、客訴初階處理。", energyCostPerDay = 55, stressGainPerDay = 35 });
        foodTrack.ranks.Add(new JobPosition { jobId = "food_3", jobTitle = "值班組長 / 領班", dailySalary = 2500, tier = 3, reqProfessional = 20, reqCommunication = 45, reqCharisma = 25, startHour = 10, endHour = 20, totalMoyuMinutes = 20, kpiDescription = "控管現場出餐速度、收銀結帳稽核、每日結算。", energyCostPerDay = 60, stressGainPerDay = 40 });
        foodTrack.ranks.Add(new JobPosition { jobId = "food_4", jobTitle = "餐廳副理", dailySalary = 3400, tier = 4, reqProfessional = 30, reqCommunication = 55, reqCharisma = 35, startHour = 10, endHour = 20, totalMoyuMinutes = 20, kpiDescription = "訂購食材原物料、控管每日耗損、安排員工班表。", energyCostPerDay = 55, stressGainPerDay = 45 });
        foodTrack.ranks.Add(new JobPosition { jobId = "food_5", jobTitle = "餐廳店經理", dailySalary = 4600, tier = 5, reqProfessional = 40, reqCommunication = 65, reqCharisma = 45, startHour = 10, endHour = 20, totalMoyuMinutes = 15, kpiDescription = "對整間店的營收負責、招募考核員工、處理大客訴。", energyCostPerDay = 50, stressGainPerDay = 50 });
        foodTrack.ranks.Add(new JobPosition { jobId = "food_6", jobTitle = "品牌營運總監", dailySalary = 7000, tier = 6, reqProfessional = 50, reqCommunication = 75, reqCharisma = 60, startHour = 9, endHour = 18, totalMoyuMinutes = 30, kpiDescription = "開發新品牌、評估新店面選址、控管跨店採購成本。", energyCostPerDay = 40, stressGainPerDay = 55 });

        industryDatabase.Add(techTrack);
        industryDatabase.Add(itOpsTrack);
        industryDatabase.Add(serviceTrack);
        industryDatabase.Add(foodTrack);

        // 預設為全職散戶
        currentJob = new JobPosition { jobId = "unemployed", jobTitle = "全職浪人散戶", dailySalary = 0, tier = 0, reqProfessional = 0, reqCommunication = 0, reqCharisma = 0, startHour = 0, endHour = 0, totalMoyuMinutes = 9999, kpiDescription = "活下去，不要破產。", energyCostPerDay = 10, stressGainPerDay = 5 };

        Debug.Log("💼 [職涯系統] 四大產業、24個階級資料庫初始化完成！");
    }

    // 1. 計算錄取率 (根據玩家的 專業/溝通/魅力 與 職位門檻的落差)
    public float CalculateAcceptanceRate(JobPosition job)
    {
        // 基礎勝率 40%，每多一點能力加 2%，少一點扣 2% (魅力權重稍微低一點)
        float profDiff = (playerCaps.professional - job.reqProfessional) * 2f;
        float commDiff = (playerCaps.communication - job.reqCommunication) * 2f;
        float charDiff = (playerCaps.charisma - job.reqCharisma) * 1.5f;

        float rate = 40f + profDiff + commDiff + charDiff;
        return Mathf.Clamp(rate, 0f, 100f); // 鎖定在 0% ~ 100% 之間
    }

    // 2. 玩家按下「應徵」時觸發的面試邏輯
    public void ApplyForJob(JobPosition newJob)
    {
        float rate = CalculateAcceptanceRate(newJob);
        float roll = Random.Range(0f, 100f); // 骰 0~100 的隨機數

        if (roll <= rate)
        {
            // 骰子小於等於錄取率 = 錄取！
            currentJob = newJob;
            currentMoyuTimeLeft = newJob.totalMoyuMinutes; // 刷新摸魚時間
            Debug.Log($"㊗️ [面試成功] 恭喜錄取！你現在是【{newJob.jobTitle}】了！日薪 ${newJob.dailySalary}。");
            
            // TODO: 稍後這裡可以呼叫 UI 更新左側的狀態卡片
        }
        else
        {
            Debug.LogWarning($"❌ [面試遺憾] 您的能力值不足以勝任 (錄取率 {rate:0}%)，收到感謝函一封！");
        }
    }

    private void Update()
    {
        CheckWorkShift();
        HandleMoyuTimer();
    }

    // 🕒 自動判斷是否處於上班時間
    private void CheckWorkShift()
    {
        if (currentJob == null || currentJob.jobId == "unemployed" || GameTimeManager.Instance == null)
        {
            if (isOnDuty) 
            {
                isOnDuty = false;
                // 【修正】：已移除舊的 ToggleWorkRestrictions(false);
            }
            return;
        }

        float hour = GameTimeManager.Instance.currentHour;
        bool nowWorking = (hour >= currentJob.startHour && hour < currentJob.endHour);

        if (nowWorking && !isOnDuty)
        {
            isOnDuty = true;
            Debug.Log($"💼 [打卡上班] 嗶！目前進入上班時間。");
            // 【修正】：已移除舊的 ToggleWorkRestrictions(true);
        }
        else if (!nowWorking && isOnDuty)
        {
            isOnDuty = false;
            // 【修正】：已移除舊的 ToggleWorkRestrictions(false);

            // 準時下班發薪水
            if (PlayerPortfolio.Instance != null)
            {
                PlayerPortfolio.Instance.cash += currentJob.dailySalary;
                PlayerPortfolio.Instance.UpdateUI();
                playerCaps.totalDaysWorked++;
                Debug.Log($"🎉 [準時下班] 嗶！打卡下班！今日薪水 ${currentJob.dailySalary} 已匯入交割戶！");
            }
        }
    }

    // 🤫 上班摸魚核心計時器
    private void HandleMoyuTimer()
    {
        // 只有在「上班時間內」看股票，才會扣除摸魚時間
        if (!isOnDuty || UIManager.Instance == null) return;

        // 檢查玩家目前是不是打開了股市面板 (請確保 UIManager 裡的變數名稱與你的看盤 Panel 對齊)
        if (UIManager.Instance.stockPanel != null && UIManager.Instance.stockPanel.activeSelf)
        {
            // 根據遊戲時間流速，扣除剩餘摸魚時間 (分鐘)
            if (GameTimeManager.Instance != null)
            {
                // deltaTime * 遊戲流速 = 實際流逝的遊戲分鐘數
                float gameMinutesPassed = Time.deltaTime * GameTimeManager.Instance.timeScale;
                currentMoyuTimeLeft -= gameMinutesPassed;

                if (currentMoyuTimeLeft <= 0f)
                {
                    currentMoyuTimeLeft = 0f;
                    
                    // 強制關閉看盤畫面，退回主畫面！
                    UIManager.Instance.stockPanel.SetActive(false);
                    if (UIManager.Instance.homePanel != null) UIManager.Instance.homePanel.SetActive(true);
                    
                    Debug.LogWarning("⚠️ [主管來了] 主管突然走過來！你驚險地關閉了看盤軟體，今天不能再看股票了！");
                }
            }
        }
    }

    // 當玩家回家睡覺換日呼叫此方法
    public void ResetMoyuTimeForNewDay()
    {
        if (currentJob != null)
        {
            currentMoyuTimeLeft = currentJob.totalMoyuMinutes;
        }
        isOnDuty = false;
    }
}
