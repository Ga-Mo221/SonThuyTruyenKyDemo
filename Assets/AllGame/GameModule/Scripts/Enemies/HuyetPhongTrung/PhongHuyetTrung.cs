// Bản đã xóa tất cả Debug.Log và hàm debug trong PhongHuyetTrung
using UnityEngine;
using System.Collections;

public class PhongHuyetTrung : EnemyBase
{
    [Header("Explosion Settings")]
    [SerializeField] private float _countdownDuration = 8f;
    [SerializeField] private float _maxMoveSpeedDuringCountdown = 10f;
    [SerializeField] private float _instantExplodeDistance = 1.2f;
    [SerializeField] private float _speedMultiplierCurve = 2f;

    private bool isCountingDown = false;
    private bool hasDetectedPlayer = false;
    private bool hasExploded = false;
    private bool isLive = true;

    private float countdownTimer = 0f;

    private GameObject rememberedPlayer;
    private Coroutine explodeCoroutine;
    private SpriteRenderer spriteRenderer;
    private InstantExplodeTrigger instantExplodeTrigger;

    private float originalMoveSpeed;
    private float originalRunSpeed;

    private bool isClose = false;
    private bool isStay = false;
    private bool isPatrol = false;

    protected override void Awake()
    {
        base.Awake();
        _canFly = true;
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalMoveSpeed = _enemyMoveSpd;
        originalRunSpeed = _enemyRunSpd;

        SetupInstantExplodeCollider();
    }

    private void SetupInstantExplodeCollider()
    {
        GameObject triggerObj = new GameObject("InstantExplodeTrigger");
        triggerObj.transform.SetParent(transform);
        triggerObj.transform.localPosition = Vector3.zero;
        triggerObj.layer = 0;

        CircleCollider2D collider = triggerObj.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = _instantExplodeDistance;
        collider.enabled = true;

        instantExplodeTrigger = triggerObj.AddComponent<InstantExplodeTrigger>();
        instantExplodeTrigger.Initialize(this);
    }

    protected override void Start()
    {
        base.Start();
        base._patrolStartPos = transform.position;
        _physicalRes = 0;
        _magicRes = 0;

        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        if (!isLive || hasExploded) return;

        base.Update();
        CheckPlayerDetection();
        CheckManualInstantExplode();
        UpdateBehaviorStates();
        UpdateAnimator();
    }

    private void CheckManualInstantExplode()
    {
        if (hasExploded || !isLive || _player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (distanceToPlayer <= _instantExplodeDistance)
        {
            if (!hasExploded)
            {
                OnPlayerEnterInstantExplodeZone();
            }
        }
    }

    private void CheckPlayerDetection()
    {
        if (_player != null && !hasDetectedPlayer)
        {
            rememberedPlayer = _player;
            hasDetectedPlayer = true;
            StartCountdown();
        }
    }

    private void StartCountdown()
    {
        if (isCountingDown || hasExploded) return;

        if (explodeCoroutine != null)
        {
            StopCoroutine(explodeCoroutine);
        }

        explodeCoroutine = StartCoroutine(ExplodeCountdownCoroutine());
        isCountingDown = true;
    }

    private void UpdateBehaviorStates()
    {
        if (hasDetectedPlayer)
        {
            isStay = false;
            isPatrol = false;

            if (rememberedPlayer != null)
            {
                _player = rememberedPlayer;
            }
        }
        else
        {
            isStay = base._isStay;
            isPatrol = base._isPatrol;
        }
    }

    protected override void moveToPlayer()
    {
        if (hasExploded) return;

        if (hasDetectedPlayer && rememberedPlayer != null)
        {
            _player = rememberedPlayer;
            CalculateSpeedBasedOnCountdown();
            base.moveToPlayer();
        }
        else if (!hasDetectedPlayer)
        {
            base._enemyRunSpd = originalRunSpeed;
            base.moveToPlayer();
        }
    }

    private void CalculateSpeedBasedOnCountdown()
    {
        if (isCountingDown && _countdownDuration > 0f)
        {
            float timeProgress = Mathf.Clamp01(1f - (countdownTimer / _countdownDuration));
            float curvedProgress = Mathf.Pow(timeProgress, _speedMultiplierCurve);
            float newSpeed = Mathf.Lerp(originalRunSpeed, _maxMoveSpeedDuringCountdown, curvedProgress);
            base._enemyRunSpd = newSpeed;
        }
        else
        {
            base._enemyRunSpd = originalRunSpeed;
        }
    }

    protected override void Patrol()
    {
        if (hasExploded) return;
        base._enemyMoveSpd = originalMoveSpeed;
        base.Patrol();
    }

    public void OnPlayerEnterInstantExplodeZone()
    {
        if (!isLive || hasExploded)
        {
            return;
        }

        if (explodeCoroutine != null)
        {
            StopCoroutine(explodeCoroutine);
            explodeCoroutine = null;
        }

        countdownTimer = 0f;
        isCountingDown = false;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
        }

        base._enemyRunSpd = _maxMoveSpeedDuringCountdown;
        TriggerExplosion();
    }

