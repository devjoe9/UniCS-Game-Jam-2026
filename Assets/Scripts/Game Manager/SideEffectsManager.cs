using System.Collections;
using UnityEngine;

public class SideEffectsManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float noWeaponsDuration = 10f;
    [SerializeField] private float immortalityDuration = 10f;
    [Tooltip("Divide duration by timeSlowMult for TRUE duration")]
    [SerializeField] private float timeSlowDuration = 1.25f;
    [SerializeField] private float timeSlowMult = 0.25f;
    [SerializeField] private float knockbackDuration = 10f;
    [SerializeField] private float knockbackMult = 2f;
    [SerializeField] private float slipperyDuration = 10f;
    [SerializeField] private float slipperyAccelMult = 0.5f;
    [SerializeField] private float slipperyDecelMult = 0.25f;

    private float curTimeScale;
    public float CurTimeScale => curTimeScale;
    private float curKnockbackMult;
    public float CurKnockbackMult => curKnockbackMult;
    private bool isEffectActive;
    private int curEffectIndex;
    private PlayerController playerController;
    private PlayerData playerData;
    
    void Start()
    {
        isEffectActive = false;
        curEffectIndex = -1;
        playerController = player.GetComponent<PlayerController>();
        playerData = player.GetComponent<PlayerData>();
        curTimeScale = 1;
    }

    public IEnumerator StartSideEffect(int index)
    {
        Debug.Log("In StartSideEffect");
        if (isEffectActive)
        {
            StopCurrentEffect();
        }
        
        isEffectActive = true;
        curEffectIndex = index;
        // Wheel order: No Weapons, Immortality, Slow Time, Knockback, Slippery
        // Effect Manager order: No Weapons, Immortality, Time Slowed Down, Knockback Increased, Slippery
        switch (index)
        {
            case 0: // No Weapons
                StartNoWeaponsMode();
                yield return new WaitForSeconds(noWeaponsDuration);
                StopCurrentEffect();
                break;
            case 1: // Immortality
                StartImmortalityMode();
                yield return new WaitForSeconds(immortalityDuration);
                StopCurrentEffect();
                break;
            case 2: // Slow Time
                StartTimeSlowedDownMode();
                yield return new WaitForSeconds(timeSlowDuration);
                StopCurrentEffect();
                break;
            case 3: // Knockback
                StartKnockbackIncreasedMode();
                yield return new WaitForSeconds(knockbackDuration);
                StopCurrentEffect();
                break;
            case 4: // Slippery
                StartSlipperyMode();
                yield return new WaitForSeconds(slipperyDuration);
                StopCurrentEffect();
                break;
            default:
                break;
        }
    }

    public void StopCurrentEffect()
    {
        switch (curEffectIndex)
        {
            case 0: // No Weapons
                StopNoWeaponsMode();
                break;
            case 1: // Immortality
                StopImmortalityMode();
                break;
            case 2: // Slow Time
                StopTimeSlowedDownMode();
                break;
            case 3: // Knockback
                StopKnockbackIncreasedMode();
                break;
            case 4: // Slippery
                StopSlipperyMode();
                break;
            default:
                break;
        }
        
        isEffectActive = false;
        curEffectIndex = -1;
    }

    private void StartNoWeaponsMode()
    {
        Debug.Log("No weapons");
        for (int i = 0; i < player.transform.childCount; i++)
        {
            GameObject child = player.transform.GetChild(i).gameObject;
            if (child.name.Length == 1)
            {
                child.SetActive(false);
            }
        }
        playerController.NoWeaponsEffect = true;
        playerController.IsBoosting = false;
    }

    private void StopNoWeaponsMode()
    {
        for (int i = 0; i < player.transform.childCount; i++)
        {
            GameObject child = player.transform.GetChild(i).gameObject;
            child.SetActive(true);
        }
        playerController.NoWeaponsEffect = false;
    }

    private void StartImmortalityMode()
    {
        Debug.Log("Immortality");
        if (playerData.ActiveInvulnerability != null) StopCoroutine(playerData.ActiveInvulnerability);
        playerData.IsVulnerable = false;
    }

    private void StopImmortalityMode()
    {
        playerData.IsVulnerable = true;
    }

    private void StartTimeSlowedDownMode()
    {
        Debug.Log("Time slow");
        Time.timeScale = timeSlowMult;
        curTimeScale = timeSlowMult;
    }

    private void StopTimeSlowedDownMode()
    {
        Time.timeScale = 1f;
        curTimeScale = 1f;
    }

    private void StartKnockbackIncreasedMode()
    {
        Debug.Log("More knockback");
        curKnockbackMult = knockbackMult;
    }

    private void StopKnockbackIncreasedMode()
    {
        curKnockbackMult = 1f;
    }

    private void StartSlipperyMode()
    {
        Debug.Log("Slippery");
        playerController.Acceleration = playerController.defaultAcceleration * slipperyAccelMult;
        playerController.Deceleration = playerController.defaultDeceleration * slipperyDecelMult;
    }

    private void StopSlipperyMode()
    {
        playerController.Acceleration = playerController.defaultAcceleration;
        playerController.Deceleration = playerController.defaultDeceleration;
    }

    
}
