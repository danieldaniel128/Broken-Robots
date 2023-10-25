using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpPlayerController : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] Rigidbody _playerRigidBody;
    [SerializeField] float _jumpForce;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _playerRigidBody.AddForce(Vector3.up*_jumpForce,ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.D))
            transform.position += Vector3.right * _speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            transform.position += Vector3.left * _speed * Time.deltaTime;
    }
}
