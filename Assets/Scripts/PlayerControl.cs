using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEditor.VersionControl;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerControl : MonoBehaviour
{
    
    public float gravity = 2.0f;
    public float movespeed = 10.0f;
    public float jumpheight = 20.0f;
    public float climbspeed = 5.0f;
    public int health = 10;
    private bool isclimbing = false;
    private bool jumpforce = false;
    private Obstacle obstacle;
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
    private BoxCollider2D playercollider;
    private Vector2 pickuphitbox = new Vector2(6, 4);
    public bool haskey = false;
    private Vector2 jumpcheck = new Vector2(0.1f, 0.1f);
    GameObject fireball;
    bool canfire = false;
    public Vector2 spawnpoint;
    public TextMeshProUGUI win;
    public TextMeshProUGUI lose;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Rigidbody2D>();
        obstacle = GameObject.FindGameObjectWithTag("Obstacle").GetComponent<Obstacle>();
        playercollider = GetComponent<BoxCollider2D>();
        
    }

    
    void Move()
    {
        ClimbingInput = Input.GetAxis("Vertical");
        horizontalmove = Input.GetAxis("Horizontal");

        Collider2D jumpcollider = Physics2D.OverlapBox((Vector2)(transform.position-transform.up) - offsetvector , jumpcheck,0f, ~playermask);
        

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

    IEnumerator FireBallSpell()
    {
        
        if (canfire) yield break;
        canfire = true;
        fireball = Resources.Load<GameObject>("Prefabs/Fireball");
        
        GameObject fireballinstant = Instantiate(fireball, transform.position + transform.right, transform.rotation);
        
        Rigidbody2D fireballrb = fireballinstant.GetComponent<Rigidbody2D>();
        fireballrb.AddForce((Vector2)(transform.right ) * 10f + new Vector2(0, 3), ForceMode2D.Impulse);
        yield return new WaitForSeconds(2);
        canfire = false;
    }

    IEnumerator Fireflysummon()
    {
        GameObject firefly = Resources.Load<GameObject>("Prefabs/fireflyplaceholder");

        for (int i = 0; i < 6; i++)
        {
            spawnpoint = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            GameObject fireflyinstant = Instantiate(firefly, (Vector2)transform.position + spawnpoint, transform.rotation);
            fireflymoves fireflymove = fireflyinstant.GetComponent<fireflymoves>();
            fireflymove.flyposition = spawnpoint;
            yield return new WaitForSeconds(0.1f);
        }
    }
    void FireflySpell()
    {
        StartCoroutine(Fireflysummon());
    }
    void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            LayerMask keymask = LayerMask.GetMask("Key", "Door");
            Collider2D interactbox = Physics2D.OverlapBox(transform.position, pickuphitbox, 0f, keymask);
            if (interactbox != null && interactbox.CompareTag("Key"))
            {
                haskey = true;
            }
            if (haskey && interactbox.CompareTag("Door"))
            {
                Destroy(interactbox.gameObject);
                win.gameObject.SetActive(true);
                Time.timeScale = 0f;
            }
            Debug.Log("haskey: " + haskey);
            Debug.Log("inside box: " + interactbox);
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
                obstacle.iskicked = true;
            }
            
            
        }
        if (obstacle.iskicked && rigidbody != null && rigidbody.velocity.magnitude < 0.1)
        {
            rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            
            obstacle.iskicked = false;
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

    void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            playercollider.size = new Vector2(1, 2f);
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            playercollider.size = new Vector2(1, 4f);
        }

    }
    void Dead()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
            lose.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
        Debug.Log("Player Health: "+health);
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
        Crouch();
        Interact();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(FireBallSpell());
        }
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Attack());
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("You have pressed Space");

        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            FireflySpell();
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
        Gizmos.DrawWireCube((Vector2)(transform.position - transform.up) - offsetvector , jumpcheck);
        Gizmos.DrawWireCube(transform.position + transform.right, attackhitbox);
        Gizmos.DrawWireCube(transform.position, pickuphitbox);
    }
}
