using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AnimalProximityAudio : MonoBehaviour
{
    [Header("Pengaturan Jarak Proximity (2D)")]
    public float minDistance = 2f;  // Jarak sangat dekat (Volume Maksimal / 100%)
    public float maxDistance = 10f; // Jarak terjauh (Suara mulai hilang / 0%)

    [Header("Pengaturan Volume")]
    [Range(0f, 1f)]
    public float maxVolume = 1f;

    private AudioSource animalAudio;
    private Transform cameraTransform;

    private void Awake()
    {
        animalAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Cari Main Camera sebagai acuan posisi AudioListener
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Pastikan AudioSource di-set Loop agar suaranya terus berputar saat dekat
        animalAudio.loop = true;
        if (!animalAudio.isPlaying)
        {
            animalAudio.Play();
        }
    }

    private void Update()
    {
        // Cek jika SFX di-mute lewat AudioManager
        bool isSoundMuted = PlayerPrefs.GetInt("IsSoundMuted", 0) == 1;
        if (isSoundMuted)
        {
            animalAudio.volume = 0f;
            return;
        }

        if (cameraTransform == null) return;

        // Hitung jarak 2D saja (abaikan nilai Z biar gak ngaco)
        Vector2 animalPos = transform.position;
        Vector2 cameraPos = cameraTransform.position;
        float distance = Vector2.Distance(animalPos, cameraPos);

        // Hitung persentase volume berdasarkan jarak 2D
        if (distance <= minDistance)
        {
            animalAudio.volume = maxVolume;
        }
        else if (distance >= maxDistance)
        {
            animalAudio.volume = 0f;
        }
        else
        {
            // Interpolasi linear halus antara minDistance dan maxDistance
            float t = (distance - minDistance) / (maxDistance - minDistance);
            animalAudio.volume = Mathf.Lerp(maxVolume, 0f, t);
        }
    }

    // Menampilkan lingkaran visual jarak di Scene View (Editor) biar gampang di-setting
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}