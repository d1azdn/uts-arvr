using UnityEngine;
using UnityEngine.UI; // untuk Button
using UnityEngine.XR.ARFoundation; // ARFaceManager, ARTrackablesChangedEventArgs
using System.Collections.Generic;

public class FilterManager : MonoBehaviour
{
    [Header("Referensi Komponen")]
    public GenderPredictor genderPredictor;
    public Button buttonFilter1;
    public Button buttonFilter2;
    public ARFaceManager faceManager;

    [Header("Prefab Filter")]
    public GameObject filterPrefabMale;
    public GameObject filterPrefabFemale;

    private GameObject currentFilterInstance;
    private Transform currentFaceTransform;
    private bool uiHasBeenUpdated = false;

    void Start()
    {
        buttonFilter1.interactable = false;
        buttonFilter2.interactable = false;

        buttonFilter1.onClick.AddListener(() => ApplyFilter(filterPrefabMale));
        buttonFilter2.onClick.AddListener(() => ApplyFilter(filterPrefabFemale));
    }

    void OnEnable()
    {
        if (faceManager != null)
        {
            // subscribe ke event baru trackablesChanged
            faceManager.trackablesChanged.AddListener(OnTrackablesChanged);
        }
    }

    void OnDisable()
    {
        if (faceManager != null)
        {
            faceManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
        }
    }

    void Update()
    {
        if (genderPredictor.IsLocked && !uiHasBeenUpdated)
        {
            UpdateButtonsBasedOnGender();
            uiHasBeenUpdated = true;
        }
    }

    void UpdateButtonsBasedOnGender()
    {
        string lockedGender = genderPredictor.LockedGender;
        Debug.Log("Gender locked: " + lockedGender + ". Updating buttons.");

        if (lockedGender == "Male")
        {
            buttonFilter1.interactable = true;
            buttonFilter2.interactable = false;
        }
        else if (lockedGender == "Female")
        {
            buttonFilter1.interactable = false;
            buttonFilter2.interactable = true;
        }
    }

    // Handler yang kompatibel dengan AR Foundation >= 6.0
    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARFace> changes)
    {
        // jika ada wajah baru
        if (changes.added != null && changes.added.Count > 0)
        {
            currentFaceTransform = changes.added[0].transform;
            return;
        }

        // jika ada update pada wajah yang sudah ada
        if (changes.updated != null && changes.updated.Count > 0)
        {
            currentFaceTransform = changes.updated[0].transform;
            return;
        }

        // jika semua wajah hilang, bisa bersihkan referensi (opsional)
        if (changes.removed != null && changes.removed.Count > 0 && 
            currentFaceTransform != null)
        {
            // jika face yang ter-removed adalah face yang kita pakai, clear
            // (alternatif sederhana: jika tidak ada face tersisa, clear)
            // Periksa apakah tidak ada face lagi di scene:
            if (faceManager.trackables == null || faceManager.trackables.count == 0)
            {
                currentFaceTransform = null;
            }
        }
    }

    public void ApplyFilter(GameObject filterPrefab)
    {
        if (currentFilterInstance != null)
        {
            Destroy(currentFilterInstance);
        }

        if (currentFaceTransform != null && filterPrefab != null)
        {
            currentFilterInstance = Instantiate(filterPrefab, currentFaceTransform);
            currentFilterInstance.transform.localPosition = new Vector3(0, 1f, 0.05f);
            currentFilterInstance.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("Cannot apply filter: Face not detected or prefab is missing.");
        }
    }
}
