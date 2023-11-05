using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBossBar : MonoBehaviour
{
    [SerializeField] int BossMaxHealth;
    [SerializeField] int BossCurrentHealth;
    [SerializeField] Slider slider;

    private void Update()
    {
        slider.value = (float)BossCurrentHealth / (float)BossMaxHealth;
    }
    public void ModifyHealth(int NewHealth)
    {
        BossCurrentHealth = NewHealth;
    }
}
