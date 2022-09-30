using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Slider barLifePlayer;
    public int maxHealth;
    int currentHealth;

    public GameObject turnoE;
    public GameObject turnoP;

    void Awake() {
        SetMaxHealth(maxHealth);
    }
    private void Start() {
        currentHealth = 0;
    }

    public void SetMaxHealth(int health) {
        barLifePlayer.maxValue = health;
        barLifePlayer.value = 0;
    }

    public void SetHealth(int health) {
        barLifePlayer.value = health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.tag == "BulletBasic") {
            Debug.Log("BulletBasic");
            currentHealth += 50;
            SetHealth(currentHealth);

        }
        if (collision.transform.tag == "BulletSpecial") {
            Debug.Log("BulletSpecial");
            currentHealth += 100;
            SetHealth(currentHealth);
        }
        if (collision.transform.tag == "BulletSuper") {
            Debug.Log("BulletSuper");
            currentHealth += 150;
            SetHealth(currentHealth);
        }
        Destroy(collision.gameObject);
        turnoE.SetActive(false);
        turnoP.SetActive(true);
        Invoke("cambioTurno", 2f);
        
    }

    void cambioTurno() {
        turnoP.SetActive(false);
        Match.estado = "idle";
    }
}
