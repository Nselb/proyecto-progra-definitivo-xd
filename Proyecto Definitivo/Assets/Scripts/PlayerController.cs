using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 _movement;
    public int vida = 100;
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
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine("Collect");
        }
        if (!Keyboard.current.eKey.isPressed)
        {
            StopCoroutine("Collect");
        }
    }

    IEnumerator Collect()
    {
        while(vida > 0)
        {
            Debug.Log("Farming...");
            yield return new WaitForSeconds(1f);
            vida-=10;
        }
    } 
}
