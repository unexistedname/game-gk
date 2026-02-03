using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QualitySlot : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public QualityChecker machine; // Drag QualityChecker ke sini
    public Image iconDisplay;      // Drag Image Icon di slot ini

    private GameObject ghostObject;
    private Canvas parentCanvas;

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    // --- MENERIMA BARANG DARI INVENTORY (DROP) ---
    public void OnDrop(PointerEventData eventData)
    {
        InventoryUI source = eventData.pointerDrag.GetComponent<InventoryUI>();
        if (source != null && source.GetItemData() != null)
        {
            // Coba masukkan ke mesin
            InventoryItem itemToSend = source.GetItemData(); // Kita butuh fungsi ini di InventoryUI

            if (machine.TryInsertItem(itemToSend))
            {
                // Jika mesin menerima, kosongkan slot inventory asal
                source.ClearSlot();
            }
        }
    }

    // --- MENGAMBIL BARANG BALIK (DRAG) ---
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (iconDisplay.sprite == null || !iconDisplay.gameObject.activeSelf) return;

        // Visual Ghost
        ghostObject = new GameObject("Ghost");
        ghostObject.transform.SetParent(parentCanvas.transform, false);
        ghostObject.transform.SetAsLastSibling();

        Image img = ghostObject.AddComponent<Image>();
        img.sprite = iconDisplay.sprite;
        img.raycastTarget = false;

        ghostObject.GetComponent<RectTransform>().sizeDelta = iconDisplay.rectTransform.sizeDelta;
        ghostObject.transform.position = eventData.position;

        iconDisplay.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostObject) ghostObject.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ghostObject) Destroy(ghostObject);
        iconDisplay.color = Color.white;
    }
}
