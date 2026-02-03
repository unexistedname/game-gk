using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MachineInputSlot : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public DrierMachine machineLogic;
    public Image iconDisplay; // Tarik InputIcon ke sini

    private GameObject ghostIconObject;
    private Canvas parentCanvas;

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    // --- MENERIMA ITEM DARI INVENTORY ---
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            InventoryUI droppedSlot = eventData.pointerDrag.GetComponent<InventoryUI>();

            // Cek apakah slot inventory itu ada isinya
            if (droppedSlot != null && droppedSlot.CurrentItem != null)
            {
                machineLogic.TryInsertItem(droppedSlot.CurrentItem, droppedSlot.Quantity, droppedSlot);
            }
        }
    }

    // --- [FIX BUG 3] MENGEMBALIKAN ITEM (CANCEL INPUT) ---
    // Logikanya: Saat didrag keluar, kita reset mesin.

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Hanya bisa drag jika ada input di mesin
        if (iconDisplay.sprite == null || !iconDisplay.enabled) return;

        // Visual Ghost Icon
        ghostIconObject = new GameObject("Ghost Input");
        ghostIconObject.transform.SetParent(parentCanvas.transform, false);
        ghostIconObject.transform.SetAsLastSibling();

        Image ghostImage = ghostIconObject.AddComponent<Image>();
        ghostImage.sprite = iconDisplay.sprite;
        ghostImage.raycastTarget = false;

        ghostIconObject.GetComponent<RectTransform>().sizeDelta = iconDisplay.rectTransform.sizeDelta;
        ghostIconObject.transform.position = eventData.position;

        // Samarkan icon asli
        iconDisplay.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostIconObject != null)
            ghostIconObject.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ghostIconObject != null) Destroy(ghostIconObject);

        // Jika didrag ke sembarang tempat (bukan slot valid lain), kita anggap CANCEL
        // Karena item aslinya masih ada di inventory (kita belum kurangi),
        // kita cukup Reset UI mesin saja.
        machineLogic.CancelInput();
    }
}
