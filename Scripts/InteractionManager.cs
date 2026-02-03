using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems; // [PENTING] Wajib ada untuk mendeteksi UI

public class InteractionManager : MonoBehaviour
{
    private InteractableObject selectedObject;

    void Update()
    {
        // 1. Logika Klik Kiri
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // [FIX UTAMA DI SINI]
            // Cek apakah mouse sedang berada di atas objek UI (seperti tombol Interact)?
            // Jika YA, hentikan proses di sini (jangan raycast ke dunia 3D).
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            PerformSelection();
        }
    }

    void PerformSelection(){
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit)){
            InteractableObject hitObj = hit.collider.GetComponent<InteractableObject>();
            if (hitObj != null){
                if (selectedObject != null && selectedObject != hitObj){
                    selectedObject.OnDeselect();
                }
                selectedObject = hitObj;
                selectedObject.OnSelect();
                selectedObject.OnClick();
            }
            else{
                DeselectCurrent();
            }
        }
        else{
            DeselectCurrent();
        }
    }

    void DeselectCurrent()
    {
        if (selectedObject != null)
        {
            selectedObject.OnDeselect();
            selectedObject = null;
        }
    }
}
