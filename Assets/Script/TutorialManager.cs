using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TutorialSlide
{
    public Sprite gambarSlide;
    [TextArea(2, 4)]
    public string teksInstruksi;
}

public class TutorialManager : MonoBehaviour
{
    [Header("Identitas Level/Key Tutorial")]
    [Tooltip("Bedakan key tiap level, contoh: Tutorial_Level1, Tutorial_Level4_NightVision")]
    public string tutorialKey = "Tutorial_Level1";

    [Header("Daftar Slide Tutorial")]
    public TutorialSlide[] daftarSlide; // Untuk Level 4 isi 1 slide, Level 1 isi banyak slide
    private int indexSlide = 0;

    [Header("UI Component")]
    public GameObject panelTutorialUI;
    public Image displayGambar;
    public TextMeshProUGUI teksInstruksi;

    [Header("Navigasi Tombol")]
    public GameObject tombolPrev;
    public Button tombolNextButton;
    public Image imageTombolNext;
    public TextMeshProUGUI teksTombolNext;

    [Header("Sprite Tombol")]
    public Sprite spriteNext;
    public Sprite spriteDone;

    private void Start()
    {
        // Cek apakah tutorial dengan key ini sudah pernah diselesaikan
        // if (PlayerPrefs.GetInt(tutorialKey, 0) == 0)
        // {
        //     BukaTutorial();
        // }
        // else
        // {
        //     if (panelTutorialUI != null) panelTutorialUI.SetActive(false);
        // }

        BukaTutorial(); // Untuk testing, selalu buka tutorial
    }

    public void BukaTutorial()
    {
        indexSlide = 0;
        if (panelTutorialUI != null) panelTutorialUI.SetActive(true);

        // Pause timer game
        if (GameManager.instance != null) GameManager.instance.isTimerRunning = false;

        UpdateSlideUI();
    }

    private void UpdateSlideUI()
    {
        if (daftarSlide.Length == 0) return;

        // 1. Update Tampilan Gambar dan Teks
        TutorialSlide slide = daftarSlide[indexSlide];
        if (displayGambar != null) displayGambar.sprite = slide.gambarSlide;
        if (teksInstruksi != null) teksInstruksi.text = slide.teksInstruksi;

        // 2. Tombol Prev (Mati jika slide pertama atau cuma ada 1 slide)
        if (tombolPrev != null)
        {
            tombolPrev.SetActive(indexSlide > 0);
        }

        // 3. Reset Listener Tombol Next
        if (tombolNextButton != null)
        {
            tombolNextButton.onClick.RemoveAllListeners();

            bool isLastSlide = (indexSlide == daftarSlide.Length - 1);

            if (!isLastSlide)
            {
                // Mode Next
                if (imageTombolNext != null && spriteNext != null) imageTombolNext.sprite = spriteNext;
                if (teksTombolNext != null) teksTombolNext.text = ">";

                tombolNextButton.onClick.AddListener(NextSlide);
            }
            else
            {
                // Mode Done / Main (Otomatis kepanggil walau cuma ada 1 slide!)
                if (imageTombolNext != null && spriteDone != null) imageTombolNext.sprite = spriteDone;
                if (teksTombolNext != null) teksTombolNext.text = "";

                tombolNextButton.onClick.AddListener(TutupTutorial);
            }
        }
    }

    public void NextSlide()
    {
        if (indexSlide < daftarSlide.Length - 1)
        {
            indexSlide++;
            UpdateSlideUI();
        }
    }

    public void PrevSlide()
    {
        if (indexSlide > 0)
        {
            indexSlide--;
            UpdateSlideUI();
        }
    }

    public void TutupTutorial()
    {
        // Simpan status khusus key level ini
        // PlayerPrefs.SetInt(tutorialKey, 1);
        // PlayerPrefs.Save();

        if (panelTutorialUI != null) panelTutorialUI.SetActive(false);

        // Resume timer game
        if (GameManager.instance != null) GameManager.instance.isTimerRunning = true;
    }
}