using UnityEngine;
using TMPro; // Make sure to include this namespace

public class Bullet : MonoBehaviour
{
    public int damage = 11; 
    public float bulletLifetime = 5f; 
    public LayerMask whatIsGround;
    public GameObject damageTextPrefab;

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
}
