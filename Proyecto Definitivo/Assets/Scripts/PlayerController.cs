using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    public float rayDistance = 5f;
    private Vector3 _movement;
    public Image farmingImage;
    void OnMove(InputValue playerActions)
    {
        _movement.x = playerActions.Get<Vector2>().x;
        _movement.z = playerActions.Get<Vector2>().y;
    }
    void OnLook(InputValue playerActions)
    {
        //Debug.Log($"Looking? x:{playerActions.Get<Vector2>().x} y:{playerActions.Get<Vector2>().y}");
    }
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.red);
        transform.Translate(_movement * speed * Time.deltaTime, Space.Self);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.gameObject.CompareTag("Recolectable"))
            {
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    farmingImage.fillAmount = 0;
                    StartCoroutine("Collect", hit.collider.gameObject);
                }
                if (!Keyboard.current.eKey.isPressed)
                {
                    farmingImage.fillAmount = 0;
                    StopCoroutine("Collect");
                }
            }

        }
        else
        {
            farmingImage.fillAmount = 0;
            StopCoroutine("Collect");
        }
    }

    IEnumerator Collect(GameObject collectable)
    {
        Collectable c = collectable.GetComponent<Collectable>();
        while (c.vida > 0)
        {
            float t = 0;
            while (farmingImage.fillAmount < 1)
            {

                farmingImage.fillAmount = Mathf.Lerp(0f, 1f, t / c.dureza);
                t += Time.deltaTime;
                yield return null;
            }
            c.vida -= 10;
            farmingImage.fillAmount = 0;
        }
        c.Die();
    }
}
