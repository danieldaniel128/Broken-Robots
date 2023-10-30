using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] int _maxHealth;
    [SerializeField] int _currentHealth;
    public int CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; OnChangeValue?.Invoke(_currentHealth); } }

    public UnityEvent OnDeath;
    public UnityEvent<int> OnChangeValue;

    // You can add events or delegate functions here to handle health changes, like onHealthChanged

    private void Start()
    {
        CurrentHealth = _maxHealth; // Set the initial health to the maximum when the game starts
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        // Check if the player is dead
        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    [ContextMenu("value change test")]
    public void ChangeValueTest()
    {
        OnChangeValue?.Invoke(CurrentHealth);
    }

    public void Heal(int amount)
    {

    }

}
