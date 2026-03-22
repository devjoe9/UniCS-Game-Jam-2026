using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordController : MonoBehaviour
{
    public Sprite blueSprite;
    public Sprite redSprite;
    public int defaultDamage = 5;
    public float defaultKnockbackForce = 5;
    
    private int damage;
    private float knockbackForce;
    private bool isBlue;
    private string initialDirection;
    private SpriteRenderer spriteRenderer;
    private SideData sideData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sideData = GetComponent<SideData>();
        isBlue = sideData.IsBlue;
        
        // condition ? if true : if false
        spriteRenderer.sprite = isBlue ? blueSprite : redSprite;
        damage = defaultDamage;
        knockbackForce = defaultKnockbackForce;

        initialDirection = transform.parent.name;
        if (initialDirection.Equals("L"))
        {
            transform.Rotate(0, 0, 180);
            transform.localPosition = new Vector3(-1.5f, 0, 0);
        }
        else if (initialDirection.Equals("U"))
        {
            transform.Rotate(0, 0, 90);
            transform.localPosition = new Vector3(0.04f, 1.5f, 0);
        }
        else if (initialDirection.Equals("D"))
        {
            transform.Rotate(0, 0, -90);
            transform.localPosition = new Vector3(0.04f, -1.5f, 0);
        }
        else
        {
            transform.localPosition = new Vector3(1.5f, 0, 0);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("in trigger");
        Debug.Log("Hit object: " + collision.name);
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log($"In compare tag, isBlue = {isBlue}");
            EnemyData enemyData = collision.GetComponent<EnemyData>();
            if (enemyData != null && !enemyData.IsDead && enemyData.IsBlue.Equals(isBlue) && enemyData.IsVulnerable)
            {
                Debug.Log("Sword colliding with enemy");
                enemyData.TakeDamage(damage);
                enemyData.TakeKnockback(transform.right, knockbackForce);
            }
        }
    }
}
