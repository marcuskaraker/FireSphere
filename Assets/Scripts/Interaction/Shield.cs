using MK.Destructible;
using System.Collections;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public Destructible target;

    public float shieldDuration = 3f;
    public float shieldCooldown = 5f;

    private bool canUseShield = true;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        SetShield(false);
    }

    public void ActivateShield()
    {
        if (!canUseShield)
        {
            return;
        }

        StartCoroutine(DoShieldCooldown());
    }

    public void SetShield(bool value)
    {
        animator.SetBool("Activated", value);
        target.canHurt = !value;
    }

    private IEnumerator DoShieldCooldown()
    {
        canUseShield = false;
        SetShield(true);

        yield return new WaitForSeconds(shieldDuration);

        SetShield(false);

        yield return new WaitForSeconds(shieldCooldown);
        canUseShield = true;
    }
}
