using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 5f;        // Kecepatan maksimal
    public float acceleration = 40f;   // Seberapa cepat mencapai maxSpeed
    public float deceleration = 50f;   // Seberapa cepat berhenti saat tombol dilepas

    private Rigidbody2D rb;
    private Vector2 moveInput;
    
    // Pastikan ini sesuai dengan nama class/file kamu (Player atau Move)
    private Player inputActions; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new Player();

        inputActions.Move.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Move.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void FixedUpdate()
    {
        // 1. Tentukan target kecepatan berdasarkan input
        Vector2 targetVelocity = moveInput * maxSpeed;

        // 2. Cek apakah player sedang menekan tombol atau melepas tombol
        // Kalau input > 0 berarti lagi jalan (pakai akselerasi)
        // Kalau input = 0 berarti lagi diam (pakai deselerasi)
        float accelRate = (moveInput.magnitude > 0) ? acceleration : deceleration;

        // 3. Ubah velocity secara mulus menuju target
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, accelRate * Time.fixedDeltaTime);
    }
}