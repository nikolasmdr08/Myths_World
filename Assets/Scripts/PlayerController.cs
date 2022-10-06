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
    public GameObject Lose;
    public Text textLife;


    void Awake() {
        SetMaxHealth(maxHealth);
    }
    private void Start() {
        currentHealth = 0;
        textLife.text = maxHealth - currentHealth + " / " + maxHealth;
    }

    public void SetMaxHealth(int health) {
        barLifePlayer.maxValue = health;
        barLifePlayer.value = 0;
    }

    public void SetHealth(int health) {
        barLifePlayer.value = health;
        textLife.text = maxHealth - currentHealth + " / " +maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (maxHealth <= currentHealth && Match.estado != "Terminado") {
            Lose.SetActive(true);
            Match.estado = "Terminado";
            Debug.Log("Player life: " + currentHealth + "/" + maxHealth);
        }
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
