using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Infos")]
    [SerializeField] int playerIndex = 0;
    [SerializeField] float runSpeed = 30f;
    [SerializeField] float percent = 0f;
    [SerializeField] bool jump = false;
    [SerializeField] bool crouch = false;
    [SerializeField] bool grounded = true;
    [SerializeField] bool ko = false;
    [SerializeField] bool spiked = false;
    [Header("Timers")]
    [SerializeField] private float DAMAGE_TIMER = 0.3f;
    [SerializeField] private float KO_TIMER = 0.3f;
    [SerializeField] private float SPIKE_TIMER = 0.6f;
    [SerializeField] float damageTimer = 0.3f;
    [SerializeField] float koTimer = 0.5f;
    [SerializeField] float spikeTimer = 1f;
    [Header("Colors")]
    [SerializeField] Color KO_COLOR = new Color(1f, 0.75f, 0f, 1f);
    [SerializeField] Color SPIKE_COLOR = new Color(0.5f, 0f, 0f, 1f);
    [Header("Components")]
    public AnimatorController animatorControl;
    public CharacterController2D controller;
    public GameObject visual;
    public Hands hands;
    bool canMove = true;
    float horizontalMove = 0f;
    float verticalMove = 0f;
    float handsVertical = 0f;
    float handsHorizontal = 0f;

    public int GetPlayerIndex()
    {
        return playerIndex;
    }
    public void SetKo(bool value, float prct)
    {
        percent = prct;
        if (value)
        {
            animatorControl.SetGetHit();
            visual.transform.Find("body").GetComponent<SpriteRenderer>().color = KO_COLOR;
        }
        if (!value) visual.transform.Find("body").GetComponent<SpriteRenderer>().color = Color.white;
        ko = value;
        koTimer = KO_TIMER * (0.01f * prct + 1);
    }
    public bool GetKo()
    {
        return ko;
    }
    public void SetSpiked(bool value)
    {
        spiked = value;
        spikeTimer = SPIKE_TIMER;
        if (value)
        {
            visual.transform.Find("body").GetComponent<SpriteRenderer>().color = SPIKE_COLOR;
        }
        if (!value) visual.transform.Find("body").GetComponent<SpriteRenderer>().color = Color.white;

    }
    public bool GetSpiked()
    {
        return spiked;
    }
    public void SetPlayerIndex(int value)
    {
        playerIndex = value;
    }
    public Vector2 GetHandsDirection()
    {
        return hands.transform.position - transform.position;
    }
    public bool GetGrounded()
    {
        return grounded;
    }
    public void SetMoveDirection(Vector3 dir)
    {
        handsVertical = dir.y;
        verticalMove = handsVertical * runSpeed;
        handsHorizontal = dir.x;
        horizontalMove = handsHorizontal * runSpeed;
    }
    public void Jump()
    {
        controller.Jump();
        jump = true;
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("PlayerMovement, SetGoingDown : " + (controller.GetRigidBody().velocity.y < 0));

        /*if (controller.GetRigidBody().velocity.y < -1f) animatorControl.SetGoingDown(true);
        else if (controller.GetRigidBody().velocity.y > 1f) animatorControl.SetGoingDown(false);

        if (controller.GetRigidBody().velocity.x < -1f) animatorControl.SetGoingDown(true);
        else if (controller.GetRigidBody().velocity.x > 1f) animatorControl.SetGoingRight(false);*/

        animatorControl.SetSpeed(controller.GetRigidBody().velocity.x, controller.GetRigidBody().velocity.y);

        controller.MoveGroundCheck(ko || spiked);
        grounded = controller.IsGrounded();
        animatorControl.SetKo(ko || spiked);

        if (ko && koTimer >= 0)
        {
            koTimer -= Time.deltaTime;
        }
        if (koTimer < 0)
        {
            SetKo(false, percent);
        }
        /*if (spiked && spikeTimer >= 0)
        {
            spikeTimer -= Time.deltaTime;
        }
        if (spikeTimer < 0)
        {
            SetSpiked(false);
        }*/


    }

    void FixedUpdate()
    {
        if (canMove)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime , verticalMove * Time.fixedDeltaTime, crouch, jump, ko, spiked);
            hands.Move(new Vector2(handsHorizontal, handsVertical));
            jump = false;
        }
    }

    public void Flip()
    {
        controller.Flip();
    }
    public void Dodge(float dir)
    {
        controller.Dodge(dir);
    }
    public void EndDodge()
    {
        controller.EndDodge();
    }

    public void StartCanMove()
    {
            canMove = true;
    }
    public void ChangeCanMove()
    {
        canMove = !canMove;
    }
    public CharacterController2D GetController()
    {
        return controller;
    }
}
