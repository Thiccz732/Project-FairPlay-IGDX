using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutsceneLevel1Manager : MonoBehaviour
{
    [Header("Video & UI Settings")]
    public VideoPlayer videoPlayer;
    public GameObject panelCutsceneUI;
    public Button tombolSkip;

    [Header("Referensi Script Selanjutnya")]
    public TutorialManager tutorialManager;

    [Header("Testing Mode")]
    [Tooltip("Centang ini jika ingin cutscene SELALU MUNCUL setiap kali di-Play (untuk keperluan tes)")]
    public bool selaluMunculkanSaatTesting = true;

    private bool isCutsceneFinished = false;

    private void Start()
    {
        // Jika mode testing aktif, hapus dulu key simpanannya
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

        if (videoPlayer != null)
        {
            videoPlayer.playbackSpeed = 1f;

            // --- KHUSUS WEBGL / BUILD ---
    #if UNITY_WEBGL && !UNITY_EDITOR
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "cutscene4.mp4");
    #else
            // Di Unity Editor tetap pakai Video Clip biasa
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

        // Simpan status
        if (!selaluMunculkanSaatTesting)
        {
            PlayerPrefs.SetInt("CutsceneLevel1_Completed", 1);
            PlayerPrefs.Save();
        }

        // Matikan UI Cutscene
        if (tombolSkip != null) tombolSkip.gameObject.SetActive(false);
        if (panelCutsceneUI != null) panelCutsceneUI.SetActive(false);

        // Unsubscribe event
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }

        // LANJUTKAN KE TUTORIAL
        if (tutorialManager != null)
        {
            tutorialManager.BukaTutorial();
        }
        else
        {
            // Jika tidak ada tutorialManager, langsung resume timer
            if (GameManager.instance != null) GameManager.instance.isTimerRunning = true;
        }
    }
}