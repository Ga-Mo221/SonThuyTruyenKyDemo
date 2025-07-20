using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private playerRada _rada;
    private Rigidbody2D _rb;

    private Coroutine _resetKnocked;
            
    private float _flyPower;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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
        PlayerManager.Instance.Stats.takeDamage(_damage, _magic);
        if (_flyPower == 1) return;
        if (_rada._attackColliders.Count != 0)
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
            Debug.Log($"x : {x} , _direction : {_direction} , scale : {transform.localScale.x}");
            _rb.linearVelocity = new Vector2(x, _flyPower);
        }
    }
    private IEnumerator resetKnocked()
    {
        yield return new WaitForSeconds(0.15f);
        PlayerManager.Instance._knocked = false;
    }
}
