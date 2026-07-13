using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class ClueInteract : MonoBehaviour
{
    [Header("Komponen Bawaan Prefab")]
    public GameObject interactPrompt;   
    public GameObject clueCamera;       

    private static Image sharedWhiteFlash;       
    private static PlayerController sharedPlayer; 
    
    private static GameObject sharedRadarUI; 

    private PlayerControls inputActions;
    private bool isPlayerNear = false;
    private bool isCameraMode = false;
    private bool hasBeenPhotographed = false;

    private void Awake()
    {
        inputActions = new PlayerControls(); 
        inputActions.Main.Interact.performed += ctx => TryInteract();
        inputActions.Main.Cancel.performed += ctx => CancelCamera();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (clueCamera != null) clueCamera.SetActive(false);

        if (sharedWhiteFlash == null)
        {
            GameObject flashObj = GameObject.Find("WhiteFlash");
            if (flashObj != null) sharedWhiteFlash = flashObj.GetComponent<Image>();
        }

        if (sharedPlayer == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) sharedPlayer = playerObj.GetComponent<PlayerController>(); 
        }

        // PERUBAHAN DI SINI: Mencari objek radar secara otomatis
        if (sharedRadarUI == null)
        {
            // Catatan: Pastikan nama objek radarmu di Canvas tetap "RawImage" 
            sharedRadarUI = GameObject.Find("RadarUI");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasBeenPhotographed && collision.CompareTag("Player"))
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

    private void CancelCamera()
    {
        if (isCameraMode && !hasBeenPhotographed)
        {
            ExitCameraMode();
            if (interactPrompt != null) interactPrompt.SetActive(true); 
        }
    }

    private void EnterCameraMode()
    {
        isCameraMode = true;
        if (interactPrompt != null) interactPrompt.SetActive(false); 
        if (sharedPlayer != null) sharedPlayer.enabled = false; 
        if (clueCamera != null) clueCamera.SetActive(true); 
        
        // PERUBAHAN DI SINI: Matikan radar saat lagi bidik
        if (sharedRadarUI != null) sharedRadarUI.SetActive(false);
    }

    private void ExitCameraMode()
    {
        isCameraMode = false;
        if (sharedPlayer != null) sharedPlayer.enabled = true; 
        if (clueCamera != null) clueCamera.SetActive(false); 
        
        // PERUBAHAN DI SINI: Nyalakan kembali radar setelah batal/selesai foto
        if (sharedRadarUI != null) sharedRadarUI.SetActive(true);
    }

    private IEnumerator TakePhotoRoutine()
    {
        hasBeenPhotographed = true;
        if (interactPrompt != null) interactPrompt.SetActive(false);

        if (sharedWhiteFlash != null)
        {
            Color flashColor = sharedWhiteFlash.color;
            flashColor.a = 1f; 
            sharedWhiteFlash.color = flashColor;

            while (sharedWhiteFlash.color.a > 0)
            {
                flashColor.a -= Time.deltaTime * 2.5f; 
                sharedWhiteFlash.color = flashColor;
                yield return null; 
            }
        }
        
        ExitCameraMode();
    }
}