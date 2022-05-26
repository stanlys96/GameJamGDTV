using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyHealthDisplay : MonoBehaviour
{
    private Enemy enemy;
    public RectTransform healthUI;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        healthUI.localScale = new Vector3((float)enemy.hitpoint / (float)enemy.maxHitpoint, 1, 1);
    }
}
