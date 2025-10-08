// using UnityEngine;
// using UnityEngine.UI; // untuk Button
// using UnityEngine.XR.ARFoundation; // ARFaceManager, ARTrackablesChangedEventArgs
// using System.Collections.Generic;

// public class FilterManager : MonoBehaviour
// {
//     [Header("Referensi Komponen")]
//     public GenderPredictor genderPredictor;
//     public Button buttonFilter1;
//     public Button buttonFilter2;
//     public ARFaceManager faceManager;

//     [Header("Prefab Filter")]
//     public GameObject filterPrefabMale;
//     public GameObject filterPrefabFemale;

//     private GameObject currentFilterInstance;
//     private Transform currentFaceTransform;
//     private bool uiHasBeenUpdated = false;

//     void Start()
//     {
//         buttonFilter1.interactable = false;
//         buttonFilter2.interactable = false;

//         buttonFilter1.onClick.AddListener(() => ApplyFilter(filterPrefabMale));
//         buttonFilter2.onClick.AddListener(() => ApplyFilter(filterPrefabFemale));
//     }

//     void OnEnable()
//     {
//         if (faceManager != null)
//         {
//             // subscribe ke event baru trackablesChanged
//             faceManager.trackablesChanged.AddListener(OnTrackablesChanged);
//         }
//     }

//     void OnDisable()
//     {
//         if (faceManager != null)
//         {
//             faceManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
//         }
//     }

//     void Update()
//     {
//         if (genderPredictor.IsLocked && !uiHasBeenUpdated)
//         {
//             UpdateButtonsBasedOnGender();
//             uiHasBeenUpdated = true;
//         }
//     }

//     void UpdateButtonsBasedOnGender()
//     {
//         string lockedGender = genderPredictor.LockedGender;
//         Debug.Log("Gender locked: " + lockedGender + ". Updating buttons.");

//         if (lockedGender == "Male")
//         {
//             buttonFilter1.interactable = true;
//             buttonFilter2.interactable = false;
//         }
//         else if (lockedGender == "Female")
//         {
//             buttonFilter1.interactable = false;
//             buttonFilter2.interactable = true;
//         }
//     }

//     // Handler yang kompatibel dengan AR Foundation >= 6.0
//     private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARFace> changes)
//     {
//         // jika ada wajah baru
//         if (changes.added != null && changes.added.Count > 0)
//         {
//             currentFaceTransform = changes.added[0].transform;
//             return;
//         }

//         // jika ada update pada wajah yang sudah ada
//         if (changes.updated != null && changes.updated.Count > 0)
//         {
//             currentFaceTransform = changes.updated[0].transform;
//             return;
//         }

//         // jika semua wajah hilang, bisa bersihkan referensi (opsional)
//         if (changes.removed != null && changes.removed.Count > 0 && 
//             currentFaceTransform != null)
//         {
//             // jika face yang ter-removed adalah face yang kita pakai, clear
//             // (alternatif sederhana: jika tidak ada face tersisa, clear)
//             // Periksa apakah tidak ada face lagi di scene:
//             if (faceManager.trackables == null || faceManager.trackables.count == 0)
//             {
//                 currentFaceTransform = null;
//             }
//         }
//     }

//     public void ApplyFilter(GameObject filterPrefab)
//     {
//         if (currentFilterInstance != null)
//         {
//             Destroy(currentFilterInstance);
//         }

//         if (currentFaceTransform != null && filterPrefab != null)
//         {
//             currentFilterInstance = Instantiate(filterPrefab, currentFaceTransform);
//             currentFilterInstance.transform.localPosition = new Vector3(0, 1f, 0.05f);
//             currentFilterInstance.transform.localRotation = Quaternion.identity;
//         }
//         else
//         {
//             Debug.LogWarning("Cannot apply filter: Face not detected or prefab is missing.");
//         }
//     }
// }

// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Networking;
// using System.Collections;
// using TMPro;
// using UnityEngine.XR.ARFoundation;

// // Kelas ini diubah untuk menampung 'prefab_name'
// [System.Serializable]
// public class FilterInfo
// {
//     public int id;
//     public string name;
//     public string prefab_name; 
// }

