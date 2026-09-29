using UnityEngine;

public class AnimalFOV : MonoBehaviour
{
    [Header("Pengaturan View Cone")]
    public float viewRadius = 5f;
    [Range(0, 360)]
    public float viewAngle = 90f;

    [Header("Visual & Putaran (Mercusuar)")]
    public Transform fovVisual;
    public float rotationSpeed = 50f;

    [Header("Deteksi & Layer")]
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [Header("Titik Kabur (Diisi oleh GameManager)")]
    public Transform[] movePoints;
    
    private int currentPointIndex = 0;
    private float currentRotation = 0f; 

    private void Update()
    {
        // 1. Cek apakah GameManager ada dan kamera sedang aktif
        if (GameManager.instance != null && GameManager.instance.isPhotoModeActive)
        {
            // Matikan objek cahaya cone jika masih menyala
            if (fovVisual != null && fovVisual.gameObject.activeSelf)
            {
                fovVisual.gameObject.SetActive(false);
            }
            
            // Hentikan fungsi di sini (Hewan berhenti berputar dan berhenti mendeteksi)
            return; 
        }

        // 2. Jika kamera tidak aktif, pastikan objek cahaya cone menyala kembali
        if (fovVisual != null && !fovVisual.gameObject.activeSelf)
        {
            fovVisual.gameObject.SetActive(true);
        }

        // 3. Jalankan logika berputar dan deteksi seperti biasa
        LookAround();      
        FindPlayerInFOV(); 
    }

    public void SetEscapePoints(Transform[] points, Transform startingPoint)
    {
        movePoints = points;
        
        for (int i = 0; i < movePoints.Length; i++)
        {
            if (movePoints[i] == startingPoint)
            {
                currentPointIndex = i;
                break;
            }
        }
    }

    private void LookAround()
    {
        if (fovVisual == null) return;

        currentRotation += rotationSpeed * Time.deltaTime;
        fovVisual.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    private void FindPlayerInFOV()
    {
        Collider2D playerInRadius = Physics2D.OverlapCircle(transform.position, viewRadius, playerMask);
        if (playerInRadius != null)
        {
            Transform target = playerInRadius.transform;
            Vector2 dirToTarget = (target.position - transform.position).normalized;
            
            Vector2 currentFacingDirection = fovVisual != null ? (Vector2)fovVisual.up : Vector2.down;

            if (Vector2.Angle(currentFacingDirection, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector2.Distance(transform.position, target.position);
                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    TeleportToNextPoint(); 
                }
            }
        }
    }

    private void TeleportToNextPoint()
    {
        if (movePoints == null || movePoints.Length == 0) return;
        currentPointIndex = (currentPointIndex + 1) % movePoints.Length;
        transform.position = movePoints[currentPointIndex].position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector2 currentFacing = fovVisual != null ? (Vector2)fovVisual.up : Vector2.down;
        float facingAngle = Mathf.Atan2(currentFacing.y, currentFacing.x) * Mathf.Rad2Deg;

        Vector3 viewAngleA = DirFromAngle(facingAngle, -viewAngle / 2);
        Vector3 viewAngleB = DirFromAngle(facingAngle, viewAngle / 2);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
    }

    private Vector3 DirFromAngle(float currentFacingAngle, float angleOffset)
    {
        float angleInDegrees = currentFacingAngle + angleOffset;
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
    }
}