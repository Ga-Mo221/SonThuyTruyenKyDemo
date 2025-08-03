using System.Collections;
using UnityEngine;

public class LeThuyNhan : EnemyBase
{
    [Header("LeThuyNhan Settings")]
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _attackAnimationDuration = 0.8f; // Thời gian animation attack
    [SerializeField] private float _hurtAnimationDuration = 0.5f;   // Thời gian animation hurt


    // State management
    private bool _isAttacking = false;
    private bool _isHurt = false;
    private bool _canAttack = true;
    private bool _isLive = true;
    private bool _isInAttackCooldown = false;
    // Timers
    private float _stateTimer = 0f;
    private Coroutine _currentStateCoroutine;
    
    // Animation control
    private bool _lastWalkingState = false;

    protected override void Awake()
    {
        base.Awake();
        _canFly = false;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (!_isLive) return;
        
        base.Update();
        UpdateStateTimer();
        CheckForAttack();
        UpdateAnimations();
    }

    protected override void FixedUpdate()
    {
        if (!_isLive) return;

        // Nếu đang trong trạng thái cố định (attack/hurt), dừng movement
        if (ShouldStopMovement())
        {
            StopMovement();
            return;
        }

        if (_player == null && _enemyRada != null && _enemyRada._player != null)
        {
            _player = _enemyRada._player;
            Debug.Log($"Rada thấy: {_enemyRada._player}");

        }

        base.FixedUpdate();
    }

    #region State Management
    
    protected override bool ShouldStopMovement()
    {
        return _isAttacking || _isHurt || _isKnockedBack;
    }
    
