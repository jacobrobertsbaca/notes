using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnClouds : MonoBehaviour
{
    private float speed = 4;
    int cloudType;
    public GameObject cloud1;
    public GameObject cloud2;
    public GameObject cloud3;
    //Vector3 clouds = new Vector3(cloud1, cloud2, cloud3);
    public GameObject[] clouds = new GameObject[3];
    private Vector4 spawnerBorder;
    float time = 1f;
    float maxX;
    float maxY;
    float minX; 
    float minY;
    bool allowSpawning = false;
    // Start is called before the first frame update
    void Start()
    {
        var spawnerBounds = GetComponent<SpriteRenderer>().bounds;
        maxY = spawnerBounds.max.y;
        minY = spawnerBounds.min.y;
        clouds[0] = cloud1;
        clouds[1] = cloud2;
        clouds[2] = cloud3;
    }

    // Update is called once per frame
    void Update()
    {   
        if (allowSpawning) {
            gameObject.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
            if (time < 0) 
            {
                minX = GetComponent<SpriteRenderer>().bounds.min.x;
                maxX = GetComponent<SpriteRenderer>().bounds.max.x;
            Vector2 spawnLocation = new Vector2(Random.Range(minX,maxX), Random.Range(minY,maxY));
            cloudType = Random.Range(0,2);
            GameObject cloudChoice = clouds[cloudType];
            Instantiate(cloudChoice, spawnLocation, Quaternion.identity);
            time = Random.Range(.3f, 2f);
            }
            time -= Time.deltaTime;
        }
    }

    public void BeginSpawning()
    {
        allowSpawning = true;
    }
}
