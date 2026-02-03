using UnityEngine;
using TMPro;

public class WalletManager : MonoBehaviour
{
    public static WalletManager Instance; // Singleton

    [Header("Settings")]
    public int currentMoney = 1000; // Bisa diedit di Inspector buat testing
    public TextMeshProUGUI moneyDisplayUI; // Tarik Text di pojok kanan atas ke sini

    void Awake()
    {
        // Setup Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    // Fungsi untuk cek apakah uang cukup DAN langsung bayar
    public bool TrySpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateUI();
            return true; // Transaksi Sukses
        }
        else
        {
            Debug.Log("Uang tidak cukup!");
            return false; // Transaksi Gagal
        }
    }

    // Fungsi nambah uang (buat nanti kalau jual barang)
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (moneyDisplayUI != null)
            moneyDisplayUI.text = "$ " + currentMoney.ToString("N0"); // Format ribuan (1,000)
    }
}
