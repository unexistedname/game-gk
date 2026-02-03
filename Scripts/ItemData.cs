using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [Header("Trading Info")]
    public int price;        
    public string unitName;  
    public int packSize;    

    [Header("System")]
    public bool isStackable = true;
    public bool driedReady;
}
