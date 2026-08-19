using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggablePhoto : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Identitas Foto")]
    public int photoID; 
    
    [Tooltip("Masukkan SlotFoto yang benar untuk foto ini")]
    public RectTransform targetSlot; 

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private bool isMatched = false; 
    
    // --- VARIABEL BARU UNTUK MENGAKALI BUG MELOMPAT ---
    private Canvas parentCanvas; 

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        // Cari Canvas terdekat secara otomatis
        parentCanvas = GetComponentInParent<Canvas>(); 
    }

    private void OnEnable()
    {
        originalPosition = rectTransform.anchoredPosition;
        isMatched = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isMatched) return; 

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling(); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isMatched) return;

        // --- KODE PERBAIKAN BUG MELOMPAT ---
        // Pergerakan mouse dibagi dengan skala Canvas agar posisi foto akurat!
        if (parentCanvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
        }
        else
        {
            rectTransform.anchoredPosition += eventData.delta;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isMatched) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (targetSlot != null && Vector2.Distance(rectTransform.anchoredPosition, targetSlot.anchoredPosition) <= 75f)
        {
            rectTransform.anchoredPosition = targetSlot.anchoredPosition; 
            isMatched = true; 

            if (GameManager.instance != null) GameManager.instance.AddMatchedPhoto();
        }
        else
        {
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}