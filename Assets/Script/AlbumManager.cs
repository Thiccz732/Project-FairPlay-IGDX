using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class DataKoleksiHewan
{
    public string namaHewan;
    public string keyPlayerPrefs;          // Harus SAMA PRESISI dengan GameManager (Misal: "Koleksi_Cendrawasih")
    public GameObject prefabAnimasiHewan;  // Prefab Hewan ber-Animator
    [TextArea(3, 5)]
    public string teksCiriCiri;            // Deskripsi/Ciri-ciri
    public Sprite spriteFotoStatis;        // Foto kecil slot kanan
}

public class AlbumManager : MonoBehaviour
{
    [Header("Daftar 5 Hewan Koleksi")]
    public DataKoleksiHewan[] daftarHewan;
    private int indexHewanAktif = 0;

    [Header("UI Sisi Kiri (Display Utama)")]
    public Transform containerPrefabHewan; 
    public TextMeshProUGUI teksNamaHewan;
    public TextMeshProUGUI teksCiriCiri;
    public GameObject overlayLocked;       

    [Header("UI Sisi Kanan (Slot Foto Statis)")]
    public Image[] slotFotoSamping;        

    [Header("UI Navigasi")]
    public TextMeshProUGUI teksIndikatorHalaman; 

    private GameObject prefabHewanTerpasang;

    private void Start()
    {
        UpdateTampilanAlbum();
    }

    public void UpdateTampilanAlbum()
    {
        if (daftarHewan.Length == 0) return;

        DataKoleksiHewan data = daftarHewan[indexHewanAktif];

        // 1. Update Indikator Total Hewan Terkumpul (Misal: 1/5)
        if (teksIndikatorHalaman != null)
        {
            int totalUnlocked = HitungTotalUnlocked();
            teksIndikatorHalaman.text = totalUnlocked + "/" + daftarHewan.Length;
        }

        // 2. Bersihkan Prefab Lama
        if (prefabHewanTerpasang != null)
        {
            Destroy(prefabHewanTerpasang);
        }

        // 3. Cek Status Unlock Hewan Aktif
        bool isUnlocked = PlayerPrefs.GetInt(data.keyPlayerPrefs, 0) == 1;

        if (isUnlocked)
        {
            if (overlayLocked != null) overlayLocked.SetActive(false);

            if (teksNamaHewan != null) teksNamaHewan.text = data.namaHewan;
            if (teksCiriCiri != null) teksCiriCiri.text = data.teksCiriCiri;

            if (data.prefabAnimasiHewan != null && containerPrefabHewan != null)
            {
                prefabHewanTerpasang = Instantiate(data.prefabAnimasiHewan, containerPrefabHewan);
                
                RectTransform rect = prefabHewanTerpasang.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = Vector2.zero;
                    rect.localScale = Vector3.one;
                }
                else
                {
                    prefabHewanTerpasang.transform.localPosition = Vector3.zero;
                    prefabHewanTerpasang.transform.localScale = Vector3.one;
                }
            }
        }
        else
        {
            if (overlayLocked != null) overlayLocked.SetActive(true);
            if (teksNamaHewan != null) teksNamaHewan.text = "???";
            if (teksCiriCiri != null) teksCiriCiri.text = "Temukan hewan ini di dalam game untuk membuka koleksi!";
        }

        UpdateSlotSamping();
    }

// Fungsi Tambahan: Menghitung berapa banyak hewan yang sudah unlocked
    private int HitungTotalUnlocked()
    {
        int jumlah = 0;
        foreach (var hewan in daftarHewan)
        {
            if (PlayerPrefs.GetInt(hewan.keyPlayerPrefs, 0) == 1)
            {
                jumlah++;
            }
        }
        return jumlah;
    }

    private void UpdateSlotSamping()
    {
        int slotIndex = 0;
        for (int i = 0; i < daftarHewan.Length; i++)
        {
            // Skip hewan yang lagi aktif di kiri
            if (i == indexHewanAktif) continue; 

            if (slotIndex < slotFotoSamping.Length && slotFotoSamping[slotIndex] != null)
            {
                slotFotoSamping[slotIndex].sprite = daftarHewan[i].spriteFotoStatis;
                
                bool sideUnlocked = PlayerPrefs.GetInt(daftarHewan[i].keyPlayerPrefs, 0) == 1;
                slotFotoSamping[slotIndex].color = sideUnlocked ? Color.white : new Color(0.2f, 0.2f, 0.2f, 1f);
                
                slotIndex++;
            }
        }
    }

    public void PilihHewanDariSlot(int indexSamping)
    {
        // Cek urutan hewan sisa tanpa pakai 'count++' yang rawan ketuker
        int slotHitung = 0;
        for (int i = 0; i < daftarHewan.Length; i++)
        {
            if (i == indexHewanAktif) continue; // Skip hewan aktif di kiri

            if (slotHitung == indexSamping)
            {
                indexHewanAktif = i; // Langsung dapet index asli hewan tersebut!
                UpdateTampilanAlbum();
                return;
            }
            slotHitung++;
        }
    }

    public void NextHalaman()
    {
        if (indexHewanAktif < daftarHewan.Length - 1)
        {
            indexHewanAktif++;
            UpdateTampilanAlbum();
        }
    }

    public void PrevHalaman()
    {
        if (indexHewanAktif > 0)
        {
            indexHewanAktif--;
            UpdateTampilanAlbum();
        }
    }

    public void KembaliKeMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}