using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // [PENTING] Tambahkan namespace ini

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [Header("UI References")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    [Header("Settings")]
    public Vector3 offset = new Vector3(15, -15, 0);

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        HideTooltip();
    }

    void Update()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            // [PERBAIKAN] Menggunakan New Input System
            if (Mouse.current != null)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();

                // Ubah posisi panel mengikuti mouse
                tooltipPanel.transform.position = new Vector3(mousePos.x, mousePos.y, 0) + offset;
            }
        }
    }

    public void ShowTooltip(string itemName)
    {
        tooltipPanel.SetActive(true);
        tooltipText.text = itemName;

        // Pindah ke paling depan agar tidak tertutup UI lain
        tooltipPanel.transform.SetAsLastSibling();
    }

    public void HideTooltip()
    {
        if (tooltipPanel) tooltipPanel.SetActive(false);
    }
}
