using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Vector3 target;
    public int speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        target = new Vector3(186.22222900390626f,1504.0f,0.0f);
        transform.position = Vector3.MoveTowards(transform.position, target, speed);
    }
}
