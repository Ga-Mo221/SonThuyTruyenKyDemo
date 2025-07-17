using UnityEngine;
using System.Collections.Generic;

public class HinhNom : MonoBehaviour
{
    public float mau = 100f;
    private Animator animator;
    private Rigidbody2D rb;

    // check gorund
    public bool _isGrounded;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;

    [SerializeField] private HinhNomCollition col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        checkGround();
    }

    private void checkGround()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer) && rb.linearVelocity.y == 0;
        if (_isGrounded)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void hit(float damage, int ID)
    {
        mau -= damage;
        animator.SetTrigger("bidanh");
        float flyPower = 1f;
        float nearestDistance;
        Vector2 directionToEnemy;
        Collider2D nearestEnemy = GetNearestTarget(transform, col.detectedColliders, out nearestDistance, out directionToEnemy);
        switch (ID)
        {
            case 1:
                flyPower = 3f;
                break;
            case 2:
                flyPower = 0f;
                break;
            case 3:
                flyPower = 8f;
                break;
        }
        if (nearestDistance > 0.1) transform.localScale = new Vector3(-1, 1, 1);
        else if (nearestDistance < -0.1) transform.localScale = new Vector3(1, 1, 1);
        rb.linearVelocity = new Vector2(directionToEnemy.x * flyPower * (-1), directionToEnemy.y * flyPower);
    }
    
    public Collider2D GetNearestTarget(Transform origin, List<Collider2D> colliders, out float distance, out Vector2 direction)
    {
        Collider2D nearest = null;
        float minDistance = float.MaxValue;
        Vector2 nearestDirection = Vector2.zero;
        distance = 0f;

        foreach (var col in colliders)
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
