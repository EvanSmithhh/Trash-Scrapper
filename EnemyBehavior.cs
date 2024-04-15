using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{

    public float health = 20;
    private bool canRndMv = true;
    private bool randCycleOver = true;
    private Transform player;
    private float maxSpeed = 20.0f;
    private NavMeshAgent agent;
    private float distanceToPlayer;
    private Rigidbody rb;
    private Transform shotOrigin;
    private bool canShoot = true;
    public GameObject projectile;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        shotOrigin = transform.Find("EnemyShotOrigin");
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }

        transform.LookAt(new Vector3 (player.position.x, transform.position.y, player.position.z));

        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        MoveEnemy();


    }

    private void FixedUpdate()
    {
        if (rb.velocity != Vector3.zero)
        {
            if (rb.velocity.magnitude > maxSpeed)
            {
                Vector3 vel = rb.velocity.normalized * maxSpeed;
                vel.y = rb.velocity.y;
                rb.velocity = vel;
            }
        }

        shotOrigin.LookAt(player.position);
    }

    void MoveEnemy()
    {
        if (distanceToPlayer > 8)
        {
            agent.speed = 3.5f;
            agent.destination = player.position;
        }
        else if (distanceToPlayer < 4)
        {
            agent.speed = 0;
            rb.AddForce(transform.forward * -2, ForceMode.Force);
        }
        else
        {
            agent.speed = 0;
            rb.velocity = Vector3.zero;
            if (randCycleOver)
            {
                StartCoroutine("MovementPause");
            }
            StartCoroutine("ShotWait");

        }
        

    }

    void ShootPlayer()
    {
        Instantiate(projectile, shotOrigin.transform.position, shotOrigin.transform.rotation);
        canShoot = false;
        StartCoroutine("ShotCooldown");
    }

    void RandomMove()
    {
        canRndMv = true;
        int randomMv = Random.Range(0, 4);

        if (randomMv == 0)
        {
           rb.AddForce(Vector3.right * 1, ForceMode.Impulse);
        }
        else if (randomMv == 1)
        {
            rb.AddForce(Vector3.left * 1, ForceMode.Impulse);
        }
        else if (randomMv == 2)
        {
            rb.AddForce(Vector3.forward * 0.5f, ForceMode.Impulse);
        }

        StartCoroutine("StopMoving");
    }

    IEnumerator StopMoving()
    {
        yield return new WaitForSeconds(1);
        canRndMv = false;
        randCycleOver = true;
    }

    IEnumerator MovementPause()
    {
        yield return new WaitForSeconds(2);
        randCycleOver = false;
        if (!canRndMv)
        {
            StartCoroutine("RandomMove");
        }
    }

    IEnumerator ShotWait()
    {
        yield return new WaitForSeconds(0.5f);
        if (canShoot)
        {
            ShootPlayer();
        }
    }

    IEnumerator ShotCooldown()
    {
        yield return new WaitForSeconds(0.7f);
        canShoot = true;
    }
}
