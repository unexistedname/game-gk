using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SellApp : MonoBehaviour
{
    [Header("UI References")]
    public Transform listContainer; // Tempat spawn row (Content di ScrollView)
    public GameObject itemRowPrefab; // Prefab yang ada script SellItemRow
    public TextMeshProUGUI totalPriceText;
    public Button sellButton;
    public TextMeshProUGUI emptyStateText; // Teks "Tidak ada barang untuk dijual"

    private List<SellItemRow> activeRows = new List<SellItemRow>();

    // Dipanggil setiap kali HP/Aplikasi dibuka
    void OnEnable()
    {
        RefreshList();
    }

    public void RefreshList()
    {
        // 1. Bersihkan List Lama
        foreach (Transform child in listContainer)
        {
            Destroy(child.gameObject);
        }
        activeRows.Clear();

        // 2. Ambil semua slot dari Inventory
        List<InventoryUI> allSlots = InventoryManager.Instance.GetAllSlots();
        bool foundItem = false;

        foreach (InventoryUI slot in allSlots)
        {
            // Ambil data detail (InventoryItem)
            InventoryItem data = slot.GetItemData();

            // FILTER:
            // - Slot tidak kosong
            // - Sudah diproses (Output mesin)
            // - Sudah dicek (Quality Checker - isChecked)
            if (data != null && !slot.IsFree() && data.isProcessed && data.isChecked)
            {
                // Spawn Row
                GameObject newRowObj = Instantiate(itemRowPrefab, listContainer);
                SellItemRow rowScript = newRowObj.GetComponent<SellItemRow>();

                // Setup Row
                rowScript.SetupRow(slot, this);
                activeRows.Add(rowScript);

                foundItem = true;
            }
        }

        // Tampilkan teks kosong jika tidak ada item
        if (emptyStateText) emptyStateText.gameObject.SetActive(!foundItem);

        // Reset Total
        RecalculateTotal();
    }

    // Dipanggil oleh SellItemRow saat tombol +/- ditekan
    public void RecalculateTotal()
    {
        int totalMoney = 0;
        int totalItems = 0;

        foreach (SellItemRow row in activeRows)
        {
            totalMoney += row.TotalRowPrice;
            totalItems += row.CurrentSellQty;
        }

        // Update UI Total
        totalPriceText.text = $"Sell Total: ${totalMoney:N0}";

        // Tombol Sell hanya aktif jika ada barang yang dipilih (> 0)
        sellButton.interactable = totalItems > 0;
    }

    // Dipanggil saat tombol SELL ditekan
    public void OnSellButtonClicked()
    {
        int totalRevenue = 0;

        // Loop semua row untuk eksekusi penjualan
        foreach (SellItemRow row in activeRows)
        {
            if (row.CurrentSellQty > 0)
            {
                // 1. Kurangi item dari Inventory Asli
                // Kita gunakan AddCount minus untuk mengurangi
                row.SourceSlot.AddCount(-row.CurrentSellQty);

                // 2. Hitung pendapatan
                totalRevenue += row.TotalRowPrice;
            }
        }

        // 3. Tambahkan Uang ke Wallet
        WalletManager.Instance.AddMoney(totalRevenue);
        Debug.Log($"Berhasil menjual item. Pendapatan: ${totalRevenue}");

        // 4. Refresh tampilan list (karena item mungkin habis/hilang dari inventory)
        RefreshList();
    }
}
