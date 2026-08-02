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
    public GameObject finalPanelUI;      
    public int totalPhotosToMatch = 4;   

    [Header("Sistem Jepretan Foto")]
    [Tooltip("Tarik 4 objek Image 'Foto Drag' dari UI Final Panel ke sini")]
    public UnityEngine.UI.Image[] draggablePhotoUI; 
    
    // Array memori untuk nyimpen foto asli saat main (Index 1-3 untuk clue, 4 untuk Hewan)
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

    // FUNGSI BARU: Menerima gambar asli dari kamera player
    public void RegisterSnapshot(Sprite snapshot, bool isAnimal)
    {
        if (isAnimal)
        {
            // Simpan foto hewan di index 4
            capturedSnapshots[4] = snapshot;
            ShowFinalPanel(); // Langsung tamat dan buka panel puzzle
        }
        else
        {
            // Tambah clue, lalu simpan fotonya di index 1, 2, atau 3
            cluesFound++;
            if (cluesFound <= 3) 
            {
                capturedSnapshots[cluesFound] = snapshot;
            }

            Debug.Log("Clue " + cluesFound + " Berhasil Difoto!");

            // Kalau clue udah 3, panggil hewan
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
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector2 randomSpawnPos = spawnCenter + (randomDirection * randomDistance);

        if (animalPrefab != null) Instantiate(animalPrefab, randomSpawnPos, Quaternion.identity);
    }

    private void ShowFinalPanel()
    {
        if (finalPanelUI != null)
        {
            finalPanelUI.SetActive(true);
            if (playerTransform != null) playerTransform.GetComponent<PlayerController>().enabled = false;
            
            // Masukkan foto-foto jepretan tadi ke dalam UI Drag & Drop
            foreach (var photoImage in draggablePhotoUI)
            {
                DraggablePhoto dragScript = photoImage.GetComponent<DraggablePhoto>();
                if (dragScript != null)
                {
                    int id = dragScript.photoID;
                    // Kalau fotonya ada di memori, timpa gambar aslinya!
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