using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "PixelTrader/Shop Item Data")]
public class ShopItemData : ScriptableObject
{
    public string itemName = "紅髮四皇公仔";
    public float cost = 3500f;         // 購買價格
    public float stressRelief = 40f;   // 減少多少壓力
}
