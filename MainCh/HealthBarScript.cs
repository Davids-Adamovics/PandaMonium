using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class HealthBarScript : MonoBehaviour
{
    public Image MainChHealthBar;
    public Image MainChBackground;
    public float healthAmount = 100;

    public PostProcessVolume postProcessVolume;
    private Vignette vignette;
    private bool isPulsing = false;

    void Start()
    {
        MainChBackground.gameObject.SetActive(true);
        postProcessVolume.profile.TryGetSettings(out vignette);
    }


    void Update()
    {
        if (healthAmount <= 0)
        {
            Application.LoadLevel(Application.loadedLevel);
            MainChBackground.gameObject.SetActive(true);
        }

        if (healthAmount <= 50 && !isPulsing)
        {
            vignette.color.value = Color.red;
        }
        else if (healthAmount > 50 && !isPulsing)
        {
            vignette.color.value = Color.black;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            TakeDamage(20);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Heal(10);
        }
    }

    public void TakeDamage(float Damage)
    {
        healthAmount -= Damage;
        MainChHealthBar.fillAmount = healthAmount / 100f;
        StartCoroutine(PulseVignette());
    }

    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);
        MainChHealthBar.fillAmount = healthAmount / 100f;
    }

    IEnumerator PulseVignette()
    {
        isPulsing = true;
        float pulseDuration = 0.2f;
        Color originalColor = vignette.color.value;

        for (float t = 0; t < 1; t += Time.deltaTime / pulseDuration)
        {
            vignette.color.value = Color.Lerp(Color.red, new Color(0.5f, 0, 0), Mathf.PingPong(t * 2, 1));
            yield return null;
        }

        vignette.color.value = healthAmount <= 50 ? Color.red : Color.black;
        isPulsing = false;
    }
}
