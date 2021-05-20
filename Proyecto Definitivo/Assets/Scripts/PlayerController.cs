using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 _movement;
    public int vida = 100;
    public Image farmingImage;
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
            farmingImage.fillAmount = 0;
            StartCoroutine("Collect");
        }
        if (!Keyboard.current.eKey.isPressed)
        {
            farmingImage.fillAmount = 0;
            StopCoroutine("Collect");
        }
    }

    IEnumerator Collect()
    {
        while (vida > 0)
        {
            yield return new WaitForSeconds(.28f);
            vida -= 10;
            farmingImage.fillAmount = vida / 100f;
        }
    }
}
