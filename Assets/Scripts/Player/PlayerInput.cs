using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] GameObject controlTarget;
    public Shield shieldEffect;

    public KeyCode fireKey = KeyCode.Mouse0;
    public KeyCode shieldKey = KeyCode.Mouse1;
    public KeyCode reloadKey = KeyCode.R;

    private Transform playerSprite;
    private Camera mainCamera;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.ReturnToMainMenu();
        }

        if (ControlTarget == null)
        {
            return;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Movement.direction = new Vector2(moveX, moveY);
        Movement.rotationTarget = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (!GameManager.Instance.arenaBounds.bounds.Contains((Vector2)ControlTarget.transform.position))
        {
            Vector2 dirBackToCenter = (-ControlTarget.transform.position).normalized;

            Movement.direction = dirBackToCenter;
            Movement.rotationTarget = Vector2.zero;

            GameManager.Instance.UIManager.PromptIfEmpty(2.5f, MK.UI.TransitionPreset.ScaleIn, "Outside of arena!");
        }

        bool automaticKeyCheck = Input.GetKey(fireKey) && Shooter.CompareWeaponFireMode(FireMode.Automatic);
        bool singleKeyCheck = Input.GetKeyDown(fireKey) && Shooter.CompareWeaponFireMode(FireMode.Single);

        if (automaticKeyCheck || singleKeyCheck)
        {
            Shooter.Shoot(Movement.Rb2D.velocity.y);
        }

        if (Input.GetKeyDown(reloadKey))
        {
            Shooter.Reload();
        }

        if (Input.GetKeyDown(shieldKey))
        {
            shieldEffect.ActivateShield();
        }

        Shooter.CurrentWeaponIndex += (int)Input.mouseScrollDelta.y;
    }
}