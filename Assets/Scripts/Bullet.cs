﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed = 3.5f;
    [SerializeField]
    private Rigidbody2D body = null;
    AudioManager audioManager;
    [SerializeField]
    private int damage;
    private float timer = 1.1f;


    private void Start()
    {
        body.velocity = transform.right * speed;
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("AudioManager non trovato");
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            speed = 0f;
            audioManager.PlaySound("Explosion");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if ((collision.CompareTag("Enemy")) && (this.CompareTag("Player")))
        {
            collision.GetComponent<EnemyHealth>().Damage(damage);
            speed = 0f;
            audioManager.PlaySound("Explosion");
            Destroy(gameObject);
        }
        else if ((collision.CompareTag("Player")) && (this.CompareTag("Enemy")))
        {
            collision.GetComponent<PlayerHealth>().Damage(damage);
            speed = 0f;
            audioManager.PlaySound("Explosion");
            Destroy(gameObject);
        }
    }
}
