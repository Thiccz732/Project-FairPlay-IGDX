using UnityEngine;
using UnityEngine.EventSystems;

public class PhotoSlot : MonoBehaviour, IDropHandler
{
    [Header("Target Foto")]
    [Tooltip("Harus sama dengan photoID di DraggablePhoto. Misal: 1 untuk Clue 1")]
    public int expectedPhotoID; 

    private bool isFilled = false;

    public void OnDrop(PointerEventData eventData)
    {
        // Kalau sudah terisi, abaikan
        if (isFilled) return;

        // Cek apakah ada objek yang di-drop
        if (eventData.pointerDrag != null)
        {
            DraggablePhoto photo = eventData.pointerDrag.GetComponent<DraggablePhoto>();
            
            // Kalau foto cocok dengan ID slot ini
            if (photo != null && photo.photoID == expectedPhotoID)
            {
                // Kunci posisi foto ke tengah slot
                photo.transform.SetParent(transform); // Jadikan anak slot
                photo.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Taruh persis di tengah
                
                isFilled = true;
                
                // Lapor ke GameManager kalau ada 1 slot yang berhasil diisi
                GameManager.instance.AddMatchedPhoto();
            }
        }
    }
}