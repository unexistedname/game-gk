using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QualityChecker : MachineBase
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI statusText;   // "Checking..." atau Kosong
    public TextMeshProUGUI qualityText;  // "Quality: 85%"
    public Button checkButton;

    // Data Internal
    private InventoryItem currentItem; // Item yang sedang ada di dalam mesin
    private bool isChecking = false;

    protected override void Start()
    {
        base.Start();
        checkButton.onClick.AddListener(StartChecking);
        ResetUI();
    }

    // --- LOGIKA INSERT (Dipanggil dari Slot) ---
    public bool TryInsertItem(InventoryItem item)
    {
        if (isChecking) return false;
        if (currentItem != null) return false; // Slot penuh

        // SYARAT 1: Item harus hasil olahan (isProcessed == true)
        if (!item.isProcessed)
        {
            Debug.Log("Item mentah tidak bisa dicek kualitasnya!");
            return false;
        }

        currentItem = item;
        UpdateUI();
        return true;
    }

    // --- LOGIKA PROSES ---
    void StartChecking()
    {
        if (currentItem == null || isChecking) return;

        // Jika sudah pernah dicek, tidak perlu cek ulang (opsional)
        if (currentItem.isChecked) return;

        StartCoroutine(CheckingRoutine());
    }

    IEnumerator CheckingRoutine()
    {
        isChecking = true;
        checkButton.interactable = false;

        // 1. Tampilkan status checking
        if (statusText) statusText.text = "Checking...";
        if (qualityText) qualityText.text = ""; // Sembunyikan hasil dulu

        // 2. Tunggu random 2 - 10 detik
        float waitTime = Random.Range(2f, 10f);
        yield return new WaitForSeconds(waitTime);

        // 3. Selesai
        currentItem.isChecked = true; // Tandai sudah dicek
        isChecking = false;

        UpdateUI(); // Tampilkan hasil
    }

    // --- UI HELPER ---
    void UpdateUI()
    {
        if (currentItem != null)
        {
            // Tampilkan Icon
            itemIcon.gameObject.SetActive(true);
            itemIcon.sprite = currentItem.data.icon;

            if (isChecking)
            {
                // Sedang Proses
                if (statusText) statusText.text = "Checking...";
                if (qualityText) qualityText.text = "";
                checkButton.interactable = false;
            }
            else
            {
                // Idle / Selesai
                if (statusText) statusText.text = ""; // Kosongkan status
                checkButton.interactable = !currentItem.isChecked; // Tombol aktif jika belum dicek

                // Tampilkan Quality HANYA jika sudah dicek
                if (currentItem.isChecked)
                {
                    if (qualityText) qualityText.text = $"{currentItem.quality:0}%";
                    // Opsional: Beri warna (Hijau bagus, Merah jelek)
                    if (qualityText) qualityText.color = Color.Lerp(Color.red, Color.green, currentItem.quality / 100f);
                }
                else
                {
                    if (qualityText) qualityText.text = "";
                    if (qualityText) qualityText.color = Color.white;
                }
            }
        }
        else
        {
            ResetUI();
        }
    }

    // Dipanggil saat item diambil kembali
    public InventoryItem TakeItem()
    {
        if (isChecking) return null; // Jangan ambil pas lagi ngecek

        InventoryItem itemToReturn = currentItem;
        currentItem = null;
        ResetUI();

        return itemToReturn;
    }

    void ResetUI()
    {
        currentItem = null;
        itemIcon.gameObject.SetActive(false);
        if (statusText) statusText.text = "";
        if (qualityText) qualityText.text = "";
        checkButton.interactable = false;
    }
}
