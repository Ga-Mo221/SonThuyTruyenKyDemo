using UnityEngine;

public class SceneGame : MonoBehaviour
{
    public void RestartGame()
    {
        SceneController.Instance.ReloadCurrentScene();
        PlayerManager.Instance.loadGameOrCreateNew();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        SceneController.Instance.LoadMap1();
    }

    public void BackToMenu()
    {
        SceneController.Instance.BackToMenu();
        PlayerManager.Instance._knocked = false;
    }
}
