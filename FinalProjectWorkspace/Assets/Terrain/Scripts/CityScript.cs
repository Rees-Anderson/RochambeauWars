using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 5.9.21
 * Game Design Project
 */

public class CityScript : MonoBehaviour
{
    public const int movementCost = 1;
    public const int defenseModifier = 4;
    public const bool passable = true;
    public const bool capturable = true;
    public const int maxHealth = 20;

    public int currentHealth;
    public int team = 1;

    public bool occupied = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void cityUnderAttack(int attackerHealth)
    {
        //Play audio of city being attacked

        currentHealth = currentHealth - attackerHealth;
        if (currentHealth <= 0)
        {
            //Swap team owner

            //Change color of city

            //Reset health
            currentHealth = maxHealth;
        }
    }
}
