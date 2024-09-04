using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarScript : MonoBehaviour
{
    public Image MainChHealthBar;
    public float healthAmount = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(healthAmount <= 0)
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        if(Input.GetKeyDown(KeyCode.Return)){
            TakeDamage(20);
        }
        if(Input.GetKeyDown(KeyCode.F)){
            Heal(10);
        }
    }

    public void TakeDamage(float Damage){
        healthAmount -= Damage;
        MainChHealthBar.fillAmount = healthAmount / 100f;
    }

    public void Heal(float healingAmount){
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);

        MainChHealthBar.fillAmount = healingAmount / 100f;
    }
}
