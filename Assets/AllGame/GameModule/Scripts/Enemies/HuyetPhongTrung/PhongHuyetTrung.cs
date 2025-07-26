using UnityEngine;

// xử lí lại flip enemy khi phát hiện người chơi
// xử lí nổ (đếm ngược) sau khi người chơi rời khỏi vùng phát hiện

public class PhongHuyetTrung : EnemyBase
{
    private Coroutine explodeCoroutine;
    private bool isCountingDown = false;
    private bool hasDetectedPlayer = false;

    [SerializeField] private float _countdownDuration = 8f;
    [SerializeField] private float _maxMoveSpeedDuringCountdown = 10f; // Tăng tốc độ tối đa
    [SerializeField] private float _instantExplodeDistance = 1.2f;
    [SerializeField] private float _speedMultiplierCurve = 2f; // Độ cong của đường tăng tốc
    private float countdownTimer = 0f;

    private float originalMoveSpeed;
    private float originalRunSpeed;
    // Lưu reference đến player để không bị mất
    private GameObject rememberedPlayer;

    // Animator-related state
    private bool isClose = false;
    private bool isStay = false;
    private bool isPatrol = false;
    private bool isLive = true;

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D instantExplodeCollider;

    protected override void Awake()
    {
        base.Awake();
        _canFly = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMoveSpeed = _enemyMoveSpd; // Lưu tốc độ gốc
        originalRunSpeed = _enemyRunSpd;
        SetupInstantExplodeCollider();
    }

    private void SetupInstantExplodeCollider()
    {
        GameObject triggerObj = new GameObject("InstantExplodeTrigger");
        triggerObj.transform.SetParent(transform);
        triggerObj.transform.localPosition = Vector3.zero;

        CircleCollider2D collider = triggerObj.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = _instantExplodeDistance;

        instantExplodeCollider = collider;

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

        // Lưu lại player reference khi phát hiện lần đầu
        if (_player != null && !hasDetectedPlayer)
        {
            rememberedPlayer = _player;
        }

        CheckPlayerDetection();

        // Sau khi đã phát hiện player, luôn ở trạng thái đuổi theo
        if (hasDetectedPlayer)
        {
            isStay = false;
            isPatrol = false;

            // Override _player bằng rememberedPlayer để không bị mất
            if (rememberedPlayer != null)
            {
                _player = rememberedPlayer;
            }
        }
        else
        {
            // Chỉ patrol khi chưa phát hiện player
            isStay = base._isStay;
            isPatrol = base._isPatrol;
        }

        UpdateAnimator();
    }

    protected override void moveToPlayer()
    {
        // Sau khi đã phát hiện player, luôn cố gắng di chuyển đến player (dù player có ra khỏi tầm hay không)
        if (hasDetectedPlayer && rememberedPlayer != null)
        {
            _player = rememberedPlayer;

            // Tính toán tốc độ dựa trên countdown
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
            // Tính toán progress: 0 = mới bắt đầu, 1 = sắp nổ
            float timeProgress = Mathf.Clamp01(1f - (countdownTimer / _countdownDuration));

            // Sử dụng curve để tạo hiệu ứng tăng tốc mượt mà hơn
            // Pow càng cao thì tăng tốc càng chậm ở đầu, nhanh ở cuối
            float curvedProgress = Mathf.Pow(timeProgress, _speedMultiplierCurve);

            // Tính tốc độ mới
            float newSpeed = Mathf.Lerp(originalMoveSpeed, _maxMoveSpeedDuringCountdown, curvedProgress);
            base._enemyRunSpd = newSpeed;
        }
        else
        {
            base._enemyRunSpd = originalRunSpeed;
        }
    }

    protected override void Patrol()
    {
        base._enemyMoveSpd = originalMoveSpeed;
        base.Patrol();
    }

    private void CheckPlayerDetection()
    {
        bool playerInRange = _player != null &&
                           Vector3.Distance(transform.position, _player.transform.position) <= _detectionRange;

        // Nếu player vào tầm phát hiện lần đầu
        if (playerInRange && !hasDetectedPlayer)
        {
            hasDetectedPlayer = true;
            rememberedPlayer = _player; // Lưu lại player reference
            explodeCoroutine = StartCoroutine(ExplodeAfterDelay(_countdownDuration));
            isCountingDown = true;
        }

    }

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

        // Set tốc độ về tối đa khi sắp nổ ngay
        base._enemyRunSpd = _maxMoveSpeedDuringCountdown;

        TriggerExplosion();
    }

    private System.Collections.IEnumerator ExplodeAfterDelay(float delay)
    {
        countdownTimer = delay;
        float flashInterval = 0.2f;
        bool flashRed = true;

        // Tính toán khoảng thời gian flash ngắn dần (tạo cảm giác gấp gáp)
        float initialFlashInterval = 0.5f;
        float finalFlashInterval = 0.1f;

        while (countdownTimer > 0)
        {
            // Tính progress để điều chỉnh flash speed
            float timeProgress = Mathf.Clamp01(1f - (countdownTimer / delay));

            // Flash nhanh dần khi gần nổ
            flashInterval = Mathf.Lerp(initialFlashInterval, finalFlashInterval, timeProgress);

            if (spriteRenderer != null)
                spriteRenderer.color = flashRed ? Color.red : Color.white;

            flashRed = !flashRed;

            yield return new WaitForSeconds(flashInterval);
            countdownTimer -= flashInterval;

            // Cập nhật tốc độ liên tục trong countdown
            if (hasDetectedPlayer)
            {
                CalculateSpeedBasedOnCountdown();
            }
        }

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        // Luôn nổ sau khi hết thời gian countdown
        TriggerExplosion();
    }

    private void TriggerExplosion()
    {
        if (rememberedPlayer != null)
        {
            float distance = Vector3.Distance(transform.position, rememberedPlayer.transform.position);
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
        HealthManager health = rememberedPlayer.GetComponent<HealthManager>();
        if (health != null)
        {
            health.takeDamage(1, base._enemyPhysicalDamage, false);
            Debug.Log($"PhongHuyetTrung attacks player with {_enemyPhysicalDamage} physical damage.");
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