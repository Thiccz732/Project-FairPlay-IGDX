using UnityEngine;
using System.Collections.Generic;

public class ClueSpawner : MonoBehaviour
{
    [Header("Pengaturan Spawner")]
    public GameObject cluePrefab;       
    
    [Tooltip("Masukkan berbagai macam gambar petunjuk (clue) ke sini")]
    public Sprite[] clueImages; 

    public int amountToSpawn = 5;       
    public float minimumDistance = 3f;  

    [Header("Luas Area Spawn")]
    public Vector2 spawnAreaSize = new Vector2(20f, 10f); 

    [Header("Validasi Area (Anti-Sungai)")]
    [Tooltip("Pilih layer Obstacle agar clue tidak spawn di air")]
    public LayerMask obstacleLayer;
    [Tooltip("Jarak aman clue dari pinggiran sungai")]
    public float safeRadius = 0.5f;

    private void Start()
    {
        SpawnClues();
    }

    private void SpawnClues()
    {
        List<Vector2> spawnedPositions = new List<Vector2>();
        
        float minDistanceSqr = minimumDistance * minimumDistance;

        for (int i = 0; i < amountToSpawn; i++)
        {
            bool spawned = false;
            int attempts = 0;

            // Dibatasi maksimal 100 kali percobaan agar game tidak freeze
            while (!spawned && attempts < 100)
            {
                float randomX = transform.position.x + Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
                float randomY = transform.position.y + Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
                Vector2 randomPos = new Vector2(randomX, randomY);

                // --- 1. CEK APAKAH NABRAK SUNGAI/TEMBOK? ---
                // Kalau ada objek ber-layer Obstacle di titik ini, langsung lewati!
                Collider2D hitObstacle = Physics2D.OverlapCircle(randomPos, safeRadius, obstacleLayer);
                if (hitObstacle != null)
                {
                    attempts++;
                    continue; // Putar dadu lagi dari awal
                }

                // --- 2. CEK JARAK DENGAN CLUE LAIN ---
                bool isTooClose = false;
                for (int j = 0; j < spawnedPositions.Count; j++)
                {
                    if ((randomPos - spawnedPositions[j]).sqrMagnitude < minDistanceSqr)
                    {
                        isTooClose = true;
                        break; 
                    }
                }

                // --- 3. JIKA AMAN, SPAWN CLUE! ---
                if (!isTooClose)
                {
                    GameObject spawnedClue = Instantiate(cluePrefab, randomPos, Quaternion.identity, transform);
                    
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