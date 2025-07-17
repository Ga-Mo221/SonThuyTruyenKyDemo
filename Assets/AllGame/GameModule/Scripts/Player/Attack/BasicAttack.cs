using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    public int _ID;
    private float _damage;

    void OnTriggerEnter2D(Collider2D collision)
    {
        HinhNom hinhnom = collision.GetComponent<HinhNom>();
        if (hinhnom != null)
        {
            _damage = PlayerManager.Instance.Stats.getPhysicDamage();
            hinhnom.hit(_damage, _ID);
        }
    }
}
