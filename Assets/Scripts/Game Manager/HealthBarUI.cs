using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Image healthBarForegroundImg;
    public void UpdatePlayerHealthBar(PlayerData playerData){
        healthBarForegroundImg.fillAmount = (float)playerData.CurHealth / playerData.maxHealth;
    }
}