// public class FilterManager : MonoBehaviour
// {
//     [Header("Referensi Komponen")]
//     public GenderPredictor genderPredictor;
//     public Button buttonFilter1;
//     public Button buttonFilter2;
//     public ARFaceManager faceManager;

//     [Header("Konfigurasi Tombol")]
//     public string button1Title = "Topi";
//     public string button1Type = "hat";
//     public string button2Title = "Aksesoris";
//     public string button2Type = "accessory";

//     [Header("Konfigurasi API")]
//     public string baseApiUrl = "http://10.157.147.250:3000/filters";

//     private Transform currentFaceTransform;
//     private GameObject currentFilterInstance;

//     void Start()
//     {
//         buttonFilter1.GetComponentInChildren<TextMeshProUGUI>().text = button1Title;
//         buttonFilter2.GetComponentInChildren<TextMeshProUGUI>().text = button2Title;
//         buttonFilter1.onClick.AddListener(() => OnFilterButtonPressed(button1Type));
//         buttonFilter2.onClick.AddListener(() => OnFilterButtonPressed(button2Type));
//         SetButtonsInteractable(false);
//     }

//     void OnEnable()
//     {
//         if (faceManager != null) faceManager.trackablesChanged.AddListener(OnFaceChanged);
//     }

//     void OnDisable()
//     {
//         if (faceManager != null) faceManager.trackablesChanged.RemoveListener(OnFaceChanged);
//     }

//     void Update()
//     {
//         if (genderPredictor.IsLocked && !buttonFilter1.interactable)
//         {
//             SetButtonsInteractable(true);
//         }
//     }

//     public void OnFilterButtonPressed(string filterType)
//     {
//         Debug.Log($"Tombol dengan tipe '{filterType}' BERHASIL DITEKAN!");

//         if (!genderPredictor.IsLocked || currentFaceTransform == null)
//         {
//             Debug.LogWarning("Tombol filter ditekan, tapi gender belum terkunci atau wajah tidak terdeteksi.");
//             return;
//         }
//         StartCoroutine(GetAndApplyFilter(genderPredictor.LockedGender, filterType));
//     }

//     private IEnumerator GetAndApplyFilter(string gender, string filterType)
//     {
//         if (currentFilterInstance != null)
//         {
//             Destroy(currentFilterInstance);
//         }
        
//         string url = $"{baseApiUrl}?gender={gender}&type={filterType.ToLower()}";
//         Debug.Log("Mengambil info filter dari: " + url);

//         using (UnityWebRequest request = UnityWebRequest.Get(url))
//         {
//             yield return request.SendWebRequest();

//             if (request.result == UnityWebRequest.Result.Success)
//             {
//                 string jsonResponse = request.downloadHandler.text;
//                 FilterInfo filterData = JsonUtility.FromJson<FilterInfo>(jsonResponse);

//                 if (!string.IsNullOrEmpty(filterData.prefab_name))
//                 {
//                     // --- INI PERUBAHAN UTAMANYA ---
//                     // Memuat prefab dari folder Resources/PrefabFilter/
//                     string resourcePath = filterData.prefab_name;
//                     GameObject prefabToLoad = Resources.Load<GameObject>(resourcePath);

//                     if (prefabToLoad != null)
//                     {
//                         currentFilterInstance = Instantiate(prefabToLoad, currentFaceTransform);
//                         currentFilterInstance.transform.localPosition = new Vector3(0, 1f, 0.05f);
//                         currentFilterInstance.transform.localRotation = Quaternion.identity;
//                         Debug.Log($"Filter '{filterData.name}' berhasil diterapkan.");
//                     }
//                     else
//                     {
//                         Debug.LogError($"Prefab dengan nama '{filterData.prefab_name}' tidak ditemukan di path '{resourcePath}'.");
//                     }
//                 }
//                 else
//                 {
//                     Debug.LogError($"Tidak ada nama prefab ditemukan untuk gender '{gender}' dan tipe '{filterType}'.");
//                 }
//             }
//             else
//             {
//                 Debug.LogError("Gagal mengambil data filter: " + request.error);
//             }
//         }
//     }

