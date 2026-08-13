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

    // --- SISTEM STATUS LENSA & LEVEL 1 ---
    public enum LensType { Normal, NightVision, Macro }
    [HideInInspector] public LensType currentLens = LensType.Normal;

    [Header("Aturan Level")]
    [Tooltip("Centang jika level ini wajib pakai lensa Normal (Level 1)")]
    public bool mustUseNormalLens = true; 
    // ----------------------------------------------------

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

    // INI DIA FUNGSI YANG DICARI OLEH UNITY:
    public bool CanCapture()
    {
        if (mustUseNormalLens && currentLens != LensType.Normal)
        {
            Debug.Log("Gagal Menjepret! Level ini mewajibkan Lensa Normal.");
            return false; 
        }
        return true; 
    }

    public void HideButtons()
    {
        if (lensButtonPanel != null) lensButtonPanel.SetActive(false);
    }
}