using UnityEngine;

public class WorkTable : MachineBase
{
    [Header("Table Settings")]
    public GameObject backButtonObject;    // Tombol Back UI di bawah layar
    public Collider[] subMachineColliders; // Collider alat-alat kecil

    private Collider tableCollider;

    protected override void Start()
    {
        base.Start();
        tableCollider = GetComponent<Collider>();

        if (backButtonObject) backButtonObject.SetActive(false);
        ToggleSubMachines(false);
    }

    public override void EnterInteraction()
    {
        base.EnterInteraction(); // Zoom ke Meja

        // Matikan Collider Meja sendiri
        if (tableCollider) tableCollider.enabled = false;

        // Nyalakan UI Back & Izinkan alat kecil diklik
        if (backButtonObject) backButtonObject.SetActive(true);
        ToggleSubMachines(true);
    }

    public void OnTableBackClicked()
    {
        base.OnCloseButtonClicked(); // Reset Kamera ke Ruangan

        // Nyalakan lagi Collider Meja sendiri
        if (tableCollider) tableCollider.enabled = true;

        // Sembunyikan UI Back & Matikan Collider Alat
        if (backButtonObject) backButtonObject.SetActive(false);
        ToggleSubMachines(false);
    }

    // [BARU] Dipanggil oleh SubMachine saat alat itu dibuka
    public void OnSubMachineActive()
    {
        // 1. Sembunyikan tombol Back Meja (Solusi Bug No. 1)
        if (backButtonObject) backButtonObject.SetActive(false);

        // 2. Matikan SEMUA collider alat (Solusi Bug No. 2)
        // Ini membuat alat sebelah (dan alat ini sendiri) tidak bisa diklik lagi
        ToggleSubMachines(false);
    }

    // [BARU] Dipanggil oleh SubMachine saat alat itu ditutup
    public void OnSubMachineClosed()
    {
        // 1. Munculkan kembali tombol Back Meja
        if (backButtonObject) backButtonObject.SetActive(true);

        // 2. Nyalakan kembali collider alat (biar bisa pilih lagi)
        ToggleSubMachines(true);
    }

    void ToggleSubMachines(bool state)
    {
        foreach (Collider col in subMachineColliders)
        {
            if (col) col.enabled = state;
        }
    }
}
