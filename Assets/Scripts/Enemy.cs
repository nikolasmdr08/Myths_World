using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if(collision.transform.tag == "Bullet") {
            currentHealth += Atack.hitPoints;
            Atack.hitPoints = 0;
            SetHealth(currentHealth);
            collision.gameObject.SetActive(false);
            Match.estado = "Turno Enemigo";
        }
    }

    int definirAtaque() {
        System.Random rn = new System.Random();
        string[] arrayvalores = new string[] { "basico", "especial","Super Ataque" };
        double[] pesos = new double[] { 0.6, 0.3, 0.1 };
        double[] pesosAcumulados = pesos.Aggregate((IEnumerable<double>)new List<double>(),
                    (x, i) => x.Concat(new[] { x.LastOrDefault() + i })).ToArray();
        double rando = 0;
        rando = rn.NextDouble() * pesos.Sum();
        int posicionArray = pesosAcumulados.ToList().IndexOf(pesosAcumulados.Where(x => x > rando).FirstOrDefault());
        return posicionArray;
    }

    private void Update() {
        if(Match.estado == "Turno Enemigo") {
            Debug.Log(definirAtaque());
            Match.estado = "idle";
        }
    }

}