//     private void OnFaceChanged(ARTrackablesChangedEventArgs<ARFace> changes)
//     {
//         bool faceFound = false;
//         foreach (var face in faceManager.trackables) {
//             if (face.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking) {
//                 currentFaceTransform = face.transform;
//                 faceFound = true;
//                 break;
//             }
//         }
//         if (!faceFound) {
//             currentFaceTransform = null;
//             SetButtonsInteractable(false);
//         }
//     }
    
//     private void SetButtonsInteractable(bool isInteractable)
//     {
//         buttonFilter1.interactable = isInteractable;
//         buttonFilter2.interactable = isInteractable;
//     }
// // }

// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Networking;
// using System.Collections;
// using TMPro;
// using UnityEngine.XR.ARFoundation;
// using UnityEngine.XR.ARSubsystems; // Diperlukan untuk TrackingState

// // Kelas untuk menampung data dari API
// [System.Serializable]
// public class FilterInfo
// {
//     public int id;
//     public string name;
//     public string prefab_name; 
// }

// public class FilterManager : MonoBehaviour
// {
//     [Header("Referensi Komponen")]
//     public GenderPredictor genderPredictor;
//     public Button buttonFilter1;
//     public Button buttonFilter2;
//     public ARFaceManager faceManager;

//     [Header("Konfigurasi Tombol")]
//     public string button1Title = "Topi";
//     public string button1Type = "hat";
//     public string button2Title = "Aksesoris";
//     public string button2Type = "accessory";

//     [Header("Konfigurasi API")]
//     public string baseApiUrl = "http://10.157.147.250:3000/filters";

//     private Transform currentFaceTransform;
//     private GameObject currentFilterInstance;
//     private bool uiHasBeenUpdated = false;

//     void Start()
//     {
//         var test = Resources.Load<GameObject>("PrefabFilter/CowboyHat");
//         Debug.Log(test ? "Prefab ditemukan!" : "Prefab TIDAK ditemukan!");
//         // Atur judul tombol
//         buttonFilter1.GetComponentInChildren<TextMeshProUGUI>().text = button1Title;
//         buttonFilter2.GetComponentInChildren<TextMeshProUGUI>().text = button2Title;

//         // Hubungkan fungsi OnClick
//         buttonFilter1.onClick.AddListener(() => OnFilterButtonPressed(button1Type));
//         buttonFilter2.onClick.AddListener(() => OnFilterButtonPressed(button2Type));
        
//         // Sembunyikan dan nonaktifkan tombol pada awalnya
//         SetButtonsActive(false);
//     }

//     void OnEnable()
//     {
//         if (faceManager != null) faceManager.trackablesChanged.AddListener(OnFaceChanged);
//     }

//     void OnDisable()
//     {
//         if (faceManager != null) faceManager.trackablesChanged.RemoveListener(OnFaceChanged);
//     }

//     void Update()
//     {
//         // Jika gender sudah terkunci dan UI belum ditampilkan, tampilkan tombol
//         if (genderPredictor.IsLocked && !uiHasBeenUpdated)
//         {
//             SetButtonsActive(true);
//             uiHasBeenUpdated = true;
//         }
//     }

//     public void OnFilterButtonPressed(string filterType)
//     {
//         Debug.Log($"Tombol dengan tipe '{filterType}' BERHASIL DITEKAN!");
        
//         if (!genderPredictor.IsLocked || currentFaceTransform == null)
//         {
//             Debug.LogWarning("Tombol filter ditekan, tapi gender belum terkunci atau wajah tidak terdeteksi.");
//             return;
//         }
//         StartCoroutine(GetAndApplyFilter(genderPredictor.LockedGender, filterType));
//     }

//     private IEnumerator GetAndApplyFilter(string gender, string filterType)
//     {
//         if (currentFilterInstance != null)
//         {
//             Destroy(currentFilterInstance);
//         }
        
//         // Konsisten menggunakan huruf kecil untuk parameter
//         string url = $"{baseApiUrl}?gender={gender.ToLower()}&type={filterType.ToLower()}";
//         Debug.Log("Mengambil info filter dari: " + url);

//         using (UnityWebRequest request = UnityWebRequest.Get(url))
//         {
//             yield return request.SendWebRequest();

//             if (request.result == UnityWebRequest.Result.Success)
//             {
//                 string jsonResponse = request.downloadHandler.text;
//                 FilterInfo filterData = JsonUtility.FromJson<FilterInfo>(jsonResponse);

