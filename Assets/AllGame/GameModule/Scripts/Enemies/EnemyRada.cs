using System.Collections.Generic;
using UnityEngine;

public class EnemyRada : MonoBehaviour
{
    public List<Collider2D> _detectedColliders = new List<Collider2D>();
    Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        _detectedColliders.Add(collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        _detectedColliders.Remove(collision);
    }
}
