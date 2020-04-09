﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    Rigidbody2D body;
    Animator animator;
    CapsuleCollider2D col;
    PlayerHealth playerHealth;
    Vector2 spawnPosition;
    Vector2 actualPosition;
    Vector2 playerPosition;

    private float movementSpeed = 0.5f;                                         ////////////////////
    private float movementTime = 2f;                                            //
    private float restTime = 3f;                                                //
    private float dangerDistance = 2.5f;                                        //
    private float attackDistance;                                               // Movement and path variables
    private float spawnDistance = 3f;                                           //
    private float timer;                                                        //
    private float h = 0;                                                        //
    private bool left = true;                                                   //
    private int n = 5;                                                          ////////////////////

    private float damage = 1;                                                   //
    private bool Attacking;                                                     //
    private bool Attacked;                                                      //
    private bool AttackRange;                                                      //
    AnimatorClipInfo[] currentClipInfo;                                         //
    private float clipNormalizedTime;                                           //


    void Start()                                                                // Start is called before the first frame update
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();
        timer = restTime;
        StartCoroutine(DoCheck());
        spawnPosition = transform.position;
        attackDistance = 0;
        Attacking = false;
        Attacked = false;
        AttackRange = false;
    }

    void Update()                                                               // Update is called once per frame
    {
        Path();
        body.velocity = new Vector2(Vector2.right.x * h * movementSpeed, body.velocity.y);
        EnemyAnimation();
        Attack();
    }

    private IEnumerator DoCheck()                                               // Check all distance 
    {
        for (; ; )
        {
            actualPosition = transform.position;
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            DistanceCheck();
            DistanceCheck(actualPosition, spawnPosition);
            yield return new WaitForSeconds(.2f);
        }
    }

    private float DistanceCheck()                                               // Check distance between player and enemy
    {
        return Vector2.Distance(actualPosition, playerPosition);
    }

    private float DistanceCheck(Vector2 Position1, Vector2 Position2)           // Check distance between 2 position
    {
        return Vector2.Distance(Position1, Position2);
    }

    private void Path()                                                         //  Path design of walking enemy
    {
        if ((DistanceCheck(actualPosition, spawnPosition) >= spawnDistance) && (DistanceCheck(playerPosition,spawnPosition) >= spawnDistance))
        {
            h = 0;
        }
        else if (DistanceCheck(actualPosition, spawnPosition) >= spawnDistance)
        {
            if ((spawnPosition.x - actualPosition.x) > 0)
            {
                h = 1;
                n = 1;
                left = false;
                timer = restTime;
            }
            else
            {
                h = -1;
                n = 1;
                left = true;
                timer = restTime;
            }
        }
        else if ((DistanceCheck() > dangerDistance) && (DistanceCheck(actualPosition, spawnPosition) < spawnDistance))
        {
            if ((timer < 0) && (n < 10))
            {
                n++;
                if (n == 10)
                {
                    n = 2;
                    left = !left;
                }
                if (n % 2 == 0)
                {
                    if (left == true)
                        h = -1;
                    else h = 1;
                    timer = movementTime;
                }
                else
                {
                    h = 0;
                    timer = restTime;
                }
            }
            else timer -= Time.deltaTime;
        }
        else if ((DistanceCheck() >= attackDistance) && (DistanceCheck() <= dangerDistance) && (DistanceCheck(actualPosition, spawnPosition) < spawnDistance))
        {
            if ((playerPosition.x - actualPosition.x) > 0)
                h = 2;
            else h = -2;
            Attacking = false;

        }
        else if ((DistanceCheck() <= attackDistance) && (DistanceCheck(actualPosition, spawnPosition) < spawnDistance))
        {
            h = 0;
            Attacking = true;
        }
    }

    private void EnemyAnimation ()                                              // Start animation
    {
        if (h < 0)
        {
            body.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (h > 0)
        {
            body.transform.localScale = new Vector3(1, 1, 1);
        }

        if (h != 0)
            animator.SetBool("IsWalking", true);
        else animator.SetBool("IsWalking", false);
    }

    private void Attack()
    {
        currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);

        if (currentClipInfo[0].clip.name.Equals("Attack"))
        {
            clipNormalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }

        if (Attacking)
            animator.SetBool("IsAttacking", true);
        if ((clipNormalizedTime >= .6f) && (clipNormalizedTime < 1f))
            if (!AttackRange)
            {
                //col.size.Set(col.size.x + .1f, col.size.y);
                AttackRange = !AttackRange;
        Debug.Log(clipNormalizedTime);
            }
        else if (clipNormalizedTime >= 1f)
        {
            if (AttackRange)
                AttackRange = !AttackRange;
            if (Attacked)
                Attacked = !Attacked;
            animator.SetBool("IsAttacking", false);
            Attacking = false;
            clipNormalizedTime = 0f;
            //col.size.Set(col.size.x - .1f, col.size.y);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if ((other.gameObject.tag == "Player")&&(attackDistance==0))
        {
            h = 0;
            attackDistance = Vector2.Distance(actualPosition, playerPosition);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if ((other.gameObject.tag == "Player") && (AttackRange))
        {
            if (!Attacked)
            {
                playerHealth = other.gameObject.GetComponent<PlayerHealth>();
                playerHealth.Damage(damage);
                Attacked = true;
            }
        }
    }
}