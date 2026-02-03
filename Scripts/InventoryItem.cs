using System;

[Serializable]
public class InventoryItem
{
    public ItemData data;       // Referensi ScriptableObject (Gambar, Nama)
    public int quantity;        // Jumlah
    public float quality;       // Kualitas (0-100)
    public bool isProcessed;    // Apakah hasil olahan mesin? (True = bisa masuk checker)
    public bool isChecked;      // Apakah sudah pernah dicek? (True = langsung tampil quality)

    // Constructor
    public InventoryItem(ItemData itemData, int qty, float qual = 0f, bool processed = false)
    {
        data = itemData;
        quantity = qty;
        quality = qual;
        isProcessed = processed;
        isChecked = false; // Default belum dicek
    }
}