    private IEnumerator ExplodeCountdownCoroutine()
    {
        countdownTimer = _countdownDuration;
        float initialFlashInterval = 0.5f;
        float finalFlashInterval = 0.1f;
        bool flashRed = true;

        while (countdownTimer > 0 && !hasExploded && isLive)
        {
            float timeProgress = Mathf.Clamp01(1f - (countdownTimer / _countdownDuration));
            float flashInterval = Mathf.Lerp(initialFlashInterval, finalFlashInterval, timeProgress);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = flashRed ? Color.red : Color.white;
            }
            flashRed = !flashRed;

            yield return new WaitForSeconds(flashInterval);
            countdownTimer -= flashInterval;

            if (hasDetectedPlayer)
            {
                CalculateSpeedBasedOnCountdown();
            }
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        if (!hasExploded && isLive)
        {
            TriggerExplosion();
        }
    }

    private void TriggerExplosion()
    {
        if (hasExploded) return;

        hasExploded = true;
        isLive = false;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
        }

        if (rememberedPlayer != null)
        {
            float distance = Vector3.Distance(transform.position, rememberedPlayer.transform.position);
            if (distance <= _enemyAttackRange)
            {
                isClose = true;
                Attack();
            }
        }

        SpawnPoisonArea();
        explodeCoroutine = null;
        isCountingDown = false;
        countdownTimer = 0f;
    }

    protected override void Attack()
    {
        if (rememberedPlayer == null) return;

        HealthManager health = rememberedPlayer.GetComponent<HealthManager>();
        if (health != null)
        {
            health.takeDamage(1, base._enemyPhysicalDamage, false);
        }
    }

    public override void Die()
    {
        if (hasExploded) return;

        hasExploded = true;
        isLive = false;

        if (explodeCoroutine != null)
        {
            StopCoroutine(explodeCoroutine);
            explodeCoroutine = null;
        }

        isCountingDown = false;
        countdownTimer = 0f;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        base.Die();
    }

    private void SpawnPoisonArea()
    {
        _animator.SetBool("isClose", false);
        _animator.SetBool("isPatrol", false);
        _animator.SetTrigger("isExplode");

        Poison poison = GetComponent<Poison>();
        if (poison != null)
        {
            poison.ActivatePoison();
        }
    }

    private void UpdateAnimator()
    {
        if (_animator == null) return;

        _animator.SetBool("isClose", isClose);
        _animator.SetBool("isPatrol", isPatrol);
        _animator.SetBool("isStay", isStay);
    }
}

public class InstantExplodeTrigger : MonoBehaviour
{
    private PhongHuyetTrung parentEnemy;
    private bool hasTriggered = false;
    private CircleCollider2D triggerCollider;

    public void Initialize(PhongHuyetTrung enemy)
    {
        parentEnemy = enemy;
        triggerCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (parentEnemy == null || hasTriggered) return;
            hasTriggered = true;
            parentEnemy.OnPlayerEnterInstantExplodeZone();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && parentEnemy != null && !hasTriggered)
        {
            hasTriggered = true;
            parentEnemy.OnPlayerEnterInstantExplodeZone();
        }
    }
} 
