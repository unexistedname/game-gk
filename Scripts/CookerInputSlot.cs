using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CookerInputSlot : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public Image iconDisplay;
    public TMP_InputField amountInput;
    public TextMeshProUGUI unitText;
    public CookerMachine machine;

    // --- DATA ---
    private ItemData currentItem;
    private int currentStackSize; // Stok Fisik (misal: 100)

    private Canvas parentCanvas;
    private GameObject ghostObject;

    public ItemData CurrentItem => currentItem;
    public int CurrentStackSize => currentStackSize;

    // [BARU] Getter untuk mengambil angka yang sedang tertulis di Input Field
    public int GetInputValue()
    {
        if (int.TryParse(amountInput.text, out int val))
        {
            return val;
        }
        return 0;
    }

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        ClearSlot();

        // [PERBAIKAN] Nyalakan interaksi agar bisa diketik
        if (amountInput)
        {
            amountInput.interactable = true;
            // Saat user selesai ngetik, jalankan validasi
            amountInput.onEndEdit.AddListener(ValidateInput);
        }
    }

    // [FUNGSI BARU] Validasi input user
    void ValidateInput(string text)
    {
        if (int.TryParse(text, out int value))
        {
            // Jika user nulis angka lebih besar dari stok, kembalikan ke max stok
            if (value > currentStackSize)
            {
                value = currentStackSize;
                amountInput.text = value.ToString();
            }
            // Jika user nulis 0 atau minus, set ke 0
            else if (value < 0)
            {
                value = 0;
                amountInput.text = "0";
            }
        }
        else
        {
            amountInput.text = "0";
        }

        // Cek resep setelah angka valid
        machine.CheckRecipes();
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryUI source = eventData.pointerDrag.GetComponent<InventoryUI>();

        if (source != null && source.CurrentItem != null)
        {
            if (currentItem == null || (currentItem == source.CurrentItem && currentItem.isStackable))
            {
                if (currentItem == null) currentItem = source.CurrentItem;

                // Tambah stok fisik
                currentStackSize += source.Quantity;

                // Update tampilan (otomatis menampilkan max stok)
                UpdateUI();

                source.ClearSlot();
                machine.CheckRecipes();
            }
        }
    }

    void UpdateUI()
    {
        if (currentItem != null && currentStackSize > 0)
        {
            iconDisplay.sprite = currentItem.icon;
            iconDisplay.enabled = true;
            if (unitText) unitText.text = currentItem.unitName;

            // [LOGIKA UI] Jika input field kosong/0, isi dengan max stok.
            // Jika tidak, biarkan angka yang diketik user (kecuali melebih stok)
            if (GetInputValue() == 0 || GetInputValue() > currentStackSize)
            {
                amountInput.text = currentStackSize.ToString();
            }
        }
        else
        {
            ClearSlot();
        }
    }

    public void DecreaseAmount(int amountToRemove)
    {
        currentStackSize -= amountToRemove;

        if (currentStackSize <= 0)
        {
            ClearSlot();
        }
        else
        {
            // Reset text ke sisa stok terbaru agar tidak membingungkan
            amountInput.text = currentStackSize.ToString();
            UpdateUI();
            machine.CheckRecipes();
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        currentStackSize = 0;

        iconDisplay.enabled = false;
        iconDisplay.sprite = null;
        amountInput.text = "";
        if (unitText) unitText.text = "";

        machine.CheckRecipes();
    }

    // ... (Bagian Drag Balik & Tooltip biarkan sama seperti sebelumnya) ...
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;
        ghostObject = new GameObject("Ghost");
        ghostObject.transform.SetParent(parentCanvas.transform, false);
        Image img = ghostObject.AddComponent<Image>();
        img.sprite = currentItem.icon;
        img.raycastTarget = false;
        ghostObject.transform.position = eventData.position;
        iconDisplay.color = new Color(1, 1, 1, 0.5f);
    }
    public void OnDrag(PointerEventData eventData) { if (ghostObject) ghostObject.transform.position = eventData.position; }
    public void OnEndDrag(PointerEventData eventData) { if (ghostObject) Destroy(ghostObject); iconDisplay.color = Color.white; }
    public void OnPointerEnter(PointerEventData eventData) { if (currentItem != null) TooltipManager.Instance.ShowTooltip(currentItem.itemName); }
    public void OnPointerExit(PointerEventData eventData) { TooltipManager.Instance.HideTooltip(); }
}
