using System.Collections;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int maxHealth;
    public float invulnerableTime;

    private int curHealth;

    public bool IsVulnerable => isVulnerable;
    private bool isVulnerable;
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

    public void TakeDamage(int damage)
    {
        if (!isDead && isVulnerable)
        {
            curHealth -= damage;

            if (curHealth <= 0)
            {
                isDead = true;
                // death
            }
            else
            {
                isVulnerable = false;
                StartCoroutine(InvulnerableTimer());
            }
        }
    }

    IEnumerator InvulnerableTimer()
    {
        yield return new WaitForSeconds(invulnerableTime);
        isVulnerable = true;
    }
}
