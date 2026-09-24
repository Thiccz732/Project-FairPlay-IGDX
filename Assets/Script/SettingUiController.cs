using UnityEngine;
using UnityEngine.UI;

public class SettingUIController : MonoBehaviour
{
    [Header("UI Images")]
    public Image btnSoundImage;
    public Image btnMusicImage;

    private void OnEnable()
    {
        // Hubungkan gambar tombol ke AudioManager singleton saat panel aktif
        if (AudioManager.instance != null)
        {
            AudioManager.instance.RegisterUIButtons(btnSoundImage, btnMusicImage);
        }
    }

    public void OnClickToggleSound()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.ToggleSound();
        }
    }

    public void OnClickToggleMusic()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.ToggleMusic();
        }
    }
}