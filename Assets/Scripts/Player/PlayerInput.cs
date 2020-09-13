using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public GameObject controlTarget;

    public KeyCode fireKey = KeyCode.Space;
    public KeyCode reloadKey = KeyCode.R;

    public Movement Movement { get; private set; }
    public Shooter Shooter { get; private set; }

    Transform playerSprite;

    Camera mainCamera;

    private void Awake()
    {
        // Get movment reference
        Movement = controlTarget.GetComponent<Movement>();
        Shooter = controlTarget.GetComponent<Shooter>();
        playerSprite = controlTarget.transform.GetChild(0).transform;

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (controlTarget == null)
        {
            return;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Movement.direction = new Vector2(moveX, moveY);
        Movement.rotationTarget = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (!GameManager.Instance.arenaBounds.bounds.Contains((Vector2)controlTarget.transform.position))
        {
            Vector2 dirBackToCenter = (-controlTarget.transform.position).normalized;

            Movement.direction = dirBackToCenter;
            Movement.rotationTarget = Vector2.zero;

            GameManager.Instance.UIManager.PromptIfEmpty("Outside of arena!");
        }

        bool automaticKeyCheck = Input.GetKey(fireKey) && Shooter.CompareWeaponFireMode(FireMode.Automatic);
        bool singleKeyCheck = Input.GetKeyDown(fireKey) && Shooter.CompareWeaponFireMode(FireMode.Single);

        if (automaticKeyCheck || singleKeyCheck)
        {
            Shooter.Shoot(Movement.Rb2D.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Shooter.Reload();
        }
    }
}