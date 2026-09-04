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

        // 1. Update Indikator Halaman
        if (teksIndikatorHalaman != null)
        {
            teksIndikatorHalaman.text = (indexHewanAktif + 1) + "/" + daftarHewan.Length;
        }

        // 2. Bersihkan Prefab Lama
        if (prefabHewanTerpasang != null)
        {
            Destroy(prefabHewanTerpasang);
        }

        // 3. Cek Status Unlock & Print Debug Log
        int statusSaved = PlayerPrefs.GetInt(data.keyPlayerPrefs, 0);
        bool isUnlocked = statusSaved == 1;

        Debug.Log($"[ALBUM CHECK] Mencek Key: '{data.keyPlayerPrefs}' | Hasil PlayerPrefs: {statusSaved} | Unlocked: {isUnlocked}");

        if (isUnlocked)
        {
            if (overlayLocked != null) overlayLocked.SetActive(false);

            if (teksNamaHewan != null) teksNamaHewan.text = data.namaHewan;
            if (teksCiriCiri != null) teksCiriCiri.text = data.teksCiriCiri;

            // Spawn Prefab Animasi
            if (data.prefabAnimasiHewan != null && containerPrefabHewan != null)
            {
                prefabHewanTerpasang = Instantiate(data.prefabAnimasiHewan, containerPrefabHewan);
                
                // Pastikan transform di-reset agar pas di tengah UI Container
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

    private void UpdateSlotSamping()
    {
        int slotIndex = 0;
        for (int i = 0; i < daftarHewan.Length; i++)
        {
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
        SceneManager.LoadScene("MainMenu");
    }
}