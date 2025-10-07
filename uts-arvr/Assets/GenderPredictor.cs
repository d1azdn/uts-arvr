using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using TMPro;

// ===================================================================
// KELAS YANG HILANG SEBELUMNYA, SEKARANG DITAMBAHKAN KEMBALI
[System.Serializable]
public class JobSubmissionResponse
{
    public string event_id;
}
// ===================================================================


// --- Kelas untuk membantu parsing JSON (Struktur Baru) ---

// Kelas ini cocok dengan format: {"Female": 0.1138, "Male": 0.8861}
[System.Serializable]
public class GenderPredictionResult
{
    public float Female;
    public float Male;
}

// Kelas-kelas wrapper untuk struktur Gradio SSE tetap diperlukan
[System.Serializable]
public class SSEDataWrapper
{
    public string msg;
    public GradioOutput output;
    public bool success;
}

[System.Serializable]
public class GradioOutput
{
    // Sekarang 'data' berisi array dari objek GenderPredictionResult
    public GenderPredictionResult[] data;
}


public class GenderPredictor : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    
    private float predictionInterval = 1.0f; // Interval dalam detik
    private string apiBaseUrl = "https://diazdn-face-detection.hf.space/predict";
    private bool isRequestInProgress = false;

    // --- Variabel Baru untuk Fungsi Lock ---
    private bool isLocked = false;
    
    // Variabel ini akan menyimpan hasil yang dikunci. 
    public string LockedGender { get; private set; }


    void Start()
    {
        LockedGender = "N/A"; // Nilai awal
        StartCoroutine(AutoPredictionLoop());
    }

    // Fungsi ini akan dipanggil oleh Tombol "Lock" Anda
    public void LockCurrentPrediction()
    {
        if (!string.IsNullOrEmpty(LockedGender) && LockedGender != "N/A")
        {
            isLocked = true;
            resultText.text = $"LOCKED: {LockedGender}";
            Debug.Log($"Prediction Locked! Final Gender: {LockedGender}");
        }
        else
        {
            Debug.LogWarning("Cannot lock, no valid prediction has been made yet.");
        }
    }

    private IEnumerator AutoPredictionLoop()
    {
        while (true)
        {
            if (!isLocked && !isRequestInProgress)
            {
                StartCoroutine(RunFullPredictionFlow());
            }
            
            yield return new WaitForSeconds(predictionInterval);
        }
    }

    private IEnumerator RunFullPredictionFlow()
{
    isRequestInProgress = true;
    resultText.text = "Detecting...";

    yield return new WaitForEndOfFrame();
    Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
    screenTexture.Apply();
    byte[] imageBytes = screenTexture.EncodeToPNG();
    Destroy(screenTexture);

    yield return StartCoroutine(SubmitPredictionJob(imageBytes, (status) => {}));

    isRequestInProgress = false;
}


    private IEnumerator SubmitPredictionJob(byte[] imageBytes, System.Action<string> onJobSubmitted)
{
    // Bikin form multipart
    WWWForm form = new WWWForm();
    form.AddBinaryData("image", imageBytes, "snapshot.png", "image/png");

    using (UnityWebRequest request = UnityWebRequest.Post(apiBaseUrl, form))
    {
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Response: " + jsonResponse);

            // Karena Flask balas hasil prediksi langsung
            GenderPredictionResult prediction = JsonUtility.FromJson<GenderPredictionResult>(jsonResponse);

            // Langsung proses hasil prediksi
            ProcessPrediction(prediction);

            onJobSubmitted("success"); // dummy id supaya loop lanjut
        }
        else
        {
            Debug.LogError("Error submitting job: " + request.error);
            onJobSubmitted(null);
        }
    }
}


    private IEnumerator PollForResult(string eventId)
    {
        string pollUrl = $"{apiBaseUrl}/{eventId}";

        using (UnityWebRequest request = UnityWebRequest.Get(pollUrl))
        {
            request.SetRequestHeader("Accept", "text/event-stream");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string sseResponse = request.downloadHandler.text;
                string[] lines = sseResponse.Split('\n');
                string finalDataLine = lines.FirstOrDefault(line => line.Contains("\"msg\": \"process_completed\""));

                if (!string.IsNullOrEmpty(finalDataLine))
                {
                    string jsonData = finalDataLine.Substring("data: ".Length);
                    SSEDataWrapper sseData = JsonUtility.FromJson<SSEDataWrapper>(jsonData);
                    
                    if (sseData != null && sseData.output.data.Length > 0)
                    {
                        ProcessPrediction(sseData.output.data[0]);
                    }
                    else
                    {
                         resultText.text = "Prediction data is empty.";
                    }
                }
                else
                {
                    resultText.text = "Prediction timed out or failed.";
                }
            }
            else
            {
                Debug.LogError("Error polling for result: " + request.error);
                resultText.text = "Error: " + request.error;
            }
        }
        isRequestInProgress = false;
    }

    private void ProcessPrediction(GenderPredictionResult prediction)
    {
        string bestLabel;
        float bestConfidence;

        if (prediction.Male > prediction.Female)
        {
            bestLabel = "Male";
            bestConfidence = prediction.Male;
        }
        else
        {
            bestLabel = "Female";
            bestConfidence = prediction.Female;
        }

        LockedGender = bestLabel;

        if (!isLocked)
        {
            resultText.text = $"{bestLabel} ({(bestConfidence * 100):F1}%)";
        }
    }
}