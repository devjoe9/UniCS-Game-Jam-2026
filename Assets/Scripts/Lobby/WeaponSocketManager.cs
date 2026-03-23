using UnityEngine;

/// <summary>
/// WeaponSocketManager
/// ────────────────────
/// Attach this to the Player GameObject.
/// Reads LoadoutData.Selected on Start and places
/// the correct weapon sprite on each side socket.
///
/// Setup:
///  1. Inside your Player GameObject create 4 empty child GameObjects:
///     TopSocket / RightSocket / BottomSocket / LeftSocket
///  2. Add a SpriteRenderer to each socket GameObject
///  3. Position each socket on the correct side of the player sprite
///  4. Assign the 4 sockets + 4 weapon sprite arrays in the Inspector
/// </summary>
public class WeaponSocketManager : MonoBehaviour
{
    [Header("Sockets (SpriteRenderer on each side of the player)")]
    public SpriteRenderer topSocket;
    public SpriteRenderer rightSocket;
    public SpriteRenderer bottomSocket;
    public SpriteRenderer leftSocket;

    [Header("Weapon Sprites (must match LoadoutSelectManager order exactly)")]
    [Tooltip("Index 0 = Sword, 1 = Gun, 2 = Shield, 3 = Rocket — match your loadout screen order")]
    public Sprite[] topWeaponSprites;
    public Sprite[] rightWeaponSprites;
    public Sprite[] bottomWeaponSprites;
    public Sprite[] leftWeaponSprites;

    [Header("Optional: hide socket if no sprite found")]
    public bool hideSocketIfMissing = true;

    private void Start()
    {
        ApplyLoadout();
    }

    private void ApplyLoadout()
    {
        LoadoutData loadout = LoadoutData.Selected;

        if (loadout == null)
        {
            ApplyFallback();
            return;
        }

        ApplyToSocket(topSocket,    topWeaponSprites,    loadout.selections[0].itemIndex, "TOP");
        ApplyToSocket(rightSocket,  rightWeaponSprites,  loadout.selections[1].itemIndex, "RIGHT");
        ApplyToSocket(bottomSocket, bottomWeaponSprites, loadout.selections[2].itemIndex, "BOTTOM");
        ApplyToSocket(leftSocket,   leftWeaponSprites,   loadout.selections[3].itemIndex, "LEFT");
    }

    private void ApplyToSocket(SpriteRenderer socket, Sprite[] sprites, int index, string sideName)
    {
        if (socket == null)
        {
            return;
        }

        if (sprites == null || sprites.Length == 0)
        {
            if (hideSocketIfMissing) socket.enabled = false;
            return;
        }

        if (index < 0 || index >= sprites.Length)
        {
            if (hideSocketIfMissing) socket.enabled = false;
            return;
        }

        socket.enabled = true;
        socket.sprite  = sprites[index];
    }

    /// <summary>
    /// Fallback — shows index 0 on every socket if no loadout data exists.
    /// Useful for testing the game scene directly without going through loadout screen.
    /// </summary>
    private void ApplyFallback()
    {
        ApplyToSocket(topSocket,    topWeaponSprites,    0, "TOP");
        ApplyToSocket(rightSocket,  rightWeaponSprites,  0, "RIGHT");
        ApplyToSocket(bottomSocket, bottomWeaponSprites, 0, "BOTTOM");
        ApplyToSocket(leftSocket,   leftWeaponSprites,   0, "LEFT");
    }

    /// <summary>
    /// Call this at runtime if you ever need to hot-swap a single socket.
    /// </summary>
    public void RefreshSocket(int side, int itemIndex)
    {
        Sprite[]       sprites = GetSpritesForSide(side);
        SpriteRenderer socket  = GetSocketForSide(side);
        string         name    = new[] { "TOP", "RIGHT", "BOTTOM", "LEFT" }[side];
        ApplyToSocket(socket, sprites, itemIndex, name);
    }

    private SpriteRenderer GetSocketForSide(int side)
    {
        switch (side)
        {
            case 0: return topSocket;
            case 1: return rightSocket;
            case 2: return bottomSocket;
            case 3: return leftSocket;
            default: return null;
        }
    }

    private Sprite[] GetSpritesForSide(int side)
    {
        switch (side)
        {
            case 0: return topWeaponSprites;
            case 1: return rightWeaponSprites;
            case 2: return bottomWeaponSprites;
            case 3: return leftWeaponSprites;
            default: return null;
        }
    }
}
