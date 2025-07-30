// ✅ EnemyBase.cs - Updated to support flying enemies
using UnityEngine;
using System.Collections;
using Pathfinding;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] protected float _enemyMoveSpd = 1f;
    [SerializeField] protected float _enemyRunSpd;
    [SerializeField] protected float _enemyHP = 10f;
    [SerializeField] protected float _enemyPhysicalDamage = 1f;
    [SerializeField] protected float _enemyMagicDamage = 1f;
    [SerializeField] protected float _enemyAttackRange = 1.5f;
    [SerializeField] protected float _enemyAttackCooldown = 1f;
    [SerializeField] protected float _detectionRange = 5f;
    [SerializeField] protected float _patrolRange = 3f;
    [SerializeField] protected float _physicalRes = 0f;
    [SerializeField] protected float _magicRes = 0f;
    [SerializeField] protected GameObject _enemyRadaGOB;
    [SerializeField] protected bool _canFly = false;

    [Header("Pathfinding")]
    [SerializeField] protected float _nextWaypointDistance = 1.2f;
    protected Path _path;
    protected int _currentWaypoint = 0;
    protected bool _reachedEndOfPath = false;

    protected Seeker _seeker;
    protected Rigidbody2D _rb;
    protected Animator _animator;
    protected EnemyRada _enemyRada;

    protected GameObject _player;
    protected Vector3 _patrolStartPos;

    protected bool _isChasing = false;
    public bool _isMoving = false;
    public bool _isPatrol = true;
    public bool _isStay = false;
    protected bool _isKnockedBack = false;
    protected bool _movingToRight = true;

    [SerializeField] protected float _patrolWaitDuration = 2f;
    [SerializeField] protected float _knockbackForce = 300f;
    [SerializeField] protected float _knockbackDuration = 0.2f;
    protected Coroutine _knockbackRoutine;


    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _seeker = GetComponent<Seeker>();
        _enemyRada = _enemyRadaGOB.GetComponent<EnemyRada>();
        _enemyRadaGOB.GetComponent<CircleCollider2D>().radius = _detectionRange;
    }

    protected virtual void Start()
    {
        _patrolStartPos = transform.position;
        StartCoroutine(updatePathCoroutine());
    }

    // Hàm cập nhật tạo đường đi
    private IEnumerator updatePathCoroutine()
    {
        while (true)
        {
            if (_player != null && _seeker.IsDone())
                _seeker.StartPath(_rb.position, _player.transform.position, onPathComplete);
            yield return new WaitForSeconds(0.75f);
        }
    }

    protected virtual void Update()
    {
        detectPlayer();
    }

    protected virtual void FixedUpdate()
    {
        if (_player != null) handleChaseMode();
        else handlePatrolMode();
    }

    private void detectPlayer()
    {
        _player = _enemyRada._player;
    }

    // Hàm xử lí truy đuổi
    private void handleChaseMode()
    {
        if (!_isChasing)
        {
            _isChasing = true;
            _path = null;
            _currentWaypoint = 0;
        }
        moveToPlayer();
    }

    // Hàm xử lí Patrol
    private void handlePatrolMode()
    {
        if (_isChasing)
        {
            _isChasing = false;
            _path = null;
            _currentWaypoint = 0;
        }
        Patrol();
    }

    //Hàm di chuyển đến Player
    protected virtual void moveToPlayer()
    {
        if (_player == null)
        {
            _rb.linearVelocity = Vector2.zero;
            _isMoving = false;
            return;
        }

        if (_path == null || _currentWaypoint >= _path.vectorPath.Count)
        {
            if (_seeker.IsDone())
                _seeker.StartPath(_rb.position, _player.transform.position, onPathComplete);
            _rb.linearVelocity = Vector2.zero;
            _isMoving = false;
            return;
        }

        Vector2 dir = ((Vector2)_path.vectorPath[_currentWaypoint] - _rb.position).normalized;
        if (_canFly)
            _rb.linearVelocity = dir * _enemyRunSpd;
        else
            _rb.linearVelocity = new Vector2(dir.x * _enemyRunSpd, _rb.linearVelocity.y);

        _isMoving = _rb.linearVelocity.sqrMagnitude > 0.01f;

        float dist = Vector2.Distance(_rb.position, _path.vectorPath[_currentWaypoint]);
        if (dist < _nextWaypointDistance) _currentWaypoint++;

        Flip();
    }

    //Hàm tuần tra
    protected virtual void Patrol()
    {
        if (!_isPatrol || _isStay || _rb == null || !_seeker.IsDone())
        {
            _isMoving = false;
            return;
        }

        if (_path == null || _currentWaypoint >= _path.vectorPath.Count)
        {
            Vector3 target = _movingToRight ? _patrolStartPos + Vector3.right * _patrolRange : _patrolStartPos + Vector3.left * _patrolRange;
            _seeker.StartPath(_rb.position, target, onPathComplete);
            _isMoving = false;
            return;
        }

        Vector2 dir = ((Vector2)_path.vectorPath[_currentWaypoint] - _rb.position).normalized;
        if (_canFly)
            _rb.linearVelocity = dir * _enemyMoveSpd;
        else
            _rb.linearVelocity = new Vector2(dir.x * _enemyMoveSpd, _rb.linearVelocity.y);

        _isMoving = _rb.linearVelocity.sqrMagnitude > 0.01f;

        float dist = Vector2.Distance(_rb.position, _path.vectorPath[_currentWaypoint]);
        if (dist < _nextWaypointDistance) _currentWaypoint++;

        if (_currentWaypoint >= _path.vectorPath.Count)
            StartCoroutine(stopAndTurnAround());

        Flip();
    }

    // Hàm xử lí việc dừng tuần tra và lật 
    protected virtual IEnumerator stopAndTurnAround()
    {
        _isPatrol = false;
        _isStay = true;
        _rb.linearVelocity = Vector2.zero;
        _isMoving = false;
        yield return new WaitForSeconds(_patrolWaitDuration);
        _movingToRight = !_movingToRight;
        _isStay = false;
        _isPatrol = true;
        _path = null;
        _currentWaypoint = 0;
    }

    //Hàm lật
    private void Flip()
    {
        bool flipRight = _rb.linearVelocity.x > 0.05f;
        bool flipLeft = _rb.linearVelocity.x < -0.05f;

        if (flipRight || flipLeft)
        {
            Vector3 scale = transform.localScale;
            scale.x = flipRight ? 1 : -1;
            transform.localScale = scale;

            transform.position = GetComponent<Collider2D>().bounds.center;
        }
    }

    // Hàm kiểm tra tạo đường đi
    protected virtual void onPathComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWaypoint = 0;
            _reachedEndOfPath = false;
        }
        else
        {
            Debug.LogError($"Pathfinding failed: {p.errorLog}");
        }
    }

    // Hàm tấn công (đa số sẽ override ở mỗi con enemy khác nhau)
    protected virtual void Attack() { }

    // Hàm nhận Dmg
    public virtual void takeDamage()
    {
        knockbackFromPlayer();
    }

    // Hàm xử lí đẩy lùi khi nhận đòn đánh từ Player
    protected void knockbackFromPlayer()
    {
        if (_player == null || _rb == null || _isKnockedBack) return;

        Vector2 dir = ((Vector2)transform.position - (Vector2)_player.transform.position).normalized;
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(dir * _knockbackForce, ForceMode2D.Impulse);

        if (_knockbackRoutine != null)
            StopCoroutine(_knockbackRoutine);
        _knockbackRoutine = StartCoroutine(knockbackCoroutine());
    }


    // Hàm xử lí thời gian bị đẩy lùi
    protected IEnumerator knockbackCoroutine()
    {
        _isKnockedBack = true;
        yield return new WaitForSeconds(_knockbackDuration);
        _rb.linearVelocity = Vector2.zero;
        _isKnockedBack = false;
    }


    // Hàm xử lí Anim Die của enemy
    public virtual void Die()
    {
        _animator.SetBool("isDead", true);
        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.simulated = false;
        this.enabled = false;
        StartCoroutine(waitAndDestroy(_animator.GetCurrentAnimatorStateInfo(0).length));
    }

    // Hàm quản lí thời gian Destroy
    protected virtual IEnumerator waitAndDestroy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
    
    // Hàm Debug
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _enemyAttackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }

    // Hàm xử lí máu của enemy khi nhận sát thương từ Player
    public void setEnemyHP(float damage, bool magic)
    {
        float calcDamage = magic ? damage - _magicRes : damage - _physicalRes;
        _enemyHP -= Mathf.Max(calcDamage, 1);

        if (_enemyHP <= 0) Die();
    }
}
