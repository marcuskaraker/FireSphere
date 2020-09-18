using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] GameObject controlTarget;
    public Shield shieldEffect;

    public float sprintFuel = 1f;
    public float sprintFuelDecreaseSpeed = 1f;
    public float sprintFuelIncreaseSpeed = 1f;

    public KeyCode fireKey = KeyCode.Mouse0;
    public KeyCode shieldKey = KeyCode.Mouse1;
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode sprintKey = KeyCode.LeftShift;

    private Transform playerSprite;
    private Camera mainCamera;

    private KeyCode[] NumberKeys = { KeyCode.Alpha0,
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
        KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6,
        KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9,
    };

    public Movement Movement { get; private set; }
    public Shooter Shooter { get; private set; }
    public GameObject ControlTarget
    {
        get { return controlTarget; }
        set 
        {
            controlTarget = value;
            if (controlTarget == null) return;

            Movement = controlTarget.GetComponent<Movement>();
            Shooter = controlTarget.GetComponent<Shooter>();

            playerSprite = controlTarget.transform.GetChild(0).transform;
        }
    }

    private void Awake()
    {
        ControlTarget = controlTarget;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Exit Key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.ReturnToMainMenu();
        }

        if (ControlTarget == null)
        {
            return;
        }

        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Movement.direction = new Vector2(moveX, moveY);
        Movement.rotationTarget = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Outside of bounds
        if (!GameManager.Instance.arenaBounds.bounds.Contains((Vector2)ControlTarget.transform.position))
        {
            Vector2 dirBackToCenter = (-ControlTarget.transform.position).normalized;

            Movement.direction = dirBackToCenter;
            Movement.rotationTarget = Vector2.zero;

            GameManager.Instance.UIManager.PromptIfEmpty(2.5f, MK.UI.TransitionPreset.ScaleIn, "Outside of arena!");
        }

        // Shoot
        bool automaticKeyCheck = Input.GetKey(fireKey) && Shooter.CompareWeaponFireMode(FireMode.Automatic);
        bool singleKeyCheck = Input.GetKeyDown(fireKey) && Shooter.CompareWeaponFireMode(FireMode.Single);

        if (automaticKeyCheck || singleKeyCheck)
        {
            Shooter.Shoot(Movement.Rb2D.velocity.y);
        }

        // Reload
        if (Input.GetKeyDown(reloadKey))
        {
            Shooter.Reload();
        }

        // Shield
        if (Input.GetKeyDown(shieldKey))
        {
            shieldEffect.ActivateShield();
        }

        // Weapon Select
        for (int i = 1; i < NumberKeys.Length; i++)
        {
            int index = (i - 1);
            if (Input.GetKeyDown(NumberKeys[i]) && index < Shooter.loadout.Length)
            {
                Shooter.CurrentWeaponIndex = index;
                break;
            }
        }

        Shooter.CurrentWeaponIndex += (int)Input.mouseScrollDelta.y;

        // Sprinting
        bool sprintKeyDown = Input.GetKey(sprintKey);
        Movement.IsSprinting = sprintFuel > 0 && sprintKeyDown;

       
        if (sprintKeyDown)
        {
            sprintFuel -= Time.deltaTime * sprintFuelDecreaseSpeed;
            sprintFuel = Mathf.Clamp01(sprintFuel);
        }
        else
        {
            sprintFuel += Time.deltaTime * sprintFuelIncreaseSpeed;
            sprintFuel = Mathf.Clamp01(sprintFuel);
        }
    }
}