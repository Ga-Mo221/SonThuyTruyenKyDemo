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

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextSmallLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LastSmallLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void LoadMap1()
    {
        SceneManager.LoadSceneAsync("Map1");
    }

    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync("Maimenu");
    }
}
