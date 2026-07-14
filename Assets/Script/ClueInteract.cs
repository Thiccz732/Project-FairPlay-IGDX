using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class ClueInteract : MonoBehaviour
{
    [Header("Tipe Objek (PENTING)")]
    [Tooltip("Centang ini HANYA JIKA objek ini adalah Hewan Utama yang harus difoto untuk menang")]
    public bool isFinalAnimal = false; 

    [Header("Komponen Bawaan Prefab")]
    public GameObject interactPrompt;   
    public GameObject clueCamera;       

    private static Image sharedWhiteFlash;       
    private static PlayerController sharedPlayer; 
    private static GameObject sharedRadarUI; 

    private GameObject radarBlip;

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

        if (sharedRadarUI == null)
        {
            sharedRadarUI = GameObject.Find("RadarUI");
        }

        // Cari objek radar blip. Karena namanya mungkin ClueBip atau AnimalBip,
        // kita cari manual dari daftar anak-anaknya.
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Bip") || child.name.Contains("Blip"))
            {
                radarBlip = child.gameObject;
                break;
            }
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
        if (sharedRadarUI != null) sharedRadarUI.SetActive(false);
    }

    private void ExitCameraMode()
    {
        isCameraMode = false;
        if (sharedPlayer != null) sharedPlayer.enabled = true; 
        if (clueCamera != null) clueCamera.SetActive(false); 
        if (sharedRadarUI != null) sharedRadarUI.SetActive(true);
    }

    private IEnumerator TakePhotoRoutine()
    {
        hasBeenPhotographed = true;
        if (interactPrompt != null) interactPrompt.SetActive(false);

        if (radarBlip != null) radarBlip.SetActive(false);

        if (GameManager.instance != null)
        {
            if (isFinalAnimal)
            {
                // Kalau ini hewan, berarti game tamat!
                Debug.Log("=================================");
                Debug.Log("HEWAN BERHASIL DIFOTO! YOU WIN!");
                Debug.Log("=================================");
                // Nanti kamu bisa tambahkan kode di GameManager untuk memunculkan layar menang (Win Screen)
            }
            else
            {
                // Kalau ini clue biasa, tambahkan poin
                GameManager.instance.AddClueFound();
            }
        }

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