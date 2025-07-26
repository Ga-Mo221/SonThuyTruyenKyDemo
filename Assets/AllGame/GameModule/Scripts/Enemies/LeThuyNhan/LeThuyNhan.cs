using UnityEngine;

public class LeThuyNhan : EnemyBase
{
    [Header("LeThuyNhan Settings")]
    [SerializeField] private float _attackCooldown = 1f;

    private bool _isAttacking = false;
    private bool _isHurt = false;
    private bool _canAttack = true;
    private bool _isLive = true;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        _canFly = false;
    }

    protected override void Update()
    {
        base.Update();

        if (!_isLive) return;

        UpdateStates();
        UpdateAnimator();
    }

    protected override void FixedUpdate()
    {
        if (!_isLive || _isAttacking || _isHurt) return;

        base.FixedUpdate();
    }

    // Hàm cập nhật trạng thái tấn công
    private void UpdateStates()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.transform.position);
        // Tấn công nếu đủ gần
        if (_canAttack && distance <= _enemyAttackRange)
        {
            Attack();
        }
    }

    // Hàm tấn công
    protected override void Attack()
    {
        _isAttacking = true;
        _canAttack = false;
        _rb.linearVelocity = Vector2.zero;
        _animator.SetTrigger("isAttack");
    }

    // Hàm nhận Dmg
    public override void takeDamage()
    {
        if (!_isLive || _isHurt) return;

        base.takeDamage(); // xử lý knockback
        _isHurt = true;
        _rb.linearVelocity = Vector2.zero;
        _animator.SetTrigger("isHurt");
    }

    public override void Die()
    {
        if (!_isLive) return;

        _isLive = false;
        base.Die(); // xử lý rigidbody, hủy object
    }

    // Hàm cập nhật về trạng thái Walk
    private void UpdateAnimator()
    {
        if (_animator == null) return;

        bool isWalking = _isMoving && !_isAttacking && !_isHurt;
        _animator.SetBool("isWalking", isWalking);
    }

    // === Animation Event Callbacks ===

    // Gọi từ animation event khi tấn công đánh trúng
    public void onAttackHit()
    {
        if (_player == null) return;

        float dist = Vector2.Distance(transform.position, _player.transform.position);
        if (dist <= _enemyAttackRange)
        {
            HealthManager hp = _player.GetComponent<HealthManager>();
            if (hp != null)
            {
                hp.takeDamage(1, _enemyPhysicalDamage, false);
                Debug.Log($"LeThuyNhan attacks player with {_enemyPhysicalDamage} damage.");
            }
        }
    }

    // Gọi từ animation event khi animation tấn công kết thúc
    public void onAttackFinished()
    {
        _isAttacking = false;
        StartCoroutine(AttackCooldownRoutine());
    }

    private System.Collections.IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(_attackCooldown);
        _canAttack = true;
    }

    // Gọi từ animation event khi hurt animation kết thúc
    public void onHurtFinished()
    {
        _isHurt = false;
    }
}
