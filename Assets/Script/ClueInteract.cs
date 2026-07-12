using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class ClueInteract : MonoBehaviour
{
    [Header("Komponen Bawaan Prefab")]
    public GameObject interactPrompt;   
    public GameObject clueCamera;       

    private Image whiteFlashImage;       
    private MonoBehaviour playerController; 
    private PlayerControls inputActions;

    private bool isPlayerNear = false;
    private bool isCameraMode = false;
    private bool hasBeenPhotographed = false;

    private void Awake()
    {
        inputActions = new PlayerControls(); 
        
        // Membaca input tombol Interact (Untuk masuk kamera & jepret)
        inputActions.Main.Interact.performed += ctx => TryInteract();

        // Membaca input tombol Cancel (Tambahan Baru)
        inputActions.Main.Cancel.performed += ctx => CancelCamera();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (clueCamera != null) clueCamera.SetActive(false);

        GameObject flashObj = GameObject.Find("WhiteFlash");
        if (flashObj != null) whiteFlashImage = flashObj.GetComponent<Image>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerController = playerObj.GetComponent<PlayerController>(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasBeenPhotographed)
        {
            isPlayerNear = true;
            if (!isCameraMode && interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
            if (isCameraMode) ExitCameraMode();
        }
    }

    private void OnMouseDown() => TryInteract();

    private void TryInteract()
    {
        if (!isPlayerNear || hasBeenPhotographed) return;

        if (!isCameraMode) EnterCameraMode();
        else StartCoroutine(TakePhotoRoutine());
    }

    // --- FUNGSI CANCEL BARU ---
    private void CancelCamera()
    {
        // Kalau lagi di dalam mode bidik kamera dan BELUM difoto, batalkan!
        if (isCameraMode && !hasBeenPhotographed)
        {
            ExitCameraMode();
            
            // Munculkan lagi teks "Interact"-nya karena masih bisa difoto
            if (interactPrompt != null) interactPrompt.SetActive(true); 
        }
    }

    private void EnterCameraMode()
    {
        isCameraMode = true;
        if (interactPrompt != null) interactPrompt.SetActive(false); // Hilangkan teks saat bidik
        if (playerController != null) playerController.enabled = false; 
        if (clueCamera != null) clueCamera.SetActive(true); 
    }

    private void ExitCameraMode()
    {
        isCameraMode = false;
        if (playerController != null) playerController.enabled = true; 
        if (clueCamera != null) clueCamera.SetActive(false); 
    }

    private IEnumerator TakePhotoRoutine()
    {
        hasBeenPhotographed = true; // Kunci objek ini biar gak bisa difoto lagi
        
        if (interactPrompt != null) interactPrompt.SetActive(false);

        if (whiteFlashImage != null)
        {
            Color flashColor = whiteFlashImage.color;
            flashColor.a = 1f; 
            whiteFlashImage.color = flashColor;

            while (whiteFlashImage.color.a > 0)
            {
                flashColor.a -= Time.deltaTime * 2.5f; 
                whiteFlashImage.color = flashColor;
                yield return null; 
            }
        }
        
        ExitCameraMode();
    }
}