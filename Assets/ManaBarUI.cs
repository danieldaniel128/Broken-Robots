using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBarUI : MonoBehaviour
{
    [SerializeField] float MaxMana;
    [SerializeField] float CurrentMana;
    [SerializeField] Slider slider;

    private void Update()
    {
        slider.value = (float)CurrentMana / (float)MaxMana;
    }
    public void ModifyMana(float newMana)
    {
        CurrentMana = newMana;
    }
}
