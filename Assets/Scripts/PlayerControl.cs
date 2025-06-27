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
 
    // Start is called before the first frame update
    void Start()
    {
        
        controller = GetComponent<CharacterController>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
       
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
