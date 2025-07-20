using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    // Level
    public int _level;

    // Exp
    public float _currentExp; // exp hiện tại
    public float _requiredExp; // exp cần để lên cấp

    // HP
    public float _maxHealth; // máu tối đa
    public float _currentHealth; // máu hiện tại

    // Stamina
    public float _stamina; // thể lực

    // Move speed
    public float _walkSpeed; // tốc độ đi bộ
    public float _runSpeed; // tốc độ chạy

    // Nhảy
    public float _jumpForce; // lực nhảy

    // Damage
    public float _physicalDamage; // ST vật lý
    public float _magicDamage; // ST phép

    // Giáp
    public float _physicalDefense; // kháng vật lý tính theo phần trăm
    public float _armor; // giáp
    public float _magicResist; // kháng phép

    // Dash
    public float _dashPower; // tốc độ trượt
    public float _dashingTime;
    public float _dashingCooldown;

    // Attack speed
    public float _attackSpeed; // tốc độ đánh 100%

    // Delay
    public float _delay; // di chuyển, khoảng cách dash, tốc độ đánh (10%, 20%,...)

    // Tỉ lệ chí mạng
    public float _critChancePhysical; // ST vật lý
    public float _critChanceMagic; // ST phép
    public float _critMultiplier;      // Nhân 1.5x khi chí mạng

    // Giảm hồi chiêu
    public float _cooldownReduction; // 10%, 20%,...

    // tiền tệ
    public int _xeng;

    // Hướng dẫn
    public bool _tutorialRun;
    public bool _tutorialJump;
    public bool _tutorialSit;
    public bool _tutorialAttack;
    public bool _tutorialDash;

    // Kỹ năng đã mở
    public bool _doubleJump;
    public bool _skillQ;
    public bool _skillW;
    public bool _skillE;



    // tăng exp
    public bool GainExp(float amount)
    {
        _currentExp += amount;

        bool leveledUp = false;

        while (_currentExp >= _requiredExp)
        {
            LevelUp();
            leveledUp = true;
        }

        return leveledUp;
    }

    // xử lý lên cấp
    private void LevelUp()
    {
        _level++;
        _currentExp -= _requiredExp;
        _requiredExp = Mathf.RoundToInt(_requiredExp * 1.25f);

        _maxHealth += 20;
        _physicalDamage += 5;
        _magicDamage += 5;
        _armor += 2;
        _magicResist += 2;
        _attackSpeed += 0.05f;

        _currentHealth = _maxHealth;
    }

    // Tính tốc độ di chuyển dựa theo trạng thái chạy hoặc đi bộ và ảnh hưởng của delay.
    public float GetMoveSpeed(bool isRunning)
    {
        return isRunning ? _runSpeed - ((_runSpeed / 100) * _delay) : _walkSpeed - ((_walkSpeed / 100) * _delay);
    }

    public float getDashingCooldown()
    {
        return _dashingCooldown - ((_dashingCooldown / 100) * _cooldownReduction);
    }

    public float getDashPower()
    {
        return _dashPower - ((_dashPower / 100) * _delay);
    }

    // 1f = 100%
    public float getAttackSpeed()
    {
        float _atkspeed = _attackSpeed;
        _atkspeed = (_atkspeed - _delay) / 100;
        return _atkspeed;
    }

    public float getPhysicDamage()
    {
        float _randomValue = Random.Range(0f, 100f);
        bool _isCrit = _randomValue <= _critChancePhysical;

        float _damage = _isCrit ? _physicalDamage * _critMultiplier : _physicalDamage;

        return _damage;
    }

    public void takeDamage(float damage, bool magic)
    {
        if (!magic)
        {
            float _damage = damage * (1f - _physicalDefense / 100f);
            _damage = _damage - _armor;
            if (_damage <= 0) _damage = 1;
            _currentHealth -= _damage;
        }
        else
        {
            float _damage = damage - _magicResist;
            if (_damage <= 0) _damage = 1;
            _currentHealth -= _damage;
        }
    }
}
