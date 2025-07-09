using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    public Transform enemyspawn;
    public Transform player;
    Vector2 enemypos = Vector2.zero;
    
    public float speed = 5.0f;
   
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

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = (targetpoint - (Vector2)transform.position);
        enemypos.x = Mathf.Sign(direction.x) * speed;

        if (Mathf.Abs(direction.x) < 0.1f)
        {
            enemypos.x = 0f;
        }

        if(targetpoint == pointa && enemycontrol.velocity.x == enemypos.x)
        {
            targetpoint = pointb;
        }
        else if(targetpoint == pointb && enemycontrol.velocity.x == enemypos.x)
        {
        targetpoint = pointa;
        }
    }
    void FixedUpdate()
    {
        enemycontrol.velocity = new Vector2(enemypos.x, enemycontrol.velocity.y);
    }
}
