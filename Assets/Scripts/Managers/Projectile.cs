using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int _damage;
    [SerializeField] float _speed;
    [SerializeField] Rigidbody projectileRigidBody;
    public Vector3 fireDirection;



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.gameObject.GetComponentInParent<Health>().TakeDamage(_damage);
        }
        gameObject.SetActive(false);
    }

    public void InitProjectileValues()
    {
        projectileRigidBody.velocity = Vector3.zero;
    }

    public void FireProjectile()
    {
        projectileRigidBody.AddForce(fireDirection * _speed, ForceMode.Impulse);
    }
}
