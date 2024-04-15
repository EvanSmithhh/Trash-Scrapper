using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float moveSpeed = 5f; // Movement speed of the enemy

    void Update()
    {
        // Check if player reference is set and not null
        if (player != null)
        {
            // Calculate the direction from the enemy to the player
            Vector3 direction = (player.position - transform.position).normalized;

            // Calculate the distance between the enemy and the player
            float distance = Vector3.Distance(transform.position, player.position);

            // Move the enemy towards the player
            transform.Translate(direction * moveSpeed * Time.deltaTime);

            // Optionally, rotate the enemy to face the player
            //transform.LookAt(player);
        }
    }
}
