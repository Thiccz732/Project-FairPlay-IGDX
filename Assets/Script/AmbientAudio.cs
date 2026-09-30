using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbientAudio : MonoBehaviour
{
    private AudioSource ambientSource;

    private void Awake()
    {
        ambientSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ApplyMute();
    }

    private void Update()
    {
        ApplyMute();
    }

    private void ApplyMute()
    {
        if (ambientSource != null)
        {
            // Ikut status mute Suara/SFX dari PlayerPrefs
            bool isSoundMuted = PlayerPrefs.GetInt("IsSoundMuted", 0) == 1;
            ambientSource.mute = isSoundMuted;
        }
    }
}