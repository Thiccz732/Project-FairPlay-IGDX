using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class DataKoleksiHewan
{
    public string namaHewan;
    public string keyPlayerPrefs;          // Misal: "Koleksi_Cendrawasih"
    public GameObject prefabAnimasiHewan;  // Prefab Hewan ber-Animator buat di Bingkai Kiri
    [TextArea(3, 5)]
    public string teksCiriCiri;            // Deskripsi/Ciri-ciri hewan
    public Sprite spriteFotoStatis;        // Foto kecil buat slot kanan
}

public class AlbumManager : MonoBehaviour
{
    [Header("Daftar 5 Hewan Koleksi")]
    public DataKoleksiHewan[] daftarHewan;
    private int indexHewanAktif = 0;

    [Header("UI Sisi Kiri (Display Utama)")]
    public Transform containerPrefabHewan; // Tempat menampung Spawn Prefab Animasi
    public TextMeshProUGUI teksNamaHewan;
    public TextMeshProUGUI teksCiriCiri;
    public GameObject overlayLocked;       // Siluet / Gembok jika belum unlocked

    [Header("UI Sisi Kanan (Slot Foto Statis)")]
    public Image[] slotFotoSamping;        // Array 4 slot foto kecil di kanan

    [Header("UI Navigasi")]
    public TextMeshProUGUI teksIndikatorHalaman; // Teks "1/5"

    private GameObject prefabHewanTerpasang;

    private void Start()
    {
        UpdateTampilanAlbum();
    }

    public void UpdateTampilanAlbum()
    {
        if (daftarHewan.Length == 0) return;

        DataKoleksiHewan data = daftarHewan[indexHewanAktif];

        // 1. Update Indikator Halaman (Misal: 1/5)
        if (teksIndikatorHalaman != null)
        {
            teksIndikatorHalaman.text = (indexHewanAktif + 1) + "/" + daftarHewan.Length;
        }

        // 2. Bersihkan Prefab Animasi Lama jika ada
        if (prefabHewanTerpasang != null)
        {
            Destroy(prefabHewanTerpasang);
        }

        // 3. Cek Status Unlock di PlayerPrefs
        bool isUnlocked = PlayerPrefs.GetInt(data.keyPlayerPrefs, 0) == 1;

        if (isUnlocked)
        {
            if (overlayLocked != null) overlayLocked.SetActive(false);

            // Tampilkan Teks Nama & Ciri-ciri
            if (teksNamaHewan != null) teksNamaHewan.text = data.namaHewan;
            if (teksCiriCiri != null) teksCiriCiri.text = data.teksCiriCiri;

            // Spawn Prefab Animasi Hewan ke dalam Container Bingkai Kiri
            if (data.prefabAnimasiHewan != null && containerPrefabHewan != null)
            {
                prefabHewanTerpasang = Instantiate(data.prefabAnimasiHewan, containerPrefabHewan);
                prefabHewanTerpasang.transform.localPosition = Vector3.zero;
                prefabHewanTerpasang.transform.localScale = Vector3.one;
            }
        }
        else
        {
            // Jika belum di-unlock
            if (overlayLocked != null) overlayLocked.SetActive(true);
            if (teksNamaHewan != null) teksNamaHewan.text = "???";
            if (teksCiriCiri != null) teksCiriCiri.text = "Temukan hewan ini di dalam game untuk membuka koleksi!";
        }

        // 4. Update Slot Foto Statis di Kanan
        UpdateSlotSamping();
    }

    private void UpdateSlotSamping()
    {
        int slotIndex = 0;
        for (int i = 0; i < daftarHewan.Length; i++)
        {
            if (i == indexHewanAktif) continue; // Skip hewan yang sedang tampil di kiri

            if (slotIndex < slotFotoSamping.Length && slotFotoSamping[slotIndex] != null)
            {
                slotFotoSamping[slotIndex].sprite = daftarHewan[i].spriteFotoStatis;
                
                // Set transparansi jika hewan sampingan belum unlocked
                bool sideUnlocked = PlayerPrefs.GetInt(daftarHewan[i].keyPlayerPrefs, 0) == 1;
                slotFotoSamping[slotIndex].color = sideUnlocked ? Color.white : Color.black;
                
                slotIndex++;
            }
        }
    }

    // Dipanggil saat klik foto slot kanan secara langsung
    public void PilihHewanDariSlot(int indexSamping)
    {
        int targetIndex = 0;
        int count = 0;

        for (int i = 0; i < daftarHewan.Length; i++)
        {
            if (i == indexHewanAktif) continue;
            if (count == indexSamping)
            {
                targetIndex = i;
                break;
            }
            count++;
        }

        indexHewanAktif = targetIndex;
        UpdateTampilanAlbum();
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
        SceneManager.LoadScene("MenuDistrikScene");
    }
}