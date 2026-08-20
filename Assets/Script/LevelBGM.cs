using UnityEngine;

public class LevelBGM : MonoBehaviour
{
    [Header("Latar Musik Scene Ini")]
    public AudioClip laguLevelIni;

    private void Start()
    {
        // Panggil AudioManager yang kebal tadi buat ganti lagunya
        if (AudioManager.instance != null && laguLevelIni != null)
        {
            AudioManager.instance.GantiBGM(laguLevelIni);
        }
    }
}