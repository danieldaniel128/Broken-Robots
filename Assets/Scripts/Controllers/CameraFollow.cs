using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector2 offset = new Vector3(0f, 0f);
    [SerializeField] private float smoothTime = 0.25f;
    [SerializeField] private Vector2 maxPosition;
    [SerializeField] private Vector2 minPosition;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;

    private void Update()
    {
        Vector3 targetPosition = new Vector3(Mathf.Clamp(target.position.x + offset.x, minPosition.x, maxPosition.x), Mathf.Clamp(target.position.y, minPosition.y, maxPosition.y),Camera.main.transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
