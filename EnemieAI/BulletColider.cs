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

            targetRenderer = FindRendererInChildren(other.transform, "Cylinder.001");

            if (targetRenderer != null)
            {
                Debug.Log("Renderer found, changing to hit material.");

                StartCoroutine(ChangeMaterialTemporarily(targetRenderer, hitMaterial, originalMaterial, 0.2f));
            }
            else
            {
                Debug.LogError("Renderer not found! Check if the child object name 'Cylinder.001' is correct.");
            }

            HealthBarScript health = other.GetComponent<HealthBarScript>();

            if (health != null)
            {
                damagePlayer(10f, health); 
            }

            Destroy(gameObject);
        }
    }

    public void damagePlayer(float damageAmount, HealthBarScript health)
    {
        health.TakeDamage(damageAmount);
    }

    private IEnumerator ChangeMaterialTemporarily(Renderer renderer, Material hitMaterial, Material originalMaterial, float delay)
    {
        ApplyMaterialToRenderer(renderer, hitMaterial);
        Debug.Log("Material switched to hit material.");

        yield return new WaitForSeconds(delay);

        ApplyMaterialToRenderer(renderer, originalMaterial);
        Debug.Log("Material switched back to original material.");
    }

    private void ApplyMaterialToRenderer(Renderer renderer, Material material)
    {
        if (renderer != null)
        {
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = material; 
            }
            renderer.materials = newMaterials;
        }
        else
        {
            Debug.LogError("Renderer is null, cannot apply materials.");
        }
    }

    private Renderer FindRendererInChildren(Transform parent, string childName)
    {
        foreach (Renderer renderer in parent.GetComponentsInChildren<Renderer>())
        {
            if (renderer.gameObject.name == childName)
            {
                return renderer;
            }
        }
        return null;
    }
}
