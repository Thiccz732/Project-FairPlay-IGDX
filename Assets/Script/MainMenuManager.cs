using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class OverlayHewanData
{
    public string namaHewan;      // Harus SAMA PERSIS dengan namaHewan di GameManager (misal: "Cendrawasih")
    public Sprite spriteOverlay;  // Sprite UI overlay yang sesuai
}

public class MainMenuManager : MonoBehaviour
{
    [Header("Lagu Main Menu")]
    public AudioClip laguMenuUtama;

    [Header("UI Overlay Notifikasi Hewan Baru")]
    public GameObject badgePopupPanel;      // Drag GameObject BadgePopupPanel ke sini
    public Image overlayImageDisplay;       // Drag komponen Image di dalam BadgePopupPanel ke sini
    public float durasiTampilOverlay = 3f;  // Durasi pop-up muncul (detik)

    [Header("Daftar Sprite Overlay Hewan")]
    public OverlayHewanData[] daftarOverlayHewan;

    private void Start()
    {
        // Putar lagu Main Menu
        if (AudioManager.instance != null && laguMenuUtama != null)
        {
            AudioManager.instance.GantiBGM(laguMenuUtama);
        }

        // Cek apakah ada notifikasi hewan baru yang belum ditampilkan
        CekOverlayHewanBaru();
    }

    private void CekOverlayHewanBaru()
    {
        // Ambil nama hewan baru yang disimpan saat menang di GameManager
        string hewanBaru = PlayerPrefs.GetString("HewanBaruDidapat", "");

        if (!string.IsNullOrEmpty(hewanBaru))
        {
            // Cari sprite overlay yang cocok berdasarkan nama hewan
            Sprite spriteKetemu = null;
            foreach (var data in daftarOverlayHewan)
            {
                if (data.namaHewan == hewanBaru)
                {
                    spriteKetemu = data.spriteOverlay;
                    break;
                }
            }

            // Tampilkan overlay jika spritenya ketemu
            if (spriteKetemu != null && badgePopupPanel != null && overlayImageDisplay != null)
            {
                overlayImageDisplay.sprite = spriteKetemu;
                badgePopupPanel.SetActive(true);

                // Hapus key simpanan sementara agar notif tidak muncul terus tiap buka Main Menu
                PlayerPrefs.DeleteKey("HewanBaruDidapat");
                PlayerPrefs.Save();

                // Jalankan timer untuk menyembunyikan overlay otomatis
                StartCoroutine(SembunyikanOverlayOtomatis());
            }
        }
        else
        {
            if (badgePopupPanel != null) badgePopupPanel.SetActive(false);
        }
    }

    private IEnumerator SembunyikanOverlayOtomatis()
    {
        yield return new WaitForSeconds(durasiTampilOverlay);

        if (badgePopupPanel != null)
        {
            badgePopupPanel.SetActive(false);
        }
    }

    public void PindahKeMenuDistrikScene()
    {
        SceneManager.LoadScene("MenuDistrikScene"); 
    }

    public void PindahKeMenuKoleksi()
    {
        PlayerPrefs.SetString("AsalSceneKoleksi", "MainMenu");
        PlayerPrefs.Save();
        SceneManager.LoadScene("AlbumScene"); 
    }

    public void KeluarGame()
    {
        Debug.Log("Game Keluar!");
        Application.Quit();
    }
}