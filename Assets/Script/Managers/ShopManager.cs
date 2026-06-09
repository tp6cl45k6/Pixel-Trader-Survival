using UnityEngine;

public class ShopManager : MonoBehaviour
{
    // 為了方便擴充，我們寫一個共用的購買邏輯
    private void TryBuyItem(string itemName, float cost, float stressRelief)
    {
        if (PlayerPortfolio.Instance == null || PlayerStatsManager.Instance == null) return;

        // 檢查交割戶餘額是否夠買公仔/周邊
        if (PlayerPortfolio.Instance.cash >= cost)
        {
            // 1. 扣除交割戶現金
            PlayerPortfolio.Instance.cash -= cost;
            
            // 強制更新交割戶 UI
            PlayerPortfolio.Instance.UpdateUI();

            // 2. 大幅降低壓力值
            PlayerStatsManager.Instance.stress -= stressRelief;
            // 確保壓力不會變成負數
            PlayerStatsManager.Instance.stress = Mathf.Clamp(PlayerStatsManager.Instance.stress, 0f, 100f);
            PlayerStatsManager.Instance.UpdateUI();

            Debug.Log($"🛒 [商城] 購買了【{itemName}】！花費 ${cost}，壓力大減 {stressRelief}。");
        }
        else
        {
            Debug.LogWarning($"❌ [餘額不足] 你的交割戶只剩 ${PlayerPortfolio.Instance.cash}，買不起【{itemName}】，再去超商上幾個大夜班吧！");
        }
    }

    // --- 給 Unity UI 按鈕綁定的具體商品 ---

    public void BuyShanksFigure()
    {
        // 高單價，強效降壓
        TryBuyItem("紅髮四皇霸氣公仔", 3500f, 40f);
    }

    public void BuyBangDreamBD()
    {
        // 中單價，溫和降壓
        TryBuyItem("BanG Dream! 演唱會 BD", 2000f, 20f);
    }
}
