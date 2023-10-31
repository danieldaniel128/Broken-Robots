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
    [SerializeField] float range, knockback;
    [Header("FrameData"), SerializeField] int startFrames;
    [SerializeField] int activeFrames, recoveryFrames;
    Collider c;
    Renderer r;

    List<NavMeshAgent> hits;

    private void Awake()
    {
        hits = new List<NavMeshAgent>();
        c = GetComponent<BoxCollider>();
        r = GetComponent<MeshRenderer>();
    }

    public float Range => range;

    public IEnumerator Perform(PlayerController player, UnityAction onFinish = null) {

        yield return StartCoroutine(player.WaitFrames(startFrames));
        //turn all on
        c.enabled = true;
        r.enabled = true;
        yield return StartCoroutine(player.WaitFrames(activeFrames));
        // turn all off
        c.enabled = false;
        r.enabled = false;
        hits.AsEnumerable().All(agent => agent.isStopped = false);
        hits.Clear();
        yield return StartCoroutine(player.WaitFrames(recoveryFrames));

        onFinish?.Invoke();
        
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.name} triggered");
        if (other.TryGetComponent<Health>(out Health enemyHealth))
        {
            Debug.Log("health");
            enemyHealth.TakeDamage(damage);
        }
        if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent enemyAgent))
        {
            enemyAgent.isStopped = true;
            hits.Add(enemyAgent);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent enemyAgent))
        {
            enemyAgent.Move(Mathf.Sign(transform.localPosition.x) * knockback * Time.fixedDeltaTime * Vector3.right);
        }
    }


    public void SetDirection(float xDirection)
    {
        Vector3 localPos = transform.localPosition;
        localPos.x = Mathf.Abs(localPos.x) * xDirection;
        transform.localPosition = localPos;
    }


}
