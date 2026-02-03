using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MachineOutputSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public Image iconDisplay;
    public DrierMachine machineLogic; // Referensi ke mesin utama

    private GameObject ghostIconObject;
    private Canvas parentCanvas;

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    // --- LOGIKA DRAG ---
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Hanya bisa didrag jika mesin sedang menampilkan output
        if (iconDisplay.sprite == null || !iconDisplay.enabled) return;

        // 1. Buat Ghost Icon (Visual saat ditarik)
        ghostIconObject = new GameObject("Ghost Output");
        ghostIconObject.transform.SetParent(parentCanvas.transform, false);
        ghostIconObject.transform.SetAsLastSibling(); // Paling depan

        Image ghostImage = ghostIconObject.AddComponent<Image>();
        ghostImage.sprite = iconDisplay.sprite;
        ghostImage.raycastTarget = false; // Tembus mouse agar bisa didrop ke inventory

        // Samakan ukuran
        ghostIconObject.GetComponent<RectTransform>().sizeDelta = iconDisplay.rectTransform.sizeDelta;
        ghostIconObject.transform.position = eventData.position;

        // 2. Samarkan icon asli (opsional)
        iconDisplay.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostIconObject == null) return;
        ghostIconObject.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ghostIconObject != null) Destroy(ghostIconObject);

        // Kembalikan warna icon asli
        iconDisplay.color = Color.white;
    }

    // Fungsi ini dipanggil oleh InventoryUI saat barang BERHASIL ditaruh di tas
    public void OnItemTaken()
    {
        // Bilang ke mesin: "Barang sudah diambil!"
        machineLogic.ClearOutput();
    }
}
