using MK.Destructible;
using System.Collections;
using UnityEngine;

public enum ShieldState { Closed, Open, Closing }

public class Shield : MonoBehaviour
{
    [SerializeField] Destructible target;

    public float shieldDuration = 3f;
    public float shieldCooldown = 5f;

    private bool canUseShield = true;
    private Animator animator;
    private Vector3 offset;

    public float ShieldTimer { get; private set; }
    public ShieldState ShieldState { get; private set; }
    public Destructible Target
    {
        get { return target; }
        set
        {
            target = value;
            if (target == null) return;
            offset = transform.position - target.transform.position;
        }
    }

    private void Awake()
    {
        Target = target;
        animator = GetComponent<Animator>();
        SetShield(false, false);
    }

    public void ActivateShield()
    {
        if (!canUseShield || target == null)
        {
            return;
        }

        StartCoroutine(DoShieldCooldown());
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = offset + target.transform.position;
        }
    }

    public void SetShield(bool value, bool affectTarget = true)
    {
        animator.SetBool("Activated", value);

        if (affectTarget)
        {
            target.canHurt = !value;
        }
    }

    private IEnumerator DoShieldCooldown()
    {
        ShieldTimer = shieldDuration;
        canUseShield = false;
        SetShield(true);

        ShieldState = ShieldState.Open;

        while (ShieldTimer > 0) 
        { 
            ShieldTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        ShieldTimer = Mathf.Max(0, ShieldTimer);

        //yield return new WaitForSeconds(shieldDuration);

        SetShield(false);

        ShieldState = ShieldState.Closing;

        while (ShieldTimer < shieldCooldown)
        {
            ShieldTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //yield return new WaitForSeconds(shieldCooldown);
        canUseShield = true;

        ShieldState = ShieldState.Closed;
    }
}
