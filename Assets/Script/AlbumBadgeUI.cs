using UnityEngine;

public class AlbumBadgeUI : MonoBehaviour
{
    [Header("GameObject Ikon Badge (Sisi Kanan Album)")]
    public GameObject iconHutanHujan;
    public GameObject iconPegunungan;
    public GameObject iconSavanna;
    public GameObject iconMaulSpeedrun;

    private void OnEnable()
    {
        RefreshBadgeDisplay();
    }

    public void RefreshBadgeDisplay()
    {
        // Tampilkan/Hide ikon berdasarkan PlayerPrefs
        if (iconHutanHujan != null)
            iconHutanHujan.SetActive(PlayerPrefs.GetInt("Badge_HutanHujan", 0) == 1);

        if (iconPegunungan != null)
            iconPegunungan.SetActive(PlayerPrefs.GetInt("Badge_Pegunungan", 0) == 1);

        if (iconSavanna != null)
            iconSavanna.SetActive(PlayerPrefs.GetInt("Badge_Savanna", 0) == 1);

        if (iconMaulSpeedrun != null)
            iconMaulSpeedrun.SetActive(PlayerPrefs.GetInt("Badge_MaulSpeedrun", 0) == 1);
    }
}