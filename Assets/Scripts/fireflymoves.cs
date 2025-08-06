using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireflymoves : MonoBehaviour
{
    public Vector2 flyposition;
    PlayerControl player;
    GameObject[] enemy;
    GameObject currentenemy;
    Enemy enemyscript;
    
    bool isattacking = false;
    float closest = Mathf.Infinity;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        enemy = GameObject.FindGameObjectsWithTag("Enemy");
    }

    GameObject FindClosestEnemy()
    {
        foreach(GameObject enemies in enemy)
        {
            Vector2 distance = enemies.transform.position - transform.position;
            if(distance.magnitude < closest)
            {
                closest = distance.magnitude;
                currentenemy = enemies;
                return currentenemy;
                
            }
            
        }
        return currentenemy;
        
    }

    IEnumerator DestroyFireflies()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        FindClosestEnemy();
        
        if(closest < 5f)
        {
            isattacking = true;
            if (isattacking)
            {
                enemyscript = currentenemy.GetComponent<Enemy>();
                transform.position = Vector2.MoveTowards((transform.position), (Vector2)currentenemy.transform.position + flyposition, 5f * Time.deltaTime);
                enemyscript.isstunned = true;
                StartCoroutine(DestroyFireflies());
            }
            
        }
        else
        {
            transform.position = Vector2.Lerp(transform.position, (Vector2)player.transform.position + flyposition, 5f * Time.deltaTime);
        }
            
    }
}
