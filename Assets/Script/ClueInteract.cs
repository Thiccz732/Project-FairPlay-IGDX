using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class ClueInteract : MonoBehaviour
{
    [Header("Tipe Objek (PENTING)")]
    public bool isFinalAnimal = false; 

    [Header("Variasi Gambar Clue")]
    [Tooltip("Masukkan gambar-gambar clue (bulu, biji, dll) ke sini")]
    public Sprite[] pilihanSpriteClue; 
    
    private SpriteRenderer sr;

    [Header("Komponen Bawaan Prefab")]
    public GameObject interactPrompt;   
    public GameObject clueCamera;       

    private static Image sharedWhiteFlash;       
    private static PlayerController sharedPlayer; 
    private static GameObject sharedRadarUI; 
    private static GameObject sharedJoystickUI; 
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
        sr = GetComponent<SpriteRenderer>();

        if (!isFinalAnimal && pilihanSpriteClue != null && pilihanSpriteClue.Length > 0 && sr != null)
        {
            int indexAcak = Random.Range(0, pilihanSpriteClue.Length);
            sr.sprite = pilihanSpriteClue[indexAcak];
        }

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
        if (sharedRadarUI == null) sharedRadarUI = GameObject.Find("RadarUI");
        if (sharedJoystickUI == null) sharedJoystickUI = GameObject.Find("Joystick_BG");

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

    private void TryInteract()
    {
        if (!isPlayerNear || hasBeenPhotographed) return;

        if (!isCameraMode) 
        {
            EnterCameraMode();
        }
        else 
        {
            CameraLensManager lensManager = GetComponentInChildren<CameraLensManager>();
            if (lensManager != null && !lensManager.CanCapture()) return;

            StartCoroutine(TakePhotoRoutine());
        }
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
        if (sharedJoystickUI != null) sharedJoystickUI.SetActive(false);
        
        if (isFinalAnimal && GameManager.instance != null) GameManager.instance.PauseTeleport(true);
    }

    private void ExitCameraMode()
    {
        isCameraMode = false;
        if (sharedPlayer != null) sharedPlayer.enabled = true; 
        if (clueCamera != null) clueCamera.SetActive(false); 
        if (sharedRadarUI != null) sharedRadarUI.SetActive(true);
        if (sharedJoystickUI != null) sharedJoystickUI.SetActive(true);
        
        if (isFinalAnimal && GameManager.instance != null) GameManager.instance.PauseTeleport(false);
    }

    private IEnumerator TakePhotoRoutine()
    {
        hasBeenPhotographed = true;
        
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (radarBlip != null) radarBlip.SetActive(false);
        if (sharedJoystickUI != null) sharedJoystickUI.SetActive(false);

        CameraLensManager lensManager = GetComponentInChildren<CameraLensManager>();
        if (lensManager != null) lensManager.HideButtons();

        yield return new WaitForEndOfFrame();

        // --- PERBAIKAN FORMAT GAMBAR RGBA32 AGAR TIDAK HILANG DI UI ---
        Texture2D snapshotTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        snapshotTex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        snapshotTex.Apply(); 

        Sprite newSnapshot = Sprite.Create(snapshotTex, new Rect(0, 0, snapshotTex.width, snapshotTex.height), new Vector2(0.5f, 0.5f));

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

        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterSnapshot(newSnapshot, isFinalAnimal);
        }
    }

    private void OnMouseDown()
    {
        if (isPlayerNear && !isCameraMode && !hasBeenPhotographed) TryInteract(); 
    }

    public void TombolJepretMobile()
    {
        if (isCameraMode && !hasBeenPhotographed)
        {
            CameraLensManager lensManager = GetComponentInChildren<CameraLensManager>();
            if (lensManager != null && !lensManager.CanCapture()) return;
            StartCoroutine(TakePhotoRoutine());
        }
    }

    public void TombolBatalMobile()
    {
        CancelCamera();
    }
}