using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 _movement;
    void Start()
    {

    }

    void OnMove(InputValue playerActions)
    {
        _movement.x = playerActions.Get<Vector2>().x;
        _movement.z = playerActions.Get<Vector2>().y;
    }
    void Update()
    {
        transform.Translate(_movement * speed * Time.deltaTime, Space.Self);
    }
}
