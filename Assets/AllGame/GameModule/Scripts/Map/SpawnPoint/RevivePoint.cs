using UnityEngine;

public class RevivePoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerManager.Instance.setRespawnPoint(transform.position);
        Destroy(gameObject);
    }
}
