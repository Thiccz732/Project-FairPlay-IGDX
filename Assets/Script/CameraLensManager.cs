using UnityEngine;
using UnityEngine.UI;

public class CameraLensManager : MonoBehaviour
{
    [Header("Efek Visual Lensa (Layar)")]
    public GameObject nightVisionEffect; 
    public GameObject macroEffect;       

    [Header("UI Tombol")]
    public GameObject lensButtonPanel;   

    [Header("Pengaturan Kamera")]
    public Camera clueCamera;
    private float normalZoom = 5f; 

    public enum LensType { Normal, NightVision, Macro }
    [HideInInspector] public LensType currentLens = LensType.Normal;

    [Header("Aturan Lensa (Centang Salah Satu)")]
    public bool wajibNormal = false; 
    public bool wajibNightVision = false;
    public bool wajibMakro = false;

    private void Start()
    {
        if (clueCamera != null) normalZoom = clueCamera.orthographicSize;
    }

    private void OnEnable()
    {
        SetNormalLens();
        if (lensButtonPanel != null) lensButtonPanel.SetActive(true);
    }

    public void SetNormalLens()
    {
        currentLens = LensType.Normal; 
        if (nightVisionEffect != null) nightVisionEffect.SetActive(false);
        if (macroEffect != null) macroEffect.SetActive(false);
        if (clueCamera != null) clueCamera.orthographicSize = normalZoom;
    }

    public void SetNightVisionLens()
    {
        currentLens = LensType.NightVision; 
        if (nightVisionEffect != null) nightVisionEffect.SetActive(true);
        if (macroEffect != null) macroEffect.SetActive(false);
        if (clueCamera != null) clueCamera.orthographicSize = normalZoom;
    }

    public void SetMacroLens()
    {
        currentLens = LensType.Macro; 
        if (nightVisionEffect != null) nightVisionEffect.SetActive(false);
        if (macroEffect != null) macroEffect.SetActive(true);
        if (clueCamera != null) clueCamera.orthographicSize = normalZoom / 3f; 
    }

    // --- SISTEM VALIDASI YANG BARU ---
    public bool CanCapture()
    {
        if (wajibNormal && currentLens != LensType.Normal)
        {
            Debug.Log("Gagal! Wajib Lensa Normal.");
            return false; 
        }
        if (wajibNightVision && currentLens != LensType.NightVision)
        {
            Debug.Log("Gagal! Wajib Lensa Night Vision.");
            return false; 
        }
        if (wajibMakro && currentLens != LensType.Macro)
        {
            Debug.Log("Gagal! Wajib Lensa Makro.");
            return false; 
        }
        return true; 
    }

    public void HideButtons()
    {
        if (lensButtonPanel != null) lensButtonPanel.SetActive(false);
    }
}