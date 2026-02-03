using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour
{
    [Header("Data")]
    public string itemName;

    [Header("Visuals")]
    private Outline outlineEffect;

    [Header("GLOBAL UI REFERENCES")]
    // Karena UI-nya cuma 1 di dunia, kita tidak drag-drag lagi, tapi cari otomatis atau drag manual object global
    public GameObject interactionUI;
    public TextMeshProUGUI nameTextLabel;
    public Button interactButton;

    private MachineBase machineLogic;

    private void Start()
    {
        outlineEffect = GetComponent<Outline>();
        machineLogic = GetComponent<MachineBase>();

        if (outlineEffect) outlineEffect.enabled = false;

        // [PENTING] Kita HAPUS logika AddListener di Start.
        // Karena tombolnya milik bersama, jangan dikuasai di awal.
    }

    public void OnSelect()
    {
        if (outlineEffect) outlineEffect.enabled = true;
    }

    public void OnDeselect()
    {
        if (outlineEffect) outlineEffect.enabled = false;
        HideInfoUI();
    }

    public void OnClick()
    {
        ShowInfoUI();
    }

    public void ShowInfoUI()
    {
        if (interactionUI)
        {
            interactionUI.SetActive(true);
            if (nameTextLabel) nameTextLabel.text = itemName;

            // [LOGIKA BARU] Pasang Listener SAAT UI DITAMPILKAN
            if (interactButton != null)
            {
                // 1. Bersihkan tombol dari pemilik sebelumnya (misal bekas Drier)
                interactButton.onClick.RemoveAllListeners();

                // 2. Pasang fungsi milik SAYA (Microwave)
                interactButton.onClick.AddListener(OnInteractPressed);

                // Cek apakah punya logic mesin
                interactButton.gameObject.SetActive(machineLogic != null);
            }
        }
    }

    public void HideInfoUI()
    {
        if (interactionUI)
        {
            interactionUI.SetActive(false);

            // Opsional: Bersihkan listener saat sembunyi agar bersih
            if (interactButton != null) interactButton.onClick.RemoveAllListeners();
        }
    }

    void OnInteractPressed()
    {
        // Walaupun script mesin mati (disabled), fungsi ini tetap bisa dipanggil.
        // Jadi kita tambahkan pengecekan .enabled agar aman.
        if (machineLogic != null && machineLogic.enabled)
        {
            machineLogic.EnterInteraction();
        }
        else
        {
            Debug.Log("Logic mesin tidak ditemukan atau script dimatikan!");
        }
    }
}
