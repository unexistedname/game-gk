using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI packInfoText;

    [Header("Cart Controls")]
    public GameObject quantityControlPanel; // Panel pembungkus tombol +/- (bisa disembunyikan kalau 0)
    public TextMeshProUGUI quantityText;    // Teks jumlah yang mau dibeli (di pojok)
    public Button plusButton;               // Tombol tambah (bisa jadi satu area besar)
    public Button minusButton;              // Tombol kurangi

    private ItemData _item;
    private ShopApp _shopApp;

    public void Setup(ItemData item, ShopApp shopApp)
    {
        _item = item;
        _shopApp = shopApp;

        // Setup Tampilan Dasar
        if (iconImage) iconImage.sprite = item.icon;
        if (nameText) nameText.text = item.itemName;
        if (priceText) priceText.text = "$" + item.price.ToString("N0");
        if (packInfoText) packInfoText.text = $"{item.packSize} {item.unitName}";

        // Setup Tombol
        if (plusButton)
        {
            plusButton.onClick.RemoveAllListeners();
            plusButton.onClick.AddListener(OnPlusClicked);
        }

        if (minusButton)
        {
            minusButton.onClick.RemoveAllListeners();
            minusButton.onClick.AddListener(OnMinusClicked);
        }

        UpdateQuantityDisplay();
    }

    // Dipanggil setiap kali ada perubahan data di keranjang
    public void UpdateQuantityDisplay()
    {
        // Tanya ke ShopApp: "Ada berapa item SAYA di keranjang?"
        int currentQty = _shopApp.GetQuantityInCart(_item);

        if (currentQty > 0)
        {
            // Jika sudah ada di keranjang, munculkan kontrol jumlah
            if (quantityControlPanel) quantityControlPanel.SetActive(true);
            if (quantityText) quantityText.text = currentQty.ToString() + " " + _item.unitName;

            // Opsional: Ubah warna background biar kelihatan aktif
        }
        else
        {
            // Jika belum dibeli, sembunyikan kontrol minus/jumlah
            if (quantityControlPanel) quantityControlPanel.SetActive(false);
        }
    }

    void OnPlusClicked()
    {
        // Tambah ke keranjang sesuai Pack Size (misal +10g)
        _shopApp.ModifyCart(_item, _item.packSize);
    }

    void OnMinusClicked()
    {
        // Kurangi dari keranjang
        _shopApp.ModifyCart(_item, -_item.packSize);
    }
}
