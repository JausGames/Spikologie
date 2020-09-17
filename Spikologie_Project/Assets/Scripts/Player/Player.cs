using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    [Header("Infos")]
    [SerializeField] private int index;
    [SerializeField] private bool inGame = false;
    [SerializeField] private bool alive = true;
    [SerializeField] private float KNOCKBACK_COEF = 45f;
    [SerializeField] private float SPIKE_COEF = 1f;
    [SerializeField] private float percent = 0f;
    private Vector3 startPosition;
    //public Animator animator;
    [Header("Components")]
    private Rigidbody2D m_Rigidbody2D;
    [SerializeField]  private CapsuleCollider2D bodyCollider;
    public PlayerMovement movement;
    public PlayerInputHandler input;
    public TimeController timeControl;
    public PlayerCombat combat;
    public GameObject tilemapGameObject;
    Tilemap tilemap;
    public PlayerUi playerUI;

    // Start is called before the first frame update
    void Awake()
    {
        bodyCollider = GetComponent<CapsuleCollider2D>();
        input = GetComponent<PlayerInputHandler>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        startPosition = transform.position;
        movement.SetPlayerIndex(index);
        combat.SetPlayerIndex(index); 
        if (tilemapGameObject != null)
        {
            tilemap = tilemapGameObject.GetComponent<Tilemap>();
        }
    }
    private void Update()
    {
        if (transform.position.y <= -7f) Die();
    }
    public Vector3 GetStartPosition()
    {
        return startPosition;
    }
    public void StopMotion()
    {
        m_Rigidbody2D.velocity = Vector2.zero;
    }

    public void TakeDamage(Vector2 dir, Vector2 force)
    {
        if (combat.GetDodging()) return;
        var xVelocity = Mathf.Clamp(Mathf.Abs(m_Rigidbody2D.velocity.x / 100f), 1f, 1.1f);
        var yVelocity = Mathf.Clamp(Mathf.Abs(m_Rigidbody2D.velocity.y / 100f), 1f, 1.1f);
        Debug.Log("Player, TakeDamage : velocityX = " + xVelocity + " velocityY = " + yVelocity);
        m_Rigidbody2D.velocity = Vector2.zero;
        var xForce = Mathf.Clamp(Mathf.Abs(force.x / 100f), 1f, 1.2f);
        var yForce = Mathf.Clamp(Mathf.Abs(force.y / 100f), 1f, 1.2f); 
        Debug.Log("Player, TakeDamage : xForce = " + xForce + " yForce = " + yForce);

        if (dir.y < 0)
        {
            yForce = Mathf.Abs(force.y);
            if (movement.GetGrounded()) GetDamage(Mathf.Clamp(yForce, 10, yForce), true);
            movement.SetSpiked(true);
        }
        movement.SetKo(true, percent);
        Debug.Log("Player, TakeDamage : FORCE = " + dir.x * KNOCKBACK_COEF * 1000f * xForce * xVelocity * Vector2.right
                                + dir.y * KNOCKBACK_COEF * 1000f * yForce * yVelocity * Vector2.up);

        timeControl.DoSlowMo();
        m_Rigidbody2D.AddForce(dir.x * KNOCKBACK_COEF * 20f * xForce * xVelocity * Vector2.right
                                + dir.y * KNOCKBACK_COEF * 20f * yForce * yVelocity * Vector2.up, ForceMode2D.Impulse);
    }
    public void GetDamage(float damage , bool grounded)
    {
        if (grounded) percent += 2*damage;
        else percent += damage;
        playerUI.SetPercent(percent);
    }
    public CapsuleCollider2D GetCollider()
    {
        return bodyCollider;
    }

    public void Die()
    {
            Debug.Log("Player Die");
            alive = false;
    }
    public void ResetPercent()
    {
        percent = 0f;
        playerUI.SetPercent(percent);
    }

    public bool IsAlive()
    {
        return alive;
    }
    public void SetAlive(bool value)
    {
        alive = value;
    }
    public void Respawn()
    {
        // animator.SetTrigger("Recover");
        if (transform.localScale.x < 0)
        {
            Flip();
        }
        alive =true;
    }
    public void SetInputBlock(bool value)
    {
        if (input != null) input.SetInputBlock(value);
    }
    public void SetInputHandler(PlayerInputHandler inp)
    {
        input = inp;
        if (inGame) input.SetInputBlock(false);
    }
    public void SetInGame(bool value)
    {
        inGame = value;
    }
    public int GetPlayerIndex()
    {
        return index;
    }

    public void Flip()
    {
        movement.Flip();
    }

    public void StartMoving()
    {
        movement.StartCanMove();
    }
    public void ChangeMoving()
    {
        movement.ChangeCanMove();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 hitPosition = Vector3.zero;
        //  If the collision gameobject is tilemap, it stores the impact force to deal damage (force),
        //  Find the tile at the contact point
        if (tilemapGameObject == collision.gameObject)
        {
            var map = collision.gameObject.GetComponent<Map>();
            var hit = collision.GetContact(0);
            var force = hit.relativeVelocity;
            Debug.Log("Player, OnCollisionEnter2D : " + gameObject.gameObject + " Contact force x = " + force.x + ", Contact force y = " + force.y);
            hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
            hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
            var tile = tilemap.GetTile(tilemap.WorldToCell(hitPosition));
            //  If  Deal damage depending on impact force
            if (force.magnitude > 5f && (movement.GetSpiked() || movement.GetKo()))
            {
                GetDamage((Mathf.Abs(force.y) + Mathf.Abs(force.x))/1.2f, false);
            }
            //  Calculate the strenght of the rebound on the wall while player is KO
            if ((movement.GetSpiked() || movement.GetKo()) && map.FindTileIndex(tile) < map.GetListCount() - 1)
            {
                Debug.Log("Player, OnCollisionEnter2D : " + gameObject.gameObject + " Contact force x = " + force.x + ", Contact force y = " + force.y);
                Debug.Log("Player, OnCollisionEnter2D : Power percent calc = " + (percent / (.1f * percent + 100f) + 1f));
                Debug.Log("Player, OnCollisionEnter2D : Power.x = " + Mathf.Round((force.x * 10f * SPIKE_COEF * (percent / (.1f * percent + 100f) + 1f) ) / 100));
                Debug.Log("Player, OnCollisionEnter2D : Power.y = " + Mathf.Round((force.y * 10f * SPIKE_COEF * (percent + 1f/ (.1f * percent + 100f) + 1f)) / 100));

                m_Rigidbody2D.AddForce(force * 10f * SPIKE_COEF * ( 1.2f * percent / (.1f * percent + 100f) + 1f));

            }
            //Break the tile and store it in Map Object to rebuild it when game in reset
            if (movement.GetSpiked() && force.y > 7f)
            {
                Debug.Log("Player, OnCollisionEnter2D : " + gameObject.gameObject + " force y = " + force.y);
                if (tilemap.WorldToCell(hitPosition) != null)
                {
                    if (!map.FindInNames(tile.name)) return;
                    tilemap.SetTile(tilemap.WorldToCell(hitPosition), map.GetNextByName(tile.name));
                    map.StoreVect(hitPosition);
                }
            }
            if (movement.GetSpiked())
            {
                movement.SetSpiked(false);
            }
        }
    }

}