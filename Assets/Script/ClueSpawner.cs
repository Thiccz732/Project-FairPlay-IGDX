using UnityEngine;
using System.Collections.Generic;

public class ClueSpawner : MonoBehaviour
{
    public static ClueSpawner instance; // Biar gampang diperintah GameManager

    [Header("Pengaturan Spawner")]
    public GameObject cluePrefab;       
    public Sprite[] clueImages; 
    
    [Tooltip("Jarak minimal antar clue agar tidak menumpuk")]
    public float minimumDistance = 3f;  

    [Header("Luas Area Spawn")]
    public Vector2 spawnAreaSize = new Vector2(20f, 10f); 

    [Header("Validasi Area (Anti-Sungai)")]
    public LayerMask obstacleLayer;
    public float safeRadius = 0.5f;

    // Menyimpan daftar clue yang ada di map saat ini
    private List<GameObject> activeClues = new List<GameObject>();

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    // FUNGSI BARU: Sapu bersih semua clue lama dari map
    public void ClearOldClues()
    {
        foreach (GameObject clue in activeClues)
        {
            if (clue != null) Destroy(clue);
        }
        activeClues.Clear(); // Kosongkan daftar
    }

    // Dipanggil oleh GameManager saat ronde baru dimulai
    public void SpawnClues(int amountToSpawn)
    {
        ClearOldClues(); // Pastikan map bersih dulu!
        
        List<Vector2> spawnedPositions = new List<Vector2>();
        float minDistanceSqr = minimumDistance * minimumDistance;

        for (int i = 0; i < amountToSpawn; i++)
        {
            bool spawned = false;
            int attempts = 0;

            while (!spawned && attempts < 100)
            {
                float randomX = transform.position.x + Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
                float randomY = transform.position.y + Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
                Vector2 randomPos = new Vector2(randomX, randomY);

                Collider2D hitObstacle = Physics2D.OverlapCircle(randomPos, safeRadius, obstacleLayer);
                if (hitObstacle != null)
                {
                    attempts++;
                    continue;
                }

                bool isTooClose = false;
                for (int j = 0; j < spawnedPositions.Count; j++)
                {
                    if ((randomPos - spawnedPositions[j]).sqrMagnitude < minDistanceSqr)
                    {
                        isTooClose = true;
                        break; 
                    }
                }

                if (!isTooClose)
                {
                    GameObject spawnedClue = Instantiate(cluePrefab, randomPos, Quaternion.identity, transform);
                    activeClues.Add(spawnedClue); // Simpan ke daftar absen
                    
                    if (clueImages.Length > 0)
                    {
                        SpriteRenderer sr = spawnedClue.GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            int randomIndex = Random.Range(0, clueImages.Length);
                            sr.sprite = clueImages[randomIndex];
                        }
                    }

                    spawnedPositions.Add(randomPos);
                    spawned = true;
                }
                attempts++;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 1f));
    }
}