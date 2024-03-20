using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject prefabToSpawn; // The prefab to spawn
    public Transform[] spawnPoint; // The spawn point
    public float spawnInterval = 2f; // Interval between spawns

    void Start()
    {
        // Call the Spawn function repeatedly with the specified interval
        InvokeRepeating("Spawn", 0f, spawnInterval);
    }

    void Spawn()
    {
        // Instantiate the prefab at the spawn point's position and rotation
        Instantiate(prefabToSpawn, spawnPoint[0].position, spawnPoint[0].rotation);
    }
}