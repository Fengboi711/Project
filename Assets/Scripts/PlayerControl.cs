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
    private bool isclimbing = false;
    private bool jumpforce = false;
    public bool iskicked = false;
    private float ClimbingInput;
    float horizontalmove;
    Rigidbody2D controller;
    Rigidbody2D rigidbody;
    
    Vector2 kickhitbox = new Vector2(1, 1);
    public float kickstrength = 10f;
 
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Rigidbody2D>();
    }

    void Kick()
    {
        Collider2D collider = Physics2D.OverlapBox(transform.position+ -transform.right, kickhitbox, 50f);
        
        if (Input.GetKeyDown(KeyCode.Q)&& collider.CompareTag("Obstacle"))
        {
            
            rigidbody = collider.GetComponent<Rigidbody2D>();
            

            if (rigidbody != null)
            {
                rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
                rigidbody.AddForce(-transform.right * kickstrength, ForceMode2D.Impulse);
                iskicked = true;
            }
            
            
        }
        if (iskicked && rigidbody != null && rigidbody.velocity.magnitude < 0.1)
        {
            rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            iskicked = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null && collider.CompareTag("Ladder"))
        {
            isclimbing = true; 
        }
        
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        isclimbing = false;
        controller.gravityScale = 1.5f;
    }

    

    // Update is called once per frame
    void Update()
    {
        Kick();
        ClimbingInput = Input.GetAxis("Vertical");
        horizontalmove = Input.GetAxis("Horizontal");
            

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpforce = true;
        }
        
        if (Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0, 0f);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0f, -180, 0f);
            
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
}
