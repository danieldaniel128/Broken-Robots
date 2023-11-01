using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] GameObject _projectilePrefab;

    public int poolSize = 10;

    private List<GameObject> projectilePool = new List<GameObject>();
    [SerializeField] private Transform spawnerGameObject;

    private void Start()
    {
        InitializeObjectPool();
    }

    private void InitializeObjectPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity, spawnerGameObject);
            projectile.SetActive(false);
            projectilePool.Add(projectile);
        }
    }

    public GameObject GetPooledProjectile()
    {
        for (int i = 0; i < projectilePool.Count; i++)
        {
            if (!projectilePool[i].activeSelf)//searches for inactive projectile
            {
                return projectilePool[i];
            }
        }

        // If there are no inactive projectiles in the pool, you can optionally expand the pool by instantiating more objects.
        GameObject newProjectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity,spawnerGameObject);
        newProjectile.SetActive(false);
        projectilePool.Add(newProjectile);
        return newProjectile;
    }


    public void SpawnProjectile(Vector3 direction)
    {
        GameObject projectileGameObject = GetPooledProjectile();
        Projectile projectile = projectileGameObject.GetComponent<Projectile>();
        projectile.fireDirection = direction;
        projectile.InitProjectileValues();
        projectile.transform.position = transform.position;
        projectileGameObject.SetActive(true);
        projectile.FireProjectile();
    }

    
}
