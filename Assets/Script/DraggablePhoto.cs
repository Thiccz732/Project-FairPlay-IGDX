using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggablePhoto : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Identitas Foto")]
    public int photoID; 

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform; 
    private Vector2 originalAnchoredPosition; 
    private bool isMatched = false; 

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        
        // PERBAIKAN: Ingat posisi asli HANYA SEKALI saat game pertama kali dijalankan
        originalAnchoredPosition = rectTransform.anchoredPosition; 
    }

    private void OnEnable()
    {
        // PERBAIKAN: Setiap kali buku dibuka (untuk hewan ke-2, ke-3, dst), paksa foto pulang ke posisi kiri!
        rectTransform.anchoredPosition = originalAnchoredPosition; 
        
        isMatched = false; // Buka kuncian foto
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
        transform.position += (Vector3)eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isMatched) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        GameObject targetSlotObj = GameObject.Find("SlotFoto_" + photoID);
        
        if (targetSlotObj != null)
        {
            RectTransform targetSlot = targetSlotObj.GetComponent<RectTransform>();

            if (targetSlot != null && Vector2.Distance(rectTransform.anchoredPosition, targetSlot.anchoredPosition) <= 75f)
            {
                rectTransform.anchoredPosition = targetSlot.anchoredPosition;
                isMatched = true; 

                if (GameManager.instance != null) GameManager.instance.AddMatchedPhoto();
                return; 
            }
        }

        rectTransform.anchoredPosition = originalAnchoredPosition;
    }
}