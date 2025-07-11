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
    public int enemyhealth = 5;
    private bool isplayer = false;
    private Vector2 lineofsight;
    private bool isattacking = false;
    private bool isstunned = false;
    Vector2 enemypos = Vector2.zero;
    public float speed = 2.0f;
    public float originalspeed = 2.0f;
    Rigidbody2D enemycontrol;
    private float offset = 2.0f;
    Vector2 pointa;
    Vector2 pointb;
    Vector2 targetpoint;
    Vector2 attackhitbox = Vector2.zero;


    // Start is called before the first frame update
    void Start()
    {
        enemycontrol = GetComponent<Rigidbody2D>();
        pointa = (Vector2)enemyspawn.position + new Vector2(offset,0);
        pointb = (Vector2)enemyspawn.position - new Vector2(offset, 0);
        targetpoint = pointa;
        playercheck = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
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
        if (enemycollide.gameObject.CompareTag("Obstacle") && playercheck.iskicked == true)
        {
            StartCoroutine(Stunned());
        }
    }
    IEnumerator Stunned()
    {
        isstunned = true;
        speed = 0;
        
        yield return new WaitForSeconds(2f);
        isstunned = false;
        speed = originalspeed;
        
    }

    IEnumerator Attack()
    {
        if (isattacking) yield break;
        attackhitbox = new Vector2(2, 2);
        Collider2D attackcollider = Physics2D.OverlapBox(transform.position-transform.right,attackhitbox, 0f);
        if(attackcollider != null && attackcollider.CompareTag("Player") && !isstunned)
        {
            isattacking = true;
        }
        while(isattacking == true)
        {
            PlayerControl playercheck = attackcollider.GetComponent<PlayerControl>();
            playercheck.health -= 1;
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
        Debug.DrawRay(transform.position - transform.right , lineofsight, Color.red);
        hitdata = Physics2D.Raycast(transform.position -transform.right, lineofsight, 10f, laddermask);
        if (hitdata.collider != null && hitdata.collider.CompareTag("Player"))
        {
            isplayer = true;
            Debug.Log(hitdata.collider.name);
            Debug.Log(isplayer);
        }
        
    }

    void Dead()
    {
        if (enemyhealth <= 0)
        {
            Destroy(gameObject);
            
        }
        Debug.Log("Enemy Health: " + enemyhealth);
    }

    // Update is called once per frame
    void Update()
    {
        CheckLineOfSight();
        StartCoroutine(Patrol());
        StartCoroutine(Attack());
        Dead();
        
        
    }
    void FixedUpdate()
    {

        if (isplayer)
        {
            if(lineofsight.magnitude > 2.5f)
            {
                lineofsight.y = 0;
                enemycontrol.velocity = (lineofsight.normalized) * speed;
            }
            else
            {
                enemycontrol.velocity = Vector2.zero;
            }
            
        }
        else
        {
            enemycontrol.velocity = new Vector2(enemypos.x, enemycontrol.velocity.y);
        }
        
        
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position-transform.right, attackhitbox);
    }
}
