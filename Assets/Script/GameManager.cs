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

    [Header("Sistem Urutan Hewan (Tahapan Level)")]
    public AnimalStage[] animalStages; 
    private int currentStageIndex = 0; 

    [Header("Pengaturan Hewan (Global)")]
    public float teleportInterval = 5f;
    [HideInInspector] public bool isTeleportPaused = false; 

    [Header("UI Tracker (Teks yang Selalu Muncul)")] 
    public TextMeshProUGUI teksSisaClue; 
    public TextMeshProUGUI teksTimer; 

    [Header("UI Akhir Game (Susun Foto)")] 
    public GameObject finalPanelUI;      
    public int totalPhotosToMatch = 4;   
    public Image[] draggablePhotoUI; 

    [Header("UI Game Over")]
    public GameObject gameOverPanel; 
    public TextMeshProUGUI teksRestartCountdown; 
    
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

    // Variabel Timer
    private float currentTimeLeft;
    [HideInInspector] public bool isTimerRunning = false; 

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (finalPanelUI != null) finalPanelUI.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false); 

        StartStage();
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
        }
    }

    private void UpdateUITimer()
    {
        if (teksTimer != null)
        {
            // Format waktu menjadi menit dan detik (00:00)
            int menit = Mathf.FloorToInt(currentTimeLeft / 60);
            int detik = Mathf.FloorToInt(currentTimeLeft % 60);
            
            teksTimer.text = string.Format("Waktu: {0:00}:{1:00}", menit, detik);
            
            // Berubah warna jadi merah saat sisa 10 detik
            if (currentTimeLeft <= 10f) teksTimer.color = Color.red;
            else teksTimer.color = Color.white;
        }
    }

    private void WaktuHabis()
    {
        isTimerRunning = false;
        
        if (playerTransform != null) playerTransform.GetComponent<PlayerController>().enabled = false;
        
        if (gameOverPanel != null) 
        {
            gameOverPanel.SetActive(true);
            StartCoroutine(RestartLevelRoutine());
        }
        else 
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        }
    }

    private IEnumerator RestartLevelRoutine()
    {
        int countdown = 5;
        
        while (countdown > 0)
        {
            if (teksRestartCountdown != null)
            {
                teksRestartCountdown.text = "Mengulang level dalam " + countdown + " detik...";
            }
            
            // Gunakan WaitForSecondsRealtime agar countdown 5 detik kebal terhadap efek Pause/Freeze di game
            yield return new WaitForSecondsRealtime(1f);
            countdown--;
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void StartStage()
    {
        cluesFound = 0;
        isAnimalSpawned = false;
        matchedPhotos = 0; 
        isStageEnding = false; 
        
        System.Array.Clear(capturedSnapshots, 0, capturedSnapshots.Length);

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
                    if (id == 4 && capturedSnapshots[9] != null) photoImage.sprite = capturedSnapshots[9];
                    else if (id >= 1 && id <= 3 && capturedSnapshots[id] != null) photoImage.sprite = capturedSnapshots[id];
                }
            }
        }
    }

    public void AddMatchedPhoto()
    {
        if (isStageEnding) return; 

        matchedPhotos++;
        if (matchedPhotos >= totalPhotosToMatch)
        {
            isStageEnding = true; 
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

        currentStageIndex++;
        
        if (currentStageIndex < animalStages.Length)
        {
            StartStage();
        }
        else
        {
            // --- SIMPAN PROGRESS UNLOCK LEVEL ---
            // Simpan data bahwa Level 2 (atau level selanjutnya) sudah kebuka!
            PlayerPrefs.SetInt("LevelUnlocked", 2); 
            PlayerPrefs.Save();
            // ------------------------------------

            if (!string.IsNullOrEmpty(nextSceneName)) SceneManager.LoadScene(nextSceneName);
        }
    }

    // ==========================================
    // FUNGSI UNTUK MENGONTROL VISIBILITAS UI
    // ==========================================
    public void SetTrackerUIVisible(bool isVisible)
    {
        if (teksSisaClue != null) teksSisaClue.gameObject.SetActive(isVisible);
        if (teksTimer != null) teksTimer.gameObject.SetActive(isVisible);
    }

    // ==========================================
    // FUNGSI UNTUK MODE FOTO (HARDCORE MODE)
    // ==========================================
    public void ToggleModeFoto(bool isKameraAktif)
    {
        SetTrackerUIVisible(!isKameraAktif);
    }
}