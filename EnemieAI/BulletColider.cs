using System.Collections;
using UnityEngine;

public class BulletCollider : MonoBehaviour
{
    public Material originalMaterial;
    public Material hitMaterial;
    private Renderer targetRenderer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthBarScript health = other.GetComponent<HealthBarScript>();

            if (health != null)
            {
                damagePlayer(15f, health);
            }

            Destroy(gameObject, 0.3f);
        }
    }
    public void damagePlayer(float damageAmount, HealthBarScript health)
    {
        health.TakeDamage(damageAmount);
    }
}
