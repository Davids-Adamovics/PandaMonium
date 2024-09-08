using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 11; 
    public float bulletLifetime = 5f; 
    public LayerMask whatIsGround;

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
        else if (other.gameObject.layer == LayerMask.NameToLayer("WhatIsGround"))
        {
            Destroy(gameObject);
        }
    }
}
