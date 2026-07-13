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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerControls();

        inputActions.Main.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Main.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable() => inputActions.Enable();
    
    // PERUBAHAN DI SINI: Saat dimatikan oleh clue, rem mendadak!
    private void OnDisable()
    {
        inputActions.Disable();
        if (rb != null) rb.linearVelocity = Vector2.zero; 
    }

    private void FixedUpdate()
    {
        Vector2 targetVelocity = moveInput * maxSpeed;
        float accelRate = (moveInput != Vector2.zero) ? acceleration : deceleration;
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, accelRate * Time.fixedDeltaTime);
    }
}