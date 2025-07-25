using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    // slider
    [SerializeField] private Slider _health;
    [SerializeField] private Slider _mana;
    [SerializeField] private Slider _Stamina;

    // text
    [SerializeField] private TextMeshProUGUI _xeng;

    // life
    [SerializeField] private Image _life1;
    [SerializeField] private Image _life2;
    [SerializeField] private Image _life3;

    // image
    [SerializeField] private Image _dash;

    private int _currentliveCount;

    void Start()
    {
        setPlayerStatus();
    }

    void Update()
    {
        updatePlayerStatus();
        life();
    }

    private void setPlayerStatus()
    {
        // slider
        _health.maxValue = PlayerManager.Instance.Stats._maxHealth;
        _mana.maxValue = PlayerManager.Instance.Stats._maxMana;
        _Stamina.maxValue = PlayerManager.Instance.Stats._stamina;

        //text
        _xeng.text = PlayerManager.Instance.Stats._xeng.ToString();

        // life
        _currentliveCount = PlayerManager.Instance.Stats._currentLifeCount;

        // dash
        _dash.fillAmount = PlayerManager.Instance._dashTime;
    }

    private void updatePlayerStatus()
    {
        // lider
        _health.value = PlayerManager.Instance.Stats._currentHealth;
        _Stamina.value = PlayerManager.Instance._stamina;

        // text
        _xeng.text = PlayerManager.Instance.Stats._xeng.ToString();

        // dash
        _dash.fillAmount = PlayerManager.Instance._dashTime;
    }

    private void life()
    {
        _currentliveCount = PlayerManager.Instance.Stats._currentLifeCount;
        switch (_currentliveCount)
        {
            case 0:
                _life1.enabled = false;
                _life2.enabled = false;
                _life3.enabled = false;
                break;
            case 1:
                _life1.enabled = true;
                _life2.enabled = false;
                _life3.enabled = false;
                break;
            case 2:
                _life1.enabled = true;
                _life2.enabled = true;
                _life3.enabled = false;
                break;
            case 3:
                _life1.enabled = true;
                _life2.enabled = true;
                _life3.enabled = true;
                break;
        }
    }
}
