using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float gravity = 2.0f;
    public float movespeed = 10.0f;
    public float jumpheight = 20.0f;
    public float climbspeed = 5.0f;
    public int health = 10;
    private bool isclimbing = false;
    private bool jumpforce = false;
    public bool iskicked = false;
    public float kickstrength = 10f;
    public bool isseen = false;
    private float ClimbingInput;
    float horizontalmove;
    private bool canattack = false;
    Rigidbody2D controller;
    Rigidbody2D rigidbody;
    Vector2 kickhitbox = new Vector2(1,1);
    Vector2 offsetvector = new Vector2(0, 1);
    Vector2 attackhitbox = Vector2.zero;
    public LayerMask obstaclemask;
    public LayerMask playermask;
      
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Rigidbody2D>();
    }

    
    void Move()
    {
        ClimbingInput = Input.GetAxis("Vertical");
        horizontalmove = Input.GetAxis("Horizontal");

        Collider2D jumpcollider = Physics2D.OverlapBox((Vector2)(transform.position-transform.up) - offsetvector , kickhitbox,0f, ~playermask);
        Debug.Log(jumpcollider.tag);

        if (Input.GetKeyDown(KeyCode.Space) && jumpcollider.CompareTag("Floor"))
        {
            jumpforce = true;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0f, -180, 0f);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0, 0f);

        }
    }

    void Kick()
    {
        Collider2D collider = Physics2D.OverlapBox(transform.position+ transform.right , kickhitbox, 50f, obstaclemask);
        
        if (Input.GetKeyDown(KeyCode.Q)&& collider.CompareTag("Obstacle"))
        {
            
            rigidbody = collider.GetComponent<Rigidbody2D>();
            

            if (rigidbody != null)
            {
                rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
                rigidbody.AddForce(transform.right * kickstrength, ForceMode2D.Impulse);
                iskicked = true;
            }
            
            
        }
        if (iskicked && rigidbody != null && rigidbody.velocity.magnitude < 0.1)
        {
            rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            
            iskicked = false;
        }
    }

    IEnumerator Attack()
    {
        if (canattack) yield break;
        attackhitbox = new Vector2(2, 2);
        Collider2D attackcollider = Physics2D.OverlapBox(transform.position + transform.right, attackhitbox, 0f, ~playermask);
        if(attackcollider != null && attackcollider.CompareTag("Enemy"))
        {
            canattack = true;
            Enemy enemy = attackcollider.GetComponent<Enemy>();
            enemy.enemyhealth -= 1;
            
        }
        yield return new WaitForSeconds(1);
        canattack = false;
    }
    void Dead()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
        Debug.Log(health);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null && collider.CompareTag("Ladder"))
        {
            isclimbing = true;
            horizontalmove = 0;
        }
        
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        isclimbing = false;
        controller.gravityScale = 6.0f;
    }

    

    // Update is called once per frame
    void Update()
    {
        Kick();
        Move();
        Dead();

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Attack());
        }
        
    }
    void FixedUpdate()
    {
        if(jumpforce == true)
        {
            controller.velocity = new Vector2(controller.velocity.x, jumpheight);
            jumpforce = false;
        }
        else
        {
            controller.velocity = new Vector2(horizontalmove * movespeed, controller.velocity.y);
        }
        if (isclimbing == true)
        {
            controller.gravityScale = 0;
            controller.velocity = new Vector2(controller.velocity.x,ClimbingInput * climbspeed);
        }
    }
    void OnDrawGizmos()
    {
        //kick hit box visualizer
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + transform.right , kickhitbox);
        Gizmos.DrawWireCube((Vector2)(transform.position - transform.up) - offsetvector , kickhitbox);
        Gizmos.DrawWireCube(transform.position + transform.right, attackhitbox);
    }
}
