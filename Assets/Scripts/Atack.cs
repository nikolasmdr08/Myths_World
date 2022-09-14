using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atack : MonoBehaviour
{
    public static int hitPoints;
    public static int valuePiece;
    public GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("KillPiece")) {
            SumarHitPoints(); //pasr tipo de ataque aqui
            Destroy(collision.gameObject);
        }
    }

    private void SumarHitPoints() {
        hitPoints += 1;
        //consultar stats
        GameObject[] array = GameObject.FindGameObjectsWithTag("KillPiece");
        if (array.Length <= 1) {
            bullet.transform.position = transform.position;
            bullet.SetActive(true);
        }
            

    }
}
