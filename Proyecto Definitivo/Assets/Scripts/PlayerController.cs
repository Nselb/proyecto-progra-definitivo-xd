using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private const float PLAYER_SPEED = 10f;
    #region PUBLICAS
    [Header("Player Stats")]
    public float vida = 100f;
    public float comida = 100f;
    public float damage;
    public int xp = 0;
    private int xpToNextLevel = 1000;
    public int level = 1;
    public float carga;
    private float attackRange;
    public GameObject pickaxe;
    public GameObject hacha;
    public GameObject espada;
    public GameObject arco;
    [Header("Player Physics")]
    public float speed = PLAYER_SPEED;
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
    public AudioClip swordswing;
    public AudioClip walkgrass;
    #endregion PUBLICAS

    #region PRIVADAS
    private Camera mainCamera;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private GameObject ui;
    private GameObject questAcceptUi;
    private Image life;
    private InputManager inputManager;
    private Transform cameraTransform;
    private bool cooldown = true;
    private bool near;
    private Quaternion startRotation;
    private Quaternion hstartRotation;
    private QuestManager questManager;
    private GameObject equipado;
    private AudioSource audioSource;
    #endregion PRIVADAS

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlaySwordAttack()
    {
        audioSource.clip = swordswing;
        audioSource.Play();
    }
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
        questAcceptUi = ui.transform.GetChild(6).gameObject;
        life.color = new Color(160 / 255f, 255 / 255f, 75 / 255f);
        farmingImage.fillAmount = 0;
        toolImage.transform.position = new Vector2(Screen.width, Screen.height) / 2;
        farmingImage.transform.position = new Vector2(Screen.width, Screen.height) / 2;
        startRotation = pickaxe.transform.localRotation;
        hstartRotation = hacha.transform.localRotation;
        questManager = QuestManager.Instance;
        hacha.SetActive(false);
        pickaxe.SetActive(false);
        espada.SetActive(false);
        //arco.SetActive(false);
        
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
        if (move != Vector3.zero)
        {
            audioSource.clip = walkgrass;
            audioSource.Play();
        }
        else audioSource.Stop();
        controller.Move(move * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if (inputManager.PlayerJumpedThisFrame() && isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
        controller.Move(playerVelocity * Time.deltaTime);
        #region COLLECT AND INTERACT
        int rayLayer = ~(1 << 8);
        if (Physics.Raycast(ray, out RaycastHit hit, 80f, rayLayer))
        {
            bool inRange = false;
            GameObject other = hit.collider.gameObject;
            if (other.CompareTag("Recolectable"))
            {
                inRange = CheckIfNear(other);
                if (inRange)
                {
                    SetActiveImages(true, other.GetComponent<Collectable>().type);
                    if (Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        farmingImage.fillAmount = 0;
                        ResourceType type = hit.collider.gameObject.GetComponent<Collectable>().type;
                        switch (type)
                        {
                            case ResourceType.Metal:
                            case ResourceType.Rock:
                                if (equipado.Equals(pickaxe))
                                {
                                    pickaxe.GetComponent<Animation>().Play();
                                    StartCoroutine("Collect", hit.collider.gameObject);
                                }
                                break;
                            case ResourceType.Wood:
                                if (equipado.Equals(hacha))
                                {
                                    hacha.GetComponent<Animation>().Play();
                                    StartCoroutine("Collect", hit.collider.gameObject);
                                }
                                break;
                        }

                    }
                    if (Keyboard.current.eKey.wasReleasedThisFrame)
                    {
                        StopCollecting();
                    }
                }
                else
                {
                    StopCollecting();
                }
            }
            else
            {
                StopCollecting();
            }
            if (other.CompareTag("Quester"))
            {
                near = CheckIfNear(other);
                if (near)
                {
                    if (Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        Quest quest = other.GetComponent<Quester>().GetQuest();
                        questAcceptUi.transform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                        $"Descripcion:\n{quest.GetDescription()}\n\nObjetivo:\n{quest.GetObjective()}";
                        speed = 0;
                        Cursor.lockState = CursorLockMode.Confined;
                        CinemachinePOVExtension.verticalSpeed = 0;
                        CinemachinePOVExtension.horizontalSpeed = 0;
                        questAcceptUi.SetActive(true);
                    }
                }
            }
            if (other.CompareTag("TalkTarget"))
            {
                near = CheckIfNear(other);
                if (near)
                {
                    bool target = false;
                    Quest questToCheck = null;
                    foreach (var quest in questManager.quests)
                    {
                        if (quest.GetTarget().Equals(other))
                        {
                            target = true;
                            questToCheck = quest;
                            break;
                        }
                        target = false;
                    }
                    if (Keyboard.current.eKey.wasPressedThisFrame && target)
                    {
                        questManager.CompleteQuest(questToCheck);
                    }
                }
            }
        }
        else
        {
            StopCollecting();
        }
        #endregion COLLECT

        #region ATTACK
        if (inputManager.PlayerAttackedThisFrame() && cooldown && equipado != null && equipado.Equals(espada))
        {
            foreach (var item in AttackCone())
            {
                item.GetComponent<EnemyScript>().GetDamage(damage);
            }
            espada.GetComponent<Animation>().Play();
            PlaySwordAttack();
            StartCoroutine(CooldownCorroutine(espada.GetComponent<Animation>().GetClip("Sword").length));
        }
        #endregion ATTACK

        #region EQUIP
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            equipado = espada;
            equipado.SetActive(true);
            hacha.SetActive(false);
            pickaxe.SetActive(false);
            //arco.SetActive(false);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            equipado = pickaxe;
            equipado.SetActive(true);
            hacha.SetActive(false);
            espada.SetActive(false);
            //arco.SetActive(false);
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            equipado = hacha;
            equipado.SetActive(true);
            pickaxe.SetActive(false);
            espada.SetActive(false);
            //arco.SetActive(false);
        }
        #endregion EQUIP
    }

    public void OnDeclineQuest()
    {
        speed = PLAYER_SPEED;
        Cursor.lockState = CursorLockMode.Locked;
        CinemachinePOVExtension.verticalSpeed = 10;
        CinemachinePOVExtension.horizontalSpeed = 10;
        questAcceptUi.SetActive(false);
    }


    private bool CheckIfNear(GameObject other)
    {
        bool inRange = false;
        foreach (var item in Physics.OverlapSphere(transform.position, collectDistance, interactionLayer))
        {
            if (other.Equals(item.gameObject))
            {
                inRange = true;
                break;
            }
            inRange = false;
        }

        return inRange;
    }

    private void StopCollecting()
    {
        farmingImage.fillAmount = 0;
        SetActiveImages(false, ResourceType.All);
        StopCoroutine("Collect");
        pickaxe.GetComponent<Animation>().Stop();
        pickaxe.transform.localRotation = startRotation;
        hacha.GetComponent<Animation>().Stop();
        hacha.transform.localRotation = hstartRotation;
    }

    public void AddXP(int quantity)
    {
        int left = 0;
        if (xp + quantity >= xpToNextLevel)
        {
            left = quantity - xpToNextLevel;
            xp = 0;
            LevelUp();
            AddXP(left);
        }
        else xp += quantity;
    }

    private void LevelUp()
    {
        level++;
        xpToNextLevel += 1000 + (50 * level);
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
        espada.GetComponent<Animation>().Stop();
        cooldown = true;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ArenaIn"))
        {
            transform.position = Arenaout.transform.position;
        }
        if (other.CompareTag("PlaceInfo"))
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