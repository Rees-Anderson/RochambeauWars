using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 5.9.21
 * Game Design Project
 */

public class TankScript : MonoBehaviour
{
    public const int maxFuelLevel = 25;
    public const int maxAmmoCount = 10;
    public const int maxMovementPoints = 12;
    public const int maxHealth = 10;
    public const int minAttackRoll = 1; //Min damage before taking into account type match-up and terrain defense modifiers
    public const int maxAttackRoll = 5; //Max damage before taking into account type match-up and terrain defense modifiers
    public const int team = 1;

    //Fuel warnings, Ammo warnings, Health visualization are likely going to need variables and connected objects -- add those here

    public bool selected = false; //Determines if the unit is selected by the GameController - enables movement listening
    public bool active = false; //Determines if the unit is ready to be moved - the GameController can't select the unit if it is inactive
    public int fuelLevel; //The amount of fuel the unit has - how many turns it can move before needing to restock
    public int ammoCount; //The number of rounds the unit has - how many offensive attacks the unit can make before needing to restock
    public int movementPoints; //The amount of movement points this unit has - units pay with unit points when crossing terrain - replenished at start of turn
    public int health; //The amount of HP the unit has - at zero the unit dies
    public int currentDefenseModifier; //The defense modifier added to the unit due to the terrain it is on

    // Start is called before the first frame update
    void Start()
    {
        fuelLevel = maxFuelLevel;
        ammoCount = maxAmmoCount;
        movementPoints = maxMovementPoints;
        health = maxHealth;
        currentDefenseModifier = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (fuelLevel <= 10 && fuelLevel > 0)
        {
            //Display Low Fuel Warning Symbol
        }

        if (fuelLevel == 0)
        {
            //Display Out of Fuel Symbol
        }

        if (ammoCount <= 3)
        {
            //Display Low Ammo Warning Symbol
        }

        if (ammoCount == 0)
        {
            //Display Out of Ammo Symbol
        }

        //Constantly reposition fuel, ammo, and health indicators to follow the unit's position - check out making them children of the unit (not sure if this can be done with a prefab)

        if (selected && Input.GetKeyDown(KeyCode.W)) //Add controller support later
        {
            moveUp();
        }

        if (selected && Input.GetKeyDown(KeyCode.A)) //Add controller support later
        {
            moveLeft();
        }

        if (selected && Input.GetKeyDown(KeyCode.S)) //Add controller support later
        {
            moveDown();
        }

        if (selected && Input.GetKeyDown(KeyCode.D)) //Add controller support later
        {
            moveRight();
        }

    }

    //A note on movement: Units assume they start at a 0.5 increment added to or subtracted from their inital x and y position for grid movement to work properly.
    public void moveLeft()
    {
        //Get the tile to the left of this space.

        if (true) //Tile to the left is passable, unoccupied, has a movementCost less than movementPoints, and current x position > -6.5
        {
            //Play movement sound effect

            //Play leftward movement animation

            //Move Left by grid fixed amount
            Vector3 pos = transform.position;
            pos.x = pos.x - 1;
            transform.position = pos;

            //If team1 face right, if team2 face left

            //Decrease movementPoints by movementCost

            //Update currentDefenseModifier

            //Set current tile to unoccupied

            //Set new tile to occupied
        }
        else
        {
            //Play "can't move" sound effect

            //Play "can't move animation (a little shake perhaps)
        }
    }

    public void moveRight()
    {
        //Get the tile to the right of this space.

        if (true) //Tile to the right is passable, unoccupied, has a movementCost less than movementPoints, and current x position < 6.5
        {
            //Play movement sound effect

            //Play rightward movement animation

            //Move Right by grid fixed amount
            Vector3 pos = transform.position;
            pos.x = pos.x + 1;
            transform.position = pos;

            //If team1 face right, if team2 face left

            //Decrease movementPoints by movementCost

            //Update currentDefenseModifier

            //Set current tile to unoccupied

            //Set new tile to occupied
        }
        else
        {
            //Play "can't move" sound effect

            //Play "can't move animation (a little shake perhaps)
        }
    }

    public void moveUp()
    {
        //Get the tile above this space.

        if (true) //Tile above is passable, unoccupied, has a movementCost less than movementPoints, and current y position < 4.5
        {
            //Play movement sound effect

            //Play upward movement animation

            //Move Up by grid fixed amount
            Vector3 pos = transform.position;
            pos.y = pos.y + 1;
            transform.position = pos;

            //If team1 face right, if team2 face left

            //Decrease movementPoints by movementCost

            //Update currentDefenseModifier

            //Set current tile to unoccupied

            //Set new tile to occupied
        }
        else
        {
            //Play "can't move" sound effect

            //Play "can't move animation (a little shake perhaps)
        }
    }

    public void moveDown()
    {
        //Get the tile below this space.

        if (true) //Tile below is passable, unoccupied, has a movementCost less than movementPoints, and current y position > -4.5
        {
            //Play movement sound effect

            //Play downward movement animation

            //Move Down by grid fixed amount
            Vector3 pos = transform.position;
            pos.y = pos.y - 1;
            transform.position = pos;

            //If team1 face right, if team2 face left

            //Decrease movementPoints by movementCost

            //Update currentDefenseModifier

            //Set current tile to unoccupied

            //Set new tile to occupied
        }
        else
        {
            //Play "can't move" sound effect

            //Play "can't move animation (a little shake perhaps)
        }
    }

    public void wait()
    {
        //Set unit inactive
        active = false;

        //Dim Unit slightly

        //Replenish MovementPoints
        movementPoints = maxMovementPoints;

        //Reduce FuelLevel
        if (fuelLevel > 0)
        {
            fuelLevel--;
        }

        //If the current terrain tile is a city, replenish 2 health, all ammo, all fuel
        
    }

    public void fireWeaponOffensive()
    {
        //Play firing animation

        //Play firing sound effect

        //Reduce ammo by 1
        ammoCount--;
    }

    public void fireWeaponDefensive()
    {
        //Play firing animation

        //Play firing sound effect

    }

    public void takeDamage()
    {
        //Play damage animation

        //Play damage sound effect

        //Reduce health by 1
        health--;

        if (health == 0)
        {
            die();
        }
    }

    public void die()
    {
        //Play death animation

        //Play death sound effect

        //Delete Unit
        Destroy(gameObject);
    }
}
