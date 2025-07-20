using System.Collections.Generic;
using UnityEngine;

public class EnemyRada : MonoBehaviour
{
    public GameObject _player;

    void OnTriggerEnter2D(Collider2D collision)
    {
        _player = collision.gameObject;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        _player = null;
    }
}
