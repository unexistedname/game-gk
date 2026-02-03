using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SellItemRow : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI qualityText;
    public TextMeshProUGUI priceText; // Harga satuan (setelah kena diskon quality)
    public TextMeshProUGUI quantityText; // Jumlah yang akan dijual
    public Button plusButton;
    public Button minusButton;

    // Data Internal
    private InventoryUI sourceSlot; // Slot asli di inventory
    private SellApp appLogic;       // Referensi ke otak utama
    private int sellPricePerUnit;   // Harga jual per 1 item
    private int currentSellQty;     // Berapa banyak yang mau dijual user
    private int maxQtyAvailable;    // Stok maksimal di slot itu

    public int TotalRowPrice => sellPricePerUnit * currentSellQty;
    public InventoryUI SourceSlot => sourceSlot;
    public int CurrentSellQty => currentSellQty;

    public void SetupRow(InventoryUI slot, SellApp app)
    {
        sourceSlot = slot;
        appLogic = app;

        InventoryItem itemData = slot.GetItemData();

        // 1. Setup Visual
        iconImage.sprite = itemData.data.icon;
        nameText.text = itemData.data.itemName;

        // 2. Hitung Harga Berdasarkan Quality
        // Rumus: Harga Base * (Quality / 100)
        float qualityPercent = itemData.quality / 100f;
        int basePrice = itemData.data.price; // Ambil harga dari ItemData

        // Harga minimal $1 agar tidak $0 kalau quality hancur
        sellPricePerUnit = Mathf.Max(1, Mathf.RoundToInt(basePrice * qualityPercent));

        qualityText.text = $"Qual: {itemData.quality:0}%";
        // Warna Quality (Hijau bagus, Merah jelek)
        qualityText.color = Color.Lerp(Color.red, Color.green, qualityPercent);

        priceText.text = $"${sellPricePerUnit}/pc";

        // 3. Setup Quantity Logic
        maxQtyAvailable = itemData.quantity;
        currentSellQty = 0; // Default 0 saat baru buka
        UpdateQtyUI();

        // 4. Listener Tombol
        plusButton.onClick.AddListener(OnPlusClicked);
        minusButton.onClick.AddListener(OnMinusClicked);
    }

    void OnPlusClicked()
    {
        if (currentSellQty < maxQtyAvailable)
        {
            currentSellQty++;
            UpdateQtyUI();
            appLogic.RecalculateTotal(); // Bilang ke App utama untuk hitung ulang total
        }
    }

    void OnMinusClicked()
    {
        if (currentSellQty > 0)
        {
            currentSellQty--;
            UpdateQtyUI();
            appLogic.RecalculateTotal();
        }
    }

    void UpdateQtyUI()
    {
        quantityText.text = currentSellQty.ToString();

        // Matikan tombol jika mentok
        minusButton.interactable = currentSellQty > 0;
        plusButton.interactable = currentSellQty < maxQtyAvailable;
    }
}
