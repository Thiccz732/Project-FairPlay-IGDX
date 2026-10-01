using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using System.Collections; 
using TMPro; 

[System.Serializable]
public class AnimalStage
{
    public string namaHewan = "Hewan 1";
    public GameObject animalPrefab;
    public int requiredClues = 3;
    public Transform[] animalSpawnPoints;
    
    [Header("Waktu Berburu (Detik)")] 
    public float timeLimit = 60f; 
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Identitas Level (Isi di Inspector)")]
    public int currentLevelNumber = 1; // Level 1 diisi 1, Level 2 diisi 2, dst.

    [Header("Sistem Urutan Hewan (Tahapan Level)")]
    public AnimalStage[] animalStages; 
    private int currentStageIndex = 0; 

    [Header("Pengaturan Hewan (Global)")]
    public float teleportInterval = 5f;
    [HideInInspector] public bool isTeleportPaused = false; 
    [HideInInspector] public bool isPhotoModeActive = false; // Penanda mode foto

    [Header("UI Tracker & Pause (Teks yang Selalu Muncul)")] 
    public TextMeshProUGUI teksSisaClue; 
    public TextMeshProUGUI teksTimer; 
    public GameObject tombolPauseUI; // BARU: Masukkan tombol pause di Inspector

    [Header("Fitur Red Flash (Waktu Kritis)")]
    [Tooltip("Image UI full-screen warna merah (Raycast Target di-uncheck)")]
    public Image redFlashOverlay; 
    public float flashSpeed = 5f; // Kecepatan kedip layar
    public float maxAlpha = 0.35f;

    [Header("UI Akhir Game (Susun Foto)")] 
    public GameObject finalPanelUI;      
    public int totalPhotosToMatch = 4;   
    public Image[] draggablePhotoUI; 

    [Header("UI Game Over")]
    public GameObject gameOverPanel; 
    
    [Header("Pengaturan Pindah Scene")]
    public string nextSceneName = "MainMenu"; 

    private Sprite[] capturedSnapshots = new Sprite[10]; 
    private int cluesFound = 0;
    private int matchedPhotos = 0; 
    private bool isAnimalSpawned = false;
    private Transform playerTransform; 

    private GameObject spawnedAnimalInstance;
    private Coroutine teleportCoroutine;
    private bool isStageEnding = false; 

