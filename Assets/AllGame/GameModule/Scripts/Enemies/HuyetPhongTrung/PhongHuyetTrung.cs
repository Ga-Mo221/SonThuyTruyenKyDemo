using UnityEngine;

public class PhongHuyetTrung : EnemyBase
{
    private Coroutine explodeCoroutine;
    private bool isCountingDown = false;
    private bool hasDetectedPlayer = false;

    [SerializeField] private float _countdownDuration = 8f;
    [SerializeField] private float _maxMoveSpeedDuringCountdown = 2f;
    [SerializeField] private float _instantExplodeDistance = 1.2f;
    private float countdownTimer = 0f;

    // Animator-related state
    private bool isClose = false;
    private bool isStay = false;
    private bool isPatrol = false;
    private bool isLive = true;

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D instantExplodeCollider; // Thêm collider cho instant explode

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Tạo trigger collider cho instant explode
        SetupInstantExplodeCollider();
    }
    
    private void SetupInstantExplodeCollider()
    {
        // Tạo GameObject con để chứa trigger collider
        GameObject triggerObj = new GameObject("InstantExplodeTrigger");
        triggerObj.transform.SetParent(transform);
        triggerObj.transform.localPosition = Vector3.zero;
        
        // Thêm CircleCollider2D và cấu hình
        CircleCollider2D collider = triggerObj.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = _instantExplodeDistance;
        
        // Lưu reference
        instantExplodeCollider = collider;
        
        // Thêm script để handle trigger
        InstantExplodeTrigger triggerScript = triggerObj.AddComponent<InstantExplodeTrigger>();
        triggerScript.Initialize(this);
    }
    
    protected override void Start()
    {
        base.Start();
        base._patrolStartPos = transform.position;
        _physicalRes = 0;
        _magicRes = 0;
        _animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();
        if (!isLive) return;
        base.MoveToPlayer();
        isStay = base._isStay;
        isPatrol = base._isPatrol;
        CheckPlayerDetection();
        UpdateAnimator();
    }

    protected override void MoveToPlayer()
    {
        base.MoveToPlayer();
        if (isCountingDown && _countdownDuration > 0f)
        {
            float progress = Mathf.Clamp01(1f - (countdownTimer / _countdownDuration));
            base._enemyMoveSpd = Mathf.Lerp(base._enemyMoveSpd, _maxMoveSpeedDuringCountdown, progress);
        }

        if (hasDetectedPlayer)
        {
            isPatrol = false;
            isStay = false;
        }
    }

    protected override void Patrol()
    {
        base.Patrol();
    }

    private void CheckPlayerDetection()
    {
        if (_player == null || hasDetectedPlayer) return;

        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance <= _detectionRange)
        {
            hasDetectedPlayer = true;
            
            // Bắt đầu countdown (instant explode sẽ được handle bởi trigger)
            explodeCoroutine = StartCoroutine(ExplodeAfterDelay(_countdownDuration));
            isCountingDown = true;
        }
    }

    // Method này được gọi bởi InstantExplodeTrigger
    public void OnPlayerEnterInstantExplodeZone()
    {
        if (!isLive) return;
        
        
        if (explodeCoroutine != null)
        {
            StopCoroutine(explodeCoroutine);
            explodeCoroutine = null;
        }
        
        countdownTimer = 0f;
        isCountingDown = false;
        TriggerExplosion();
    }

    private System.Collections.IEnumerator ExplodeAfterDelay(float delay)
    {
        countdownTimer = delay;
        float flashInterval = 0.2f;
        bool flashRed = true;

        while (countdownTimer > 0)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = flashRed ? Color.red : Color.white;

            flashRed = !flashRed;

            yield return new WaitForSeconds(flashInterval);
            countdownTimer -= flashInterval;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        TriggerExplosion();
    }

    private void TriggerExplosion()
    {
        if (_player != null)
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);
            if (distance <= _enemyAttackRange)
            {
                isClose = true;
                Attack();
            }
            SpawnPoisonArea();
        }

        explodeCoroutine = null;
        isCountingDown = false;
        hasDetectedPlayer = false;
    }

    private void UpdateAnimator()
    {
        if (_animator == null) return;

        _animator.SetBool("isClose", isClose);
        _animator.SetBool("isPatrol", isPatrol);
        _animator.SetBool("isStay", isStay);
    }

    private void SpawnPoisonArea()
    {
        isLive = false;
        _animator.SetBool("isClose", !isClose);
        _animator.SetBool("isPatrol", !isPatrol);
        _animator.SetTrigger("isExplode");
        Poison poison = GetComponent<Poison>();
        if (poison != null)
        {
            poison.ActivatePoison();
        }
        else
        {
            Debug.LogError("Poison component NOT FOUND!");
        }
    }

    protected override void Attack()
    {
        HealthManager health = _player.GetComponent<HealthManager>();
        if (health != null)
        {
            health.takeDamage(1, base._enemyPhysicalDamage, false);
            Debug.Log($"PhongHuyetTrung attacks player with {_enemyPhysicalDamage} physical damage.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BasicAttack attack = collision.GetComponent<BasicAttack>();
        if (attack != null)
        {
            TakeDamage();
        }
    }
}

// Script riêng để handle instant explode trigger
public class InstantExplodeTrigger : MonoBehaviour
{
    private PhongHuyetTrung parentEnemy;
    
    public void Initialize(PhongHuyetTrung enemy)
    {
        parentEnemy = enemy;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && parentEnemy != null)
        {
            parentEnemy.OnPlayerEnterInstantExplodeZone();
        }
    }
}