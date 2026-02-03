using UnityEngine;
using TMPro;
using System; // Perlu ini untuk mengambil waktu sistem

public class RealTimeClock : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI clockText; // Drag text jam kamu ke sini

    [Header("Settings")]
    public bool use24HourFormat = true; // Ceklis untuk 24 jam (14:00), uncheck untuk AM/PM (02:00 PM)
    public bool showSeconds = false;    // Ceklis jika ingin detik bergerak

    void Update()
    {
        DateTime now = DateTime.Now; // Mengambil waktu laptop/hp pemain saat ini

        string format = "";

        // Tentukan format string berdasarkan settingan
        if (use24HourFormat)
        {
            // HH = 24 jam, mm = menit
            format = showSeconds ? "HH:mm:ss" : "HH:mm";
        }
        else
        {
            // hh = 12 jam, tt = AM/PM
            format = showSeconds ? "hh:mm:ss tt" : "hh:mm tt";
        }

        // Tampilkan ke teks
        if (clockText != null)
        {
            clockText.text = now.ToString(format);
        }
    }
}
