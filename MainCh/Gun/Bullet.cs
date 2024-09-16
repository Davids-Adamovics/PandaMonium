using UnityEngine;
using TMPro;

public class Bullet : MonoBehaviour
{
    public int damage = 11;
    public float bulletLifetime = 5f;
    public LayerMask whatIsGround;
    public GameObject damageTextPrefab;
    public GameObject hitParticlePrefab;

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
                int previousDamage = enemy.TakeDamage(damage);
                ShowDamageIndicator(other.transform.position, previousDamage);

                PlayHitParticle(other.transform.position);
            }

            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("WhatIsGround"))
        {
            Destroy(gameObject);
        }
    }

    void ShowDamageIndicator(Vector3 position, int damageAmount)
    {
        Vector3 spawnPosition = position + Vector3.up * 2 + Vector3.forward * -2f;
        GameObject damageText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity);

        TextMeshPro textMeshPro = damageText.GetComponent<TextMeshPro>();
        if (textMeshPro != null)
        {
            textMeshPro.text = damageAmount.ToString();
        }

        damageText.transform.LookAt(Camera.main.transform);
        damageText.transform.Rotate(0, 180, 0);

        Destroy(damageText, 1f);
    }

    void PlayHitParticle(Vector3 position)
    {
        if (hitParticlePrefab != null)
        {

            GameObject particleInstance = Instantiate(hitParticlePrefab, position, Quaternion.identity);

            particleInstance.transform.LookAt(Camera.main.transform);

            particleInstance.transform.Rotate(0, 180, 0);

            ParticleSystem particleSystem = particleInstance.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }

            Destroy(particleInstance, 2f);
        }
    }

}
