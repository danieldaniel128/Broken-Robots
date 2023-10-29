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

    private void Update()
    {
        transform.position += fireDirection * _speed * Time.deltaTime;
    }
    private void Start()
    {
        projectileRigidBody.AddForce(fireDirection*_speed,ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("hit player");
        }
    }

}
