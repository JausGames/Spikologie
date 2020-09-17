using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] private Animator body;

    public void SetWalking(bool value)
    {
        Debug.Log("AnimatorController, SetWalking : " + value);
        body.SetBool("IsWalking", value);
    }
    public void SetInAir(bool value)
    {
        Debug.Log("AnimatorController, SetInAir : " + value);
        body.SetBool("InAir", value);
    }
    public void SetOnWall(bool value)
    {
        Debug.Log("AnimatorController, SetOnWall : " + value);
        body.SetBool("OnWall", value);
    }
    public void SetKo(bool value)
    {
        Debug.Log("AnimatorController, SetKo : " + value);
        body.SetBool("Ko", value);
    }
    public void SetGetHit()
    {
        Debug.Log("AnimatorController, SetOnWall");
        body.SetTrigger("GetHit");
    }
    public void SetAttackForward()
    {
        Debug.Log("AnimatorController, SetAttackForward");
        body.SetTrigger("AttackForward");
    }
    public void SetAttackUp()
    {
        Debug.Log("AnimatorController, SetAttackUp");
        body.SetTrigger("AttackUp");
    }
    public void SetAttackAerialDown()
    {
        Debug.Log("AnimatorController, SetAttackAerialDown");
        body.SetTrigger("AttackAerialDown");
    }
    public void SetGoingDown(bool value)
    {
        Debug.Log("AnimatorController, SetGoingDown");
        body.SetBool("GoingDown", value);
    }
    public void SetGoingRight(bool value)
    {
        Debug.Log("AnimatorController, SetGoingRight");
        body.SetBool("GoingRight", value);
    }
    public void SetAttackAerialForward()
    {
        Debug.Log("AnimatorController, SetAttackAerialForward");
        body.SetTrigger("AttackAerialForward");
    }
    public void SetSpeed(float x, float y)
    {
        Debug.Log("AnimatorController, SetAttackAerialForward : x = " + x + ", y = " + y );
        body.SetFloat("HorizontalSpeed", x);
        body.SetFloat("VerticalSpeed", y);
    }
    public void SetDodging()
    {
        Debug.Log("AnimatorController, SetDodging");
        body.SetTrigger("Dodge");
    }
}
