using UnityEngine;
using UnityEngine.SceneManagement; // Wajib ditambahkan untuk urusan pindah scene

public class MainMenuManager : MonoBehaviour
{
    // Fungsi ini dipanggil saat tombol Mulai diklik
    public void PindahKeSampleScene()
    {
        // Pastikan ejaan "SampleScene" sama persis dengan nama file scene lu
        SceneManager.LoadScene("SampleScene"); 
    }

    // Fungsi tambahan buat tombol Keluar sekalian
    public void KeluarGame()
    {
        Debug.Log("Game Keluar!");
        Application.Quit();
    }
}