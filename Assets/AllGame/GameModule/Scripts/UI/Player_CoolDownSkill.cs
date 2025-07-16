using TMPro;
using UnityEngine;

public class Player_CoolDownSkill : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dashCoolDown;
    [SerializeField] private TextMeshProUGUI _currentStamina;

    void Start()
    {
        _dashCoolDown.text = $"Dash : {PlayerManager.Instance._dashTime}";
        _currentStamina.text = $"Stamina : {PlayerManager.Instance._stamina}";
    }

    void Update()
    {
        _dashCoolDown.text = "Dash : " + PlayerManager.Instance._dashTime.ToString("F2");
        _currentStamina.text = "Stamina : " + PlayerManager.Instance._stamina.ToString("F2");
    }
}