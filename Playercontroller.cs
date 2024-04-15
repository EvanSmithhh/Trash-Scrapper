using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Playercontroller : MonoBehaviour
{
    private Rigidbody playerRb;
    public Transform orientation;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;

    private bool canShoot = true;
    private bool canReload = true;
    private bool crouched = false;
    public int harpoonChamb = 2;
    private int maxChamb = 2;
    public Text ammoCount;
    public Text ammoCountMax;
    
    private bool gameOver = false;
    public static int health = 100;
    public Text healthCount;

    private bool jumpOff = false;
    public float groundDrag;
    public float speed = 15000.0f;
    private float crouchYScale = 0.5f;
    private float baseAcc = 15000.0f;
    public float jumpHeight = 10;
    private bool isInAir = false;

    private CapsuleCollider plCollider;
    private float baseHeight;
    private float maxSpeed = 7;
    private float baseSpeed = 7;

    public GameObject harpoon;
    public Transform shotOrigin;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        playerRb = GetComponent<Rigidbody>();
        plCollider = GetComponent<CapsuleCollider>();
        baseHeight = transform.localScale.y;
    }

    private void Update()
    {

        // Decrease player air control
        if (isInAir)
        {
            if (maxSpeed <= baseSpeed * 1.5 && playerRb.velocity.magnitude <= maxSpeed)
            {
                maxSpeed = playerRb.velocity.magnitude;
            }
        }

        

        // Shoots the harpoon on left-click
        if (Input.GetKeyDown(KeyCode.Mouse0) && canShoot && harpoonChamb > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
            {
                Vector3 aimPoint = hit.point;
                shotOrigin.LookAt(aimPoint);
            }

            ShootHarpoon();

            harpoonChamb -= 1;

            canShoot = false;
            StartCoroutine("ShotCooldown");
        }

        // Display counters
        ammoCount.text = harpoonChamb.ToString();
        ammoCountMax.text = maxChamb.ToString();

        healthCount.text = health.ToString();
        healthCount.color = new Color((125 - health)/100f, health/100f, 0);

        // Health bottom cap
        if (health <= 0)
        {
            health = 0;
            gameOver = true;
        }

        // Send player to death screen when killed
        if (gameOver == true)
        {
            SceneManager.LoadScene("GameOver");
        }

        // Reload the weapon if ammo hits zero or if r key is pressed
        if (harpoonChamb < 1 || Input.GetKeyDown(KeyCode.R) && canReload)
        {
            canShoot = false;
            harpoonChamb = 0;
            canReload = false;
            StartCoroutine("HarpoonReload");
        }

        // Give the player friction while touching the ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        Debug.DrawRay(transform.position, Vector3.down, Color.green);
        grounded = Physics.Raycast(new Vector3(transform.position.x + 0.4f, transform.position.y, transform.position.z + 0.36f), Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround) ||
            Physics.Raycast(new Vector3(transform.position.x + 0.4f, transform.position.y, transform.position.z - 0.4f), Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround) ||
            Physics.Raycast(new Vector3(transform.position.x + 0.4f, transform.position.y, transform.position.z - 0.4f), Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround) ||
            Physics.Raycast(new Vector3(transform.position.x - 0.46f, transform.position.y, transform.position.z - 0.4f), Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround) ||
            Physics.Raycast(new Vector3(transform.position.x - 0.46f, transform.position.y, transform.position.z + 0.36f), Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

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

        // Set the player's movement speed while on the ground
        if (grounded)
        {
            speed = baseAcc;
            maxSpeed = baseSpeed;
            isInAir = false;
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
            if (!crouched)
            {
                crouched = true;
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                playerRb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
                baseAcc *= 0.5f;
                baseSpeed *= 0.5f;
            }
            else
            {
                crouched = false;
                transform.localScale = new Vector3(transform.localScale.x, baseHeight, transform.localScale.z);
                baseAcc *= 2f;
                baseSpeed *= 2f;
            }
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
        speed = baseAcc * 0.35f;
        isInAir = true;
    }

    IEnumerator ShotCooldown()
    {
        yield return new WaitForSeconds(0.3f);
        canShoot = true;
    }
    
    IEnumerator HarpoonReload()
    {
        yield return new WaitForSeconds(0.5f);
        harpoonChamb = maxChamb;
        StartCoroutine("ReloadCooldown");
    }

    IEnumerator ReloadCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        canShoot = true;
        canReload = true;
    }

    private void ShootHarpoon()
    {
        // Create a harpoon at the shot origin
        Instantiate(harpoon, shotOrigin.position, shotOrigin.rotation);
    }


}

