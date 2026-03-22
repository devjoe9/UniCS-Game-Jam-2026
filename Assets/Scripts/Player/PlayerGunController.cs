using UnityEngine;

public class PlayerGunController : MonoBehaviour
{
    public Sprite blueSprite;
    public Sprite redSprite;
    public int defaultDamage = 5;
    public float defaultKnockbackForce = 5;
    public float manualShotCooldown = 0.25f;
    public float autoShotCooldown = 0.5f;
    public float bulletSpeed = 40;
    public GameObject bulletPrefab;
    public float bulletHoriOffset = 1;
    public float bulletVertOffset = 0.5f;
    
    private float timeSinceLastShot;
    private int damage;
    private float knockbackForce;
    private bool isBlue;
    private string initialDirection;
    private SpriteRenderer spriteRenderer;
    private SideData sideData;
    private Vector3 shootDirection;
    private bool isAutoShooting;
    public bool IsAutoShooting
    {
        get {return isAutoShooting;}
        set {isAutoShooting = value;}
    }

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
        timeSinceLastShot = 0;
        isAutoShooting = false;

        initialDirection = transform.parent.name;
        if (initialDirection.Equals("L"))
        {
            // spriteRenderer.flipX = true;
            transform.Rotate(0, 0, 180);
            transform.localPosition = new Vector3(-1.2f, 0, 0);
        }
        else if (initialDirection.Equals("U"))
        {
            transform.Rotate(0, 0, 90);
            transform.localPosition = new Vector3(0, 1.2f, 0);
        }
        else if (initialDirection.Equals("D"))
        {
            transform.Rotate(0, 0, -90);
            transform.localPosition = new Vector3(0, -1.2f, 0);
        }
        else
        {
            transform.localPosition = new Vector3(1.2f, 0, 0);
        }
    }

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (isAutoShooting && timeSinceLastShot >= autoShotCooldown)
        {
            Shoot();
            timeSinceLastShot = 0;
        }
    }

    public void TryManualShot()
    {
        if (timeSinceLastShot < manualShotCooldown) return;

        Shoot();
        timeSinceLastShot = 0;
    }

    public void Shoot()
    {
        // shootDirection = initialDirection.Equals("L") ? -transform.right : transform.right;
        shootDirection = transform.right;
                
        Vector3 newPosition = transform.position + shootDirection*bulletHoriOffset + transform.up*bulletVertOffset;
        GameObject newBullet = Instantiate(bulletPrefab, newPosition, transform.rotation);
        newBullet.GetComponent<PlayerShot>().Initialize(shootDirection, bulletSpeed, damage, knockbackForce, isBlue);
    }
}
