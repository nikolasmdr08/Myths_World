using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public int speed;
    Vector3 target;

    // Start is called before the first frame update
    void Start()
    {
        target = new Vector3(916.5550537109375f, 1516.364013671875f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, target, speed);
    }
}
