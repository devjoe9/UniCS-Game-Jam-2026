using System.Collections;
using UnityEngine;

public class SideEffectsManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float timeSlowMult = 0.25f;
    [SerializeField] private float knockbackMult = 2f;
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

    public IEnumerator StartSideEffect(int index, float effectDuration)
    {
        if (isEffectActive)
        {
            StopCurrentEffect(curEffectIndex);
        }
        
        isEffectActive = true;
        curEffectIndex = index;
        // Wheel order: No Weapons, Immortality, Slow Time, Knockback, Slippery
        // Effect Manager order: No Weapons, Immortality, Time Slowed Down, Knockback Increased, Slippery
        switch (index)
        {
            case 0: // No Weapons
                StartNoWeaponsMode();
                yield return new WaitForSeconds(effectDuration);
                StopCurrentEffect(index);
                break;
            case 1: // Immortality
                StartImmortalityMode();
                yield return new WaitForSeconds(effectDuration);
                StopCurrentEffect(index);
                break;
            case 2: // Slow Time
                StartTimeSlowedDownMode();
                yield return new WaitForSeconds(effectDuration);
                StopCurrentEffect(index);
                break;
            case 3: // Knockback
                StartKnockbackIncreasedMode();
                yield return new WaitForSeconds(effectDuration);
                StopCurrentEffect(index);
                break;
            case 4: // Slippery
                StartSlipperyMode();
                yield return new WaitForSeconds(effectDuration);
                StopCurrentEffect(index);
                break;
            default:
                break;
        }
    }

    private void StopCurrentEffect(int index)
    {
        if (isEffectActive)
        {
            switch (index)
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
                    Debug.LogWarning($"[SideEffectDisplay] Unknown effect index: {index}");
                    break;
            }
        }
        
        isEffectActive = false;
        curEffectIndex = -1;
    }

    private void StartNoWeaponsMode()
    {
        GameObject[] children = new GameObject[player.transform.childCount];

        for (int i = 0; i < player.transform.childCount; i++)
        {
            children[i] = player.transform.GetChild(i).gameObject;
            if (children[i].name.Length == 1)
            {
                children[i].SetActive(false);
            }
        }
    }

    private void StopNoWeaponsMode()
    {
        GameObject[] children = new GameObject[player.transform.childCount];

        for (int i = 0; i < player.transform.childCount; i++)
        {
            children[i] = player.transform.GetChild(i).gameObject;
            children[i].SetActive(true);
        }
    }

    private void StartImmortalityMode()
    {
        playerData.IsVulnerable = false;
    }

    private void StopImmortalityMode()
    {
        playerData.IsVulnerable = true;
    }

    private void StartTimeSlowedDownMode()
    {
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
        curKnockbackMult = knockbackMult;
    }

    private void StopKnockbackIncreasedMode()
    {
        curKnockbackMult = 1f;
    }

    private void StartSlipperyMode()
    {
        playerController.Acceleration = playerController.defaultAcceleration * slipperyAccelMult;
        playerController.Deceleration = playerController.defaultDecceleration * slipperyDecelMult;
    }

    private void StopSlipperyMode()
    {
        playerController.Acceleration = playerController.defaultAcceleration;
        playerController.Deceleration = playerController.defaultDecceleration;
    }
}
