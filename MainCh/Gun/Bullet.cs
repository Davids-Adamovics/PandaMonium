using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 11; 
    public float bulletLifetime = 5f; 

    private void Start()
    {
        Destroy(gameObject, bulletLifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy_AI enemy = other.GetComponentInParent<Enemy_AI>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
