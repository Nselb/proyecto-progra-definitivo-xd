using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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
    private float attackRange;

    [Header("Player Physics")]
    public float speed = 10f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;
    public float collectDistance = 5f;
    public LayerMask interactionLayer;
    public LayerMask attackLayer;

    [Header("Recollection")]
    public GameObject collectInfo;
    public Image farmingImage;
    public Image toolImage;
    public Sprite[] toolSprites;
    public Sprite[] resourceSprites;
    public Transform Arenaout;

    public GameObject placename;
    public GameObject Axe;
    public bool Swinging = false;
    #endregion PUBLICAS
    #region PRIVADAS
    private Camera mainCamera;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private GameObject ui;
    private Image life;
    private InputManager inputManager;
    private Transform cameraTransform;
    private bool cooldown = true;
    private bool near;
    private Quaternion startRotation;
    private QuestManager questManager;
    #endregion PRIVADAS

    private void Start()
    {
        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        SetActiveImages(false, ResourceType.All);
        inputManager = InputManager.Instance;
        controller = GetComponent<CharacterController>();
        ui = GameObject.Find("UI");
        life = ui.transform.GetChild(2).GetComponent<Image>();
        life.color = new Color(160 / 255f, 255 / 255f, 75 / 255f);
        farmingImage.fillAmount = 0;
        toolImage.transform.position = new Vector2(Screen.width, Screen.height) / 2;
        farmingImage.transform.position = new Vector2(Screen.width, Screen.height) / 2;
        startRotation = Axe.transform.localRotation;
        questManager = QuestManager.Instance;
    }

    public void Update()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Vector3 move = new Vector3(inputManager.GetPlayerMovement().x, 0f, inputManager.GetPlayerMovement().y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0;
        controller.Move(move * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if (inputManager.PlayerJumpedThisFrame() && isGrounded)
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
            if (other.CompareTag("Recolectable"))
            {
                foreach (var item in Physics.OverlapSphere(transform.position, collectDistance, interactionLayer))
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
                    if (Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        farmingImage.fillAmount = 0;
                        StartCoroutine("Collect", hit.collider.gameObject);
                        Axe.GetComponent<Animation>().Play();

                    }
                    if (Keyboard.current.eKey.wasReleasedThisFrame)
                    {
                        OnCollectionStop();
                    }
                }
                else
                {
                    OnCollectionStop();
                }
            }
            else
            {
                OnCollectionStop();
            }
            if (other.CompareTag("Quester"))
            {
                foreach (var item in Physics.OverlapSphere(transform.position, collectDistance, interactionLayer))
                {
                    if (other.Equals(item.gameObject))
                    {
                        near = true;
                        break;
                    }
                    near = false;
                }
                if (near)
                {
                    if (Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        questManager.AddQuest(other.GetComponent<Quester>().GetQuest());
                    }
                }
            }

        }
        else
        {
            OnCollectionStop();
        }
        #endregion COLLECT
        #region ATTACK
        if (inputManager.PlayerAttackedThisFrame() && cooldown)
        {
            foreach (var item in AttackCone())
            {
                item.GetComponent<EnemyScript>().GetDamage(damage);
            }
            StartCoroutine(CooldownCorroutine(5));
        }
        #endregion ATTACK
    }

    private void OnCollectionStop()
    {
        farmingImage.fillAmount = 0;
        SetActiveImages(false, ResourceType.All);
        StopCoroutine("Collect");
        Axe.GetComponent<Animation>().Stop();
        Axe.transform.localRotation = startRotation;
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
    GameObject[] AttackCone()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + Vector3.up * 1f, 5f, attackLayer);
        List<GameObject> objects = new List<GameObject>();
        foreach (Collider collider in colliders)
        {
            if (Vector3.Angle(cameraTransform.forward, collider.transform.position - transform.position) < 45)
            {
                objects.Add(collider.gameObject);
            }
        }
        return objects.ToArray();
    }
    IEnumerator CooldownCorroutine(float duration)
    {
        cooldown = false;
        yield return new WaitForSeconds(duration);
        cooldown = true;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ArenaIn"))
        {
            transform.position = Arenaout.transform.position;
        }
        if(other.CompareTag("PlaceInfo"))
        {
            var place = Instantiate(placename);
            place.transform.SetParent(ui.transform);
            place.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 350);
            place.GetComponent<TextMeshProUGUI>().text = other.name;
            Destroy(place, 4);

        }
    }
    public void Die()
    {
        Destroy(this);
    }
}