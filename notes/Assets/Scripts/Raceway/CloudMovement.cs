using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    private float speed = .5f;
    private float xMovement;
    private float yMovement;
    private float time = 2f;
    // Start is called before the first frame update
    void Start()
    {
        xMovement = Random.Range(-.2f,.2f);
        yMovement = Random.Range(-.2f,.2f);
        float opacity = Random.Range(.2f, 1);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, opacity);
    }
    // Update is called once per frame
    void Update()
    {
        
        gameObject.transform.position += new Vector3(xMovement *speed*Time.deltaTime, yMovement*speed*Time.deltaTime, 0);
        if (time < 0){
            xMovement = Random.Range(-1,1);
            yMovement = Random.Range(-.2f,.2f);
            time = 2f;
        }
        time -= Time.deltaTime;

    }
}
