using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    
    [Tooltip("The Prefab to be spawned into the scene.")]
    public GameObject spawnPrefab = null;

    [Tooltip("The time between spawns")]
    public float spawnTime = 5.0f;

    // keep track of time passed for next spawn
    private float nextSpawn = 0f;

    // Start is called before the first frame update
    void Start()
    {
        nextSpawn = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // update the time until nextSpawn
        nextSpawn += Time.deltaTime;

        // if time to spawn
        if (!(nextSpawn > spawnTime)) return;
        
        var transform2 = transform;
        
        // add some randomness to the rotation
        transform2.rotation = Random.rotation;
        
        // add some randomness to the location
        var randomLocation = transform2.position + new Vector3(
            Random.Range(-5.0f, 5.0f), 
            Random.Range(-5.0f, 5.0f), 
            Random.Range(-5.0f, 5.0f)
            );
        
        // Spawn the gameObject at the spawners current position and rotation
        var projectileGameObject = Instantiate(spawnPrefab, randomLocation, transform2.rotation, null);

        // reset the time until nextSpawn
        nextSpawn = 0f;

    }
}
