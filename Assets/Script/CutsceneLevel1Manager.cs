using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutsceneLevel1Manager : MonoBehaviour
{
    [Header("Video & UI Settings")]
    public VideoPlayer videoPlayer;
    public GameObject panelCutsceneUI;
    public Button tombolSkip;

    [Header("BGM Audio Source (Drag GameObject BGM ke Sini)")]
    public AudioSource bgmAudioSource;

    [Header("Referensi Script Selanjutnya")]
    public TutorialManager tutorialManager;

    [Header("Testing Mode")]
    [Tooltip("Centang ini jika ingin cutscene SELALU MUNCUL setiap kali di-Play (untuk keperluan tes)")]
    public bool selaluMunculkanSaatTesting = true;

    private bool isCutsceneFinished = false;

    private void Start()
    {
        // Jika mode testing aktif, hapus dulu key simpanannyaf
        if (selaluMunculkanSaatTesting)
        {
            PlayerPrefs.DeleteKey("CutsceneLevel1_Completed");
        }

        // Cek apakah cutscene perlu diputar
        if (PlayerPrefs.GetInt("CutsceneLevel1_Completed", 0) == 0)
        {
            MulaiCutscene();
        }
        else
        {
            SelesaiCutscene();
        }
    }

    public void MulaiCutscene()
    {
        isCutsceneFinished = false;

        if (panelCutsceneUI != null) panelCutsceneUI.SetActive(true);

        if (tombolSkip != null)
        {
            tombolSkip.gameObject.SetActive(true);
            tombolSkip.onClick.RemoveAllListeners();
            tombolSkip.onClick.AddListener(SkipCutscene);
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.isTimerRunning = false;
        }

        // --- PASTI MATI: Stop Langsung AudioSource BGM ---
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Pause();
        }

        if (videoPlayer != null)
        {
            videoPlayer.playbackSpeed = 1f;

    #if UNITY_WEBGL && !UNITY_EDITOR
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "cutscene4.mp4");
    #else
            videoPlayer.source = VideoSource.VideoClip;
    #endif

            videoPlayer.loopPointReached -= OnVideoEnd;
            videoPlayer.loopPointReached += OnVideoEnd;

            videoPlayer.Play();
        }
        else
        {
            SelesaiCutscene();
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        SelesaiCutscene();
    }

    public void SkipCutscene()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
        SelesaiCutscene();
    }

    private void SelesaiCutscene()
    {
        if (isCutsceneFinished) return;
        isCutsceneFinished = true;

        if (!selaluMunculkanSaatTesting)
        {
            PlayerPrefs.SetInt("CutsceneLevel1_Completed", 1);
            PlayerPrefs.Save();
        }

        if (tombolSkip != null) tombolSkip.gameObject.SetActive(false);
        if (panelCutsceneUI != null) panelCutsceneUI.SetActive(false);

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }

        // --- NYALAKAN LAGI BGM PAS CUTSCENE SELESAI ---
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Play();
        }

        if (tutorialManager != null)
        {
            tutorialManager.BukaTutorial();
        }
        else
        {
            if (GameManager.instance != null) GameManager.instance.isTimerRunning = true;
        }
    }
}