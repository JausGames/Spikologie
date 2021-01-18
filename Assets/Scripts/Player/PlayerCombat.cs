using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Infos")]
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    [SerializeField] int playerIndex;
    [SerializeField] bool dodging = false;
    [Header("Position & Direction")]
    public Transform attackPoint;
    //public Animator animator;
    [SerializeField] private Vector2 attackDirection = Vector2.zero;
    [SerializeField] private Vector2 attackDirectionOnHit = Vector2.zero;

    [Header("Timers")]
    [SerializeField] const float HIT_TIME = 0.2f;
    [SerializeField] const float COOLDOWN = 0.3f;
    [SerializeField] const float DODGE_TIMER = 0.3f;
    float hitTime = 1f;
    float coolDown = -0.3f;
    float dodgeTime = -0.3f;

    [Header("Colors")]
    [SerializeField] Color coolDownColor = new Color(1f, 0.75f, 0f, 1f);
    [SerializeField] Color attackColor = new Color(0.5f, 0f, 0f, 1f);

    [Header("Components")]
    public AnimatorController animatorControl;
    [SerializeField] private PlayerMovement movement;
    public GameObject visual;
    // Update is called once per frame
    void Update()
    {
        attackDirection = movement.GetHandsDirection();
        if (coolDown >= 0) coolDown -= Time.deltaTime;
        if (dodgeTime >= 0) dodgeTime -= Time.deltaTime;
        if (visual.transform.Find("hands").GetComponent<SpriteRenderer>().color == attackColor && hitTime < 0)
        {
            coolDown = COOLDOWN;
            Debug.Log("PlayerCombat, Hand Color = 03");
            visual.transform.Find("hands").GetComponent<SpriteRenderer>().color = coolDownColor;
        }
        if (visual.transform.Find("hands").GetComponent<SpriteRenderer>().color == coolDownColor && coolDown < 0)
        {
            Debug.Log("PlayerCombat, Hand Color = 04");
            visual.transform.Find("hands").GetComponent<SpriteRenderer>().color = Color.white;
        }
        //if (hitTime > 0) Attack();
    }
    private void FixedUpdate()
    {
    }

    public int GetPlayerIndex()
    {
        return playerIndex;
    }
    public void SetPlayerIndex(int value)
    {
        playerIndex = value;
    }
    void OnDrawGizmos()
    {
        if (attackPoint == null || attackRange == 0f)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    public void StartAttack()
    {
        attackDirectionOnHit = attackDirection;
        Debug.Log("Player Combat, StartAttack : Y = " + attackDirectionOnHit.y + ", X = " + attackDirectionOnHit.x);
        if (!movement.GetGrounded())
        {
            if (attackDirectionOnHit.y < -0.25f) { animatorControl.SetAttackAerialDown(); return; }
            if (attackDirectionOnHit.y > 0.25f) { animatorControl.SetAttackUp(); return; }
            animatorControl.SetAttackAerialForward();
            return;
        }
        
        if (attackDirectionOnHit.y > 0.25f) { animatorControl.SetAttackUp(); return; }
        animatorControl.SetAttackForward();
    }
    public void Dodge(float dir)
    {
        if (dodgeTime > 0 || dodging || movement.GetSpiked()) return;
        dodging = true;
        movement.Dodge(dir);
        animatorControl.SetDodging();
    }
    public void EndDodge()
    {
        dodgeTime = DODGE_TIMER;
        dodging = false;
        movement.EndDodge();
    }
    public bool GetDodging()
    {
        return dodging;
    }
    public void Attack()
    {
        if (coolDown > 0 || movement.GetSpiked()) return;
        if (hitTime < 0)
        {
            visual.transform.Find("hands").GetComponent<SpriteRenderer>().color = attackColor;
            hitTime = HIT_TIME;
        }
        if (hitTime > 0f)
        {

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            /*Collider2D[] hitEnemies = Physics2D.OverlapCapsuleAll(attackPoint.position - transform.position, 
                                                                new Vector2(1f, 2f), 
                                                                CapsuleDirection2D.Horizontal, 
                                                                90f);*/
            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log("We hit " + enemy.name + ", We are " + name);
                if (enemy.name != name)
                {
                    Debug.Log("PlayerCombat, Hand Color = 02  " + enemy.GetComponent<Player>());
                    enemy.GetComponent<Player>().TakeDamage(attackDirectionOnHit, GetComponent<Rigidbody2D>().velocity);
                    hitTime = 0f;
                    coolDown = COOLDOWN;
                    visual.transform.Find("hands").GetComponent<SpriteRenderer>().color = coolDownColor;
                }
            }
        }
        hitTime -= Time.deltaTime;

    }
}
