using UnityEngine;
using UnityEngine.SceneManagement; // Wajib ditambahkan untuk urusan pindah scene

public class MainMenuManager : MonoBehaviour
{
    [Header("Lagu Main Menu")]
    public AudioClip laguMenuUtama;
    private void Start()
    {
        // Langsung putar lagu saat Main Menu terbuka
        if (AudioManager.instance != null && laguMenuUtama != null)
        {
            AudioManager.instance.GantiBGM(laguMenuUtama);
        }
    }
    // Fungsi ini dipanggil saat tombol Mulai diklik
    public void PindahKeMenuDistrikScene()
    {
        // Pastikan ejaan "SampleScene" sama persis dengan nama file scene lu
        SceneManager.LoadScene("MenuDistrikScene"); 
    }

    // Fungsi tambahan buat tombol Keluar sekalian
    public void KeluarGame()
    {
        Debug.Log("Game Keluar!");
        Application.Quit();
    }
}