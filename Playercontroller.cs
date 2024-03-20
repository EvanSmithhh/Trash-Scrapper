using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;

public class Playercontroller : MonoBehaviour
{
    private Rigidbody playerRb;
    public Transform orientation;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;

    private bool jumpOff = false;
    public float groundDrag;
    public float speed = 15000.0f;
    private float crouchYScale = 0.5f;
    private float baseAcc = 15000.0f;
    public float jumpHeight = 10;
    private float airSpeed = 500;
    private bool canStand = true;

    private CapsuleCollider plCollider;
    private float baseHeight;
    private float maxSpeed = 7;
    private float baseSpeed = 7;

    public GameObject harpoon;
    public Transform shotOrigin;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        plCollider = GetComponent<CapsuleCollider>();
        baseHeight = transform.localScale.y;
    }

    private void Update()
    {

        // Check if an object above the player is blocking their ability to stand up
        canStand = !Physics.Raycast(transform.position, Vector3.up, playerHeight * 0.5f);

        // Shoots the harpoon on left-click
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShootHarpoon();
        }

        // Give the player friction while touching the ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded)
        {
            playerRb.drag = groundDrag;
        }
        else
        {
            playerRb.drag = 0;
        }

        // Make the player jump when space is pressed
        if  (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded && !jumpOff)
            {
                playerRb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                jumpOff = true;
                StartCoroutine("ResetJump");
                StartCoroutine("SlowPlayer");
            }
        }

        if (grounded)
        {
            speed = baseAcc;
        }

        // Sprint while shift is held
        if (Input.GetKey(KeyCode.LeftShift) && grounded)
        {
            maxSpeed = baseSpeed * 1.5f;
            speed = baseAcc * 1.5f;
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && grounded)
        {
            maxSpeed = baseSpeed;
            speed = baseAcc;
        }

        // Cut the player's size in half and make them slower while crouched
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            playerRb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
            baseAcc *= 0.5f;
            baseSpeed *= 0.5f;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        { 
            transform.localScale = new Vector3(transform.localScale.x, baseHeight, transform.localScale.z);
            baseAcc *= 2f;
            baseSpeed *= 2f;  
        }


        MovePlayer();

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        

        // Stop the character from moving too fast
        if (playerRb.velocity != Vector3.zero)
        {
            if (playerRb.velocity.magnitude > maxSpeed)
            {
                Vector3 vel = playerRb.velocity.normalized * maxSpeed;
                vel.y = playerRb.velocity.y;
                playerRb.velocity = vel;
            }
        }
        
        
        

    }

    public void MovePlayer()
    {
        // Get input for movement
        float HorizontalInput = Input.GetAxis("Horizontal");
        float VerticalInput = Input.GetAxis("Vertical");

        // Calculate forward movement
        Vector3 movement = orientation.transform.forward * speed * Time.deltaTime * VerticalInput;
        Vector3 Movement = orientation.transform.right * speed * Time.deltaTime * HorizontalInput;

        // Apply movement to the character's position
        playerRb.AddForce(movement, ForceMode.Force);
        playerRb.AddForce(Movement, ForceMode.Force);
    }

    IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(0.5f);
        jumpOff = false;
    }

    IEnumerator SlowPlayer()
    {
        yield return new WaitForSeconds(0.4f);
        speed = baseAcc * 0.25f;
    }

    private void ShootHarpoon()
    {
        // Create a harpoon at the shot origin
        Instantiate(harpoon, shotOrigin.position, shotOrigin.rotation);
    }

}

