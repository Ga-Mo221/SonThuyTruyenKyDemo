using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Pathfinding;
using UnityEditor.Callbacks;



public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] protected float _enemyMoveSpd; // tốc độ di chuyển của enemy
    [SerializeField] protected float _enemyRunSpd; // tốc độ chạy của enemy
    [SerializeField] protected float _enemyHP; // máu của enemy
    [SerializeField] protected float _enemyPhysicalDamage; // sát thương vật lí của enemy
    [SerializeField] protected float _enemyMagicDamage; // sát thương phép của enemy
    [SerializeField] protected float _enemyAttackRange; // phạm vi tấn công của enemy
    [SerializeField] protected float _enemyAttackCooldown; // cooldown giữa các đòn tấn công
    [SerializeField] protected float _detectionRange; // Phạm vi phát hiện người chơi
    [SerializeField] protected float _patrolRange; // phạm vi tuần tra
    [SerializeField] protected float _physicalRes; // Kháng vật lí
    [SerializeField] protected float _magicRes; // Kháng phép
    [SerializeField] protected GameObject _enemyRadaGOB; // Rada phát hiện va chạm

    protected Path _path; // Đường đi của AI
    protected int _currentWaypoint = 0; // Chỉ số waypoint hiện tại
    protected bool _reachedEndOfPath = false; // Đã đến cuối đường đi chưa
    [SerializeField] protected float _nextWaypointDistance = 1.2f; // Khoảng cách đến waypoint tiếp theo
    protected Seeker _seeker; // Seeker component để tìm đường
    [SerializeField] protected Vector2 _force; // Lực tác động lên enemy

    //protected float _patrolWaitTimer = 0f;
    protected float _patrolWaitDuration = 2f;
    //protected AIPath _aiPath; // Đường đi của AI
    protected EnemyRada _enemyRada; // Rada phát hiện va chạm
    protected float _distance; // khoảng cách đến người chơi
    protected Vector3 _patrolStartPos; // vị trí bắt đầu tuần tra
    public int _patrolDirection = 1; // hướng tuần tra
    protected GameObject _player;
    public bool _isStay = false; // trạng thái đứng yên
    public bool _isPatrol = true; // trạng thái tuần tra

    protected bool _isKnockedBack = false;
    [SerializeField] protected float _knockbackForce = 300f;
    [SerializeField] protected float _knockbackDuration;

    protected Animator _animator;
    protected Rigidbody2D _rb;

    protected bool _movingToRight = true;
    private bool _isChasing = false;


    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemyRada = _enemyRadaGOB.GetComponent<EnemyRada>();
        _enemyRadaGOB.GetComponent<CircleCollider2D>().radius = _detectionRange;
    }

    protected virtual void Start()
    {
        if (_enemyRadaGOB.GetComponent<CircleCollider2D>() == null)
        {
            Debug.Log("loi");
        }
        _enemyRadaGOB.GetComponent<CircleCollider2D>().radius = _detectionRange;
        _seeker = GetComponent<Seeker>();
        _rb = GetComponent<Rigidbody2D>();
        StartCoroutine(updatePath());
        _patrolStartPos = transform.position;
    }

    private IEnumerator updatePath()
    {
        while (true)
        {
            if (_player != null && _seeker.IsDone())
            {
                _seeker.StartPath(_rb.position, _player.transform.position, onPathComplete);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }


    protected virtual void Update()
    {
        detechtPlayer();
    }

    protected virtual void FixedUpdate()
    {
        if (_player != null)
        {
            if (!_isChasing)
            {
                _isChasing = true;
                if (_seeker.IsDone())
                {
                    _seeker.StartPath(_rb.position, _player.transform.position, onPathComplete);
                }
            }
            MoveToPlayer();
        }
        else
        {
            _isChasing = false;
            Patrol();
        }

    }

    // Hàm xử lý khi đường đi hoàn thành
    protected void onPathComplete(Path _p)
    {
        if (!_p.error)
        {
            _path = _p;
            _currentWaypoint = 0;
            _reachedEndOfPath = false;
        }
    }

    // Hàm phát hiện người chơi
    private void detechtPlayer()
    {

        foreach (var col in _enemyRada._detectedColliders)
        {
            if (col.CompareTag("Player"))
            {

                Transform _pos = col.transform;
                while (_pos != null)
                {
                    if (_pos.name == "Player")
                    {
                        _player = _pos.gameObject;
                        break;
                    }
                    _pos = _pos.parent;
                }
            }
        }
    }

    // Hàm di chuyển đến người chơi
    protected virtual void MoveToPlayer()
    {
        if (_player == null)
        {
            Patrol();
            return;
        }
        if (_rb == null) return;
        if (_path == null)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }
        if (_currentWaypoint >= _path.vectorPath.Count)
        {
            _reachedEndOfPath = true;
            _rb.linearVelocity = Vector2.zero;
            return;
        }
        else
        {
            _reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rb.position).normalized;
        float currentSpeed = _enemyMoveSpd;
        _force = direction * currentSpeed * Time.deltaTime* 15f;
        _distance = Vector2.Distance(_rb.position, _path.vectorPath[_currentWaypoint]);
        _rb.MovePosition(_rb.position + direction * currentSpeed * Time.fixedDeltaTime);
        
        if (_distance < _nextWaypointDistance)
        {
            _currentWaypoint++;
        }
        FlipEnemy();
    }

    // Hàm tuần tra
protected virtual void Patrol()
{
    if (_rb == null || !_seeker.IsDone()) return;

    // Nếu không có đường thì yêu cầu Seeker tạo đường mới
    if (_path == null || _currentWaypoint >= _path.vectorPath.Count)
    {
        Vector3 patrolPoint = _movingToRight
            ? _patrolStartPos + Vector3.right * _patrolRange
            : _patrolStartPos + Vector3.left * _patrolRange;

        _seeker.StartPath(_rb.position, patrolPoint, onPathComplete);
        return;
    }

    Vector2 direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rb.position).normalized;
    float currentSpeed = _enemyMoveSpd;
    Vector2 _force = direction * currentSpeed * Time.deltaTime* 10f;
    if (_isPatrol)
        _rb.AddForce(_force);

    _distance = Vector2.Distance(_rb.position, _path.vectorPath[_currentWaypoint]);
    if (_distance < _nextWaypointDistance)
    {
        _currentWaypoint++;
    }

    if (_currentWaypoint >= _path.vectorPath.Count)
    {
        StartCoroutine(stopPatrol());
        _rb.linearVelocity = Vector2.zero; // Dừng lại khi đến cuối đường tuần
        _movingToRight = !_movingToRight; // Đổi hướng tuần tra
    }

    FlipEnemy();
}
    private IEnumerator stopPatrol()
    {
        _isPatrol = false;
        _isStay = true;
        yield return new WaitForSeconds(_patrolWaitDuration);
        _patrolDirection *= -1;
        transform.localScale = new Vector3(_patrolDirection, 1, 1);
        _isStay = false;
        _isPatrol = true;
    }

    // Hàm lật hướng
    protected void FlipEnemy()
    {
        if (_player != null)
        {
            if (_rb.linearVelocity.x >= 0.01f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (_rb.linearVelocity.x <= 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    // Hàm tấn công
    protected virtual void Attack()
    {
    }

    // Hàm nhận sát thương
    public virtual void TakeDamage()
    {
        knockbackFromPlayer();
        //Die();
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    // Hàm xử lý knockback từ người chơi
    private void knockbackFromPlayer()
    {
        if (_player == null || _rb == null || _isKnockedBack) return;
        Vector2 knockDir = ((Vector2)transform.position - (Vector2)_player.transform.position).normalized;
        _rb.AddForce(knockDir * _knockbackForce); // Lùi nhẹ về sau
        _isKnockedBack = true;
        StartCoroutine(knockbackCoroutine());
        
    }


    private IEnumerator knockbackCoroutine()
    {
        enabled = false; // Tắt tạm thời các hành động khác
        yield return new WaitForSeconds(_knockbackDuration);
        _rb.linearVelocity = Vector2.zero;
        _isKnockedBack = false;
        enabled = true; // Bật lại các hành động khác
    }

    // Hàm vẽ Debug tầm đánh
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _enemyAttackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
    

}