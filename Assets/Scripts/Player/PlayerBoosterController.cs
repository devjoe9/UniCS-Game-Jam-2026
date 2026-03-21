using System.Collections;
using UnityEngine;

public class PlayerBoosterController : MonoBehaviour
{
    public Sprite blueSprite;
    public Sprite redSprite;
    public Sprite blueBoostingSprite;
    public Sprite redBoostingSprite;
    public float boostForce = 20f;
    
    private Sprite defaultSprite;
    private Sprite defaultBoostingSprite;
    private bool isBlue;
    private string initialDirection;
    public Vector2 ThrustDirection => -(Vector2)transform.right;
    public bool IsDisabled => isDisabled;
    private bool isDisabled;
    private SpriteRenderer spriteRenderer;
    private SideData sideData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sideData = GetComponent<SideData>();
        isBlue = sideData.IsBlue;
        
        // condition ? if true : if false
        defaultSprite= isBlue ? blueSprite : redSprite;
        defaultBoostingSprite = isBlue ? blueBoostingSprite : redBoostingSprite;
        spriteRenderer.sprite = defaultSprite;
        isDisabled = false;

        initialDirection = transform.parent.name;
        if (initialDirection.Equals("L"))
        {
            spriteRenderer.flipX = true;
            transform.localPosition = new Vector3(-0.75f, 0, 0);
        }
        else if (initialDirection.Equals("U"))
        {
            transform.Rotate(0, 0, 90);
            transform.localPosition = new Vector3(0, 0.75f, 0);
        }
        else if (initialDirection.Equals("D"))
        {
            transform.Rotate(0, 0, -90);
            transform.localPosition = new Vector3(0, -0.75f, 0);
        }
        else
        {
            transform.localPosition = new Vector3(0.75f, 0, 0);
        }
    }

    public void UseBoostingSprite(bool isBoosting)
    {
        spriteRenderer.sprite = isBoosting ? defaultBoostingSprite : defaultSprite;
    }
}
