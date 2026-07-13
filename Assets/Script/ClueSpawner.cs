using UnityEngine;
using System.Collections.Generic;

public class ClueSpawner : MonoBehaviour
{
    [Header("Pengaturan Spawner")]
    public GameObject cluePrefab;       
    public int amountToSpawn = 5;       
    public float minimumDistance = 3f;  

    [Header("Luas Area Spawn")]
    public Vector2 spawnAreaSize = new Vector2(20f, 10f); 

    private void Start()
    {
        SpawnClues();
    }

    private void SpawnClues()
    {
        List<Vector2> spawnedPositions = new List<Vector2>();
        
        // OPTIMISASI: Menghitung jarak menggunakan sqrMagnitude jauh lebih ringan untuk CPU
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

                bool isTooClose = false;
                
                // Cek jarak dengan clue yang sudah ada pakai metode hemat baterai
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
                    Instantiate(cluePrefab, randomPos, Quaternion.identity, transform);
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