using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private GameObject _enemy;

    public void takeDamage(float damage, bool magic)
    {
        EnemyBase enemyBase = _enemy.GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
            enemyBase.takeDamage();
            Debug.Log($"Enemy {enemyBase.name} took {damage} damage.");
            enemyBase.setEnemyHP(damage, magic);
        }
    }

}