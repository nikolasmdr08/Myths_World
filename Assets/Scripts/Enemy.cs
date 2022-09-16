using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public Slider barLifeEnemy;
    public int maxHealth;
    int currentHealth;
    // Start is called before the first frame update
    void Awake()
    {
        SetMaxHealth(maxHealth);
    }
    private void Start() {
        currentHealth = 0;
    }

    public void SetMaxHealth(int health) {
        barLifeEnemy.maxValue = health;
        barLifeEnemy.value = 0;
    }

    public void SetHealth(int health) {
        barLifeEnemy.value = health;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log("Choco bala enter");
        if(collision.transform.tag == "Bullet") {
            currentHealth += Atack.hitPoints;
            Atack.hitPoints = 0;
            SetHealth(currentHealth);
            collision.gameObject.SetActive(false);
            Match.estado = "Turno Enemigo";
        }
    }

}
