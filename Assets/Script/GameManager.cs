using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Pengaturan Hewan")]
    public GameObject animalPrefab;      
    public int requiredClues = 3;        

    [Header("Radius Spawn Hewan (Dari Player)")]
    public float minSpawnRadius = 15f;
    public float maxSpawnRadius = 30f;

    [Header("Validasi Area Spawn (BARU)")]
    [Tooltip("Layer untuk objek batas sungai dan border map")]
    public LayerMask obstacleLayer;
    [Tooltip("Jarak aman hewan dari sungai biar gak kecelup sedikitpun")]
    public float safeRadius = 1f;
    [Tooltip("Titik tengah map")]
    public Vector2 mapCenter = Vector2.zero;
    [Tooltip("Luas maksimal map (X dan Y)")]
    public Vector2 mapSize = new Vector2(100f, 60f);

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
        Vector2 spawnCenter = playerTransform != null ? (Vector2)playerTransform.position : (Vector2)transform.position;

        Vector2 finalSpawnPos = spawnCenter; // Titik default kalau gagal
        bool foundSafeSpot = false;
        int maxAttempts = 30; // Coba cari titik aman maksimal 30 kali

        for (int i = 0; i < maxAttempts; i++)
        {
            // 1. Tentukan titik acak pakai radius
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector2 randomPos = spawnCenter + (randomDirection * randomDistance);

            // 2. Cek apakah titik ini keluar dari kotak biru (batas map)?
            bool isInsideMap =
                randomPos.x >= mapCenter.x - (mapSize.x / 2) &&
                randomPos.x <= mapCenter.x + (mapSize.x / 2) &&
                randomPos.y >= mapCenter.y - (mapSize.y / 2) &&
                randomPos.y <= mapCenter.y + (mapSize.y / 2);

            if (!isInsideMap) continue; // Kalau di luar, lewati dan putar dadu lagi

            // 3. Cek apakah titik ini nabrak sungai/tembok?
            Collider2D hit = Physics2D.OverlapCircle(randomPos, safeRadius, obstacleLayer);

            if (hit == null)
            {
                // Aman! Nggak nabrak apa-apa
                finalSpawnPos = randomPos;
                foundSafeSpot = true;
                break;
            }
        }

        if (!foundSafeSpot)
        {
            Debug.LogWarning("Hewan kesulitan mencari titik aman, memaksakan spawn di titik terdekat.");
        }

        if (animalPrefab != null)
        {
            Instantiate(animalPrefab, finalSpawnPos, Quaternion.identity);
            Debug.Log("Hewan berhasil muncul di: " + finalSpawnPos);
        }
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

    private void OnDrawGizmosSelected()
    {
        // Lingkaran radius (Merah & Hijau)
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, minSpawnRadius);
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, maxSpawnRadius);

        // Kotak biru (Batas Maksimal Map)
        Gizmos.color = new Color(0, 0, 1, 0.3f);
        Gizmos.DrawCube(mapCenter, new Vector3(mapSize.x, mapSize.y, 1f));
    }
}