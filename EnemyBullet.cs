using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{

    private float speed = 10.0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate (Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Playercontroller player = other.gameObject.GetComponent<Playercontroller>();
            int randDmg = Random.Range(2, 8);
            Playercontroller.health -= randDmg;
            Destroy(gameObject);
        }
    }

}
