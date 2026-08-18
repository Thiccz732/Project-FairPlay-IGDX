using UnityEngine;
using System.Collections; 

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Pengaturan Hewan")]
    public GameObject animalPrefab;      
    public int requiredClues = 3;        

    [Header("Sistem Teleport Hewan")]
    [Tooltip("Masukkan 4 titik lokasi (Empty GameObject) ke sini")]
    public Transform[] animalSpawnPoints;
    [Tooltip("Waktu tunggu sebelum hewan pindah (detik)")]
    public float teleportInterval = 5f;

    [Header("UI Akhir Game")]
    public GameObject finalPanelUI;      
    public int totalPhotosToMatch = 4;   

    [Header("Sistem Jepretan Foto")]
    public UnityEngine.UI.Image[] draggablePhotoUI; 
    
    private Sprite[] capturedSnapshots = new Sprite[5]; 
    private int cluesFound = 0;
    private int matchedPhotos = 0; 
    private bool isAnimalSpawned = false;
    private Transform playerTransform; 

    private GameObject spawnedAnimalInstance;
    private Coroutine teleportCoroutine;

    // --- Variabel untuk Pause Teleport ---
    [HideInInspector] public bool isTeleportPaused = false; 

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
    }

    public void RegisterSnapshot(Sprite snapshot, bool isAnimal)
    {
        if (isAnimal)
        {
            capturedSnapshots[4] = snapshot;
            ShowFinalPanel(); 
        }
        else
        {
            cluesFound++;
            if (cluesFound <= 3) 
            {
                capturedSnapshots[cluesFound] = snapshot;
            }

            Debug.Log("Clue " + cluesFound + " Berhasil Difoto!");

            if (cluesFound >= requiredClues && !isAnimalSpawned) 
            {
                SpawnAnimal();
            }
        }
    }

    private void SpawnAnimal()
    {
        isAnimalSpawned = true;

        if (animalSpawnPoints == null || animalSpawnPoints.Length == 0)
        {
            Debug.LogError("Titik Spawn Hewan belum diisi di GameManager!");
            return;
        }

        if (animalPrefab != null)
        {
            spawnedAnimalInstance = Instantiate(animalPrefab, animalSpawnPoints[0].position, Quaternion.identity);
            Debug.Log("Hewan muncul di titik 1");

            teleportCoroutine = StartCoroutine(AnimalTeleportRoutine());
        }
    }

    // --- Sistem Timer Teleport dengan Pause ---
    private IEnumerator AnimalTeleportRoutine()
    {
        int currentIndex = 0;

        while (spawnedAnimalInstance != null)
        {
            float timer = 0f;
            while (timer < teleportInterval)
            {
                // Timer hanya berjalan kalau tidak sedang di-pause
                if (!isTeleportPaused) 
                {
                    timer += Time.deltaTime;
                }
                yield return null; 
            }

            if (spawnedAnimalInstance == null) break; 

            currentIndex++;
            if (currentIndex >= animalSpawnPoints.Length) currentIndex = 0;

            spawnedAnimalInstance.transform.position = animalSpawnPoints[currentIndex].position;
            Debug.Log("Hewan teleport ke titik " + (currentIndex + 1));
        }
    }

    // --- Fungsi Pause yang dipanggil oleh Kamera ---
    public void PauseTeleport(bool isPaused)
    {
        isTeleportPaused = isPaused;
        if (isPaused) Debug.Log("Teleport Hewan Di-pause (Player sedang membidik)!");
        else Debug.Log("Teleport Hewan Dilanjutkan!");
    }

    private void ShowFinalPanel()
    {
        if (teleportCoroutine != null) StopCoroutine(teleportCoroutine);

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
                    if (id >= 1 && id <= 4 && capturedSnapshots[id] != null)
                    {
                        photoImage.sprite = capturedSnapshots[id];
                    }
                }
            }
        }
    }

    public void AddMatchedPhoto()
    {
        matchedPhotos++;
        if (matchedPhotos >= totalPhotosToMatch)
        {
            Debug.Log("SEMUA FOTO COCOK! GAME TAMAT!");
        }
    }
}