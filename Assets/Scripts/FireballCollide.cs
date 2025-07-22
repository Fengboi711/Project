using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballCollide : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        Collider2D[] fireballaoe = Physics2D.OverlapBoxAll(transform.position, new Vector2(5, 5),0f);
        foreach (Collider2D i in fireballaoe)
        {
            if (i.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = i.gameObject.GetComponent<Enemy>();
                enemy.enemyhealth -= 3;
            }
            
        }
        
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
