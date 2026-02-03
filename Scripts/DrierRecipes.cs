using UnityEngine;

[CreateAssetMenu(fileName = "Drier Recipe", menuName = "Machine/Drier Recipe")]
public class DrierRecipes : ScriptableObject
{
    [Header("Input")]
    public ItemData inputItem; // Barang mentah (misal: Daun Basah)

    [Header("Process Settings")]
    public float optimalTemperature; // Suhu ideal (misal: 80 derajat)
    public float optimalDuration;    // Waktu ideal (misal: 10 detik)

    [Header("Output")]
    public ItemData outputItem;      // Barang jadi (misal: Daun Kering)
}
