using System.Collections;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class CurrencyDisplay : MonoBehaviour
{
    public float _flyPower;
    public bool _item = false;

    [HideIf(nameof(_item))]
    [SerializeField] private Collider2D _collider;
    [HideIf(nameof(_item))]
    public float _dropTime;
    [HideIf(nameof(_item))]
    public float _moveSpeedToPlayer;
    [HideIf(nameof(_item))]
    public bool _coin = true;

    [HideIf(nameof(_item))]
    [ShowIf(nameof(_coin))]
    public int _XengValue;

    [HideIf(nameof(_coin))]
    public int _LinhAnValue;


    private int _direction;
    private Rigidbody2D _rb;
    private bool _drop;
    private Transform _target;
    private TrailRenderer _trailrenderer;

    void Awake()
    {
        _trailrenderer = GetComponent<TrailRenderer>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Chọn góc ngẫu nhiên
        _direction = Random.Range(45, 135);

        // Bay lên
        fly();
    }

    void Update()
    {
        if (!_item)
        {
            if (_rb.linearVelocity.y <= 0 && !_drop)
            {
                StartCoroutine(redyToDrop());
                _rb.gravityScale = 0f;
            }
            moveToPlayer();
        }
    }

    private IEnumerator redyToDrop()
    {
        yield return new WaitForSeconds(1f);
        _drop = true;
        StartCoroutine(EnableDrop());
    }

    private void fly()
    {
        // Chuyển góc sang vector hướng
        Vector2 dir = new Vector2(
            Mathf.Cos(_direction * Mathf.Deg2Rad),
            Mathf.Sin(_direction * Mathf.Deg2Rad)
        ).normalized;

        // Đẩy theo lực và hướng
        _rb.linearVelocity = dir * _flyPower;
    }

    private IEnumerator EnableDrop()
    {
        _rb.gravityScale = 0f;
        _rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(_dropTime);
        _rb.gravityScale = 1f;
        _rb.linearDamping = 0f;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !_item && _drop)
        {
            _target = collision.transform;
        }
    }

    private void moveToPlayer()
    {
        if (_target != null && _drop)
        {
            // Bay dần về vị trí player
            transform.position = Vector2.MoveTowards(transform.position, _target.position, _moveSpeedToPlayer * Time.deltaTime);

            // Nếu quá gần thì thu thập coin
            if (Vector2.Distance(transform.position, _target.position) < 1.3f)
            {
                _collider.enabled = false;
                if (_trailrenderer != null)
                {
                    _trailrenderer.emitting = false;
                    _trailrenderer.enabled = false;
                }

                if (this != null && gameObject.activeInHierarchy)
                {
                    StartCoroutine(destroy());
                }
            }
        }
    }

    private IEnumerator destroy()
    {
        yield return new WaitForSeconds(0.3f);
        if (_coin)
            PlayerManager.Instance.setCoin(_XengValue, true);
        Destroy(gameObject);
    }
}
