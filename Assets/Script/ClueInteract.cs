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
    public GameObject interactButton;   
    public GameObject clueCamera;       

    private Image whiteFlash;       
    private PlayerController player; 
    
    private GameObject radarUtama; 
    private GameObject joystickUI; 
    private GameObject backgroundUI; 
    private GameObject finalPanelUI; 
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
        hasBeenPhotographed = false;

        sr = GetComponent<SpriteRenderer>();

        if (!isFinalAnimal && pilihanSpriteClue != null && pilihanSpriteClue.Length > 0 && sr != null)
        {
            int indexAcak = Random.Range(0, pilihanSpriteClue.Length);
            sr.sprite = pilihanSpriteClue[indexAcak];
        }

        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (interactButton != null) interactButton.SetActive(false); 
        if (clueCamera != null) clueCamera.SetActive(false);

        GameObject flashObj = GameObject.Find("WhiteFlash");
        if (flashObj != null) whiteFlash = flashObj.GetComponent<Image>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.GetComponent<PlayerController>(); 
        
        GameObject canvasObj = GameObject.Find("Canvas");
        if (canvasObj != null)
        {
            Transform radarTransform = canvasObj.transform.Find("Radar");
            if (radarTransform != null) radarUtama = radarTransform.gameObject;
        }
        
        if (radarUtama == null) radarUtama = GameObject.Find("RadarUI");

        joystickUI = GameObject.Find("Joystick_BG");
        backgroundUI = GameObject.Find("Background"); 
        finalPanelUI = GameObject.Find("FinalPanel"); 

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
            if (!isCameraMode) 
            {
                if (interactPrompt != null) interactPrompt.SetActive(true);
                if (interactButton != null) interactButton.SetActive(true); 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
            if (interactButton != null) interactButton.SetActive(false); 
            if (isCameraMode) ExitCameraMode();
        }
    }

    public void BukaKameraLewatTombolUI()
    {
        if (!isPlayerNear || hasBeenPhotographed) return;

        if (!isCameraMode) 
        {
            EnterCameraMode();
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
            if (interactButton != null) interactButton.SetActive(true); 
        }
    }

    private void EnterCameraMode()
    {
        isCameraMode = true;
        if (interactPrompt != null) interactPrompt.SetActive(false); 
        if (interactButton != null) interactButton.SetActive(false); 
        
        if (player != null) 
        {
            player.enabled = false; 
            
            Renderer[] playerRenderers = player.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in playerRenderers) r.enabled = false;
            Canvas[] playerCanvases = player.GetComponentsInChildren<Canvas>();
            foreach (Canvas c in playerCanvases) c.enabled = false;
        }

        if (clueCamera != null) clueCamera.SetActive(true); 
        
        if (radarUtama != null) radarUtama.SetActive(false);
        if (joystickUI != null) joystickUI.SetActive(false);
        if (backgroundUI != null) backgroundUI.SetActive(false); 
        if (finalPanelUI != null) finalPanelUI.SetActive(false); 
        
        if (isFinalAnimal && GameManager.instance != null) GameManager.instance.PauseTeleport(true);
        if (GameManager.instance != null) GameManager.instance.ToggleModeFoto(true);
    }

    private void ExitCameraMode()
    {
        isCameraMode = false;
        
        if (player != null) 
        {
            player.enabled = true; 
            
            Renderer[] playerRenderers = player.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in playerRenderers) r.enabled = true;
            Canvas[] playerCanvases = player.GetComponentsInChildren<Canvas>();
            foreach (Canvas c in playerCanvases) c.enabled = true;
        }

        if (clueCamera != null) clueCamera.SetActive(false); 
        
        bool isGameEnding = isFinalAnimal && hasBeenPhotographed;
        
        if (!isGameEnding) 
        {
            if (radarUtama != null) radarUtama.SetActive(true);
            if (joystickUI != null) joystickUI.SetActive(true);
            if (backgroundUI != null) backgroundUI.SetActive(true); 
            if (finalPanelUI != null) finalPanelUI.SetActive(true); 
        }
        
        if (isFinalAnimal && GameManager.instance != null) GameManager.instance.PauseTeleport(false);
        if (GameManager.instance != null) GameManager.instance.ToggleModeFoto(false);
    }

    private IEnumerator TakePhotoRoutine()
    {
        hasBeenPhotographed = true;
        
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (interactButton != null) interactButton.SetActive(false);
        if (radarBlip != null) radarBlip.SetActive(false);
        
        if (joystickUI != null) joystickUI.SetActive(false);
        if (backgroundUI != null) backgroundUI.SetActive(false); 
        if (finalPanelUI != null) finalPanelUI.SetActive(false); 

        // 1. Matikan clueCamera agar UI kayu & NVG hilang dari jepretan DAN dari layar flash
        if (clueCamera != null) clueCamera.SetActive(false); 
        
        if (PauseManager.instance != null && PauseManager.instance.iconTombolPause != null)
        {
            PauseManager.instance.iconTombolPause.gameObject.SetActive(false);
        }

        // 2. Tunggu 1 frame lalu jepret foto yang 100% bersih
        yield return new WaitForEndOfFrame();

        Texture2D snapshotTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        snapshotTex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        snapshotTex.Apply(); 

        Sprite newSnapshot = Sprite.Create(snapshotTex, new Rect(0, 0, snapshotTex.width, snapshotTex.height), new Vector2(0.5f, 0.5f));

        // 3. KHUSUS: Nyalakan Canvas milik Player saja agar WhiteFlash bisa tayang
        // ClueCamera dibiarkan tetap MATI agar layar bersih tanpa bingkai kamera.
        if (player != null) 
        {
            Canvas[] playerCanvases = player.GetComponentsInChildren<Canvas>(true);
            foreach (Canvas c in playerCanvases) c.enabled = true;
        }

        // 4. Mainkan efek kilatan cahaya (Kondisi Joystick, Radar, & Bingkai Kamera tersembunyi!)
        if (whiteFlash != null)
        {
            Color flashColor = whiteFlash.color;
            flashColor.a = 1f; 
            whiteFlash.color = flashColor;

            while (whiteFlash.color.a > 0)
            {
                flashColor.a -= Time.deltaTime * 2.5f; 
                whiteFlash.color = flashColor;
                yield return null; 
            }
        }
        
        // 5. Nyalakan kembali tombol pause
        if (PauseManager.instance != null && PauseManager.instance.iconTombolPause != null)
        {
            PauseManager.instance.iconTombolPause.gameObject.SetActive(true);
        }

        // 6. BARU SEKARANG kita memunculkan semua UI Gameplay
        ExitCameraMode();

        // 7. Daftarkan snapshot ke album
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