using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private playerRada _rada;
    private Rigidbody2D _rb;

    private Coroutine _resetKnocked;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void takeDamage(int _id, float _damage, bool _magic)
    {
        PlayerManager.Instance.Stats.takeDamage(_damage, _magic);
        if (_rada._attackColliders.Count != 0)
        {
            float _flyPower = 1f;
            float _nearestDistance;
            Vector2 _directionToEnemy;
            _rada.GetNearestTarget(transform, out _nearestDistance, out _directionToEnemy);

            switch (_id)
            {
                case 1:
                    _flyPower = 3f;
                    break;
                case 2:
                    _flyPower = 0f;
                    break;
                case 3:
                    _flyPower = 8f;
                    break;
            }

            if (_nearestDistance > 0.1) transform.localScale = new Vector3(-1, 1, 1);
            else if (_nearestDistance < -0.1) transform.localScale = new Vector3(1, 1, 1);

            PlayerManager.Instance._knocked = false;
            if (_resetKnocked != null)
            {
                StopCoroutine(_resetKnocked);
                _resetKnocked = null;
            }
            _resetKnocked = StartCoroutine(resetKnocked());

            _rb.linearVelocity = new Vector2(_directionToEnemy.x * _flyPower * (_flyPower == 3 ? 0 : (-1)), _directionToEnemy.y * _flyPower);
        }
    }
    private IEnumerator resetKnocked()
    {
        yield return new WaitForSeconds(0.15f);
        PlayerManager.Instance._knocked = true;
    }
}
