using UnityEngine;

public class LastScene : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        SceneController.Instance.LastLevel();
    }
}
