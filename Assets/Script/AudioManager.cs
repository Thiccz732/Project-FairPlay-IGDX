using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Pengaturan SFX UI & Game")]
    public AudioSource sfxSource;
    public AudioClip clickSound;
    
    // SFX UI Baru sesuai list asset kamu
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

    // Fungsi universal untuk memutar SFX apa saja
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        // Pastikan sfxSource tidak null dan komponennya ter-enable
        if (sfxSource != null && sfxSource.enabled && clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }
    // Tombol UI tinggal panggil fungsi ini atau PlaySFX(clickSound)
    public void PlayButtonSFX()
    {
        // Cek apakah instance singleton-nya aktif di scene
        if (instance != null)
        {
            instance.PlaySFX(instance.clickSound);
        }
    }

    // Fungsi khusus kalau mau dipanggil langsung dari OnClick UI
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
        if (sfxSource != null && sfxSource.enabled && clip != null)
        {
            // Stop suara langkah sebelumnya biar gak numpuk/dobel!
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