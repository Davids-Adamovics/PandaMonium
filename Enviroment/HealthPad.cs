using System.Collections;
using UnityEngine;
using TMPro;

public class HealingPad : MonoBehaviour
{
    public GameObject[] objectsToDisable;
    public float healAmount = 25f;
    public float reactivationTime = 30f;
    public GameObject player;
    public TextMeshPro TimeLeft;
    public AudioSource HealingPadTaken;

    private bool isActive = true;

    void Update()
    {
        if (TimeLeft != null)
        {
            TimeLeft.transform.position = transform.position + new Vector3(0, 2, 0);
            Vector3 directionToPlayer = player.transform.position - TimeLeft.transform.position;
            directionToPlayer.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            TimeLeft.transform.rotation = lookRotation * Quaternion.Euler(0, 180, 0);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && isActive)
        {
            HealPlayer();
            HealingPadTaken.Play();
            StartCoroutine(DisableHealingPad());
        }
    }

    private void HealPlayer()
    {
        HealthBarScript healthBar = player.GetComponent<HealthBarScript>();
        if (healthBar != null)
        {
            healthBar.Heal(healAmount);
        }
    }

    private IEnumerator DisableHealingPad()
    {
        isActive = false;

        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        float timeRemaining = reactivationTime;

        while (timeRemaining > 0)
        {
            TimeLeft.text = timeRemaining.ToString("F0");
            yield return new WaitForSeconds(1f);
            timeRemaining--;
        }

        TimeLeft.text = "";

        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        isActive = true;
    }
}
