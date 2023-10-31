using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class Shock : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float range, duration, knockback;
    [Header("FrameData"), SerializeField] int startFrames;
    [SerializeField] int activeFrames, recoveryFrames;
    
    public float Range => range;

    public IEnumerator Perform(PlayerController player, UnityAction onFinish = null) {

        yield return StartCoroutine(player.WaitFrames(startFrames));
        yield return StartCoroutine(player.WaitFrames(activeFrames));
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
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent enemyAgent))
        {
            enemyAgent.Move(knockback * Time.fixedDeltaTime * Vector3.left);
        }
    }


    public void SetPosition(Vector3 pos) => transform.position = pos;


}
