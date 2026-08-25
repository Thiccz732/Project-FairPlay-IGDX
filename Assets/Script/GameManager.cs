using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using System.Collections; 

[System.Serializable]
public class AnimalStage
{
    public string namaHewan = "Hewan 1";
    public GameObject animalPrefab;
    public int requiredClues = 3;
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
    public int totalPhotosToMatch = 4;   
    public Image[] draggablePhotoUI; 
    
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
        isStageEnding = false; 
        
        System.Array.Clear(capturedSnapshots, 0, capturedSnapshots.Length);

        if (ClueSpawner.instance != null && animalStages.Length > 0)
        {
            int amountToSpawn = animalStages[currentStageIndex].requiredClues + 2;
            ClueSpawner.instance.SpawnClues(amountToSpawn);
        }
    }

    public void RegisterSnapshot(Sprite snapshot, bool isAnimal)
    {
        if (isAnimal)
        {
            if (spawnedAnimalInstance != null) Destroy(spawnedAnimalInstance);
            if (teleportCoroutine != null) StopCoroutine(teleportCoroutine);

            capturedSnapshots[9] = snapshot; 
            ShowFinalPanel(); 
        }
        else
        {
            cluesFound++;
            if (cluesFound < capturedSnapshots.Length) capturedSnapshots[cluesFound] = snapshot;

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

        if (currentStage.animalSpawnPoints != null && currentStage.animalSpawnPoints.Length > 0)
        {
            spawnedAnimalInstance = Instantiate(currentStage.animalPrefab, currentStage.animalSpawnPoints[0].position, Quaternion.identity);
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
        Debug.Log("FOTO MASUK SLOT! Skor saat ini: " + matchedPhotos + " / " + totalPhotosToMatch);

        if (matchedPhotos >= totalPhotosToMatch)
        {
            isStageEnding = true; 
            Debug.Log("Semua foto pas! Menunggu 3 detik sebelum lanjut...");
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
            if (!string.IsNullOrEmpty(nextSceneName)) SceneManager.LoadScene(nextSceneName);
            else Debug.LogWarning("Nama Scene selanjutnya belum diisi!");
        }
    }
}