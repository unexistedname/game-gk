using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MixerOutputSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public MixerMachine machine; // [PENTING] Terhubung ke MixerMachine
    public Image iconDisplay;

    private GameObject ghostObject;
    private Canvas parentCanvas;

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (machine.CurrentResult == null) return;

        ghostObject = new GameObject("Ghost Output");
        ghostObject.transform.SetParent(parentCanvas.transform, false);
        Image img = ghostObject.AddComponent<Image>();
        img.sprite = machine.CurrentResult.icon;
        img.raycastTarget = false;
        ghostObject.transform.position = eventData.position;

        iconDisplay.color = new Color(1, 1, 1, 0f);
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

    public void OnItemTaken()
    {
        machine.CraftItem();
    }
    // Tambahkan: IPointerEnterHandler, IPointerExitHandler di deklarasi class atas

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (machine != null && machine.CurrentResult != null)
        {
            TooltipManager.Instance.ShowTooltip(machine.CurrentResult.itemName);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
}
