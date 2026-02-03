using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    // Gunakan Singleton pattern agar mudah dipanggil dari script Mesin
    public static InventoryManager Instance;

    [Header("Configuration")]
    public int totalSlots = 20;
    public int visibleRowsCollapsed = 1;
    public float animationSpeed = 10f;
    public float heightModifier = 0f; // Tambahan padding jika perlu

    [Header("UI References")]
    public GameObject slotPrefab;
    public Transform slotContainer;
    public RectTransform inventoryPanel;
    // expandCollapseButton SUDAH DIHAPUS

    private bool isExpanded = false;
    private Coroutine animationCoroutine;

    // List untuk menyimpan semua slot
    private List<InventoryUI> uiSlots = new List<InventoryUI>();

    [Header("Testing Only")]
    public ItemData testItem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        InitializeInventory();

        // Atur tinggi awal panel
        GridLayoutGroup grid = slotContainer.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            float currentCellSize = grid.cellSize.y;
            float startHeight = currentCellSize * visibleRowsCollapsed;
            inventoryPanel.sizeDelta = new Vector2(inventoryPanel.sizeDelta.x, startHeight);
        }
    }

    void InitializeInventory()
    {
        // Bersihkan slot lama (jika ada anak dari container, hancurkan dulu biar bersih)
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }
        uiSlots.Clear();

        for (int i = 0; i < totalSlots; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotContainer);

            // Pastikan Prefab memiliki script InventoryUI
            InventoryUI slotScript = newSlot.GetComponent<InventoryUI>();

            if (slotScript != null)
            {
                uiSlots.Add(slotScript);
                slotScript.ClearSlot();
            }
            else
            {
                Debug.LogError("FATAL: Prefab Slot tidak memiliki script InventoryUI!");
            }
        }

        Debug.Log($"Inventory Berhasil Diinisialisasi. Total Slot Aktif: {uiSlots.Count}");
    }

    public void ToggleInventoryView()
    {
        isExpanded = !isExpanded;

        GridLayoutGroup grid = slotContainer.GetComponent<GridLayoutGroup>();
        if (grid == null) return;

        float currentCellSize = grid.cellSize.y;
        float currentSpacing = grid.spacing.y;

        float targetHeight;

        if (isExpanded)
        {
            // Hitung tinggi total berdasarkan jumlah baris
            int totalRows = Mathf.CeilToInt((float)totalSlots / 4.0f); // Asumsi 4 kolom
            targetHeight = (totalRows * currentCellSize) + ((totalRows - 1) * currentSpacing) + heightModifier;
        }
        else
        {
            targetHeight = currentCellSize * visibleRowsCollapsed;
        }

        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimateHeight(targetHeight));
    }

    IEnumerator AnimateHeight(float targetHeight)
    {
        float currentHeight = inventoryPanel.sizeDelta.y;

        while (Mathf.Abs(currentHeight - targetHeight) > 0.1f)
        {
            currentHeight = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * animationSpeed);
            inventoryPanel.sizeDelta = new Vector2(inventoryPanel.sizeDelta.x, currentHeight);
            yield return null;
        }

        inventoryPanel.sizeDelta = new Vector2(inventoryPanel.sizeDelta.x, targetHeight);
    }

    // UpdateButtonPosition DIHAPUS karena tombolnya sudah tidak ada

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleInventoryView();
        }

        // ======= FOR TESTING ONLY ==========
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (testItem != null)
            {
                AddItem(testItem, 2);
            }
            else
            {
                Debug.LogWarning("Test Item belum diisi di Inspector!");
            }
        }
    }

    public void AddItem(ItemData item, int count)
    {
        // Debugging awal untuk memastikan fungsi terpanggil
        // Debug.Log($"Mencoba menambahkan: {item.itemName} x{count}");

        // TAHAP 1: Stacking
        if (item.isStackable)
        {
            foreach (InventoryUI slot in uiSlots)
            {
                if (slot.CurrentItem == item)
                {
                    slot.AddCount(count);
                    Debug.Log($"[Inventory] Menambahkan {count} ke tumpukan {item.itemName}");
                    return;
                }
            }
        }

        // TAHAP 2: Slot Kosong
        foreach (InventoryUI slot in uiSlots)
        {
            if (slot.IsFree())
            {
                slot.SetItem(item, count);
                Debug.Log($"[Inventory] Item Baru: {item.itemName} di slot kosong.");
                return;
            }
        }

        Debug.Log("[Inventory] Penuh! Tidak bisa masuk.");
    }
    public List<InventoryUI> GetAllSlots()
    {
        return uiSlots;
    }
}
