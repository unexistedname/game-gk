using UnityEngine;
using UnityEngine.InputSystem;

public class PhoneController : MonoBehaviour
{
    [Header("Animation Settings")]
    public float animationSpeed = 5f;
    public float hiddenYPosition = -500f;
    public float shownYPosition = -50f;

    [Header("References")]
    public RectTransform phonePanel;
    public GameObject homeScreen;
    public GameObject shopAppScreen;
    public GameObject sellAppScreen; // [BARU] Tambahkan slot untuk Aplikasi Jual

    private bool isPhoneOpen = false;
    private GameObject currentApp;

    void Start()
    {
        // Set posisi awal sembunyi
        phonePanel.anchoredPosition = new Vector2(phonePanel.anchoredPosition.x, hiddenYPosition);
        OpenHomeScreen();
    }

    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            isPhoneOpen = !isPhoneOpen;
        }

        // Animasi Lerp Naik/Turun
        float targetY = isPhoneOpen ? shownYPosition : hiddenYPosition;
        float currentY = Mathf.Lerp(phonePanel.anchoredPosition.y, targetY, Time.deltaTime * animationSpeed);

        phonePanel.anchoredPosition = new Vector2(phonePanel.anchoredPosition.x, currentY);
    }

    public void OpenHomeScreen()
    {
        if (currentApp != null) currentApp.SetActive(false);
        homeScreen.SetActive(true);
        currentApp = null;
    }

    public void OpenShopApp()
    {
        // [UPDATE] Tutup app lain jika ada (misal pindah dari Sell ke Shop langsung)
        if (currentApp != null) currentApp.SetActive(false);

        homeScreen.SetActive(false);
        shopAppScreen.SetActive(true);
        currentApp = shopAppScreen;
    }

    // [FUNGSI BARU] Panggil ini dari tombol SellIcon di Home
    public void OpenSellApp()
    {
        // Tutup app lain jika ada
        if (currentApp != null) currentApp.SetActive(false);

        homeScreen.SetActive(false);
        sellAppScreen.SetActive(true);
        currentApp = sellAppScreen;
    }

    public void OnHomeButtonPressed()
    {
        OpenHomeScreen();
    }
}
