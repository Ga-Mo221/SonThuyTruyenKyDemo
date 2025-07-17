using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class ActiveDemo : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;


    private string fileUrl = "https://docs.google.com/document/d/16rfUUY8qvkMe9q8qQRt-6OdpuuO4owYfa9Q7sF1C8qw/export?format=txt";
    private string savePath => Path.Combine(Application.persistentDataPath, "Active.txt");

    void Start()
    {
        // Đặt game chạy ở độ phân giải 2K, toàn màn hình
        Screen.SetResolution(2560, 1440, true);

        PlayerManager.Instance._isAlive = false;
        StartCoroutine(CheckInternetConnection((isConnected) =>
        {
            if (isConnected)
            {
                Debug.Log("O-Đã Kết Nối Thành Công.");
                StartCoroutine(DownloadFileCoroutine(fileUrl, savePath));
            }
            else
            {
                Debug.LogError("X-Không có Internet. Không thể tải file.");
            }
        }));
    }

    IEnumerator DownloadFileCoroutine(string url, string savePath)
    {
        using (UnityWebRequest uwr = UnityWebRequest.Get(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"X-Download failed: {uwr.error}");
            }
            else
            {
                File.WriteAllBytes(savePath, uwr.downloadHandler.data);
                Debug.Log("O-Download completed: " + savePath);
                OpenFileCheck();
            }
        }
    }

    private void OpenFileCheck()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogError("X-File không tồn tại: " + savePath);
            return;
        }

        string fileContent = File.ReadAllText(savePath).Trim();

        if (fileContent == "Run = OK")
        {
            Debug.Log("O-Có Thể Chơi Bản Beta");
            PlayerManager.Instance._isAlive = true;
        }
        else
        {
            _canvas.enabled = true;
            PlayerManager.Instance._isAlive = false;
            Debug.LogWarning("X-Bản Beta đã đóng Hẹn Bạn Một Ngày Sớm Nhất");
            Debug.Log(fileContent);
        }
    }

    private IEnumerator CheckInternetConnection(System.Action<bool> callback)
    {
        using (UnityWebRequest uwr = UnityWebRequest.Get("https://www.google.com"))
        {
            uwr.timeout = 5; // Timeout sau 5 giây
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                callback?.Invoke(false);
            }
            else
            {
                callback?.Invoke(true);
            }
        }
    }

    public void outGame()
    {
        Application.Quit();
    }

}
