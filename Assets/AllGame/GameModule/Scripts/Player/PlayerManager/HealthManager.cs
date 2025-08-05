using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private playerRada _rada;
    [SerializeField] private GameObject _damagePrefab;
    [SerializeField] private Transform _profile;


    [SerializeField] private float _knockedTime = 0.8f; // Thời gian bị knockback
    private Rigidbody2D _rb;
    private Transform _displayDamageTextPos;
    private GameObject _GameOver;

    private Coroutine _resetKnocked;

    private float _flyPower;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_profile == null)
        {
            Debug.LogError("Chưa gắn Profile vào Player.component<HealthManager>");
        }
        _displayDamageTextPos = _profile.Find("DamageText");
        _GameOver = _profile.Find("GameOver").gameObject;
    }

    public void takeDamage(int _id, float _damage, bool _magic)
    {
        switch (_id)
        {
            case 1:
                _flyPower = 8f;
                break;
            case 2:
                _flyPower = 0f;
                break;
            case 3:
                _flyPower = 20f;
                break;
            default:
                _flyPower = 1;
                break;
        }
        float damage = PlayerManager.Instance.Stats.takeDamage(_damage, _magic);
        damageText(damage, _magic);
        checkIsALive();
        if (_flyPower == 1) return;
        if (_rada._attackColliders.Count != 0 && PlayerManager.Instance._isAlive)
        {
            handleKnockback();
        }
    }

    private void damageText(float damage, bool magic)
    {
        if (_displayDamageTextPos == null) Debug.LogError("_displayDamageTextPos Null");
        if (_damagePrefab == null) Debug.LogError("_damagePrefab Null");
        GameObject _Text = Instantiate(_damagePrefab, transform.position, Quaternion.identity, _displayDamageTextPos);
        var _text = _Text.GetComponent<DamageText>();

        _text.setDamage(damage, magic);
    }


    private void handleKnockback()
    {
        float _nearestDistance;
        Vector2 _directionToEnemy;
        _rada.GetNearestTarget(transform, out _nearestDistance, out _directionToEnemy);

        if (_nearestDistance > 0.1) transform.localScale = new Vector3(1, 1, 1);
        else if (_nearestDistance < -0.1) transform.localScale = new Vector3(-1, 1, 1);

        PlayerManager.Instance._knocked = true;
        if (_resetKnocked != null)
        {
            StopCoroutine(_resetKnocked);
            _resetKnocked = null;
        }
        _resetKnocked = StartCoroutine(resetKnocked());
        _rb.linearVelocity = new Vector2(0, 0);
        float _direction = _directionToEnemy.x > 0 ? -1 : 1;
        float x = _direction * _flyPower;
        _rb.linearVelocity = new Vector2(x, _flyPower);
    }


    private IEnumerator resetKnocked()
    {
        yield return new WaitForSeconds(_knockedTime);
        PlayerManager.Instance._knocked = false;
    }

    private void checkIsALive()
    {
        if (PlayerManager.Instance.Stats._currentHealth <= 0)
        {
            PlayerManager.Instance._isAlive = false;
            PlayerManager.Instance.Stats.lifeCount(false);
            if (PlayerManager.Instance.Stats._currentLifeCount > 0)
            {
                StartCoroutine(revive());
            }
            else
            {
                Debug.Log("Player chết");
                StartCoroutine(restartGame());
            }
        }
    }

    private IEnumerator revive()
    {
        transform.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(1f);
        transform.GetComponent<SpriteRenderer>().enabled = true;
        PlayerManager.Instance._isAlive = true;
        Vector3 _point = PlayerManager.Instance._respawnPoint;
        if (_point != Vector3.zero || _point != null)
            transform.position = _point;
    }

    private IEnumerator restartGame()
    {
        transform.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(1f);
        _GameOver.SetActive(true);
    }
}
