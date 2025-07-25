using UnityEngine;

public class TrapTest : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.name);
        HealthManager _player;
        _player = collision.GetComponent<HealthManager>();
        if (_player != null)
        {
            _player.takeDamage(0, 9999999999, false);
        }
    }
}
