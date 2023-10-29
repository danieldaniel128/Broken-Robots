using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Transform target;


    public void SpawnProjectile(Vector3 direction)
    {
        GameObject projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().fireDirection = direction;
    }
}
