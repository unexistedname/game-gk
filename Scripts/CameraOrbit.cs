using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbit : MonoBehaviour
{
    [Header("Sensitivity & Smoothing")]
    public float sensitivity = 0.2f;
    [Range(0.01f, 1.0f)]
    public float smoothness = 0.1f;

    [Header("Horizontal Limits")]
    public float minRotationX = -45f;
    public float maxRotationX = 45f;

    [Header("Zoom Settings")]
    public Transform cameraChild;
    public float zoomSpeed = 5f;

    // State Variables
    private float targetRotationX;
    private float currentRotationX;
    private float constantRotationY;

    private Vector3 defaultPivotPos;
    private Vector3 defaultCameraLocalPos;
    private Quaternion defaultCameraLocalRot;

    private bool isFocused = false;
    private Transform currentTargetPoint;

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        targetRotationX = rot.y;
        currentRotationX = targetRotationX;
        constantRotationY = rot.x;

        defaultPivotPos = transform.position;

        if (cameraChild != null)
        {
            defaultCameraLocalPos = cameraChild.localPosition;
            defaultCameraLocalRot = cameraChild.localRotation;
        }
    }

    void LateUpdate()
    {
        if (isFocused && currentTargetPoint != null)
        {
            // --- MODE FOKUS ---
            // Lerp Posisi Pivot ke CamPos
            transform.position = Vector3.Lerp(transform.position, currentTargetPoint.position, Time.deltaTime * zoomSpeed);

            // Slerp Rotasi Pivot ke CamPos
            transform.rotation = Quaternion.Slerp(transform.rotation, currentTargetPoint.rotation, Time.deltaTime * zoomSpeed);

            // Reset Kamera Anak ke 0,0,0
            if (cameraChild != null)
            {
                cameraChild.localPosition = Vector3.Lerp(cameraChild.localPosition, Vector3.zero, Time.deltaTime * zoomSpeed);
                cameraChild.localRotation = Quaternion.Slerp(cameraChild.localRotation, Quaternion.identity, Time.deltaTime * zoomSpeed);
            }
        }
        else
        {
            // --- MODE ORBIT ---

            // Kembalikan Posisi Pivot ke tengah ruangan
            transform.position = Vector3.Lerp(transform.position, defaultPivotPos, Time.deltaTime * zoomSpeed);

            // Kembalikan Kamera Anak ke posisi orbit
            if (cameraChild != null)
            {
                cameraChild.localPosition = Vector3.Lerp(cameraChild.localPosition, defaultCameraLocalPos, Time.deltaTime * zoomSpeed);
                cameraChild.localRotation = Quaternion.Slerp(cameraChild.localRotation, defaultCameraLocalRot, Time.deltaTime * zoomSpeed);
            }

            // Input Rotasi Mouse (Hanya aktif jika TIDAK fokus)
            if (Mouse.current.rightButton.isPressed && !isFocused)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                targetRotationX += mouseDelta.x * sensitivity;
                targetRotationX = Mathf.Clamp(targetRotationX, minRotationX, maxRotationX);
            }

            // --- PERBAIKAN LOGIC ROTASI ---
            // Kita gunakan Quaternion.Slerp agar transisi dari "Rotasi Mesin" kembali ke "Rotasi Orbit" berjalan mulus
            // Hitung target rotasi orbit berdasarkan input mouse terakhir
            Quaternion targetOrbitRotation = Quaternion.Euler(constantRotationY, currentRotationX, 0);

            // Update nilai float untuk smoothing input mouse selanjutnya
            currentRotationX = Mathf.Lerp(currentRotationX, targetRotationX, smoothness);

            // Terapkan rotasi dengan Slerp agar tidak "snapping" (patah) saat kembali dari mesin
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(constantRotationY, currentRotationX, 0), Time.deltaTime * zoomSpeed);
        }
    }

    public void FocusOnTarget(Transform targetPoint)
    {
        currentTargetPoint = targetPoint;
        isFocused = true;
    }

    // [BAGIAN YANG DIPERBAIKI]
    public void ResetFocus()
    {
        isFocused = false;
        currentTargetPoint = null;

        // KITA HAPUS kode yang mereset 'targetRotationX' di sini.
        // Biarkan 'targetRotationX' tetap menyimpan nilai terakhir sebelum kita nge-zoom.
        // Dengan begini, kamera akan kembali ke sudut pandang terakhir sebelum kita klik mesin.
    }
}
