using UnityEngine;

public class PlayerGunController : MonoBehaviour
{
    public Sprite blueSprite;
    public Sprite redSprite;
    public int defaultDamage = 5;
    public float defaultKnockbackForce = 5;
    public float shotCooldown = 0.5f;
    public float bulletSpeed = 40;
    public GameObject bulletPrefab;
    public float bulletHoriOffset = 1;
    public float bulletVertOffset = 0.5f;
    
    private float timeSinceLastShot;
    private int damage;
    private float knockbackForce;
    [SerializeField]private bool isBlue;
    private string initialDirection;
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // condition ? if true : if false
        spriteRenderer.sprite = isBlue ? blueSprite : redSprite;
        damage = defaultDamage;
        knockbackForce = defaultKnockbackForce;
        timeSinceLastShot = 0;

        initialDirection = transform.parent.name;
        if (initialDirection.Equals("L"))
        {
            spriteRenderer.flipX = true;
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

    void Update()
    {
        if (timeSinceLastShot <= shotCooldown)
        {
            timeSinceLastShot += Time.deltaTime;
            return;
        }
        else
        {
            Vector3 newPosition = transform.position + transform.right*bulletHoriOffset + transform.up*bulletVertOffset;
            GameObject newBullet = Instantiate(bulletPrefab, newPosition, transform.rotation);
            newBullet.GetComponent<PlayerShot>().Initialize(transform.right, bulletSpeed, damage, knockbackForce, isBlue);

            timeSinceLastShot = 0;
        }
    }
}
