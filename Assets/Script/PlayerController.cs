using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 5f;
    public float acceleration = 40f;
    public float deceleration = 50f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerControls inputActions; 

    // --- FITUR ANIMASI & SPRITE ---
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Mengambil komponen Animator & SpriteRenderer pada objek Player
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        inputActions = new PlayerControls();

        inputActions.Main.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Main.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable() => inputActions.Enable();
    
    // PERUBAHAN DI SINI: Saat dimatikan oleh clue, rem mendadak & hentikan animasi jalan!
    private void OnDisable()
    {
        inputActions.Disable();
        if (rb != null) rb.linearVelocity = Vector2.zero; 
        
        // Pastikan karakter langsung kembali ke pose Idle saat kuncian kamera aktif
        if (anim != null) anim.SetBool("isWalking", false); 
    }

    private void Update()
    {
        // 1. Cek apakah ada input pergerakan
        bool isMoving = moveInput != Vector2.zero;

        // 2. Set bool "isWalking" di Animator sesuai status pergerakan
        if (anim != null)
        {
            anim.SetBool("isWalking", isMoving);
        }

        // 3. Otomatis balik badan (Flip) sesuai arah horizontal Kiri/Kanan
        if (spriteRenderer != null && moveInput.x != 0)
        {
            spriteRenderer.flipX = (moveInput.x > 0);
        }
    }

    private void FixedUpdate()
    {
        Vector2 targetVelocity = moveInput * maxSpeed;
        float accelRate = (moveInput != Vector2.zero) ? acceleration : deceleration;
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, accelRate * Time.fixedDeltaTime);
    }
}