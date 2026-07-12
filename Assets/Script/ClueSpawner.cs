using UnityEngine;
using System.Collections.Generic;

public class ClueSpawner : MonoBehaviour
{
    [Header("Pengaturan Spawner")]
    public GameObject cluePrefab;       // Masukkan prefab Clue kamu ke sini
    public int amountToSpawn = 5;       // Mau spawn berapa Clue di area ini?
    
    [Tooltip("Jarak minimal antar Clue agar tidak saling berdempetan")]
    public float minimumDistance = 3f;  

    [Header("Luas Area Spawn")]
    [Tooltip("Ukuran kotak area random (Sumbu X dan Y)")]
    public Vector2 spawnAreaSize = new Vector2(20f, 10f); 

    private void Start()
    {
        SpawnClues();
    }

    private void SpawnClues()
    {
        // Menyimpan kordinat clue yang berhasil di-spawn agar bisa dicek jaraknya
        List<Vector2> spawnedPositions = new List<Vector2>();

        for (int i = 0; i < amountToSpawn; i++)
        {
            bool spawned = false;
            int attempts = 0;

            // Coba cari posisi kosong maksimal 100 kali per clue (mencegah game macet/infinite loop)
            while (!spawned && attempts < 100)
            {
                // 1. Tentukan titik random di dalam batas kotak area
                float randomX = transform.position.x + Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
                float randomY = transform.position.y + Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
                Vector2 randomPos = new Vector2(randomX, randomY);

                // 2. Cek apakah posisi baru ini terlalu dekat dengan clue yang sudah ada
                bool isTooClose = false;
                foreach (Vector2 pos in spawnedPositions)
                {
                    if (Vector2.Distance(randomPos, pos) < minimumDistance)
                    {
                        isTooClose = true;
                        break; // Batal, cari titik lain!
                    }
                }

                // 3. Kalau posisinya aman (jauh dari clue lain), langsung spawn!
                if (!isTooClose)
                {
                    // Spawn clue dan jadikan spawner ini sebagai parent-nya (opsional biar Hierarchy rapi)
                    Instantiate(cluePrefab, randomPos, Quaternion.identity, transform);
                    
                    // Catat posisinya ke daftar
                    spawnedPositions.Add(randomPos);
                    spawned = true;
                }

                attempts++;
            }
            
            // Peringatan kalau areanya terlalu sempit untuk nampung semua clue
            if (!spawned)
            {
                Debug.LogWarning("Gagal spawn clue ke-" + (i+1) + ". Area terlalu sempit atau Minimum Distance terlalu besar!");
            }
        }
    }

    // Fitur tambahan: Menggambar kotak hijau transparan di Scene View
    // Biar kamu bisa lihat batas area spawn-nya secara visual
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 1f));
    }
}