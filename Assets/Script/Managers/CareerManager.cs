using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CareerManager : MonoBehaviour
{
    public static CareerManager Instance { get; private set; }

    [Header("玩家長期能力值")]
    public PlayerCapabilities playerCaps = new PlayerCapabilities();

    [Header("當前職涯狀態")]
    public JobPosition currentJob = null;
    public float currentMoyuTimeLeft = 0f;
    public bool isOnDuty = false;

    // 🔥【今日新增】當前職位的考績分數 (0~100)，滿 100 才能面試下一階
    public float currentPerformance = 0f; 

    [Header("防連點與限制機制")]
    public float workHardCooldown = 2.0f; 
    private float lastWorkHardTime = 0f;  
    
    // 🔥【修改】面試限制機制
    public List<string> interviewedJobsToday = new List<string>(); // 記錄今天面試過的職位
    public float currentStress = 0f; // 當前壓力值 (0~100)
    public float maxStressForInterview = 50f; // 壓力超過此值不給面試

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
        // 設定 JSON 存檔路徑 (StreamingAssets 資料夾最適合放外部可修改的資料)
        string folderPath = Application.streamingAssetsPath;
        string filePath = Path.Combine(folderPath, "CareerDatabase.json");

        if (File.Exists(filePath))
        {
            // 📖【讀取模式】如果 JSON 已經存在，直接讀取它！
            string jsonContent = File.ReadAllText(filePath);
            CareerDatabaseRoot dbRoot = JsonUtility.FromJson<CareerDatabaseRoot>(jsonContent);
            industryDatabase = dbRoot.industries;
            
            Debug.Log($"💼 [職涯系統] 成功從外部 JSON 讀取資料庫！(共 {industryDatabase.Count} 個產業)");
        }
        else
        {
            // 🏭【生成模式】如果找不到 JSON，用現有資料自動生成一份並存成 JSON 檔！
            Debug.Log("💼 [職涯系統] 找不到 JSON，開始自動產生預設檔案...");

            // === 1. 科技業 ===
            IndustryTrack techTrack = new IndustryTrack { industryId = "tech", industryName = "科技業" };
            techTrack.ranks.Add(new JobPosition { jobId = "tech_1", jobTitle = "助理工程師", dailySalary = 1800, tier = 1, reqProfessional = 15, reqCommunication = 10, reqCharisma = 0, startHour = 9, endHour = 18, totalMoyuMinutes = 45, kpiDescription = "負責修小 Bug、查資料、寫寫簡單的測試。", energyCostPerDay = 30, stressGainPerDay = 15 });
            techTrack.ranks.Add(new JobPosition { jobId = "tech_2", jobTitle = "軟體工程師", dailySalary = 2600, tier = 2, reqProfessional = 30, reqCommunication = 15, reqCharisma = 0, startHour = 9, endHour = 18, totalMoyuMinutes = 35, kpiDescription = "獨立開發模組、串接外部 API、維護舊系統。", energyCostPerDay = 35, stressGainPerDay = 20 });
            techTrack.ranks.Add(new JobPosition { jobId = "tech_3", jobTitle = "資深工程師", dailySalary = 3800, tier = 3, reqProfessional = 45, reqCommunication = 25, reqCharisma = 10, startHour = 9, endHour = 18, totalMoyuMinutes = 25, kpiDescription = "規劃系統架構、微調資料庫效能、 Code Review。", energyCostPerDay = 40, stressGainPerDay = 25 });
            techTrack.ranks.Add(new JobPosition { jobId = "tech_4", jobTitle = "技術主導 (Tech Lead)", dailySalary = 5200, tier = 4, reqProfessional = 60, reqCommunication = 35, reqCharisma = 20, startHour = 9, endHour = 18, totalMoyuMinutes = 20, kpiDescription = "帶領開發小組、排除阻礙、配置 CI/CD 自動化。", energyCostPerDay = 45, stressGainPerDay = 30 });
            techTrack.ranks.Add(new JobPosition { jobId = "tech_5", jobTitle = "研發部經理", dailySalary = 6800, tier = 5, reqProfessional = 70, reqCommunication = 55, reqCharisma = 35, startHour = 9, endHour = 18, totalMoyuMinutes = 15, kpiDescription = "控管專案進度、評估團隊 KPI、與產品經理扯皮。", energyCostPerDay = 50, stressGainPerDay = 35 });
            techTrack.ranks.Add(new JobPosition { jobId = "tech_6", jobTitle = "技術總監 (CTO)", dailySalary = 9500, tier = 6, reqProfessional = 85, reqCommunication = 75, reqCharisma = 55, startHour = 9, endHour = 18, totalMoyuMinutes = 10, kpiDescription = "決定公司未來技術走向、無限的跨部門戰略會議。", energyCostPerDay = 55, stressGainPerDay = 40 });

            // === 2. 資訊運維業 ===
            IndustryTrack itOpsTrack = new IndustryTrack { industryId = "it_ops", industryName = "資訊運維業" };
            itOpsTrack.ranks.Add(new JobPosition { jobId = "it_1", jobTitle = "電腦維運專員", dailySalary = 1600, tier = 1, reqProfessional = 10, reqCommunication = 15, reqCharisma = 0, startHour = 8, endHour = 17, totalMoyuMinutes = 90, kpiDescription = "檢查伺服器燈號、接聽報修電話、基本硬體重啟。", energyCostPerDay = 20, stressGainPerDay = 10 });
            itOpsTrack.ranks.Add(new JobPosition { jobId = "it_2", jobTitle = "系統工程師", dailySalary = 2400, tier = 2, reqProfessional = 25, reqCommunication = 20, reqCharisma = 0, startHour = 9, endHour = 18, totalMoyuMinutes = 60, kpiDescription = "管理 AD 伺服器驗證、帳號同步、權限控管。", energyCostPerDay = 25, stressGainPerDay = 15 });
            itOpsTrack.ranks.Add(new JobPosition { jobId = "it_3", jobTitle = "網路安全專員", dailySalary = 3500, tier = 3, reqProfessional = 40, reqCommunication = 30, reqCharisma = 10, startHour = 9, endHour = 18, totalMoyuMinutes = 45, kpiDescription = "執行資安掃描 (Mend/Snyk)、評估漏洞、處理 Proxy。", energyCostPerDay = 30, stressGainPerDay = 20 });
            itOpsTrack.ranks.Add(new JobPosition { jobId = "it_4", jobTitle = "資深運維工程師", dailySalary = 4800, tier = 4, reqProfessional = 55, reqCommunication = 40, reqCharisma = 20, startHour = 9, endHour = 18, totalMoyuMinutes = 35, kpiDescription = "升級核心系統版本、排除重大機房障礙。", energyCostPerDay = 35, stressGainPerDay = 25 });
            itOpsTrack.ranks.Add(new JobPosition { jobId = "it_5", jobTitle = "資訊部門主管", dailySalary = 6200, tier = 5, reqProfessional = 65, reqCommunication = 55, reqCharisma = 40, startHour = 9, endHour = 18, totalMoyuMinutes = 25, kpiDescription = "規劃年度 IT 預算、主導基礎設施升級、外包稽核。", energyCostPerDay = 40, stressGainPerDay = 30 });
            itOpsTrack.ranks.Add(new JobPosition { jobId = "it_6", jobTitle = "資訊長 (CIO)", dailySalary = 8800, tier = 6, reqProfessional = 75, reqCommunication = 70, reqCharisma = 60, startHour = 9, endHour = 18, totalMoyuMinutes = 15, kpiDescription = "簽核重大資訊合約、應付董事會資安大稽核。", energyCostPerDay = 45, stressGainPerDay = 35 });

            // === 3. 服務業 ===
            IndustryTrack serviceTrack = new IndustryTrack { industryId = "service", industryName = "服務業" };
            serviceTrack.ranks.Add(new JobPosition { jobId = "srv_1", jobTitle = "兼職門市人員", dailySalary = 1300, tier = 1, reqProfessional = 0, reqCommunication = 15, reqCharisma = 5, startHour = 10, endHour = 18, totalMoyuMinutes = 60, kpiDescription = "商品上架、店鋪清潔、收銀結帳。", energyCostPerDay = 35, stressGainPerDay = 20 });
            serviceTrack.ranks.Add(new JobPosition { jobId = "srv_2", jobTitle = "正職門市專員", dailySalary = 1800, tier = 2, reqProfessional = 5, reqCommunication = 25, reqCharisma = 15, startHour = 10, endHour = 19, totalMoyuMinutes = 50, kpiDescription = "處理退換貨、協助新進員工、每日業績交班。", energyCostPerDay = 35, stressGainPerDay = 25 });
            serviceTrack.ranks.Add(new JobPosition { jobId = "srv_3", jobTitle = "副店長", dailySalary = 2400, tier = 3, reqProfessional = 15, reqCommunication = 40, reqCharisma = 25, startHour = 10, endHour = 19, totalMoyuMinutes = 40, kpiDescription = "店員排班、進貨盤點、處理現場突發客訴。", energyCostPerDay = 40, stressGainPerDay = 30 });
            serviceTrack.ranks.Add(new JobPosition { jobId = "srv_4", jobTitle = "門市店長", dailySalary = 3200, tier = 4, reqProfessional = 25, reqCommunication = 55, reqCharisma = 35, startHour = 10, endHour = 19, totalMoyuMinutes = 30, kpiDescription = "扛整間店的單月業績、向總部提報損益。", energyCostPerDay = 45, stressGainPerDay = 35 });
            serviceTrack.ranks.Add(new JobPosition { jobId = "srv_5", jobTitle = "區域督導", dailySalary = 4800, tier = 5, reqProfessional = 35, reqCommunication = 65, reqCharisma = 50, startHour = 9, endHour = 18, totalMoyuMinutes = 45, kpiDescription = "巡視旗下 5 間分店、抽查陳列、指導店長。", energyCostPerDay = 30, stressGainPerDay = 40 });
            serviceTrack.ranks.Add(new JobPosition { jobId = "srv_6", jobTitle = "營業部區域經理", dailySalary = 6800, tier = 6, reqProfessional = 45, reqCommunication = 75, reqCharisma = 65, startHour = 9, endHour = 18, totalMoyuMinutes = 40, kpiDescription = "掌管整個縣市的通路、制定區域大型行銷戰略。", energyCostPerDay = 35, stressGainPerDay = 45 });

            // === 4. 餐飲業 ===
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

            // 設定預設解鎖狀態 (只有 Tier 1 解鎖)
            foreach (var track in industryDatabase)
            {
                foreach (var job in track.ranks)
                {
                    job.isUnlocked = (job.tier == 1); 
                }
            }

            // 📦 打包轉成 JSON 字串 (true 代表要漂亮地換行縮排)
            CareerDatabaseRoot dbRoot = new CareerDatabaseRoot { industries = this.industryDatabase };
            string jsonOutput = JsonUtility.ToJson(dbRoot, true);

            // 💾 寫入實體檔案
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            File.WriteAllText(filePath, jsonOutput);
            
            Debug.Log($"💾 [職涯系統] JSON 資料庫已成功「自動匯出」至:\n{filePath}");
        }

        // 預設為全職散戶
        currentJob = new JobPosition { jobId = "unemployed", jobTitle = "全職浪人散戶", dailySalary = 0, tier = 0, reqProfessional = 0, reqCommunication = 0, reqCharisma = 0, startHour = 0, endHour = 0, totalMoyuMinutes = 9999, kpiDescription = "活下去，不要破產。", energyCostPerDay = 10, stressGainPerDay = 5 };
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

    public void ApplyForJob(JobPosition newJob)
    {
        if (currentJob != null && currentJob.jobId == newJob.jobId)
        {
            Debug.LogWarning("⚠️ 你已經在這個職位了，請選擇其他職缺！");
            return;
        }

        // ⛔ 1. 檢查時間：面試只能在 10:00 ~ 15:00 進行
        float currentHour = GameTimeManager.Instance.currentHour;
        if (currentHour < 10f || currentHour >= 15f)
        {
            Debug.LogWarning("🛑 [人資拒絕] 面試時間為早上 10 點到下午 3 點！");
            return;
        }

        // ⛔ 2. 檢查壓力值：壓力超過 50 拒絕面試
        if (currentStress > maxStressForInterview)
        {
            Debug.LogWarning($"🛑 [壓力過大] 你看起來精神太差了 (壓力: {currentStress:0})，人資請你回去休息！");
            return;
        }

        // ⛔ 3. 檢查單一崗位每日限制
        if (interviewedJobsToday.Contains(newJob.jobId))
        {
            Debug.LogWarning($"🛑 [人資拒絕] 你今天已經面試過【{newJob.jobTitle}】了，請明天再來！");
            return;
        }

        // 📝 記錄已面試並增加壓力
        interviewedJobsToday.Add(newJob.jobId);
        currentStress += 15f; // 每次面試增加 15 點壓力
        currentStress = Mathf.Clamp(currentStress, 0f, 100f);

        float rate = CalculateAcceptanceRate(newJob);
        float roll = Random.Range(0f, 100f); 

        if (roll <= rate)
        {
            currentJob = newJob;
            currentMoyuTimeLeft = newJob.totalMoyuMinutes; 
            currentPerformance = 0f; 
            Debug.Log($"㊗️ [面試成功] 錄取【{newJob.jobTitle}】！考績已歸零，當前壓力: {currentStress:0}");
        }
        else
        {
            currentPerformance -= 30f;
            currentPerformance = Mathf.Clamp(currentPerformance, 0f, 100f);
            Debug.LogWarning($"❌ [面試遺憾] 錄取率 {rate:0}% 挑戰失敗！考績扣除 30 分，當前壓力: {currentStress:0}");
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
                
                // 🔥【修改】每日平安下班沒被開除，給予基本考績獎勵
                currentPerformance += 20f;
                currentPerformance = Mathf.Clamp(currentPerformance, 0f, 100f);
                Debug.Log($"🎉 [準時下班] 嗶！薪水入帳！今日平安度過，考績 +20 (目前: {currentPerformance:0}/100)");
                
                CheckPromotionUnlock(); // 下班時也檢查一下有沒有達標解鎖
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

                // 🔥【修改】摸魚懲罰：只要開著股市看盤，考績就會隨時間偷偷往下掉！
                currentPerformance -= gameMinutesPassed * 0.5f; 
                currentPerformance = Mathf.Clamp(currentPerformance, 0f, 100f);

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

    public void ResetMoyuTimeForNewDay()
    {
        if (currentJob != null)
        {
            currentMoyuTimeLeft = currentJob.totalMoyuMinutes;
        }
        isOnDuty = false;
        
        // 🌞 新的一天，清空面試紀錄，並透過休息恢復 20 點壓力！
        interviewedJobsToday.Clear(); 
        currentStress -= 20f; 
        currentStress = Mathf.Max(0f, currentStress); // 最低不小於 0
    }

    // 🔥【修改】認真工作功能 (加入冷卻防連點)
    public void WorkHard()
    {
        if (!isOnDuty)
        {
            Debug.LogWarning("❌ [警告] 現在不是上班時間，無法加班！");
            return;
        }

        // ⛔ 檢查冷卻時間
        if (Time.time < lastWorkHardTime + workHardCooldown)
        {
            Debug.LogWarning("⏳ [冷卻中] 專心做事！不要瘋狂敲鍵盤假裝很忙！");
            return;
        }
        
        lastWorkHardTime = Time.time; // 更新最後一次工作的時間

        // 每次點擊大幅增加考績，並鎖定最高 100 分
        currentPerformance += 15f;
        currentPerformance = Mathf.Clamp(currentPerformance, 0f, 100f);
        
        Debug.Log($"💻 [認真工作] 你埋頭苦幹了一陣子！考績提升，目前：{currentPerformance:0}/100");

        CheckPromotionUnlock();
    }

    // 🔥【今日新增】解鎖下一階職位的判定
    public void CheckPromotionUnlock()
    {
        if (currentPerformance >= 100f && currentJob != null && currentJob.jobId != "unemployed")
        {
            // 找出目前的產業線
            foreach (var track in industryDatabase)
            {
                bool foundCurrent = false;
                foreach (var job in track.ranks)
                {
                    if (job.jobId == currentJob.jobId) 
                    {
                        foundCurrent = true; 
                    }
                    else if (foundCurrent && job.tier == currentJob.tier + 1)
                    {
                        // 找到下一階了！如果是未解鎖狀態，就把它解鎖
                        if (!job.isUnlocked)
                        {
                            job.isUnlocked = true;
                            Debug.Log($"🌟 [升遷解鎖] 你的考績達標了！已解鎖下一階職位：【{job.jobTitle}】，可以準備投遞履歷了！");
                            
                            // 呼叫 UI 重新整理佈告欄，讓鎖頭啪一聲解開！
                            CareerUIController ui = FindObjectOfType<CareerUIController>();
                            if (ui != null) ui.PopulateJobBoard();
                        }
                        break; 
                    }
                }
            }
        }
    }
}
