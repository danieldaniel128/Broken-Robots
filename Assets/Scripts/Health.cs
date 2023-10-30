using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] int _maxHealth;
    public int _currentHealth;

    public UnityEvent OnDeath;

    // You can add events or delegate functions here to handle health changes, like onHealthChanged

    private void Start()
    {
        _currentHealth = _maxHealth; // Set the initial health to the maximum when the game starts
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        // Check if the player is dead
        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }

        // You can also trigger an event here to notify other scripts/UI elements of the health change
        // For example: onHealthChanged?.Invoke(currentHealth);
    }

    public void Heal(int amount)
    {

    }

}
