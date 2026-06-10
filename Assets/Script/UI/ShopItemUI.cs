using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public ShopItemData itemData; // 這裡可以拖入你的紅髮公仔或 BD 資料
    public Text buttonText;

    private void Start()
    {
        // 遊戲開始時，自動根據資料庫更新按鈕上的文字！
        if (itemData != null && buttonText != null)
        {
            buttonText.text = $"購買 {itemData.itemName} \n($ {itemData.cost})";
        }
    }

    public void OnButtonClicked()
    {
        ShopManager.Instance.BuyItem(itemData);
    }
}
