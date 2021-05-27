using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    #region PUBLICAS
    [Header("Player Stats")]
    public float vida = 100f;
    public float comida = 100f;
    public float damage;
    public float xp = 0f;
    public int level;
    public float carga;
    public float attackRange;

    [Header("Player Physics")]
    public float speed = 10f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;
    public float collectDistance = 5f;
    public LayerMask collectLayer;
    public LayerMask attackLayer;

    [Header("Camera Settings")]
    public Transform lookAt;
    private float distance;
    public float angleMax;
    [Range(0, 1)]
    public float mouseSpeedX;
    [Range(0, 1)]
    public float mouseSpeedY;

    [Header("Recollection")]
    public GameObject collectInfo;
    public Image farmingImage;
    public Image toolImage;
    public Sprite[] toolSprites;
    public Sprite[] resourceSprites;

    [Header("Misc")]
    public Texture2D cursorTexture;
    #endregion PUBLICAS
    #region PRIVADAS
    private Vector2 mouseOffset = new Vector2(80, 50);
    private Vector3 _movement;
    private Camera mainCamera;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private GameObject ui;
    private Image life;
    private Vector3 cameraTransform;
    #endregion PRIVADAS

    private void Start()
    {
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Confined;
        SetActiveImages(false, ResourceType.All);
        Cursor.SetCursor(cursorTexture, mouseOffset, CursorMode.Auto);
        Cursor.visible = true;
        controller = GetComponent<CharacterController>();
        ui = transform.GetChild(0).gameObject;
        life = ui.transform.GetChild(2).GetComponent<Image>();
        life.color = new Color(160 / 255f, 255 / 255f, 75 / 255f);
        cameraTransform = mainCamera.transform.parent.GetComponent<Transform>().localPosition;
        distance = Vector3.Distance(mainCamera.transform.parent.transform.position, transform.position);
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
            cameraTransform += new Vector3(0f, mouseLook.Get<Vector2>().normalized.y * mouseSpeedY, 0f);
            cameraTransform = new Vector3(0f, Mathf.Clamp(cameraTransform.y, 1f, 5f), -Mathf.Sqrt(Mathf.Pow(distance, 2) - Mathf.Pow(cameraTransform.y, 2)));
            mainCamera.transform.parent.GetComponent<Transform>().localPosition = cameraTransform;
            transform.eulerAngles += (new Vector3(0f, mouseLook.Get<Vector2>().x, 0f).normalized * mouseSpeedX);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, collectDistance);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    void Update()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        mainCamera.transform.LookAt(lookAt);
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Vector3 move = transform.right.normalized * _movement.x + transform.forward.normalized * _movement.z;
        move.y = 0;
        controller.Move(move * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if (Keyboard.current.spaceKey.isPressed && isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
        controller.Move(playerVelocity * Time.deltaTime);
        #region COLLECT
        int rayLayer = ~(1 << 8);
        if (Physics.Raycast(ray, out RaycastHit hit, 80f, rayLayer))
        {
            bool inRange = false;
            bool inAttackRange = false;
            GameObject other = hit.collider.gameObject;
            if (other.CompareTag("Enemy"))
            {
                foreach (var item in Physics.OverlapSphere(transform.position, attackRange, attackLayer))
                {
                    if (other.Equals(item.gameObject))
                    {
                        inAttackRange = true;
                        break;
                    }
                    inAttackRange = false;
                }
                if (inAttackRange)
                {
                    toolImage.sprite = toolSprites[3];
                    toolImage.gameObject.SetActive(true);
                    toolImage.transform.position = Mouse.current.position.ReadValue();
                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        other.GetComponent<EnemyScript>().GetDamage(damage);
                    }
                }
                else
                {
                    toolImage.gameObject.SetActive(false);
                }
            }
            else
            {
                toolImage.gameObject.SetActive(false);
            }
            if (other.CompareTag("Recolectable"))
            {
                foreach (var item in Physics.OverlapSphere(transform.position, collectDistance, collectLayer))
                {
                    if (other.Equals(item.gameObject))
                    {
                        inRange = true;
                        break;
                    }
                    inRange = false;
                }
                if (inRange)
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
                        StopCoroutine("Collect");
                        farmingImage.fillAmount = 0;
                    }
                }
                else
                {
                    farmingImage.fillAmount = 0;
                    SetActiveImages(false, ResourceType.All);
                    StopCoroutine("Collect");
                }
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
        text.transform.GetChild(text.transform.childCount - 2).GetComponent<TextMeshProUGUI>().text = $"+{collectable.dropQuantity}";
        text.transform.GetChild(text.transform.childCount - 1).GetComponent<Image>().sprite = GetResourceSprite(collectable.type);
        Vector2 pos = text.GetComponent<RectTransform>().anchoredPosition;
        float t = 0;
        while (pos.y < 80f)
        {
            pos.y = Mathf.Lerp(0f, 80f, t / 1f);
            t += Time.deltaTime;
            text.GetComponent<RectTransform>().anchoredPosition = pos;
            yield return null;
        }
        Destroy(text);
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
            case ResourceType.Plant:
                toolImage.sprite = toolSprites[2];
                break;
            case ResourceType.Leather:
                toolImage.sprite = toolSprites[3];
                break;
            case ResourceType.All:
                toolImage.gameObject.SetActive(value);
                break;
        }
        farmingImage.gameObject.SetActive(value);
        toolImage.gameObject.SetActive(value);
    }
    private Sprite GetResourceSprite(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Wood:
                return resourceSprites[0];
            case ResourceType.Metal:
                return resourceSprites[4];
            case ResourceType.Rock:
                return resourceSprites[1];
            case ResourceType.Leather:
                return resourceSprites[2];
            case ResourceType.Plant:
                return resourceSprites[3];
        }
        return null;
    }
    public void GetDamage(float damage)
    {
        vida -= damage;
        life.fillAmount = vida / 100f;
        if (vida >= 75)
        {
            life.color = new Color(150 / 255f, 255 / 255f, 40 / 255f);
        }
        if (vida < 75 && vida > 50)
        {
            life.color = new Color(255 / 255f, 207 / 255f, 54 / 255f);
        }
        if (vida < 50 && vida > 25)
        {
            life.color = new Color(255 / 255f, 144 / 255f, 60 / 255f);
        }
        if (vida < 25)
        {
            life.color = new Color(255 / 255f, 80 / 255f, 59 / 255f);
        }
        if (vida <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Destroy(this);
    }
}