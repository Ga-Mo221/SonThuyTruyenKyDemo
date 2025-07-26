using System.Collections.Generic;
using UnityEngine;

public class EnemyRada : MonoBehaviour
{
    public GameObject _player;
    private HashSet<Collider2D> _playerColliders = new HashSet<Collider2D>();

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerColliders.Add(collision);
            _player = collision.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (_playerColliders.Contains(collision))
        {
            _playerColliders.Remove(collision);
            if (_playerColliders.Count == 0)
            {
                _player = null;
            }
        }
    }
}