using System.Diagnostics.Contracts;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Singleton để truy cập PlayerManager ở bất kỳ đâu
    public static PlayerManager Instance { get; private set; }

    // player
    [SerializeField] private PlayerAnimationManager _animManager;

    // Dữ liệu chỉ số nhân vật chính
    public PlayerStats Stats = new PlayerStats();

    // player state
    public bool _isAlive = true; // còn sống
    public bool _knocked = false; // bị trúng đòn

    public float _dashTime = 0;
    public float _stamina = 0;


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

        LoadGameOrCreateNew();
    }

    public void SaveGame()
    {
        SaveSystem.SavePlayer(Stats);
    }

    public void LoadGameOrCreateNew()
    {
        PlayerStats loaded = SaveSystem.LoadPlayer();
        if (loaded != null)
        {
            Stats = loaded;
            Debug.Log("Đã load dữ liệu cũ");
        }
        else
        {
            Stats = CreateNewStats(); // Khi không có save file
            Debug.Log("Tạo mới nhân vật");
            //SaveGame();
        }
        _stamina = Stats._stamina;
        _dashTime = Stats._dashingCooldown;
    }

    // cộng thì số dương, giảm thì số âm 
    public void setAttackSpeed(float number)
    {
        Stats._attackSpeed += number;
        _animManager.setAttackSpeed();
    }

    private PlayerStats CreateNewStats()
    {
        return new PlayerStats
        {
            // Level
            _level = 1,

            // Exp
            _currentExp = 0f,
            _requiredExp = 100f,

            // HP
            _maxHealth = 100f,
            _currentHealth = 0f,

            // Stamina
            _stamina = 20000f,

            // Move speed
            _walkSpeed = 2f,
            _runSpeed = 6f,

            // Nhảy
            _jumpForce = 15f,

            // Damage
            _physicalDamage = 10f,
            _magicDamage = 5f,

            // Giáp
            _armor = 10f,
            _magicResist = 5f,

            // Dash
            _dashPower = 20f,
            _dashingTime = 0.2f,
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

            // Kỹ năng đã mở
            _doubleJump = false,
            _skillQ = false,
            _skillW = false,
            _skillE = false
        };
    }

    public void ResetGame() // gọi khi chọn "Chơi mới"
    {
        SaveSystem.DeleteSave();
        Stats = CreateNewStats();
    }
}

