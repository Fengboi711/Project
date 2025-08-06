using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    public Transform enemyspawn;
    GameObject player;
    PlayerControl playercheck;
    public LayerMask mask;
    private bool isclimbing = false;
    public int enemyhealth = 5;
    private bool isplayer = false;
    private Vector2 lineofsight;
    private bool isattacking = false;
    public bool isstunned = false;
    Vector2 enemypos = Vector2.zero;
    public float speed = 2.0f;
    public float originalspeed = 2.0f;
    Rigidbody2D enemycontrol;
    private float offset = 2.0f;
    Vector2 pointa;
    Vector2 pointb;
    Vector2 targetpoint;
    Vector2 attackhitbox = Vector2.zero;
    Vector2 vectoroffset = new Vector2(0, 1);
    Obstacle obstacle;
    public Animator animator;
    bool isdead = false;
    BoxCollider2D box;
    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<BoxCollider2D>();
        enemycontrol = GetComponent<Rigidbody2D>();
        pointa = (Vector2)enemyspawn.position + new Vector2(offset,0);
        pointb = (Vector2)enemyspawn.position - new Vector2(offset, 0);
        targetpoint = pointa;
        playercheck = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        obstacle = GameObject.FindGameObjectWithTag("Obstacle").GetComponent<Obstacle>();
        player = GameObject.FindWithTag("Player");
    }

    IEnumerator Patrol()
    {
        while(isplayer == false)
        {
            Vector2 direction = (targetpoint - (Vector2)transform.position);
            enemypos.x = Mathf.Sign(direction.x) * speed;

            if (Mathf.Abs(direction.x) < 0.1f)
            {
                enemypos.x = 0f;
                
            }

            if (targetpoint == pointa && enemycontrol.velocity.x == enemypos.x)
            {
                yield return new WaitForSeconds(5f);
                targetpoint = pointb;
            }
            else if (targetpoint == pointb && enemycontrol.velocity.x == enemypos.x)
            {
                yield return new WaitForSeconds(5f);
                targetpoint = pointa;
            }
            yield return null;
        }
    }

    

    void OnCollisionEnter2D(Collision2D enemycollide)
    {
        if (enemycollide.gameObject.CompareTag("Obstacle") && obstacle.iskicked == true)
        {
            isstunned = true;
            
        }
        
        //isclimbing = false;
        Debug.Log("isclimbing:" + isclimbing);
    }

    private void OnCollisionStay2D(Collision2D enemycollision)
    {
        if (enemycollision.gameObject.CompareTag("Obstacle") && obstacle.iskicked == false)

        {
            isclimbing = true;

            StartCoroutine(ClimbOver());

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        enemycontrol.gravityScale = 6;
        speed = 2.0f;
        isclimbing = false;

    }
    IEnumerator Stunned()
    {
        
        speed = 0;
        Debug.Log("Enemy is stunned");
        yield return new WaitForSeconds(5f);
        isstunned = false;
        speed = originalspeed;
        Debug.Log("Enemy is no longer stunned");
        
    }

    IEnumerator Attack()
    {
        if (isattacking) yield break;
        attackhitbox = new Vector2(2, 2);
        Collider2D attackcollider = Physics2D.OverlapBox((Vector2)(transform.position+transform.right)+ vectoroffset,attackhitbox, 0f);
        if(attackcollider != null && attackcollider.CompareTag("Player") && !isstunned)
        {
            isattacking = true;
            
        }
        while (isattacking == true)
        {
            
            PlayerControl playercheck = attackcollider.GetComponent<PlayerControl>();
            animator.Play("HeroKnight_Attack1");
            playercheck.currenthealth -= 1;
            yield return new WaitForSeconds(1f);
            isattacking = false;
            
        }




    }
    void CheckLineOfSight()
    {
        
        RaycastHit2D hitdata;
        LayerMask laddermask = ~mask;
        lineofsight = player.transform.position - transform.position;
        //Raycast visualizer
        Debug.DrawRay(transform.position + transform.right , lineofsight, Color.red);
        hitdata = Physics2D.Raycast(transform.position +transform.right, lineofsight, 10f, laddermask);
        if (hitdata.collider != null && hitdata.collider.CompareTag("Player"))
        {
            isplayer = true;
            Debug.Log(hitdata.collider.name);
            Debug.Log(isplayer);
        }
        
    }

    
    IEnumerator ClimbOver()
    {
        
        while(isclimbing == true)
        {
            
           
            enemycontrol.velocity = new Vector2(0, 3);
            yield return new WaitForSeconds(3f);
            enemycontrol.velocity = new Vector2(1,enemycontrol.velocity.y);
            yield return null;
        }
        if (isclimbing) yield break;

    }
    IEnumerator Dead()
    {
        if (enemyhealth <= 0)
        {
            isdead = true;
            speed = 0;
            box.isTrigger = true;
            animator.Play("HeroKnight_Death");

            yield return new WaitForSeconds(7f);
            Destroy(gameObject);


        }
        Debug.Log("Enemy Health: " + enemyhealth);
    }

    // Update is called once per frame
    void Update()
    {
        
        CheckLineOfSight();
        StartCoroutine(Patrol());
        animator.SetFloat("Speed", Mathf.Abs(enemycontrol.velocity.x));
        StartCoroutine(Attack());
        
        StartCoroutine(Dead());
        
        if (isstunned)
        {
            StartCoroutine(Stunned());
        }
        
    }
    void FixedUpdate()
    {

        if (isplayer && isclimbing == false)
        {
            if(lineofsight.magnitude > 2f)
            {
                lineofsight.y = 0;
                enemycontrol.velocity = (lineofsight.normalized) * speed;
            }
            else
            {
                enemycontrol.velocity = Vector2.zero;
            }
            
        }
        else if(!isplayer)
        {
            enemycontrol.velocity = new Vector2(enemypos.x, enemycontrol.velocity.y);
        }
        
        if(enemycontrol.velocity.x > 0.001f)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        else if(enemycontrol.velocity.x < -0.001f)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)(transform.position+transform.right), attackhitbox);
    }
}
