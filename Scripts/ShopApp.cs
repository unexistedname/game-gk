using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopApp : MonoBehaviour
{
    [Header("Configuration")]
    public List<ItemData> itemsForSale;

    [Header("Shop References")]
    public GameObject mainStoreView; // Panel pembungkus tampilan toko utama
    public Transform shopContentContainer;
    public GameObject shopItemPrefab;

    [Header("Cart References")]
    public GameObject cartFloatingButton;   // Tombol Keranjang melayang
    public TextMeshProUGUI cartTotalLabel;  // Teks di tombol keranjang
    public GameObject cartWindowUI;         // Layar Menu Keranjang
    public Transform cartContentContainer;  // Tempat spawn list item di Menu Keranjang
    public TextMeshProUGUI finalTotalPriceText; // Total harga di menu checkout

    [Header("System")]
    public InventoryManager inventoryManager;

    // DATA KERANJANG: Item -> Jumlah (Satuan)
    private Dictionary<ItemData, int> shoppingCart = new Dictionary<ItemData, int>();

    private void OnEnable()
    {
        // Reset tampilan saat HP dibuka: Selalu mulai dari Toko Utama
        if (mainStoreView) mainStoreView.SetActive(true);
        if (cartWindowUI) cartWindowUI.SetActive(false);
        UpdateCartUI();
    }

    void Start()
    {
        GenerateShopItems();
        UpdateCartUI();
    }

    // --- BAGIAN SHOP (MENU UTAMA) ---

    void GenerateShopItems()
    {
        foreach (Transform child in shopContentContainer) Destroy(child.gameObject);

        foreach (ItemData item in itemsForSale)
        {
            GameObject newObj = Instantiate(shopItemPrefab, shopContentContainer, false);
            newObj.transform.localPosition = Vector3.zero;
            newObj.transform.localScale = Vector3.one;

            ShopItemUI uiScript = newObj.GetComponent<ShopItemUI>();
            uiScript.Setup(item, this);
        }
    }

    // --- LOGIKA KERANJANG (BAGIAN YANG HILANG SEBELUMNYA) ---

    public int GetQuantityInCart(ItemData item)
    {
        if (shoppingCart.ContainsKey(item))
            return shoppingCart[item];
        return 0;
    }

    // Fungsi ini yang dicari oleh ShopItemUI
    public void ModifyCart(ItemData item, int amount)
    {
        if (shoppingCart.ContainsKey(item))
        {
            shoppingCart[item] += amount;
            if (shoppingCart[item] <= 0) shoppingCart.Remove(item); // Hapus kalau 0
        }
        else
        {
            if (amount > 0) shoppingCart.Add(item, amount);
        }

        // Refresh Tampilan
        RefreshShopListUI();
        UpdateCartUI();
    }

    void RefreshShopListUI()
    {
        // Update di Toko Utama
        foreach (Transform child in shopContentContainer)
        {
            ShopItemUI ui = child.GetComponent<ShopItemUI>();
            if (ui) ui.UpdateQuantityDisplay();
        }

        // Update di Menu Keranjang (jika sedang terbuka)
        if (cartWindowUI.activeSelf)
        {
            GenerateCartItems();
        }
    }

    void UpdateCartUI()
    {
        int totalItems = 0;
        int totalPrice = 0;

        foreach (var pair in shoppingCart)
        {
            int packs = pair.Value / pair.Key.packSize;
            totalPrice += packs * pair.Key.price;
            totalItems++;
        }

        // Atur Tombol Melayang
        if (totalItems > 0)
        {
            if (cartFloatingButton)
            {
                // Hanya munculkan tombol floating jika sedang di menu utama toko
                // Jika sedang di dalam menu cart, sembunyikan tombol floating biar bersih
                bool isShopOpen = mainStoreView != null && mainStoreView.activeSelf;
                cartFloatingButton.SetActive(isShopOpen);

                if (cartTotalLabel) cartTotalLabel.text = $"Cart ({totalItems}) - ${totalPrice:N0}";
            }
        }
        else
        {
            if (cartFloatingButton) cartFloatingButton.SetActive(false);
        }

        if (finalTotalPriceText) finalTotalPriceText.text = $"Total: ${totalPrice:N0}";
    }

    // --- BAGIAN WINDOW KERANJANG ---

    public void OpenCartWindow()
    {
        // Toggle: Matikan Toko, Nyalakan Keranjang
        if (mainStoreView) mainStoreView.SetActive(false);
        if (cartWindowUI) cartWindowUI.SetActive(true);
        if (cartFloatingButton) cartFloatingButton.SetActive(false);

        GenerateCartItems();
    }

    public void CloseCartWindow()
    {
        // Toggle: Nyalakan Toko, Matikan Keranjang
        if (mainStoreView) mainStoreView.SetActive(true);
        if (cartWindowUI) cartWindowUI.SetActive(false);

        UpdateCartUI(); // Munculkan lagi floating button
    }

    void GenerateCartItems()
    {
        foreach (Transform child in cartContentContainer) Destroy(child.gameObject);

        foreach (var pair in shoppingCart)
        {
            ItemData item = pair.Key;

            GameObject newObj = Instantiate(shopItemPrefab, cartContentContainer, false);
            newObj.transform.localPosition = Vector3.zero;
            newObj.transform.localScale = Vector3.one;

            ShopItemUI uiScript = newObj.GetComponent<ShopItemUI>();
            uiScript.Setup(item, this);
        }
    }

    // --- CHECKOUT ---

    public void Checkout()
    {
        int totalCost = 0;
        foreach (var pair in shoppingCart)
        {
            int packs = pair.Value / pair.Key.packSize;
            totalCost += packs * pair.Key.price;
        }

        if (WalletManager.Instance.TrySpendMoney(totalCost))
        {
            foreach (var pair in shoppingCart)
            {
                inventoryManager.AddItem(pair.Key, pair.Value);
            }

            shoppingCart.Clear();
            RefreshShopListUI();

            // Kembali ke menu toko utama
            CloseCartWindow();

            Debug.Log("Checkout Berhasil!");
        }
        else
        {
            Debug.Log("Uang tidak cukup!");
        }
    }
}
