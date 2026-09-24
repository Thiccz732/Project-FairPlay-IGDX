using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Pengaturan SFX UI & Game")]
    public AudioSource sfxSource;
    public AudioClip clickSound;
    
    // SFX UI
    public AudioClip animalFoundSound;
    public AudioClip balikKertasSound;
    public AudioClip cameraShutterSound;
    public AudioClip confirmFotoSound;
    public AudioClip dragFotoSound;
    public AudioClip itemFoundSound;
    public AudioClip snapFotoSound;
    public AudioClip stepsSound;

    [Header("Pengaturan BGM")]
    public AudioSource bgmSource; 

    [Header("UI Tombol Settings (Bisa di-assign per Scene)")]
    public Image btnSoundImage;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    [Space(5)]
    public Image btnMusicImage;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    [Header("Status Audio")]
    public bool isSoundMuted = false;
    public bool isMusicMuted = false;

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
            return;
        }
    }

    private void Start()
    {
        // Load data simpanan (Default 0 = Unmute)
        isSoundMuted = PlayerPrefs.GetInt("IsSoundMuted", 0) == 1;
        isMusicMuted = PlayerPrefs.GetInt("IsMusicMuted", 0) == 1;

        ApplyAudioSettings();
    }

    // Fungsi untuk mendaftarkan UI Tombol jika AudioManager sifatnya DontDestroyOnLoad
    public void RegisterUIButtons(Image soundBtn, Image musicBtn)
    {
        btnSoundImage = soundBtn;
        btnMusicImage = musicBtn;
        UpdateUISprites();
    }

    // Toggle khusus Suara (SFX / Ambient)
    public void ToggleSound()
    {
        isSoundMuted = !isSoundMuted;
        PlayerPrefs.SetInt("IsSoundMuted", isSoundMuted ? 1 : 0);
        PlayerPrefs.Save();

        ApplyAudioSettings();
    }

    // Toggle khusus Musik (BGM)
    public void ToggleMusic()
    {
        isMusicMuted = !isMusicMuted;
        PlayerPrefs.SetInt("IsMusicMuted", isMusicMuted ? 1 : 0);
        PlayerPrefs.Save();

        ApplyAudioSettings();
    }

    private void ApplyAudioSettings()
    {
        // Mute / Unmute AudioSource bawaan Unity
        if (sfxSource != null) sfxSource.mute = isSoundMuted;
        if (bgmSource != null) bgmSource.mute = isMusicMuted;

        UpdateUISprites();
    }

    private void UpdateUISprites()
    {
        // Ganti Sprite Ikon Tombol Suara
        if (btnSoundImage != null)
        {
            if (isSoundMuted && soundOffSprite != null) btnSoundImage.sprite = soundOffSprite;
            else if (!isSoundMuted && soundOnSprite != null) btnSoundImage.sprite = soundOnSprite;
        }

        // Ganti Sprite Ikon Tombol Musik
        if (btnMusicImage != null)
        {
            if (isMusicMuted && musicOffSprite != null) btnMusicImage.sprite = musicOffSprite;
            else if (!isMusicMuted && musicOnSprite != null) btnMusicImage.sprite = musicOnSprite;
        }
    }

    // --- FUNGSI PEMUTAR AUDIO SAMA SEPERTI KODE LAMA KAMU ---

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (sfxSource != null && sfxSource.enabled && clip != null && !isSoundMuted)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayButtonSFX()
    {
        if (instance != null)
        {
            instance.PlaySFX(instance.clickSound);
        }
    }

    public void PlayBalikKertasSFX()
    {
        if (instance != null)
        {
            instance.PlaySFX(instance.balikKertasSound);
        }
    }

    public void PlayAnimalFoundSFX()
    {
        if (instance != null)
        {
            instance.PlaySFX(instance.animalFoundSound);
        }
    }

    public void PlayConfirmFotoSFX()
    {
        if (instance != null)
        {
            instance.PlaySFX(instance.confirmFotoSound);
        }
    }

    public void PlayCameraShutterSFX()
    {
        if (instance != null)
        {
            instance.PlaySFX(instance.cameraShutterSound);
        }
    }

    public void PlayStepSFX(AudioClip clip, float volume = 0.3f)
    {
        if (sfxSource != null && sfxSource.enabled && clip != null && !isSoundMuted)
        {
            sfxSource.Stop(); 
            sfxSource.PlayOneShot(clip, volume);
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