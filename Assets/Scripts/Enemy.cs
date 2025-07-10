using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    public Transform enemyspawn;
    public Transform player;
    public LayerMask mask;
    private bool isplayer = false;
    private Vector2 lineofsight;
    Vector2 enemypos = Vector2.zero;
    public float speed = 2.0f;
   
    Rigidbody2D enemycontrol;
    private float offset = 2.0f;
    Vector2 pointa;
    Vector2 pointb;
    Vector2 targetpoint;

    
    // Start is called before the first frame update
    void Start()
    {
        enemycontrol = GetComponent<Rigidbody2D>();
        pointa = (Vector2)enemyspawn.position + new Vector2(offset,0);
        pointb = (Vector2)enemyspawn.position - new Vector2(offset, 0);
        targetpoint = pointa;
        
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
    void CheckLineOfSight()
    {
        RaycastHit2D hitdata;
        LayerMask laddermask = ~mask;
        lineofsight = player.transform.position - transform.position;
        //Raycast visualizer
        Debug.DrawRay((Vector2)transform.position + Vector2.left , lineofsight, Color.red);
        hitdata = Physics2D.Raycast((Vector2)transform.position + Vector2.left, lineofsight, 10f, laddermask);
        if (hitdata.collider != null && hitdata.collider.CompareTag("Player"))
        {
            isplayer = true;
            Debug.Log(hitdata.collider.name);
            Debug.Log(isplayer);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckLineOfSight();
        StartCoroutine(Patrol());
        
        
    }
    void FixedUpdate()
    {

        if (isplayer)
        {
            if(lineofsight.magnitude > 3)
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
}
