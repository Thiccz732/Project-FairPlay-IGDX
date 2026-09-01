using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelButtonData
{
    public Button button;          // Komponen Tombol UI
    public Sprite spriteLocked;    // Gambar Gembok
    public Sprite spriteUnlocked;  // Gambar Siluet / Hewan
}

public class MenuDistrikManager : MonoBehaviour
{
    [Header("Daftar Data Tombol Level")]
    public LevelButtonData[] levelDataList;

    private void Start()
    {
        // Cek data level berapa yang sudah terbuka (Default: Level 1)
        int levelUnlocked = PlayerPrefs.GetInt("LevelUnlocked", 1);

        for (int i = 0; i < levelDataList.Length; i++)
        {
            Image buttonImage = levelDataList[i].button.GetComponent<Image>();

            // Jika level belum terbuka (terkunci)
            if (i + 1 > levelUnlocked)
            {
                levelDataList[i].button.interactable = false; // Gak bisa diklik
                if (buttonImage != null && levelDataList[i].spriteLocked != null)
                {
                    buttonImage.sprite = levelDataList[i].spriteLocked; // Pasang Gambar Gembok
                }
            }
            // Jika level sudah terbuka
            else
            {
                levelDataList[i].button.interactable = true; // Bisa diklik
                if (buttonImage != null && levelDataList[i].spriteUnlocked != null)
                {
                    buttonImage.sprite = levelDataList[i].spriteUnlocked; // Pasang Gambar Hewan
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
        SceneManager.LoadScene("MainMenuScene"); 
    }
}