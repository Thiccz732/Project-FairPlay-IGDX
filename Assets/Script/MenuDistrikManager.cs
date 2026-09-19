using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // 1. Tambahkan namespace TMPro ini

[System.Serializable]
public class LevelButtonData
{
    public Button button;          // Komponen Tombol UI
    public Sprite spriteLocked;    // Gambar Gembok
    public Sprite spriteUnlocked;  // Gambar Terbuka
    public Sprite spriteCompleted; // Gambar Siluet / Completed
}

public class MenuDistrikManager : MonoBehaviour
{
    [Header("Daftar Data Tombol Level")]
    public LevelButtonData[] levelDataList;

    [Header("UI Indikator Distrik")]
    public TextMeshProUGUI teksIndikatorDistrik; // 2. Variabel baru untuk teks indikator

    private void Start()
    {
        // Cek data level berapa yang sudah terbuka (Default: Level 1)
        int levelUnlocked = PlayerPrefs.GetInt("LevelUnlocked", 1);
        
        // Cek berapa level yang sudah diselesaikan (Default: 0)
        int levelCompleted = PlayerPrefs.GetInt("LevelCompleted", 0);

        // Batasi levelUnlocked agar tidak melebihi total level yang ada
        if (levelUnlocked > levelDataList.Length)
        {
            levelUnlocked = levelDataList.Length;
        }

        // 3. Update Teks Indikator Distrik (Hasilnya misal: "1/4")
        if (teksIndikatorDistrik != null)
        {
            teksIndikatorDistrik.text = levelUnlocked + "/" + levelDataList.Length;
        }

        for (int i = 0; i < levelDataList.Length; i++)
        {
            Image buttonImage = levelDataList[i].button.GetComponent<Image>();
            int currentLevel = i + 1;

            // 1. Jika level sudah tamat/selesai (Completed)
            if (currentLevel <= levelCompleted)
            {
                levelDataList[i].button.interactable = true; // Tetap bisa diklik buat replay
                if (buttonImage != null && levelDataList[i].spriteCompleted != null)
                {
                    buttonImage.sprite = levelDataList[i].spriteCompleted; // Pasang Sprite Completed
                }
            }
            // 2. Jika level sudah terbuka tapi belum tamat (Unlocked)
            else if (currentLevel <= levelUnlocked)
            {
                levelDataList[i].button.interactable = true; // Bisa diklik
                if (buttonImage != null && levelDataList[i].spriteUnlocked != null)
                {
                    buttonImage.sprite = levelDataList[i].spriteUnlocked; // Pasang Sprite Terbuka
                }
            }
            // 3. Jika level belum terbuka (Locked)
            else
            {
                levelDataList[i].button.interactable = false; // Gak bisa diklik
                if (buttonImage != null && levelDataList[i].spriteLocked != null)
                {
                    buttonImage.sprite = levelDataList[i].spriteLocked; // Pasang Gambar Gembok
                }
            }
        }
    }

    public void BukaLevel(string namaSceneLevel)
    {
        SceneManager.LoadScene(namaSceneLevel);
    }

    public void KembaliKeMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); 
    }

    public void PindahKeMenuKoleksi()
    {
        SceneManager.LoadScene("AlbumScene"); 
    }
}