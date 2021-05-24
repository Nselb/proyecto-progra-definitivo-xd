using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;
    public float collectDistance = 5f;
    public LayerMask sphereLayer;
    [Range(0, 1)]
    public float mouseSpeed;
    public GameObject collectInfo;
    public Image farmingImage;
    public Image toolImage;
    public Sprite[] toolSprites;
    public Texture2D cursorTexture;
    private Vector2 mouseOffset = new Vector2(80, 50);
    private Vector3 _movement;
    private Camera mainCamera;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private GameObject ui;
    private void Start()
    {
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Confined;
        SetActiveImages(false, ResourceType.All);
        Cursor.SetCursor(cursorTexture, mouseOffset, CursorMode.Auto);
        Cursor.visible = true;
        controller = GetComponent<CharacterController>();
        ui = transform.GetChild(0).gameObject;
    }
    void OnMove(InputValue playerActions)
    {
        _movement.x = playerActions.Get<Vector2>().x;
        _movement.z = playerActions.Get<Vector2>().y;
    }
    void OnLook(InputValue mouseLook)
    {
        if (Mouse.current.rightButton.isPressed)
        {
            transform.eulerAngles += (new Vector3(0f, mouseLook.Get<Vector2>().x, 0f).normalized * mouseSpeed);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, collectDistance);
    }
    void Update()
    {
        bool isGrounded = controller.isGrounded;
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        Vector3 move = transform.right.normalized * _movement.x + transform.forward.normalized * _movement.z;
        move.y = 0;
        controller.Move(move * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
        controller.Move(playerVelocity * Time.deltaTime);
        #region COLLECT
        int rayLayer = ~(1 << 8);
        if (Physics.Raycast(ray, out RaycastHit hit, 80f, rayLayer))
        {
            bool inRange = false;
            GameObject other = hit.collider.gameObject;
            foreach (var item in Physics.OverlapSphere(transform.position, collectDistance, sphereLayer))
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
        #endregion
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
            c.vida--;
            farmingImage.fillAmount = 0;
            StartCoroutine("ShowInfoText", c);
        }
        c.Die();
    }
    IEnumerator ShowInfoText(Collectable collectable)
    {
        GameObject text = Instantiate(collectInfo, Vector2.zero, Quaternion.identity);
        text.transform.SetParent(ui.transform, false);
        Destroy(text, collectable.dureza);
        text.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"+{collectable.dropQuantity} {collectable.type.ToString()}";
        yield return null;
    }
    private void SetActiveImages(bool value, ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Rock:
                toolImage.sprite = toolSprites[0];
                break;
            case ResourceType.Metal:
                toolImage.sprite = toolSprites[0];
                break;
            case ResourceType.Wood:
                toolImage.sprite = toolSprites[1];
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
        farmingImage.gameObject.SetActive(value);
        toolImage.gameObject.SetActive(value);
    }
}
