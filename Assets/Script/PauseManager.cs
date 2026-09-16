using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    [Header("UI Panel & Tombol Single")]
    public GameObject pausePanelUI;
    public Image iconTombolPause;   // Drag komponen Image milik Tombol Pause
    public Sprite spriteIconPause;  // Drag aset Sprite Icon Pause (II)
    public Sprite spriteIconResume; // Drag aset Sprite Icon Resume (Play/X)

    [Header("Pengaturan Scene")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    private void Awake() => instance = this;

    private void Start()
    {
        if (pausePanelUI != null) pausePanelUI.SetActive(false);
        if (iconTombolPause != null && spriteIconPause != null) iconTombolPause.sprite = spriteIconPause;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pausePanelUI != null) pausePanelUI.SetActive(true);
        if (iconTombolPause != null && spriteIconResume != null) iconTombolPause.sprite = spriteIconResume; // Ganti gambar icon
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanelUI != null) pausePanelUI.SetActive(false);
        if (iconTombolPause != null && spriteIconPause != null) iconTombolPause.sprite = spriteIconPause; // Balikin gambar icon
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void MainMenu()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}