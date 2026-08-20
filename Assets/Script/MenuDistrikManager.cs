using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuDistrikManager : MonoBehaviour
{
    // Fungsi ini dinamis, bisa dipake buat tombol level mana aja
    public void BukaLevel(string namaSceneLevel)
    {
        // Pastikan namaSceneLevel yang diketik di Inspector persis dengan nama Scene di Unity
        SceneManager.LoadScene(namaSceneLevel);
    }

    // Fungsi khusus buat tombol "Kembali" / "Back"
    public void KembaliKeMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene"); 
    }
}