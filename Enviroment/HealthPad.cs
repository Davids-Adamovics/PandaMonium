using System.Collections;
using UnityEngine;

public class HealingPad : MonoBehaviour
{
    public GameObject[] objectsToDisable; // Array of objects to deactivate (can assign in the inspector)
    public float healAmount = 25f;        // Amount of health to restore
    public float reactivationTime = 30f;  // Time to wait before reactivating the pad
    public GameObject player;             // Reference to the player GameObject (drag into the Inspector)

    private bool isActive = true;         // To check if the healing pad is active

    // This method is called when something enters the collider attached to the HealingPad object
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && isActive)
        {
            HealPlayer();
            StartCoroutine(DisableHealingPad());
        }
    }


    // Heal the player using the HealthBarScript
    private void HealPlayer()
    {
        HealthBarScript healthBar = player.GetComponent<HealthBarScript>();
        if (healthBar != null)
        {
            healthBar.Heal(healAmount); // Heal the player for the specified amount
            Debug.Log("Player healed");
        }
    }

    // Coroutine to deactivate objects and reactivate them after a delay
    private IEnumerator DisableHealingPad()
    {
        // Deactivate the healing pad
        isActive = false;

        // Deactivate specified objects
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false); // Disable the object
            }
        }

        // Wait for the specified reactivation time
        yield return new WaitForSeconds(reactivationTime);

        // Reactivate specified objects
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(true); // Re-enable the object
            }
        }

        // Mark the healing pad as active again
        isActive = true;
    }
}
