using UnityEngine;
using System.Collections; // Wajib ditambahkan untuk sistem timer (Coroutine)

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Pengaturan Hewan")]
    public GameObject animalPrefab;      
    public int requiredClues = 3;        

    [Header("Sistem Teleport Hewan (BARU)")]
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

    // Variabel untuk menyimpan data hewan dan timernya
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

        // Cek apakah slot spawn point sudah diisi di Unity
        if (animalSpawnPoints == null || animalSpawnPoints.Length == 0)
        {
            Debug.LogError("Titik Spawn Hewan belum diisi di GameManager!");
            return;
        }

        if (animalPrefab != null)
        {
            // 1. Munculkan hewan di titik pertama (Index 0)
            spawnedAnimalInstance = Instantiate(animalPrefab, animalSpawnPoints[0].position, Quaternion.identity);
            Debug.Log("Hewan muncul di titik 1");

            // 2. Mulai timer hitung mundur untuk teleportasi
            teleportCoroutine = StartCoroutine(AnimalTeleportRoutine());
        }
    }

    private IEnumerator AnimalTeleportRoutine()
    {
        int currentIndex = 0;

        // Loop ini akan terus berjalan selama hewannya belum difoto
        while (spawnedAnimalInstance != null)
        {
            // Tunggu selama 5 detik
            yield return new WaitForSeconds(teleportInterval);

            // Pindah ke titik selanjutnya
            currentIndex++;

            // Jika sudah mencapai batas akhir (titik ke-4), balik lagi ke titik 1 (Index 0)
            if (currentIndex >= animalSpawnPoints.Length)
            {
                currentIndex = 0;
            }

            // Pindahkan posisi hewannya
            spawnedAnimalInstance.transform.position = animalSpawnPoints[currentIndex].position;
            Debug.Log("Hewan teleport ke titik " + (currentIndex + 1));
        }
    }

    private void ShowFinalPanel()
    {
        // Hentikan timer teleportasi agar hewannya tidak pindah-pindah lagi saat game tamat
        if (teleportCoroutine != null)
        {
            StopCoroutine(teleportCoroutine);
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