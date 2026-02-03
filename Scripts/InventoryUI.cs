using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public Image iconDisplay;
    public Image quantityBG;
    public TextMeshProUGUI quantityText;

    [Header("System")]
    public CanvasGroup iconCanvasGroup;

    // --- DATA ---
    private ItemData currentItem;   // Data Gambar/Nama
    private int quantity;           // Jumlah
    private InventoryItem currentSlotItem; // Dompet Data (Quality, Checked, Processed)

    private Canvas parentCanvas;
    private static GameObject ghostIconObject;

    public ItemData CurrentItem => currentItem;
    public int Quantity => quantity;

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        if (iconCanvasGroup == null)
            iconCanvasGroup = iconDisplay.GetComponent<CanvasGroup>();
    }

    public void SetItem(ItemData item, int count, float qual = 0f, bool processed = false)
    {
        currentItem = item;
        quantity = count;
        currentSlotItem = new InventoryItem(item, count, qual, processed);
        UpdateUI();
    }

    public void AddCount(int count)
    {
        quantity += count;
        if (currentSlotItem != null) currentSlotItem.quantity = quantity;

        if (quantity <= 0) ClearSlot();
        else UpdateUI();
    }

    public InventoryItem GetItemData()
    {
        if (currentSlotItem == null && currentItem != null)
        {
            currentSlotItem = new InventoryItem(currentItem, quantity);
        }
        return currentSlotItem;
    }

    void UpdateUI()
    {
        if (currentItem != null)
        {
            iconDisplay.sprite = currentItem.icon;
            iconDisplay.enabled = true;
            quantityText.text = $"{quantity} {currentItem.unitName}";

            if (quantityBG != null) quantityBG.enabled = true;

            Color iconCol = iconDisplay.color; iconCol.a = 1f; iconDisplay.color = iconCol;
            if (quantityText != null) { Color tCol = quantityText.color; tCol.a = 1f; quantityText.color = tCol; }
            if (quantityBG != null) { Color bgCol = quantityBG.color; bgCol.a = 1f; quantityBG.color = bgCol; }

            if (iconCanvasGroup) iconCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        quantity = 0;
        currentSlotItem = null;

        iconDisplay.sprite = null;
        iconDisplay.enabled = false;
        quantityText.text = "";
        if (quantityBG != null) quantityBG.enabled = false;
        if (iconCanvasGroup) iconCanvasGroup.blocksRaycasts = false;
    }

    public bool IsFree()
    {
        return currentItem == null;
    }

    // --- DRAG & DROP SYSTEM ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        ghostIconObject = new GameObject("Ghost Icon");
        ghostIconObject.transform.SetParent(parentCanvas.transform, false);
        ghostIconObject.transform.SetAsLastSibling();

        Image ghostImage = ghostIconObject.AddComponent<Image>();
        ghostImage.sprite = currentItem.icon;
        ghostImage.raycastTarget = false;

        RectTransform ghostRect = ghostIconObject.GetComponent<RectTransform>();
        ghostRect.sizeDelta = iconDisplay.rectTransform.sizeDelta;
        ghostIconObject.transform.position = eventData.position;

        Color c = iconDisplay.color; c.a = 0f; iconDisplay.color = c;
        if (quantityText) { Color t = quantityText.color; t.a = 0f; quantityText.color = t; }
        if (quantityBG) { Color b = quantityBG.color; b.a = 0f; quantityBG.color = b; }

        iconCanvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostIconObject)
        {
            if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                ghostIconObject.transform.position = eventData.position;
            else
            {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, eventData.position, parentCanvas.worldCamera, out pos);
                ghostIconObject.transform.position = parentCanvas.transform.TransformPoint(pos);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ghostIconObject) Destroy(ghostIconObject);

        if (currentItem != null)
        {
            Color c = iconDisplay.color; c.a = 1f; iconDisplay.color = c;
            if (quantityText) { Color t = quantityText.color; t.a = 1f; quantityText.color = t; }
            if (quantityBG) { Color b = quantityBG.color; b.a = 1f; quantityBG.color = b; }
            iconCanvasGroup.blocksRaycasts = true;
        }
    }

    // [BAGIAN PENTING YANG DIPERBAIKI]
    public void OnDrop(PointerEventData eventData)
    {
        // KASUS 1: Swap sesama Inventory
        InventoryUI sourceSlot = eventData.pointerDrag.GetComponent<InventoryUI>();
        if (sourceSlot != null)
        {
            SwapItems(sourceSlot, this);
            return;
        }

        // KASUS 2: Dari Mesin DRIER (MachineOutputSlot)
        MachineOutputSlot machineSlot = eventData.pointerDrag.GetComponent<MachineOutputSlot>();
        if (machineSlot != null)
        {
            InventoryItem itemFromMachine = machineSlot.machineLogic.TakeOutputItem();
            if (itemFromMachine != null)
            {
                if (IsFree())
                {
                    SetItem(itemFromMachine.data, itemFromMachine.quantity, itemFromMachine.quality, itemFromMachine.isProcessed);
                    machineSlot.OnItemTaken();
                }
            }
            return; // Selesai
        }

        // [KASUS 3 - FIX BUG] Dari Mesin QUALITY CHECKER (QualitySlot)
        QualitySlot qualitySlot = eventData.pointerDrag.GetComponent<QualitySlot>();
        if (qualitySlot != null)
        {
            // Cek dulu apakah slot inventory ini kosong?
            if (IsFree())
            {
                // Ambil item dari Quality Checker
                InventoryItem itemFromQC = qualitySlot.machine.TakeItem();

                if (itemFromQC != null)
                {
                    // Masukkan ke inventory
                    SetItem(itemFromQC.data, itemFromQC.quantity, itemFromQC.quality, itemFromQC.isProcessed);

                    // [PENTING] Pastikan status "isChecked" terbawa juga!
                    if (this.currentSlotItem != null)
                    {
                        this.currentSlotItem.isChecked = itemFromQC.isChecked;
                    }
                }
            }
        }
        // [KASUS 4] Drag Balik dari COOKER INPUT (Batalkan input)
        CookerInputSlot cookerInput = eventData.pointerDrag.GetComponent<CookerInputSlot>();
        if (cookerInput != null)
        {
            // Cek: Apakah Inventory kosong atau Itemnya sama (biar bisa ditumpuk)
            if (IsFree() || (CurrentItem == cookerInput.CurrentItem && CurrentItem.isStackable))
            {
                // [UBAH] Ambil SEMUA jumlah yang ada di slot input
                int qtyToReturn = cookerInput.CurrentStackSize;

                // Masukkan ke Inventory
                SetItem(cookerInput.CurrentItem, qtyToReturn);
                // Atau gunakan AddCount(qtyToReturn) jika itemnya sama/stackable

                // Kosongkan slot Input Cooker
                cookerInput.ClearSlot();
            }
            return;
        }
        // [KASUS 5] Ambil Hasil dari COOKER OUTPUT
        CookerOutputSlot cookerOutput = eventData.pointerDrag.GetComponent<CookerOutputSlot>();
        if (cookerOutput != null && cookerOutput.machine.CurrentResult != null)
        {
            if (IsFree())
            {
                // Buat item baru (Processed = true, Quality = 100 default dulu)
                // Kamu bisa tambah logika quality nanti di CookerRecipe jika mau
                SetItem(cookerOutput.machine.CurrentResult, cookerOutput.machine.CurrentResultCount, 100f, true);

                cookerOutput.OnItemTaken();
            }
        }
        // [KASUS 6] Drag Balik dari MIXER INPUT
        MixerInputSlot mixerInput = eventData.pointerDrag.GetComponent<MixerInputSlot>();
        if (mixerInput != null)
        {
            if (IsFree() || (CurrentItem == mixerInput.CurrentItem && CurrentItem.isStackable))
            {
                // [UBAH] Ambil SEMUA jumlah
                int qtyToReturn = mixerInput.CurrentStackSize;

                SetItem(mixerInput.CurrentItem, qtyToReturn);
                mixerInput.ClearSlot();
            }
            return;
        }

        // [KASUS 7] Ambil Hasil dari MIXER OUTPUT
        MixerOutputSlot mixerOutput = eventData.pointerDrag.GetComponent<MixerOutputSlot>();
        if (mixerOutput != null && mixerOutput.machine.CurrentResult != null)
        {
            if (IsFree())
            {
                // Mixer juga menghasilkan barang 'Processed' dengan Quality 100 (Default)
                SetItem(mixerOutput.machine.CurrentResult, mixerOutput.machine.CurrentResultCount, 100f, true);

                mixerOutput.OnItemTaken();
            }
        }
    }

    void SwapItems(InventoryUI slotA, InventoryUI slotB)
    {
        ItemData dataA = slotA.currentItem;
        int qtyA = slotA.quantity;
        InventoryItem wrapperA = slotA.GetItemData();

        ItemData dataB = slotB.currentItem;
        int qtyB = slotB.quantity;
        InventoryItem wrapperB = slotB.GetItemData();

        float qualB = (wrapperB != null) ? wrapperB.quality : 0;
        bool procB = (wrapperB != null) ? wrapperB.isProcessed : false;
        bool checkB = (wrapperB != null) ? wrapperB.isChecked : false; // Bawa status Checked
        slotA.SetItem(dataB, qtyB, qualB, procB);
        if (slotA.currentSlotItem != null) slotA.currentSlotItem.isChecked = checkB;

        float qualA = (wrapperA != null) ? wrapperA.quality : 0;
        bool procA = (wrapperA != null) ? wrapperA.isProcessed : false;
        bool checkA = (wrapperA != null) ? wrapperA.isChecked : false; // Bawa status Checked
        slotB.SetItem(dataA, qtyA, qualA, procA);
        if (slotB.currentSlotItem != null) slotB.currentSlotItem.isChecked = checkA;
    }

    // --- TOOLTIP SYSTEM ---

    // Dipanggil saat mouse menyentuh slot (Hover)
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            TooltipManager.Instance.ShowTooltip(currentItem.itemName);
        }
    }

    // Dipanggil saat mouse keluar dari slot
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
}
