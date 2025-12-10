using UnityEngine;

public class FrikandelSpawnerScript : MonoBehaviour
{
    public GameObject frikandel;
    public float spawnRate = 5;
    private float timer = 0;
    public float heightOffset = 15;
    public bool isSpawning = false; // Default to false
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isSpawning)
        {
            spawnFrikandel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSpawning) return; // Stop if not spawning

        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
        }
        
        else
        {
            spawnFrikandel();
            timer = 0;
        }

    }
    void spawnFrikandel()
    {
            float lowestPoint = transform.position.y - heightOffset;
            float highestPoint = transform.position.y + heightOffset;
            
            Instantiate(frikandel, new Vector3(transform.position.x, Random.Range(lowestPoint, highestPoint), 0), transform.rotation);
  
    }
}
