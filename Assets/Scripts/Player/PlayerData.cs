using System.Collections;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public float maxHealth;
    public float invulnerableTime;
    public float flashInterval = 0.2f;
    public Color defaultColour = new Color(1f, 0.922f, 0.016f, 1f);
    public Color flashColour = new Color(1f, 0.922f, 0.016f, 0.5f);


    private float curHealth;
    public bool IsVulnerable => isVulnerable;
    private bool isVulnerable;
    public bool IsDead => isDead;
    private bool isDead;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curHealth = maxHealth;
        isVulnerable = true;
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        if (!isDead && isVulnerable)
        {
            curHealth -= damage;

            if (curHealth <= 0)
            {
                isDead = true;
                // death
                Debug.Log("you died cuh"); //remove this 
            }
            else
            {
                isVulnerable = false;
                StartCoroutine(InvulnerableTimer());
            }
        }
    }

    public void TakeHealing(float healing)
    {
        if (!isDead)
        {
            curHealth += healing;
        }
    }

    IEnumerator InvulnerableTimer()
    {
        float elapsedTime = 0f;

        while (elapsedTime < invulnerableTime)
        {
            elapsedTime += flashInterval;
            yield return new WaitForSeconds(flashInterval);
        }

        isVulnerable = true;
    }

    public bool isMaxHealth()
    {
        return curHealth.Equals(maxHealth);
    }
}