    private void StopMovement()
    {
        if (_rb != null)
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            _isMoving = false;
        }
    }
        
    private void UpdateStateTimer()
    {
        if (_stateTimer > 0f)
        {
            _stateTimer -= Time.deltaTime;
            if (_stateTimer <= 0f)
            {
                OnStateTimerExpired();
            }
        }
    }
    
    private void OnStateTimerExpired()
    {
        if (_isAttacking)
        {
            EndAttack();
        }
        else if (_isHurt)
        {
            EndHurt();
        }
    }
    
    #endregion

    #region Attack Logic
    
    private void CheckForAttack()
    {
        if (!CanInitiateAttack()) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, _player.transform.position);
        
        float attackCheckRange = _enemyAttackRange * 1.1f;
        
        if (distanceToPlayer <= attackCheckRange)
        {
            InitiateAttack();
        }
    }
    
    private bool CanInitiateAttack()
    {
        return _player != null && 
               _isLive && 
               _canAttack && 
               !_isAttacking && 
               !_isHurt && 
               !_isKnockedBack &&
               !_isInAttackCooldown;
    }
    
    private void InitiateAttack()
    {
        Debug.Log("LeThuyNhan: Initiating attack");
        
        // Set states
        _isAttacking = true;
        _canAttack = false;
        _stateTimer = _attackAnimationDuration;
        
        // Stop movement completely
        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _isMoving = false;
        }
        
        // Face player before attacking
        FacePlayer();
        
        // Trigger animation
        if (_animator != null)
        {
            _animator.SetTrigger("isAttack");
        }
        
        // Clear any existing coroutine
        if (_currentStateCoroutine != null)
        {
            StopCoroutine(_currentStateCoroutine);
        }
    }
    
    private void EndAttack()
    {
        Debug.Log("LeThuyNhan: Attack ended");
        
        _isAttacking = false;
        _stateTimer = 0f;
        
        // Start attack cooldown
        StartAttackCooldown();
    }
    
    private void StartAttackCooldown()
    {
        _isInAttackCooldown = true;
        _currentStateCoroutine = StartCoroutine(AttackCooldownCoroutine());
    }
    
    private IEnumerator AttackCooldownCoroutine()
    {
        yield return new WaitForSeconds(_attackCooldown);
        
        if (_isLive)
        {
            _canAttack = true;
            _isInAttackCooldown = false;
        }
        
        _currentStateCoroutine = null;
    }
    
    protected override void Attack()
    {
        if (!_isLive || _player == null) return;
        
        float distance = Vector2.Distance(transform.position, _player.transform.position);
        float damageRange = _enemyAttackRange * 1.2f;
        
        if (distance <= damageRange)
        {
            HealthManager playerHealth = _player.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.takeDamage(1, _enemyPhysicalDamage, false);
                Debug.Log($"LeThuyNhan deals {_enemyPhysicalDamage} damage to player (distance: {distance:F2})");
            }
        }
        else
        {
            Debug.Log($"LeThuyNhan attack missed - distance: {distance:F2}, range: {damageRange:F2}");
        }
    }
    
    #endregion

    #region Damage and Death
    
    public override void takeDamage()
    {
        if (!_isLive) return;
        
        Debug.Log("LeThuyNhan: Taking damage");
        
        // Call base knockback
        base.takeDamage();
        
        // Set hurt state
        _isHurt = true;
        _stateTimer = _hurtAnimationDuration;
        
        // Interrupt attack if currently attacking
        if (_isAttacking)
        {
            _isAttacking = false;
        }
        
        // Stop movement completely
        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _isMoving = false;
        }
        
        // Trigger hurt animation
        if (_animator != null)
        {
            _animator.SetTrigger("isHurt");
        }
        
        // Clear any existing state coroutine but keep attack cooldown running if active
        if (_currentStateCoroutine != null && !_isInAttackCooldown)
        {
            StopCoroutine(_currentStateCoroutine);
            _currentStateCoroutine = null;
        }
    }
    
    private void EndHurt()
    {
        Debug.Log("LeThuyNhan: Hurt state ended");
        
        _isHurt = false;
        _stateTimer = 0f;
        
        // If not in attack cooldown, can attack again
        if (!_isInAttackCooldown)
        {
            _canAttack = true;
        }
    }
    
    public override void Die()
    {
        if (!_isLive) return;
        
        Debug.Log("LeThuyNhan: Dying");
        
        _isLive = false;
        _isAttacking = false;
        _isHurt = false;
        _canAttack = false;
        _isInAttackCooldown = false;
        _stateTimer = 0f;
        
        // Stop all coroutines
        StopAllCoroutines();
        
        // Call base die
        base.Die();
    }
    
    #endregion

    #region Animation and Visual
    
    private void UpdateAnimations()
    {
        if (_animator == null || !_isLive) return;
        
        // Walking animation - only when actually moving and not in special states
        bool shouldWalk = _isMoving && 
                         !_isAttacking && 
                         !_isHurt && 
                         !_isKnockedBack;
        
        // Only update if state changed to reduce animator calls
        if (shouldWalk != _lastWalkingState)
        {
            _animator.SetBool("isWalking", shouldWalk);
            _lastWalkingState = shouldWalk;
        }
    }
    
    private void FacePlayer()
    {
        if (_player == null) return;
        
        float direction = _player.transform.position.x - transform.position.x;
        
        // Only flip if there's significant difference to avoid jittering
        if (Mathf.Abs(direction) > 0.1f)
        {
            Vector3 scale = transform.localScale;
            scale.x = direction > 0 ? 1f : -1f;
            transform.localScale = scale;
        }
    }
    
    #endregion

    #region Animation Events (called from Animator)
    
    public void onAttackHit()
    {
        // Called by animation event at the moment of impact
        Attack();
    }
    
    public void onAttackFinished()
    {
        // Called by animation event when attack animation completes
        if (_isLive && _isAttacking)
        {
            EndAttack();
        }
    }
    
    public void onHurtFinished()
    {
        // Called by animation event when hurt animation completes
        if (_isLive && _isHurt)
        {
            EndHurt();
        }
    }
    
    #endregion
}