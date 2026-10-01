using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BadgeManager : MonoBehaviour
{
    public static BadgeManager instance;

    [Header("UI Pop-Up Badge (Optional)")]
    public GameObject badgePopupPanel;
    public Image popupBadgeImage;
    public TextMeshProUGUI popupBadgeTitleText; // atau TextMeshProUGUI jika pakai TMP

    [Header("Sprite Badge")]
    public Sprite badgeHutanHujan;
    public Sprite badgeHutanPegunungan;
    public Sprite badgeSavanna;
    public Sprite badgeMaulSpeedrun;

    private float levelTimer = 0f;
    private bool isTimerRunning = false;

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

    // --- TIMER SPEEDRUN ---
    public void StartLevelTimer()
    {
        levelTimer = 0f;
        isTimerRunning = true;
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            levelTimer += Time.deltaTime;
        }
    }

    public void StopLevelTimer()
    {
        isTimerRunning = false;
    }

    // --- LOGIKA BUKA BADGE (UNLOCKED) ---

    // 1. Badge Hutan Hujan (Setelah 2 Level Hutan Hujan)
    public void CheckHutanHujanBadge()
    {
        if (PlayerPrefs.GetInt("Badge_HutanHujan", 0) == 0)
        {
            PlayerPrefs.SetInt("Badge_HutanHujan", 1);
            PlayerPrefs.Save();
            ShowBadgePopup("Badge Hutan Hujan", badgeHutanHujan);
        }
    }

    // 2. Badge Hutan Pegunungan (Setelah 2 Level Hutan Pegunungan)
    public void CheckPegununganBadge()
    {
        if (PlayerPrefs.GetInt("Badge_Pegunungan", 0) == 0)
        {
            PlayerPrefs.SetInt("Badge_Pegunungan", 1);
            PlayerPrefs.Save();
            ShowBadgePopup("Badge Pegunungan", badgeHutanPegunungan);
        }
    }

    // 3. Badge Savanna (Setelah 1 Level Terakhir Savanna)
    public void CheckSavannaBadge()
    {
        if (PlayerPrefs.GetInt("Badge_Savanna", 0) == 0)
        {
            PlayerPrefs.SetInt("Badge_Savanna", 1);
            PlayerPrefs.Save();
            ShowBadgePopup("Badge Savanna", badgeSavanna);
        }
    }

    // 4. Badge Maul Speedrun (Selesai di bawah 60 detik di level mana saja)
    public void CheckSpeedrunBadge()
    {
        StopLevelTimer();

        // Cek jika waktu di bawah 75 detik dan belum pernah dapet badgenya
        if (levelTimer < 75f && PlayerPrefs.GetInt("Badge_MaulSpeedrun", 0) == 0)
        {
            PlayerPrefs.SetInt("Badge_MaulSpeedrun", 1);
            PlayerPrefs.Save();
            ShowBadgePopup("Badge Kilat Maul", badgeMaulSpeedrun);
        }
    }

    // --- SISTEM POP-UP NOTIFIKASI ---
    private void ShowBadgePopup(string title, Sprite badgeIcon)
    {
        if (badgePopupPanel != null && popupBadgeImage != null)
        {
            if (popupBadgeTitleText != null) popupBadgeTitleText.text = title;
            popupBadgeImage.sprite = badgeIcon;

            badgePopupPanel.SetActive(true);
            StartCoroutine(HidePopupRoutine(3f)); // Otomatis hilang setelah 3 detik
        }
    }

    private IEnumerator HidePopupRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (badgePopupPanel != null) badgePopupPanel.SetActive(false);
    }
}