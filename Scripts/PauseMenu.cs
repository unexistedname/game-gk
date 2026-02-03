using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // PENTING

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    [Header("UI References")]
    public GameObject pauseMenuUI; // Panel Pause (Isinya tombol Resume & Quit)

    void Update()
    {
        // Deteksi tombol ESC (Escape)
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Waktu berjalan normal
        isPaused = false;

        // Kunci kursor lagi (biar bisa main FPS/TPS lagi)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Waktu berhenti total (Freeze)
        isPaused = true;

        // Bebaskan kursor (biar bisa klik tombol)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMenu()
    {
        // PENTING: Kembalikan waktu ke normal sebelum pindah scene
        // Kalau tidak, Main Menu bakal nge-freeze animasinya
        Time.timeScale = 1f;
        isPaused = false;

        // Load Scene index 0 (MainMenu)
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game...");
        Application.Quit();
    }
}
