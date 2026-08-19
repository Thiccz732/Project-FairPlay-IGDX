using UnityEngine;
using UnityEngine.UI; // TAMBAHAN: Agar script mengenali komponen Button
using System.Collections; 

[System.Serializable]
public class AnimalStage
{
    public string namaHewan = "Hewan 1";
    public GameObject animalPrefab;
    [Tooltip("Berapa clue yang harus difoto untuk hewan ini?")]
    public int requiredClues = 3;
    [Tooltip("Titik teleportasi khusus untuk hewan ini")]
    public Transform[] animalSpawnPoints;
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

    [Header("UI Akhir Game")]
    public GameObject finalPanelUI;      
    public Button nextButton;            // VARIABEL BARU: Referensi ke tombol "Lanjut"
    public int totalPhotosToMatch = 4;   
    public Image[] draggablePhotoUI; 
    
    private Sprite[] capturedSnapshots = new Sprite[10]; 
    private int cluesFound = 0;
    private int matchedPhotos = 0; 
    private bool isAnimalSpawned = false;
    private Transform playerTransform; 

    private GameObject spawnedAnimalInstance;
    private Coroutine teleportCoroutine;

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

        StartStage();
    }

    private void StartStage()
    {
        cluesFound = 0;
        isAnimalSpawned = false;
        matchedPhotos = 0; 
        
        System.Array.Clear(capturedSnapshots, 0, capturedSnapshots.Length);

        if (ClueSpawner.instance != null && animalStages.Length > 0)
        {
            int amountToSpawn = animalStages[currentStageIndex].requiredClues + 2;
            ClueSpawner.instance.SpawnClues(amountToSpawn);
        }
        
        Debug.Log("== MULAI MENCARI: " + animalStages[currentStageIndex].namaHewan + " ==");
    }

    public void RegisterSnapshot(Sprite snapshot, bool isAnimal)
    {
        if (isAnimal)
        {
            if (spawnedAnimalInstance != null) Destroy(spawnedAnimalInstance);
            if (teleportCoroutine != null) StopCoroutine(teleportCoroutine);

            capturedSnapshots[9] = snapshot; 
            
            Debug.Log(animalStages[currentStageIndex].namaHewan + " BERHASIL DITANGKAP!");

            ShowFinalPanel(); 
        }
        else
        {
            cluesFound++;
            if (cluesFound < capturedSnapshots.Length) 
            {
                capturedSnapshots[cluesFound] = snapshot;
            }

            Debug.Log("Clue " + cluesFound + " Berhasil Difoto!");

            if (cluesFound >= animalStages[currentStageIndex].requiredClues && !isAnimalSpawned) 
            {
                SpawnAnimal();
            }
        }
    }

    private void SpawnAnimal()
    {
        isAnimalSpawned = true;
        AnimalStage currentStage = animalStages[currentStageIndex];

        if (currentStage.animalSpawnPoints == null || currentStage.animalSpawnPoints.Length == 0)
        {
            Debug.LogError("Titik Spawn Hewan belum diisi di Level ini!");
            return;
        }

        if (currentStage.animalPrefab != null)
        {
            spawnedAnimalInstance = Instantiate(currentStage.animalPrefab, currentStage.animalSpawnPoints[0].position, Quaternion.identity);
            Debug.Log(currentStage.namaHewan + " muncul!");

            teleportCoroutine = StartCoroutine(AnimalTeleportRoutine(currentStage.animalSpawnPoints));
        }
    }

    private IEnumerator AnimalTeleportRoutine(Transform[] spawnPoints)
    {
        int currentIndex = 0;
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
        if (finalPanelUI != null)
        {
            finalPanelUI.SetActive(true);
            if (playerTransform != null) playerTransform.GetComponent<PlayerController>().enabled = false;
            
            // --- KODE BARU: Matikan/redupkan tombol Lanjut saat panel baru muncul! ---
            if (nextButton != null) nextButton.interactable = false;
            // -------------------------------------------------------------------------

            foreach (var photoImage in draggablePhotoUI)
            {
                DraggablePhoto dragScript = photoImage.GetComponent<DraggablePhoto>();
                if (dragScript != null)
                {
                    int id = dragScript.photoID;
                    if (id == 4 && capturedSnapshots[9] != null) 
                    {
                        photoImage.sprite = capturedSnapshots[9];
                    }
                    else if (id >= 1 && id <= 3 && capturedSnapshots[id] != null)
                    {
                        photoImage.sprite = capturedSnapshots[id];
                    }
                }
            }
        }
    }

    public void LanjutKeHewanBerikutnya()
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
            Debug.Log("SELURUH HEWAN DI LEVEL INI SUDAH HABIS DITANGKAP! TAMAT!");
        }
    }

    public void AddMatchedPhoto()
    {
        matchedPhotos++;
        if (matchedPhotos >= totalPhotosToMatch)
        {
            Debug.Log("SEMUA FOTO COCOK!");
            
            // --- KODE BARU: Nyalakan kembali tombolnya saat semua foto sudah beres! ---
            if (nextButton != null) nextButton.interactable = true;
            // --------------------------------------------------------------------------
        }
    }
}