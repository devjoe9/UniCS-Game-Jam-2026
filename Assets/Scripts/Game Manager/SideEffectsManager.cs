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
    [SerializeField] float visualEffectDelay = 2f;

    [Header("UI References")]
    public EventSign eventSign;
    public Sprite[]  signSprites  = new Sprite[5];
    public GameObject panel;

    [Header("Audio")]
    public AudioSource resultAudioSource;
    public AudioClip   positiveSound;
    public AudioClip   negativeSound;
    public AudioClip   neutralSound;

    private int[] eventCategory = new int[] { 2, 0, 1, 1, 1 };
    private float[] effectDurations;
    private float curTimeScale;
    public float CurTimeScale => curTimeScale;
    private float curKnockbackMult;
    public float CurKnockbackMult => curKnockbackMult;
    private bool isEffectActive;
    private int curEffectIndex;
    private PlayerController playerController;
    private PlayerData playerData;
    private bool isAnimating = false;
    
    void Start()
    {
        isEffectActive = false;
        curEffectIndex = -1;
        playerController = player.GetComponent<PlayerController>();
        playerData = player.GetComponent<PlayerData>();
        curTimeScale = 1;
        effectDurations = new float[] {noWeaponsDuration, immortalityDuration, timeSlowDuration, knockbackDuration, slipperyDuration};
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

    public void TriggerRandomEvent()
    {
        if (isAnimating)
        {
            eventSign.HideSign();
            StopCurrentEffect();
        }
        int finalIndex = Random.Range(0, signSprites.Length);
        StartCoroutine(StartRandomEvent(finalIndex));
        // StartCoroutine(Spin(finalIndex));
    }

    private IEnumerator StartRandomEvent(int finalIndex)
    {
        Debug.Log("Start random event");
        isAnimating = true;
        panel.SetActive(true);

        PlayResultSound(finalIndex);

        // Show event sign
        if (eventSign != null && finalIndex < signSprites.Length)
            eventSign.ShowSign(signSprites[finalIndex], effectDurations[finalIndex], visualEffectDelay);

        yield return new WaitForSeconds(visualEffectDelay);
        StartCoroutine(StartSideEffect(finalIndex));

        // yield return new WaitForSeconds(holdDuration);

        // panel.SetActive(false);
        isAnimating = false;
    }
    
    private void PlayResultSound(int index)
    {
        if (resultAudioSource == null) return;
        int category = eventCategory[index];
        AudioClip clip = category == 0 ? positiveSound :
                         category == 2 ? negativeSound :
                         neutralSound;
        if (clip != null)
            resultAudioSource.PlayOneShot(clip);
    }
}
