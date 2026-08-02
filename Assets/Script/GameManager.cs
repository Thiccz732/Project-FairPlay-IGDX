using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Pengaturan Hewan")]
    public GameObject animalPrefab;      
    public int requiredClues = 3;        

    [Header("Radius Spawn Hewan (Dari Player)")]
    public float minSpawnRadius = 8f;
    public float maxSpawnRadius = 15f;

    [Header("UI Akhir Game")]
    public GameObject finalPanelUI;      // Panel tempat pasang foto
    public int totalPhotosToMatch = 4;   // 3 Clue + 1 Hewan

    private int cluesFound = 0;
    private int matchedPhotos = 0; // Menghitung foto yang sudah benar di slot
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

        // Sembunyikan panel foto di awal permainan
        if (finalPanelUI != null) finalPanelUI.SetActive(false);
    }

    public void AddClueFound()
    {
        if (isAnimalSpawned) return;

        cluesFound++;
        if (cluesFound >= requiredClues) SpawnAnimal();
    }

    private void SpawnAnimal()
    {
        isAnimalSpawned = true;
        Vector2 spawnCenter = playerTransform != null ? (Vector2)playerTransform.position : (Vector2)transform.position;
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector2 randomSpawnPos = spawnCenter + (randomDirection * randomDistance);

        if (animalPrefab != null) Instantiate(animalPrefab, randomSpawnPos, Quaternion.identity);
    }

    // Fungsi ini dipanggil dari ClueInteract hewan
    public void ShowFinalPanel()
    {
        if (finalPanelUI != null)
        {
            finalPanelUI.SetActive(true);
            
            // Matikan pergerakan player biar gak jalan-jalan pas main puzzle
            if (playerTransform != null) 
                playerTransform.GetComponent<PlayerController>().enabled = false;
        }
    }

    // Fungsi ini dipanggil dari PhotoSlot setiap kali tebakan benar
    public void AddMatchedPhoto()
    {
        matchedPhotos++;
        if (matchedPhotos >= totalPhotosToMatch)
        {
            Debug.Log("SEMUA FOTO COCOK! GAME TAMAT!");
            // Di sini kamu bisa munculkan panel WIN atau Load Scene Credit Title
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f); 
        Gizmos.DrawWireSphere(transform.position, minSpawnRadius);
        Gizmos.color = new Color(0, 1, 0, 0.5f); 
        Gizmos.DrawWireSphere(transform.position, maxSpawnRadius);
    }
}