using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : MonoBehaviour
{
    [SerializeField] private float _poisonDuration = 2f;
    [SerializeField] private float _poisonDamage = 1f;
    [SerializeField] private float _poisonedTime = 3f;
    [SerializeField] private CircleCollider2D _triggerCollider;
    [SerializeField] private Animator _explodeAnimator;

    private bool _isActive = false;
    private HashSet<GameObject> _playersInPoison = new HashSet<GameObject>();

    private void Update()
    {
        // Debug log chỉ chạy một lần khi active
        if (_isActive)
        {
            
            // Tìm player trong scene
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                
                // Kiểm tra xem player có trong vùng trigger không
                if (_triggerCollider != null && distance <= _triggerCollider.radius)
                {
                    
                    // Force check collision if player is in range but not detected
                    if (!_playersInPoison.Contains(player))
                    {
                        _playersInPoison.Add(player);
                        
                        HealthManager health = player.GetComponent<HealthManager>();
                        if (health != null && player.GetComponent<PoisonedMarker>() == null)
                        {
                            StartCoroutine(ApplyPoison(health, player));
                        }
                    }
                }
            }
        }
    }

    private void Awake()
    {
        if (_triggerCollider != null)
            _triggerCollider.enabled = false; // Tắt va chạm ban đầu
    }

    public void ActivatePoison() // Gọi từ Animation Event
    {
        StartCoroutine(EnablePoison());
    }

    private IEnumerator EnablePoison()
    {
        
        if (_explodeAnimator != null)
        {
            _explodeAnimator.SetTrigger("isExplode");
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll; // Khóa vị trí + xoay
        }

        yield return new WaitForSeconds(0.3f); // Đợi animation bắt đầu

        // 3. Bật collider trước khi set active
        if (_triggerCollider != null)
        {
            _triggerCollider.enabled = true;
        }
        else
        {
        }

        yield return new WaitForFixedUpdate();

        _isActive = true;

        yield return new WaitForSeconds(0.1f);
        CheckPlayersInArea();

        yield return new WaitForSeconds(_poisonDuration - 0.1f);

        if (_triggerCollider != null)
        {
            _triggerCollider.enabled = false;
        }

        _isActive = false;

        Destroy(gameObject);
    }

    private void CheckPlayersInArea()
    {
        
        if (_triggerCollider == null || !_triggerCollider.enabled)
        {
            return;
        }

        // Tìm tất cả collider trong vùng
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _triggerCollider.radius);

        foreach (Collider2D col in colliders)
        {
            GameObject player = GetPlayerObject(col);
            if (player != null && !_playersInPoison.Contains(player))
            {
                _playersInPoison.Add(player);
                
                HealthManager health = player.GetComponent<HealthManager>();
                if (health != null && player.GetComponent<PoisonedMarker>() == null)
                {
                    StartCoroutine(ApplyPoison(health, player));
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        // Bỏ qua các object không phải Player
        if (collision.name.Contains("Rada") || collision.name.Contains("Enemy"))
        {
            return;
        }

        GameObject player = GetPlayerObject(collision);
        if (player != null && !_playersInPoison.Contains(player))
        {
            _playersInPoison.Add(player);
            
            // Apply poison immediately if active, or wait if not active yet
            if (_isActive)
            {
                HealthManager health = player.GetComponent<HealthManager>();
                if (health != null && player.GetComponent<PoisonedMarker>() == null)
                {
                    StartCoroutine(ApplyPoison(health, player));
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Bỏ qua các object không phải Player
        if (collision.name.Contains("Rada") || collision.name.Contains("Enemy"))
        {
            return;
        }

        GameObject player = GetPlayerObject(collision);
        if (player != null && _playersInPoison.Contains(player))
        {
            _playersInPoison.Remove(player);
            
            // Dừng coroutine poison nếu player rời khỏi vùng độc
            PoisonedMarker marker = player.GetComponent<PoisonedMarker>();
            if (marker != null)
            {
                marker.StopPoison();
            }
        }
    }

    private GameObject GetPlayerObject(Collider2D collision)
    {
        
        // Sử dụng logic giống EnemyBase.cs - tìm parent có tên "Player"
        Transform current = collision.transform;
        while (current != null)
        {
            if (current.name == "Player")
            {
                return current.gameObject;
            }
            current = current.parent;
        }

        // Kiểm tra theo tag nếu không tìm thấy theo tên
        current = collision.transform;
        while (current != null)
        {
            if (current.CompareTag("Player"))
            {
                return current.gameObject;
            }
            current = current.parent;
        }

        return null;
    }

    private IEnumerator ApplyPoison(HealthManager health, GameObject playerObj)
    {
        var marker = playerObj.AddComponent<PoisonedMarker>();

        for (float t = 0; t < _poisonedTime; t += 1f)
        {
            // Kiểm tra xem player còn trong vùng độc không
            if (!_playersInPoison.Contains(playerObj) || !_isActive)
            {
                break;
            }

            Debug.Log($"Applying poison damage to {playerObj.name}: damage = {_poisonDamage}");
            
            // Gọi takeDamage với parameters đúng format
            health.takeDamage(0, _poisonDamage, true);
            
            Debug.Log($"Poison damage applied: {t + 1} seconds");
            yield return new WaitForSeconds(1f);
        }

        if (marker != null)
        {
            Destroy(marker);
        }
        
    }
}

public class PoisonedMarker : MonoBehaviour 
{
    private bool _shouldStop = false;
    
    public void StopPoison()
    {
        _shouldStop = true;
    }
    
    public bool ShouldStop()
    {
        return _shouldStop;
    }
}