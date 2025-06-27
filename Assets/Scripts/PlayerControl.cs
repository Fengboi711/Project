using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float gravity = 2.0f;
    public float movespeed = 1.0f;
    public float jumpheight = 1.0f;
    Vector2 movedirection = Vector2.zero;
    CharacterController controller;
    Rigidbody rigidbody;
    Vector2 kickhitbox = new Vector2(1, 1);
    public float kickstrength = 10f;
 
    // Start is called before the first frame update
    void Start()
    {
        
        controller = GetComponent<CharacterController>();
        

        
    }

    void Kick()
    {
        Collider[] collider = Physics.OverlapBox(transform.position+ -transform.right, kickhitbox);
        if (Input.GetKeyDown(KeyCode.Q))
        {
            foreach (Collider kicked in collider)
            {
                rigidbody = kicked.GetComponent<Rigidbody>();

                if (rigidbody != null)
                {
                    rigidbody.AddForce(-transform.right * kickstrength, ForceMode.Impulse);
                }
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Kick();
       
        if (controller.isGrounded)
        {
            movedirection.x = Input.GetAxis("Horizontal");
            movedirection.x *= movespeed;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                movedirection.y = jumpheight;
            }
        }
        if (Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0, 0f);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0f, -180, 0f);
        }

        movedirection.y -= gravity * Time.deltaTime;


        controller.Move(movedirection*Time.deltaTime);
    }
}
