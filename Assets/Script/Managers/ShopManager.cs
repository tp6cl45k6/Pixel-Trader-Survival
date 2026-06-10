using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }

    // 接收 ScriptableObject 作為參數
    public void BuyItem(ShopItemData itemData)
    {
        if (PlayerPortfolio.Instance == null || PlayerStatsManager.Instance == null) return;

        if (PlayerPortfolio.Instance.cash >= itemData.cost)
        {
            PlayerPortfolio.Instance.cash -= itemData.cost;
            PlayerPortfolio.Instance.UpdateUI();

            PlayerStatsManager.Instance.stress -= itemData.stressRelief;
            PlayerStatsManager.Instance.stress = Mathf.Clamp(PlayerStatsManager.Instance.stress, 0f, 100f);
            PlayerStatsManager.Instance.UpdateUI();

            Debug.Log($"🛒 [商城] 購買了【{itemData.itemName}】！花費 ${itemData.cost}，壓力大減 {itemData.stressRelief}。");
        }
        else
        {
            Debug.LogWarning($"❌ [餘額不足] 買不起【{itemData.itemName}】！");
        }
    }
}
