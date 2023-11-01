using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider), typeof(MeshRenderer))]
public class Shock : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float range, knockback, cooldown;
    float timer;
    [Header("FrameData"), SerializeField] int startFrames;
    [SerializeField] int activeFrames, recoveryFrames;
    Collider c;
    Renderer r;
    List<EnemyStatus> hits;

    public bool isAvailable => timer <= 0;

    private void Awake()
    {
        timer = 0;
        hits = new List<EnemyStatus>();
        c = GetComponent<BoxCollider>();
        r = GetComponent<MeshRenderer>();
    }

    public float Range => range;

    public IEnumerator Perform(PlayerController player, UnityAction onFinish = null) {
        timer = Mathf.Infinity;
        yield return StartCoroutine(player.WaitFrames(startFrames));
        //turn all on
        c.enabled = true;
        r.enabled = true;
        yield return StartCoroutine(player.WaitFrames(activeFrames));
        // turn all off
        c.enabled = false;
        r.enabled = false;
        hits.Clear();
        yield return StartCoroutine(player.WaitFrames(recoveryFrames));

        onFinish?.Invoke();
        timer = cooldown;
        
        yield return null;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        foreach (EnemyStatus enemy in hits)
        {
            enemy.transform.position += knockback * Mathf.Sign(transform.localPosition.x) * Time.deltaTime * Vector3.right;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyStatus>(out EnemyStatus enemy) && !enemy.IsDead)
        {
            hits.Add(enemy);
            if (enemy.TryGetComponent<Health>(out Health enemyHealth))
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }


    public void SetDirection(float xDirection)
    {
        Vector3 localPos = transform.localPosition;
        localPos.x = range * xDirection;
        transform.localPosition = localPos;
    }


}
