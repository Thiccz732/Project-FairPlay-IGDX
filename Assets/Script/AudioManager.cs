using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Pengaturan SFX")]
    public AudioSource sfxSource;
    public AudioClip clickSound;

    [Header("Pengaturan BGM")]
    public AudioSource bgmSource; 

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else 
        {
            Destroy(gameObject); 
        }
    }

    public void PlayButtonSFX()
    {
        if (sfxSource != null && clickSound != null)
        {
            sfxSource.PlayOneShot(clickSound);
        }
    }

    public void GantiBGM(AudioClip laguBaru)
    {
        if (bgmSource == null || laguBaru == null) return;

        if (bgmSource.clip == laguBaru) return; 

        bgmSource.clip = laguBaru;
        bgmSource.Play();
    }
}