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
    public Image toolImage;
    public Sprite[] toolSprites;
    public Texture2D cursorTexture;
    private Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Confined;
        SetActiveImages(false, ResourceType.All);
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
    void OnMove(InputValue playerActions)
    {
        _movement.x = playerActions.Get<Vector2>().x;
        _movement.z = playerActions.Get<Vector2>().y;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, rayDistance);
    }
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        transform.Translate(_movement * speed * Time.deltaTime);
        Debug.DrawRay(ray.origin, ray.direction * 100f);

        int sphereLayer = 1 << 7;
        int rayLayer = ~(1<<8);
        if (Physics.Raycast(ray, out RaycastHit hit, 80f, rayLayer))
        {
            bool inRange = false;
            GameObject other = hit.collider.gameObject;
            foreach (var item in Physics.OverlapSphere(transform.position, rayDistance, sphereLayer))
            {
                if (other.Equals(item.gameObject))
                {
                    inRange = true;
                    break;
                }
                inRange = false;
            }
            if (other.CompareTag("Recolectable") && inRange)
            {
                SetActiveImages(true, other.GetComponent<Collectable>().type);
                toolImage.transform.position = Mouse.current.position.ReadValue();
                farmingImage.transform.position = Mouse.current.position.ReadValue();
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
            else
            {
                farmingImage.fillAmount = 0;
                SetActiveImages(false, ResourceType.All);
                StopCoroutine("Collect");
            }
        }
        else
        {
            farmingImage.fillAmount = 0;
            SetActiveImages(false, ResourceType.All);
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
    private void SetActiveImages(bool value, ResourceType type)
    {
        farmingImage.gameObject.SetActive(value);
        switch (type)
        {
            case ResourceType.Rock:
                toolImage.sprite = toolSprites[0];
            break;
            case ResourceType.Wood:
                toolImage.sprite = toolSprites[1];
            break;
            case ResourceType.Metal:
                toolImage.sprite = toolSprites[0];
            break;
            case ResourceType.Leather:
                toolImage.sprite = toolSprites[3];
            break;
            case ResourceType.Plant:
                toolImage.sprite = toolSprites[2];
            break;
            case ResourceType.All:
                toolImage.gameObject.SetActive(value);
                break;
        }
        toolImage.gameObject.SetActive(value);
    }
}