//                 if (!string.IsNullOrEmpty(filterData.prefab_name))
//                 {
//                     // --- PERBAIKAN PENTING PADA PATH ---
//                     // Path harus menyertakan nama subfolder di dalam Resources
//                     string resourcePath = "PrefabFilter/" + filterData.prefab_name;
//                     Debug.Log("Mencoba memuat prefab dari path: " + resourcePath);

//                     GameObject prefabToLoad = Resources.Load<GameObject>(resourcePath);

//                     if (prefabToLoad != null)
//                     {
//                         currentFilterInstance = Instantiate(prefabToLoad, currentFaceTransform);
//                         // Atur posisi lokal jika perlu
//                         currentFilterInstance.transform.localPosition = new Vector3(0, 0.1f, 0); 
//                         currentFilterInstance.transform.localRotation = Quaternion.identity;
//                         Debug.Log($"Filter '{filterData.name}' berhasil diterapkan.");
//                     }
//                     else
//                     {
//                         Debug.LogError($"GAGAL MEMUAT PREFAB! Prefab dengan nama '{filterData.prefab_name}' tidak ditemukan di path '{resourcePath}'.");
//                     }
//                 }
//                 else
//                 {
//                     Debug.LogError($"API tidak mengembalikan 'prefab_name' untuk gender '{gender}' dan tipe '{filterType}'.");
//                 }
//             }
//             else
//             {
//                 Debug.LogError("Gagal mengambil data filter: " + request.error);
//             }
//         }
//     }

//     // Logika pelacakan wajah yang disederhanakan
//     private void OnFaceChanged(ARTrackablesChangedEventArgs<ARFace> changes)
//     {
//         // Cari wajah yang sedang aktif
//         Transform activeFace = null;
//         foreach (var face in faceManager.trackables)
//         {
//             if (face.trackingState == TrackingState.Tracking)
//             {
//                 activeFace = face.transform;
//                 break; // Cukup satu wajah saja
//             }
//         }

//         currentFaceTransform = activeFace;

//         // Jika tidak ada wajah aktif, reset UI
//         if (currentFaceTransform == null)
//         {
//             ResetUI();
//         }
//     }
    
//     // Fungsi baru untuk menyembunyikan/menampilkan tombol
//     private void SetButtonsActive(bool isActive)
//     {
//         buttonFilter1.gameObject.SetActive(isActive);
//         buttonFilter2.gameObject.SetActive(isActive);
//     }

//     private void ResetUI()
//     {
//         if(uiHasBeenUpdated)
//         {
//             Debug.Log("Wajah hilang, mereset UI.");
//             uiHasBeenUpdated = false;
//             SetButtonsActive(false);
            
//             if (currentFilterInstance != null)
//             {
//                 Destroy(currentFilterInstance);
//             }
//         }
//     }
// }

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// Kelas untuk menampung data filter dari API
[System.Serializable]
public class FilterInfo
{
    public int id;
    public string name;
    public string prefab_name; // Kunci/Alamat dari prefab di folder Resources
}

public class FilterManager : MonoBehaviour
{
    [Header("Referensi Komponen")]
    public GenderPredictor genderPredictor;
    public Button buttonFilter1;
    public Button buttonFilter2;
    public ARFaceManager faceManager;

    [Header("Konfigurasi API")]
    public string baseApiUrl = "http://10.157.147.250:3000/marker/"; // JANGAN LUPA TRAILING SLASH '/'

    private GameObject currentFilterInstance;
    private Transform currentFaceTransform;
    private bool uiHasBeenUpdated = false;

    void Start()
    {
        // Nonaktifkan tombol pada awalnya
        buttonFilter1.interactable = false;
        buttonFilter2.interactable = false;

        // Atur listener untuk setiap tombol. Tombol 1 akan selalu meminta ID 1. Tombol 2 akan selalu meminta ID 2.
        buttonFilter1.onClick.AddListener(() => OnFilterButtonPressed(1));
        buttonFilter2.onClick.AddListener(() => OnFilterButtonPressed(2));
    }

    void OnEnable()
    {
        if (faceManager != null) faceManager.trackablesChanged.AddListener(OnFaceChanged);
    }

