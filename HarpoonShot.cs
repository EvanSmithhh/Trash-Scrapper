using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HarpoonShot : MonoBehaviour
{

    public float speed = 20.0f;
    public float maxSpeed = 20.0f;

    private Rigidbody rb;
    private CapsuleCollider hCollide;
    private EnemyBehavior enemyBehavior;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        hCollide = GetComponent<CapsuleCollider>();
        StartCoroutine("DestroySelfTimer");
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move harpoon forward constantly
        rb.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.Force);

        if (rb.velocity != Vector3.zero)
        {
            if (rb.velocity.magnitude > maxSpeed)
            {
                Vector3 vel = rb.velocity.normalized * maxSpeed;
                vel.y = rb.velocity.y;
                rb.velocity = vel;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        
        if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Projectile"))
        {
            // Makes the harpoon stick to a non-player or non-harpoon object that it hits
            rb.isKinematic = true;
            hCollide.enabled = false;
            transform.SetParent(other.transform);

            if (other.gameObject.CompareTag("Enemy"))
            {
                enemyBehavior = other.gameObject.GetComponent<EnemyBehavior>();
                enemyBehavior.health -= 10;
            }
        }
    }

    IEnumerator DestroySelfTimer()
    {
        // Destroy the harpoon after 10 seconds of existing
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
