using UnityEngine;

public class NextScene : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        SceneController.Instance.NextLevel();
    }
}