    void OnDisable()
    {
        if (faceManager != null) faceManager.trackablesChanged.RemoveListener(OnFaceChanged);
    }

    void Update()
    {
        // Jika gender sudah terkunci dan UI belum di-update, panggil fungsi untuk mengaktifkan tombol yang sesuai.
        if (genderPredictor.IsLocked && !uiHasBeenUpdated)
        {
            UpdateButtonsBasedOnGender();
            uiHasBeenUpdated = true;
        }
    }

    // Fungsi ini hanya mengatur tombol mana yang bisa ditekan berdasarkan gender.
    void UpdateButtonsBasedOnGender()
    {
        string lockedGender = genderPredictor.LockedGender;
        Debug.Log("Gender locked: " + lockedGender + ". Updating buttons.");

        // Jika Pria, aktifkan Tombol 1. Jika Wanita, aktifkan Tombol 2.
        if (lockedGender == "Male") // Sesuaikan string ini jika hasil dari server berbeda (misal: "male")
        {
            buttonFilter1.interactable = true;
            buttonFilter2.interactable = false;
        }
        else if (lockedGender == "Female") // Sesuaikan string ini jika perlu
        {
            buttonFilter1.interactable = false;
            buttonFilter2.interactable = true;
        }
    }

    // Fungsi ini dipanggil saat tombol ditekan, dengan membawa ID filter yang sudah ditentukan.
    public void OnFilterButtonPressed(int filterId)
    {
        Debug.Log($"Tombol untuk filter ID '{filterId}' ditekan!");
        if (currentFaceTransform != null)
        {
            StartCoroutine(GetAndApplyFilter(filterId));
        }
        else
        {
            Debug.LogWarning("Tombol filter ditekan, tapi wajah tidak terdeteksi.");
        }
    }

    // Coroutine untuk mengambil data dari API dan menerapkan filter.
    private IEnumerator GetAndApplyFilter(int filterId)
    {
        if (currentFilterInstance != null)
        {
            Destroy(currentFilterInstance);
        }
        
        string url = baseApiUrl + filterId;
        Debug.Log("Mengambil info filter dari: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                FilterInfo filterData = JsonUtility.FromJson<FilterInfo>(jsonResponse);

                if (!string.IsNullOrEmpty(filterData.prefab_name))
                {
                    string resourcePath = "PrefabFilter/" + filterData.prefab_name;
                    GameObject prefabToLoad = Resources.Load<GameObject>(resourcePath);

                    if (prefabToLoad != null)
                    {
                        currentFilterInstance = Instantiate(prefabToLoad, currentFaceTransform);
                        currentFilterInstance.transform.localPosition = new Vector3(0, 0.1f, 0); // Sesuaikan posisi jika perlu
                        currentFilterInstance.transform.localRotation = Quaternion.identity;
                        Debug.Log($"Filter '{filterData.name}' berhasil diterapkan.");
                    }
                    else
                    {
                        Debug.LogError($"GAGAL MEMUAT PREFAB! Prefab dengan nama '{filterData.prefab_name}' tidak ditemukan di path '{resourcePath}'.");
                    }
                }
                else
                {
                    Debug.LogError($"API tidak mengembalikan 'prefab_name' untuk filter ID '{filterId}'.");
                }
            }
            else
            {
                Debug.LogError("Gagal mengambil data filter: " + request.error);
            }
        }
    }

    // Melacak posisi wajah yang sedang aktif
    private void OnFaceChanged(ARTrackablesChangedEventArgs<ARFace> changes)
    {
        // Cari wajah yang sedang aktif
        Transform activeFace = null;
        foreach (var face in faceManager.trackables)
        {
            if (face.trackingState == TrackingState.Tracking)
            {
                activeFace = face.transform;
                break;
            }
        }
        currentFaceTransform = activeFace;

        if (currentFaceTransform == null) ResetUI();
    }
    
    // Reset UI jika wajah hilang
    private void ResetUI()
    {
        if(uiHasBeenUpdated)
        {
            uiHasBeenUpdated = false;
            buttonFilter1.interactable = false;
            buttonFilter2.interactable = false;
            if (currentFilterInstance != null) Destroy(currentFilterInstance);
        }
    }
}