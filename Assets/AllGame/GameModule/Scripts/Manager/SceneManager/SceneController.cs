using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    void Awake()
    {
        // Đảm bảo chỉ có một SceneController tồn tại trong game
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Huỷ đối tượng trùng lặp
            return;
        }

        Instance = this;
    }
    
    public void NextSmallLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LastSmallLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
