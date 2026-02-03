using UnityEngine;
using UnityEngine.InputSystem;

public class SubMachine : MachineBase
{
    [Header("Navigation")]
    public Transform tableCameraTransform; // Drag 'CamPos' Meja ke sini
    public WorkTable parentTable;          // [BARU] Drag Objek Meja ke sini

    void Update()
    {
        // Deteksi Klik Kiri Mouse (Raycast Manual)
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Cek apakah yang diklik adalah objek ini
                if (hit.collider.gameObject == this.gameObject)
                {
                    EnterInteraction();
                }
            }
        }
    }

    public override void EnterInteraction()
    {
        // [FIX BUG] Beritahu Meja untuk sembunyikan UI & matikan collider teman
        if (parentTable != null)
        {
            parentTable.OnSubMachineActive();
        }
        else
        {
            Debug.LogError("Lupa drag 'Parent Table' (Objek Meja) di inspector alat ini!");
        }

        base.EnterInteraction(); // Zoom Kamera ke Alat & Buka UI
    }

    public override void OnCloseButtonClicked()
    {
        // Tutup UI Alat
        if (machineWindowUI != null) machineWindowUI.SetActive(false);

        // [FIX BUG] Beritahu Meja untuk munculkan UI & nyalakan collider lagi
        if (parentTable != null)
        {
            parentTable.OnSubMachineClosed();
        }

        // Kembalikan kamera ke shot Meja
        if (cameraOrbit && tableCameraTransform != null)
        {
            cameraOrbit.FocusOnTarget(tableCameraTransform);
        }
    }
}