    private float currentTimeLeft;
    [HideInInspector] public bool isTimerRunning = false; 

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (finalPanelUI != null) finalPanelUI.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false); 
        
        if (redFlashOverlay != null)
        {
            Color c = redFlashOverlay.color;
            c.a = 0f;
            redFlashOverlay.color = c;
        }

        if (BadgeManager.instance != null)
        {
            BadgeManager.instance.StartLevelTimer(); // Mulai hitung waktu speedrun
        }

        Invoke(nameof(StartStage), 0.1f);
    }

    private void Update()
    {
        if (isTimerRunning && !isStageEnding)
        {
            currentTimeLeft -= Time.deltaTime;

            if (currentTimeLeft <= 0)
            {
                currentTimeLeft = 0;
                WaktuHabis();
            }

            UpdateUITimer();
            HandleRedFlashOverlay();
        }
    }

    private void HandleRedFlashOverlay()
    {
        if (redFlashOverlay == null) return;

        // Jika waktu sisa <= 10 detik dan stage belum selesai, bikin efek kelap-kelip
        if (currentTimeLeft <= 10f && currentTimeLeft > 0f && !isStageEnding)
        {
            float alpha = Mathf.PingPong(Time.time * flashSpeed, maxAlpha);
            Color c = redFlashOverlay.color;
            c.a = alpha;
            redFlashOverlay.color = c;
        }
        else
        {
            // Matikan warna merah (kembalikan ke transparan)
            Color c = redFlashOverlay.color;
            c.a = 0f;
            redFlashOverlay.color = c;
        }
    }

    private void UpdateUITimer()
    {
        if (teksTimer != null)
        {
            int menit = Mathf.FloorToInt(currentTimeLeft / 60);
            int detik = Mathf.FloorToInt(currentTimeLeft % 60);
            
            teksTimer.text = string.Format("Waktu: {0:00}:{1:00}", menit, detik);
            
            if (currentTimeLeft <= 10f) teksTimer.color = Color.red;
            else teksTimer.color = Color.white;
        }
    }

    private void WaktuHabis()
    {
        isTimerRunning = false;
        
        if (redFlashOverlay != null)
        {
            Color c = redFlashOverlay.color;
            c.a = 0f;
            redFlashOverlay.color = c;
        }

        if (playerTransform != null) playerTransform.GetComponent<PlayerController>().enabled = false;
        
        if (gameOverPanel != null) 
        {
            gameOverPanel.SetActive(true);
        }
        
    }

    private void StartStage()
    {
        cluesFound = 0;
        isAnimalSpawned = false;
        matchedPhotos = 0; 
        isStageEnding = false; 
        
        System.Array.Clear(capturedSnapshots, 0, capturedSnapshots.Length);

        if (redFlashOverlay != null)
        {
            Color c = redFlashOverlay.color;
            c.a = 0f;
            redFlashOverlay.color = c;
        }

        if (ClueSpawner.instance != null && animalStages.Length > 0)
        {
            int amountToSpawn = animalStages[currentStageIndex].requiredClues + 2;
            ClueSpawner.instance.SpawnClues(amountToSpawn);
        }

        if (animalStages.Length > 0)
        {
            currentTimeLeft = animalStages[currentStageIndex].timeLimit;
            isTimerRunning = true;
        }

        UpdateUISisaClue(); 
        SetTrackerUIVisible(true); 
    }

    public void RegisterSnapshot(Sprite snapshot, bool isAnimal)
    {
        if (isAnimal)
        {
            if (spawnedAnimalInstance != null) Destroy(spawnedAnimalInstance);
            if (teleportCoroutine != null) StopCoroutine(teleportCoroutine);

            isTimerRunning = false; 
            capturedSnapshots[9] = snapshot; 
            ShowFinalPanel(); 
        }
        else
        {
            cluesFound++;
            UpdateUISisaClue(); 

            if (cluesFound < capturedSnapshots.Length) capturedSnapshots[cluesFound] = snapshot;

            if (cluesFound >= animalStages[currentStageIndex].requiredClues && !isAnimalSpawned) 
            {
                SpawnAnimal();
            }
        }
    }

    private void UpdateUISisaClue()
    {
        if (teksSisaClue != null && animalStages.Length > 0)
        {
            int targetClue = animalStages[currentStageIndex].requiredClues;
            int sisa = targetClue - cluesFound;

            if (sisa > 0)
            {
                teksSisaClue.text = "Sisa Clue: " + sisa;
            }
            else
            {
                teksSisaClue.text = "Hewan Muncul!";
            }
        }
    }

    private void SpawnAnimal()
    {
        isAnimalSpawned = true;
        AnimalStage currentStage = animalStages[currentStageIndex];

        if (currentStage.animalSpawnPoints != null && currentStage.animalSpawnPoints.Length > 0)
        {
            Transform chosenSpawnPoint = currentStage.animalSpawnPoints[0];
            
            float chance = Random.Range(1f, 100f);

            if (chance <= 45f && playerTransform != null) 
            {
                float closestDistance = Mathf.Infinity;
                foreach (Transform sp in currentStage.animalSpawnPoints)
                {
                    float distance = Vector2.Distance(playerTransform.position, sp.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        chosenSpawnPoint = sp;
                    }
                }
            }
            else
            {
                int randomIndex = Random.Range(0, currentStage.animalSpawnPoints.Length);
                chosenSpawnPoint = currentStage.animalSpawnPoints[randomIndex];
            }

            spawnedAnimalInstance = Instantiate(currentStage.animalPrefab, chosenSpawnPoint.position, Quaternion.identity);
            
            // Menyuntikkan titik kabur ke hewan
            AnimalFOV fovScript = spawnedAnimalInstance.GetComponent<AnimalFOV>();
            if (fovScript != null)
            {
                fovScript.SetEscapePoints(currentStage.animalSpawnPoints, chosenSpawnPoint);
            }

            teleportCoroutine = StartCoroutine(AnimalTeleportRoutine(currentStage.animalSpawnPoints, chosenSpawnPoint));
        }
    }

    private IEnumerator AnimalTeleportRoutine(Transform[] spawnPoints, Transform startingPoint)
    {
        int currentIndex = System.Array.IndexOf(spawnPoints, startingPoint);
        if (currentIndex == -1) currentIndex = 0;

        while (spawnedAnimalInstance != null)
        {
            float timer = 0f;
            while (timer < teleportInterval)
            {
                if (!isTeleportPaused) timer += Time.deltaTime;
                yield return null; 
            }

            if (spawnedAnimalInstance == null) break; 

            currentIndex++;
            if (currentIndex >= spawnPoints.Length) currentIndex = 0;

            spawnedAnimalInstance.transform.position = spawnPoints[currentIndex].position;
        }
    }

    public void PauseTeleport(bool isPaused)
    {
        isTeleportPaused = isPaused;
    }

    private void ShowFinalPanel()
    {
        SetTrackerUIVisible(false); 

        if (redFlashOverlay != null)
        {
            Color c = redFlashOverlay.color;
            c.a = 0f;
            redFlashOverlay.color = c;
        }

        if (finalPanelUI != null)
        {
            finalPanelUI.SetActive(true);
            if (playerTransform != null) playerTransform.GetComponent<PlayerController>().enabled = false;
            
            foreach (var photoImage in draggablePhotoUI)
            {
                DraggablePhoto dragScript = photoImage.GetComponent<DraggablePhoto>();
                if (dragScript != null)
                {
                    int id = dragScript.photoID;
                    
                    Image targetImage = photoImage; 
                    Transform areaDalam = photoImage.transform.Find("IsiFoto"); 
                    if (areaDalam != null)
                    {
                        targetImage = areaDalam.GetComponent<Image>();
                    }

                    if (id == 4 && capturedSnapshots[9] != null) targetImage.sprite = capturedSnapshots[9];
                    else if (id >= 1 && id <= 3 && capturedSnapshots[id] != null) targetImage.sprite = capturedSnapshots[id];
                }
            }
        }
    }

    public void AddMatchedPhoto()
    {
        if (isStageEnding) return; 

        matchedPhotos++;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.snapFotoSound);
        }

        if (matchedPhotos >= totalPhotosToMatch)
        {
            isStageEnding = true; 

            // SFX District Complete
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayDistrictCompleteSFX();
            }

            // --- FIX BADGE 1: Cek Speedrun Badge saat seluruh puzzle foto selesai ---
            if (BadgeManager.instance != null)
            {
                BadgeManager.instance.CheckSpeedrunBadge();
            }

            StartCoroutine(NextStageRoutine());
        }
    }

    private IEnumerator NextStageRoutine()
    {
        yield return new WaitForSeconds(3f);
        LanjutKeHewanBerikutnya();
    }

    private void LanjutKeHewanBerikutnya()
    {
        if (finalPanelUI != null) finalPanelUI.SetActive(false);
        if (playerTransform != null) playerTransform.GetComponent<PlayerController>().enabled = true;

        if (animalStages.Length > 0 && currentStageIndex < animalStages.Length)
        {
            string namaHewanSaatIni = animalStages[currentStageIndex].namaHewan;
            
            string keyKoleksi = "Koleksi_" + namaHewanSaatIni; 
            
            PlayerPrefs.SetInt(keyKoleksi, 1);
            PlayerPrefs.Save();
            Debug.Log("Berhasil menyimpan koleksi: " + keyKoleksi);
        }

        currentStageIndex++;
        
        if (currentStageIndex < animalStages.Length)
        {
            StartStage();
        }
        else
        {
            PlayerPrefs.SetInt("LevelCompleted", currentLevelNumber);

            int levelTerbukaSaatIni = PlayerPrefs.GetInt("LevelUnlocked", 1);
            if (currentLevelNumber + 1 > levelTerbukaSaatIni)
            {
                PlayerPrefs.SetInt("LevelUnlocked", currentLevelNumber + 1);
            }
            
            if (BadgeManager.instance != null)
            {
                // Contoh logic: Jika ini Level 2 Hutan Hujan
                if (currentLevelNumber == 2) 
                {
                    BadgeManager.instance.CheckHutanHujanBadge();
                }
                // Contoh logic: Jika ini Level 4 (Level 2 Hutan Pegunungan)
                else if (currentLevelNumber == 4) 
                {
                    BadgeManager.instance.CheckPegununganBadge();
                }
                // Contoh logic: Jika ini Level 5 (Level Savanna)
                else if (currentLevelNumber == 5) 
                {
                    BadgeManager.instance.CheckSavannaBadge();
                }
            }

            PlayerPrefs.Save();

            if (!string.IsNullOrEmpty(nextSceneName)) SceneManager.LoadScene(nextSceneName);
        }
    }

    public void SetTrackerUIVisible(bool isVisible)
    {
        if (teksSisaClue != null) teksSisaClue.gameObject.SetActive(isVisible);
        if (teksTimer != null) teksTimer.gameObject.SetActive(isVisible);
    }

    public void ToggleModeFoto(bool isKameraAktif)
    {
        isPhotoModeActive = isKameraAktif; 
        SetTrackerUIVisible(!isKameraAktif);

        // FITUR BARU: Menyembunyikan tombol pause saat kamera menyala
        if (tombolPauseUI != null)
        {
            tombolPauseUI.SetActive(!isKameraAktif);
        }
    }
}