using UnityEngine;
using UnityEngine.EventSystems;

// Wajib menambahkan CanvasGroup agar foto tidak menghalangi deteksi slot saat ditarik
[RequireComponent(typeof(CanvasGroup))]
public class DraggablePhoto : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Identitas Foto")]
    [Tooltip("Isi 1 untuk Clue 1, 2 untuk Clue 2, dst. Hewan bisa isi 4.")]
    public int photoID; 

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Simpan posisi awal kalau salah taruh
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;

        // Bikin agak transparan saat ditarik dan matikan raycast biar nembus ke slot
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        
        // Pindahkan ke paling depan agar tidak tertumpuk UI lain
        transform.SetAsLastSibling(); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Ikuti posisi kursor/jari
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Kalau dilepas BUKAN di dalam slot yang benar, kembalikan ke tempat asal
        if (transform.parent == originalParent)
        {
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}