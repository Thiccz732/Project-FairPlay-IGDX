using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AutoSorting : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Pengaturan")]
    [Tooltip("Centang ini untuk benda yang tidak bergerak (seperti Batu/Pohon) agar game lebih enteng")]
    public bool isStatic = false;

    [Tooltip("Geser angka ini jika tumpukannya masih kurang pas")]
    public float yOffset = 0f;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        // Script ini otomatis mengubah angka Order in Layer berdasarkan posisi Y
        // Semakin ke bawah posisinya di layar, angkanya akan semakin besar
        sr.sortingOrder = Mathf.RoundToInt((transform.position.y + yOffset) * -100f);

        // Jika ini benda mati, matikan script setelah menghitung 1 kali agar hemat baterai/CPU
        if (isStatic)
        {
            enabled = false; 
        }
    }
}