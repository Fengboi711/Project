using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireflymoves : MonoBehaviour
{
    public Vector2 flyposition;
    PlayerControl player;
    Enemy enemy;
    Vector2 directionofenemy;
    bool isattacking = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        directionofenemy = enemy.transform.position - transform.position;
        if(directionofenemy.magnitude < 5f)
        {
            isattacking = true;
            if (isattacking)
            {
                transform.position = Vector2.MoveTowards((transform.position), (Vector2)enemy.transform.position + flyposition, 5f * Time.deltaTime);
                enemy.isstunned = true;
            }
            
        }
        else
        {
            transform.position = Vector2.Lerp(transform.position, (Vector2)player.transform.position + flyposition, 5f * Time.deltaTime);
        }
            
    }
}
