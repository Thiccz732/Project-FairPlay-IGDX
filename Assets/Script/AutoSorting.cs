using UnityEngine;
using UnityEngine.Rendering; // Wajib ditambahkan agar sistem mengenali Sorting Group

public class AutoSorting : MonoBehaviour
{
    [Header("Pengaturan")]
    public bool isStatic = false;
    public float yOffset = 0f;

    private SpriteRenderer sr;
    private SortingGroup sg;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sg = GetComponent<SortingGroup>(); // Mengambil komponen Sorting Group di objek induk
    }

    private void LateUpdate()
    {
        // Menghitung kedalaman
        int order = Mathf.RoundToInt((transform.position.y + yOffset) * -100f);

        // Jika objek ini punya SpriteRenderer (seperti Player), terapkan ke sana
        if (sr != null) sr.sortingOrder = order;
        
        // Jika objek ini adalah Induk yang punya SortingGroup (seperti grup Semak+Batu), terapkan ke sana
        if (sg != null) sg.sortingOrder = order;

        if (isStatic)
        {
            enabled = false; 
        }
    }
}