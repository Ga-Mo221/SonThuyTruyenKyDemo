using UnityEngine;
using System.Collections;

public class PhongHuyetTrung : EnemyBase
{
    [Header("Explosion Settings")]
    [SerializeField] private float _countdownDuration = 8f;
    [SerializeField] private float _maxMoveSpeedDuringCountdown = 10f;
    [SerializeField] private float _speedMultiplierCurve = 2f;

    // Trạng thái chính
    private enum EnemyState
    {
        Patrolling,
        Chasing,
        Exploding,
        Poisoning,
        Dead
    }

    private EnemyState currentState = EnemyState.Patrolling;
    private GameObject rememberedPlayer;
    private float countdownTimer = 0f;
    private float originalMoveSpeed;
    private float originalRunSpeed;

    protected override void Awake()
    {
        base.Awake();
        _canFly = true;
        
        // Lưu tốc độ gốc
        originalMoveSpeed = _enemyMoveSpd;
        originalRunSpeed = _enemyRunSpd;
    }

    protected override void Start()
    {
        base.Start();
        _patrolStartPos = transform.position;
        _physicalRes = 0;
        _magicRes = 0;
        
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        if (currentState == EnemyState.Dead || currentState == EnemyState.Poisoning) 
            return;

        base.Update();
        updateStateMachine();
        updateAnimator();
    }

    private void updateStateMachine()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                handlePatrollingState();
                break;
                
            case EnemyState.Chasing:
                handleChasingState();
                break;
                
            case EnemyState.Exploding:
                // Không cần xử lý gì, chờ animation event
                break;
        }
    }

    private void handlePatrollingState()
    {
        // Kiểm tra phát hiện player
        if (_player != null)
        {
            rememberedPlayer = _player;
            currentState = EnemyState.Chasing;
            countdownTimer = _countdownDuration;
            
            // Reset patrol states
            _isPatrol = false;
            _isStay = false;
        }
    }

    private void handleChasingState()
    {
        if (rememberedPlayer == null)
        {
            currentState = EnemyState.Patrolling;
            resetToPatrol();
            return;
        }

        _player = rememberedPlayer;

        // Cập nhật countdown và tốc độ
        countdownTimer = Mathf.Max(0f, countdownTimer - Time.deltaTime);
        updateChaseSpeed();

        // Kiểm tra khoảng cách để nổ
        float distance = Vector3.Distance(transform.position, rememberedPlayer.transform.position);
        if (distance <= _enemyAttackRange)
        {
            triggerExplosion();
        }
    }

    private void updateChaseSpeed()
    {
        // Tốc độ tăng dần theo thời gian
        float timeProgress = Mathf.Clamp01(1f - (countdownTimer / _countdownDuration));
        float curvedProgress = Mathf.Pow(timeProgress, _speedMultiplierCurve);
        float newSpeed = Mathf.Lerp(originalRunSpeed, _maxMoveSpeedDuringCountdown, curvedProgress);
        _enemyRunSpd = newSpeed;
    }

    private void resetToPatrol()
    {
        _enemyMoveSpd = originalMoveSpeed;
        _enemyRunSpd = originalRunSpeed;
        _isPatrol = true;
        _isStay = false;
        rememberedPlayer = null;
        countdownTimer = 0f;
    }

    protected override void moveToPlayer()
    {
        if (currentState != EnemyState.Chasing) return;

        if (rememberedPlayer != null)
        {
            _player = rememberedPlayer; // Đảm bảo base class biết player để chase
            base.moveToPlayer();
        }
    }

    protected override void Patrol()
    {
        if (currentState != EnemyState.Patrolling) return;
        
        _enemyMoveSpd = originalMoveSpeed;
        base.Patrol();
    }

    private void triggerExplosion()
    {
        if (currentState != EnemyState.Chasing) return;

        
        currentState = EnemyState.Exploding;

        // Dừng di chuyển
        if (_rb != null)
            _rb.linearVelocity = Vector2.zero;

        // Reset animation states
        _animator.SetBool("isClose", false);
        _animator.SetBool("isPatrol", false);
        _animator.SetBool("isStay", false);
        
        // Trigger explosion animation
        _animator.SetTrigger("isExplode");
    }

    // Animation Event được gọi trong animation Explode để gây damage
    public void onExplodeDamageEvent()
    {
        if (currentState != EnemyState.Exploding) return;

        if (rememberedPlayer != null)
        {
            float distance = Vector3.Distance(transform.position, rememberedPlayer.transform.position);
            if (distance <= _enemyAttackRange)
            {
                Attack();
            }
        }
    }

    // Animation Event được gọi khi animation Explode kết thúc
    public void onExplodeAnimationComplete()
    {
        if (currentState != EnemyState.Exploding) return;
        
        currentState = EnemyState.Poisoning;
        
        // Chuyển sang animation Poison
        _animator.SetTrigger("isPoison");
        
        // Kích hoạt poison effect
        Poison poison = GetComponent<Poison>();
        if (poison != null)
        {
            poison.startPoisonEffect();
        }
        else
        {
            Die();
        }
    }

    // Animation Event
    public void onPoisonAnimationComplete()
    {
    }

    protected override void Attack()
    {
        if (rememberedPlayer == null) return;

        HealthManager health = rememberedPlayer.GetComponent<HealthManager>();
        if (health != null)
        {
            health.takeDamage(0, _enemyPhysicalDamage, false);
            Debug.Log($"Explosion dealt {_enemyPhysicalDamage} physical damage");
        }
    }

    public override void Die()
    {
        
        currentState = EnemyState.Dead;
        this.enabled = false;
        
        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.simulated = false;
        }
        
        Destroy(gameObject);
    }

    private void updateAnimator()
    {
        if (_animator == null || currentState == EnemyState.Exploding || currentState == EnemyState.Poisoning) 
            return;

        bool isClose = (currentState == EnemyState.Chasing);
        bool isPatrol = (currentState == EnemyState.Patrolling && _isPatrol);
        bool isStay = (currentState == EnemyState.Patrolling && _isStay);

        _animator.SetBool("isClose", isClose);
        _animator.SetBool("isPatrol", isPatrol);
        _animator.SetBool("isStay", isStay);
    }

    public override void takeDamage()
    {
        if (currentState == EnemyState.Patrolling || currentState == EnemyState.Chasing)
        {
            base.takeDamage();
        }
    }
}