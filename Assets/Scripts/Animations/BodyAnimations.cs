using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAnimations : MonoBehaviour
{
    [SerializeField] private PlayerCombat combat;

    private void Attack()
    {
        Debug.Log("BodyAnimations, Dodge");
        combat.Attack();
    }
    private void Dodge()
    {
        combat.EndDodge();
    }
}
