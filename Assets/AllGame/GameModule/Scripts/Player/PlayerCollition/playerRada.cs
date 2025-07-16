using System.Collections.Generic;
using UnityEngine;

public class playerRada : MonoBehaviour
{
    public List<Collider2D> _attackColliders = new List<Collider2D>();
    Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        _attackColliders.Add(collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        _attackColliders.Remove(collision);
    }

    public Collider2D GetNearestTarget(Transform origin, out float distance, out Vector2 direction)
    {
        Collider2D nearest = null;
        float minDistance = float.MaxValue;
        Vector2 nearestDirection = Vector2.zero;
        distance = 0f;

        foreach (var col in _attackColliders)
        {
            if (col == null) continue; // Bỏ qua nếu enemy đã chết
            float dx = col.transform.position.x - origin.position.x;
            float absDist = Mathf.Abs(dx); // khoảng cách tuyệt đối để tìm nearest

            if (absDist < minDistance)
            {
                minDistance = absDist;
                nearest = col;
                nearestDirection = (col.transform.position - origin.position).normalized;
                distance = dx;
            }
        }
        
        direction = nearestDirection;
        return nearest;
    }
}
