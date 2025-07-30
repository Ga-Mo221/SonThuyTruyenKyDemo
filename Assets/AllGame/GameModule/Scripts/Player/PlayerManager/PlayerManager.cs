using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Singleton để truy cập PlayerManager ở bất kỳ đâu
    public static PlayerManager Instance { get; private set; }

    // player
    [SerializeField] private GameObject _player;
    private PlayerAnimationManager _animManager;

    // Dữ liệu chỉ số nhân vật chính
    public PlayerStats Stats = new PlayerStats();

    // player state
    public bool _isAlive = true; // còn sống
    public bool _knocked = false; // bị trúng đòn
    public bool _canMoveAttack = false; // có thể di chuyển khi tấn công

    public float _dashTime = 0;
    public float _stamina = 0;

    public Vector3 _respawnPoint;


    private void Awake()
    {
        // Đảm bảo chỉ có một PlayerManager tồn tại trong game
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Huỷ đối tượng trùng lặp
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject); // Giữ lại object này khi chuyển scene

        loadGameOrCreateNew();
        if (_player == null)
            Debug.LogError("chưa gắn player vào");
        else
            _animManager = _player.GetComponent<PlayerAnimationManager>();
    }

    public void saveGame()
    {
        SaveSystem.SavePlayer(Stats);
    }

    public void loadGameOrCreateNew()
    {
        PlayerStats loaded = SaveSystem.LoadPlayer();
        if (loaded != null)
        {
            Stats = loaded;
            Debug.Log("Đã load dữ liệu cũ");
        }
        else
        {
            Stats = createNewStats(); // Khi không có save file
            Debug.Log("Tạo mới nhân vật");
            //SaveGame();
        }
        _isAlive = true;
        _knocked = false;
        _stamina = Stats._stamina;
        _dashTime = Stats._dashingCooldown;
    }

    // cộng thì số dương, giảm thì số âm %. hãy gọi mỗi lần có thay đổi về speed
    public void setAttackSpeed()
    {
        Debug.Log("Đang set AttackSpeed");
        //Stats._attackSpeed += number;
        if (_animManager == null)
            Debug.LogError("_animManager là null");
        else 
            Debug.Log("Đang set AttackSpeed");
        _animManager.setAttackSpeed();
    }

    private PlayerStats createNewStats()
    {
        return new PlayerStats
        {
            // Level
            _level = 1,

            // Exp
            _currentExp = 0f,
            _requiredExp = 100f,

            // Mạng sống
            _lifeCount = 3,
            _currentLifeCount = 3,

            // Mana 
            _maxMana = 100,
            _currentMana = 0,

            // HP
            _maxHealth = 100f,
            _currentHealth = 100f,

            // Stamina
            _stamina = 200f,

            // Move speed
            _walkSpeed = 5f,
            _runSpeed = 12f,

            // Nhảy
            _jumpForce = 22f,

            // Damage
            _physicalDamage = 10f,
            _magicDamage = 5f,

            // Giáp
            _armor = 10f,
            _magicResist = 5f,

            // Dash
            _dashPower = 20f,
            _dashingTime = 0.4f,
            _dashingCooldown = 1f,

            // Attack speed (%)
            _attackSpeed = 100f,

            // Delay (%)
            _delay = 0f,

            // Tỉ lệ chí mạng
            _critChancePhysical = 0f,
            _critChanceMagic = 0f,
            _critMultiplier = 1.5f,

            // Giảm hồi chiêu (%)
            _cooldownReduction = 0,

            // Tiền tệ
            _xeng = 0,

            // Hướng dẫn
            _tutorialRun = true,
            _tutorialJump = true,
            _tutorialSit = true,
            _tutorialAttack = true,
            _tutorialDash = true,

            // Kỹ năng đã mở
            _doubleJump = true,
            _skillQ = false,
            _skillW = false,
            _skillE = false
        };
    }

    public void resetGame() // gọi khi chọn "Chơi mới"
    {
        SaveSystem.DeleteSave();
        Stats = createNewStats();
    }

    // set vị trí trước khi chết để có thể quay lại vị trí đó
    public void setRespawnPoint(Vector3 pos)
    {
        _respawnPoint = pos;
    }

    // tăng mana hiện tại lên, nếu tăng thì add = true
    public void setMana(float mana, bool add)
    {
        if (add && Stats._currentMana < Stats._maxMana)
        {
            Stats._currentMana += mana;
            if (Stats._currentMana > Stats._maxMana)
                Stats._currentMana = Stats._maxMana;
        }
        else if (!add && Stats._currentMana > 0)
        {
            Stats._currentMana -= mana;
            if (Stats._currentMana < 0)
                Stats._currentMana = 0;
        }
    }

    // trừ thể lực mỗi khi dash
    public bool dash()
    {
        if (_stamina >= 40)
        {
            _stamina -= 40;
            return true;
        }
        else
        {
            return false;
        }
    }

    // add = true là bán và nhặt được xèng, add = false là bán đồ
    public bool setCoin(int value, bool add)
    {
        if (add)
        {
            Stats._xeng += value;
            return false;
        }
        else
        {
            if (Stats._xeng >= value)
            {
                Stats._xeng -= value;
                return true;
            }
            return false;
        }
    }
}
