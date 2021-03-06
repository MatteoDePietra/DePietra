﻿using UnityEngine;
using UnityEngine.UI;

public class EnemyCounter : MonoBehaviour
{
    private Text text;
    internal static int enemyCount = 0;
    [SerializeField]
    internal static int minimum = 5;
    void Start()
    {
        text = GetComponent<Text> ();
    }
    void Update()
    {
        if (enemyCount>=minimum)
            text.color = Color.green;
        else 
            text.color = Color.red;
        text.text = enemyCount + "/" + minimum.ToString();
    }
    internal static void AddEnemy() {
        enemyCount += 1;
    } 
    internal static void CounterReset() {
        enemyCount = 0;
    }
}