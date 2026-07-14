using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;

    [Header("Pengaturan Hewan")]
    public GameObject animalPrefab;      
    public int requiredClues = 3;        

    [Header("Radius Spawn Hewan (Dari Player)")]
    [Tooltip("Jarak paling dekat hewan boleh muncul (biar gak nabrak player)")]
    public float minSpawnRadius = 8f;
    
    [Tooltip("Jarak paling jauh hewan boleh muncul")]
    public float maxSpawnRadius = 15f;

    private int cluesFound = 0;
    private bool isAnimalSpawned = false;
    private Transform playerTransform; 

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Cari player di awal biar game tetap enteng
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    public void AddClueFound()
    {
        if (isAnimalSpawned) return;

        cluesFound++;
        Debug.Log("Clue Difoto! Total sekarang: " + cluesFound);

        if (cluesFound >= requiredClues)
        {
            SpawnAnimal();
        }
    }

    private void SpawnAnimal()
    {
        isAnimalSpawned = true;

        // Ambil titik pusat dari posisi Player saat ini (kalau playernya ada)
        Vector2 spawnCenter = playerTransform != null ? (Vector2)playerTransform.position : (Vector2)transform.position;

        // 1. Tentukan arah secara acak 360 derajat
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        
        // 2. Tentukan jarak secara acak antara radius minimum dan maksimum
        float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);
        
        // 3. Gabungkan jadi kordinat akhir
        Vector2 randomSpawnPos = spawnCenter + (randomDirection * randomDistance);

        if (animalPrefab != null)
        {
            Instantiate(animalPrefab, randomSpawnPos, Quaternion.identity);
            Debug.Log("Hewan Muncul di jarak: " + randomDistance + " dari player");
        }
    }

    // Menggambar lingkaran bantu di Scene View
    private void OnDrawGizmosSelected()
    {
        // Lingkaran Merah: Batas area terlarang (Hewan nggak akan spawn di dalam sini)
        Gizmos.color = new Color(1, 0, 0, 0.5f); 
        Gizmos.DrawWireSphere(transform.position, minSpawnRadius);

        // Lingkaran Hijau: Batas area maksimal (Hewan akan spawn di antara merah dan hijau)
        Gizmos.color = new Color(0, 1, 0, 0.5f); 
        Gizmos.DrawWireSphere(transform.position, maxSpawnRadius);
    }
}