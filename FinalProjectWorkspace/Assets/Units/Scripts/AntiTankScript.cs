using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 6.8.21
 * Game Design Project
 */

public class AntiTankScript : MonoBehaviour
{
    public CentralGameLogic centralGameLogic;

    public Animator animator;

    public int maxFuelLevel = 35;
    public int maxAmmoCount = 10;
    public int maxMovementPoints = 4;
    public int maxHealth = 10;
    public int attackRoll = 5; //Damage dealt before taking into account type match-up and terrain defense modifiers
    //public const int team = 1;

    public Sprite[] healthSprites;
    public Sprite[] ammoSprites;
    public Sprite[] fuelSprites;
    public Sprite[] appearanceSprites;

    public GenericDisappearReappearScript ammoDisappear;
    public GenericDisappearReappearScript fuelDisappear;
    public GenericDisappearReappearScript healthDisappear;

    public SpriteRenderer healthSprite;
    public SpriteRenderer ammoSprite;
    public SpriteRenderer fuelSprite;
    public SpriteRenderer infantrySprite;

    public RiverScript currentRiverTile; //null if not on a river tile
    public GrassScript currentGrassTile; //null if not on a grass tile
    public ForestScript currentForestTile; //null if not on a forest tile
    public SmallMountainScript currentSmallMountainTile; //null if not on a small mountain tile
    public LargeMountainScript currentLargeMountainTile; //null if not on a large mountain tile
    public CityScript currentCityTile; //null if not on a city tile
    public HeadQuartersScript currentHeadQuartersTile; //null if not on a headquarters tile

    public RiverScript nextRiverTile; //null if not on a river tile
    public GrassScript nextGrassTile; //null if not on a grass tile
    public ForestScript nextForestTile; //null if not on a forest tile
    public SmallMountainScript nextSmallMountainTile; //null if not on a small mountain tile
    public LargeMountainScript nextLargeMountainTile; //null if not on a large mountain tile
    public CityScript nextCityTile; //null if not on a city tile
    public HeadQuartersScript nextHeadQuartersTile; //null if not on a headquarters tile

    public bool selected = false; //Determines if the unit is selected by the GameController - enables movement listening
    public bool active = true; //Determines if the unit is ready to be moved - the GameController can't select the unit if it is inactive
    public int fuelLevel; //The amount of fuel the unit has - how many turns it can move before needing to restock
    public int ammoCount; //The number of rounds the unit has - how many offensive attacks the unit can make before needing to restock
    public int movementPoints; //The amount of movement points this unit has - units pay with unit points when crossing terrain - replenished at start of turn
    public int health; //The amount of HP the unit has - at zero the unit dies
    public int currentDefenseModifier; //The defense modifier added to the unit due to the terrain it is on

    public bool alive = true;
    public Vector3 graveSite = new Vector3(-8.5f, -4.5f, 0);

    private float r;
    private float g;
    private float b;
    private float defaultAlpha;

    private IEnumerator coroutine;

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Renderer>().material.color.r;
        g = GetComponent<Renderer>().material.color.g;
        b = GetComponent<Renderer>().material.color.b;
        defaultAlpha = GetComponent<Renderer>().material.color.a;

        fuelLevel = maxFuelLevel;
        ammoCount = maxAmmoCount;
        movementPoints = maxMovementPoints;
        health = maxHealth;
        currentDefenseModifier = 0;

        storeTileAtCurrentPosition();

        animatorToIdleActive();
    }

    // Update is called once per frame
    void Update()
    {
        healthSprite.sprite = healthSprites[health];

        if (fuelLevel > 10)
        {
            fuelDisappear.dissappear();
        }

        if (fuelLevel <= 7 && fuelLevel > 0 && alive)
        {
            //Display Low Fuel Warning Symbol
            fuelSprite.sprite = fuelSprites[0];
            fuelDisappear.reappear();
        }

        if (fuelLevel == 0 && alive)
        {
            //Display Out of Fuel Symbol
            fuelSprite.sprite = fuelSprites[1];
            fuelDisappear.reappear();
        }

        if (ammoCount > 3)
        {
            ammoDisappear.dissappear();
        }

        if (ammoCount <= 3 && alive)
        {
            //Display Low Ammo Warning Symbol
            ammoSprite.sprite = ammoSprites[0];
            ammoDisappear.reappear();
        }

        if (ammoCount == 0 && alive)
        {
            //Display Out of Ammo Symbol
            ammoSprite.sprite = ammoSprites[1];
            ammoDisappear.reappear();
        }

        /*
        if (!active)
        {
            //Be grey
            infantrySprite.sprite = appearanceSprites[3];
        }
        else
        {
            //Be team color
            infantrySprite.sprite = appearanceSprites[0];
        }
        */

        if (selected && Input.GetKeyDown(KeyCode.W)) //Add controller support later
        {
            animatorToRunningUp();
            moveUp();
        }

        if (selected && Input.GetKeyDown(KeyCode.A)) //Add controller support later
        {
            animatorToRunningLeft();
            moveLeft();
        }

        if (selected && Input.GetKeyDown(KeyCode.S)) //Add controller support later
        {
            animatorToRunningDown();
            moveDown();
        }

        if (selected && Input.GetKeyDown(KeyCode.D)) //Add controller support later
        {
            animatorToRunningRight();
            moveRight();
        }

    }

    public void animatorToIdleActive()
    {
        animator.SetBool("isIdleActive", true);
        animator.SetBool("isIdleGrey", false);
        animator.SetBool("isRunningUp", false);
        animator.SetBool("isRunningDown", false);
        animator.SetBool("isRunningLeft", false);
        animator.SetBool("isRunningRight", false);
        animator.SetBool("isDying", false);
        animator.SetBool("isFiringRight", false);
        animator.SetBool("isFiringLeft", false);
        animator.SetBool("isFiringUp", false);
        animator.SetBool("isFiringDown", false);
    }

    public void animatorToIdleGrey()
    {
        animator.SetBool("isIdleActive", false);
        animator.SetBool("isIdleGrey", true);
        animator.SetBool("isRunningUp", false);
        animator.SetBool("isRunningDown", false);
        animator.SetBool("isRunningLeft", false);
        animator.SetBool("isRunningRight", false);
        animator.SetBool("isDying", false);
        animator.SetBool("isFiringRight", false);
        animator.SetBool("isFiringLeft", false);
        animator.SetBool("isFiringUp", false);
        animator.SetBool("isFiringDown", false);
    }

    public void animatorToRunningUp()
    {
        animator.SetBool("isIdleActive", false);
        animator.SetBool("isIdleGrey", false);
        animator.SetBool("isRunningUp", true);
        animator.SetBool("isRunningDown", false);
        animator.SetBool("isRunningLeft", false);
        animator.SetBool("isRunningRight", false);
        animator.SetBool("isDying", false);
        animator.SetBool("isFiringRight", false);
        animator.SetBool("isFiringLeft", false);
        animator.SetBool("isFiringUp", false);
        animator.SetBool("isFiringDown", false);
    }

    public void animatorToRunningDown()
    {
        animator.SetBool("isIdleActive", false);
        animator.SetBool("isIdleGrey", false);
        animator.SetBool("isRunningUp", false);
        animator.SetBool("isRunningDown", true);
        animator.SetBool("isRunningLeft", false);
        animator.SetBool("isRunningRight", false);
        animator.SetBool("isDying", false);
        animator.SetBool("isFiringRight", false);
        animator.SetBool("isFiringLeft", false);
        animator.SetBool("isFiringUp", false);
        animator.SetBool("isFiringDown", false);
    }

    public void animatorToRunningLeft()
    {
        animator.SetBool("isIdleActive", false);
        animator.SetBool("isIdleGrey", false);
        animator.SetBool("isRunningUp", false);
        animator.SetBool("isRunningDown", false);
        animator.SetBool("isRunningLeft", true);
        animator.SetBool("isRunningRight", false);
        animator.SetBool("isDying", false);
        animator.SetBool("isFiringRight", false);
        animator.SetBool("isFiringLeft", false);
        animator.SetBool("isFiringUp", false);
        animator.SetBool("isFiringDown", false);
    }

    public void animatorToRunningRight()
    {
        animator.SetBool("isIdleActive", false);
        animator.SetBool("isIdleGrey", false);
        animator.SetBool("isRunningUp", false);
        animator.SetBool("isRunningDown", false);
        animator.SetBool("isRunningLeft", false);
        animator.SetBool("isRunningRight", true);
        animator.SetBool("isDying", false);
        animator.SetBool("isFiringRight", false);
        animator.SetBool("isFiringLeft", false);
        animator.SetBool("isFiringUp", false);
        animator.SetBool("isFiringDown", false);
    }

    public void animatorToDeath()
    {
        animator.SetBool("isIdleActive", false);
        animator.SetBool("isIdleGrey", false);
        animator.SetBool("isRunningUp", false);
        animator.SetBool("isRunningDown", false);
        animator.SetBool("isRunningLeft", false);
        animator.SetBool("isRunningRight", false);
        animator.SetBool("isDying", true);
        animator.SetBool("isFiringRight", false);
        animator.SetBool("isFiringLeft", false);
        animator.SetBool("isFiringUp", false);
        animator.SetBool("isFiringDown", false);
    }

    public void animatorToFiringRight()
    {
        animator.SetBool("isIdleActive", false);
        animator.SetBool("isIdleGrey", false);
        animator.SetBool("isRunningUp", false);
        animator.SetBool("isRunningDown", false);
        animator.SetBool("isRunningLeft", false);
        animator.SetBool("isRunningRight", false);
        animator.SetBool("isDying", false);
        animator.SetBool("isFiringRight", true);
        animator.SetBool("isFiringLeft", false);
        animator.SetBool("isFiringUp", false);
        animator.SetBool("isFiringDown", false);
    }

    public void animatorToFiringLeft()
    {
        animator.SetBool("isIdleActive", false);
        animator.SetBool("isIdleGrey", false);
        animator.SetBool("isRunningUp", false);
        animator.SetBool("isRunningDown", false);
        animator.SetBool("isRunningLeft", false);
        animator.SetBool("isRunningRight", false);
        animator.SetBool("isDying", false);
        animator.SetBool("isFiringRight", false);
        animator.SetBool("isFiringLeft", true);
        animator.SetBool("isFiringUp", false);
        animator.SetBool("isFiringDown", false);
    }

    public void animatorToFiringUp()
    {
        animator.SetBool("isIdleActive", false);
        animator.SetBool("isIdleGrey", false);
        animator.SetBool("isRunningUp", false);
        animator.SetBool("isRunningDown", false);
        animator.SetBool("isRunningLeft", false);
        animator.SetBool("isRunningRight", false);
        animator.SetBool("isDying", false);
        animator.SetBool("isFiringRight", false);
        animator.SetBool("isFiringLeft", false);
        animator.SetBool("isFiringUp", true);
        animator.SetBool("isFiringDown", false);
    }

    public void animatorToFiringDown()
    {
        animator.SetBool("isIdleActive", false);
        animator.SetBool("isIdleGrey", false);
        animator.SetBool("isRunningUp", false);
        animator.SetBool("isRunningDown", false);
        animator.SetBool("isRunningLeft", false);
        animator.SetBool("isRunningRight", false);
        animator.SetBool("isDying", false);
        animator.SetBool("isFiringRight", false);
        animator.SetBool("isFiringLeft", false);
        animator.SetBool("isFiringUp", false);
        animator.SetBool("isFiringDown", true);
    }

    public void startRunningBecauseSelected()
    {
        if (tag == "Red")
        {
            animatorToRunningRight();
        }
        else
        {
            animatorToRunningLeft();
        }
    }

    public void readyUnitForNewTurn()
    {
        if (alive)
        {
            active = true;

            animatorToIdleActive();
        }

    }

    //A note on movement: Units assume they start at a 0.5 increment added to or subtracted from their inital x and y position for grid movement to work properly.
    public void moveLeft()
    {
        //Get the tile to the left of this space.
        Vector3 targetPosition = transform.position;
        targetPosition.x -= 1f;
        targetPosition.y -= 0.1f;
        targetPosition.z = 0f;

        storeTileAtNextRequestedPosition(targetPosition);

        if (nextRiverTile != null)
        {
            if ((!nextRiverTile.occupied) && (movementPoints >= nextRiverTile.movementCost) && (transform.position.x > -6.5))
            {
                //Play movement sound effect

                //Play leftward movement animation

                //Move Left by grid fixed amount
                Vector3 pos = transform.position;
                pos.x = pos.x - 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextRiverTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextRiverTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextRiverTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextGrassTile != null)
        {
            if ((!nextGrassTile.occupied) && (movementPoints >= nextGrassTile.movementCost) && (transform.position.x > -6.5))
            {
                //Play movement sound effect

                //Play leftward movement animation

                //Move Left by grid fixed amount
                Vector3 pos = transform.position;
                pos.x = pos.x - 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextGrassTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextGrassTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextGrassTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextForestTile != null)
        {
            if ((!nextForestTile.occupied) && (movementPoints >= nextForestTile.movementCost) && (transform.position.x > -6.5))
            {
                //Play movement sound effect

                //Play leftward movement animation

                //Move Left by grid fixed amount
                Vector3 pos = transform.position;
                pos.x = pos.x - 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextForestTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextForestTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextForestTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextSmallMountainTile != null)
        {
            if ((!nextSmallMountainTile.occupied) && (movementPoints >= nextSmallMountainTile.movementCost) && (transform.position.x > -6.5))
            {
                //Play movement sound effect

                //Play leftward movement animation

                //Move Left by grid fixed amount
                Vector3 pos = transform.position;
                pos.x = pos.x - 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextSmallMountainTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextSmallMountainTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextSmallMountainTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextLargeMountainTile != null)
        {
            if ((!nextLargeMountainTile.occupied) && (movementPoints >= nextLargeMountainTile.movementCost) && (transform.position.x > -6.5))
            {
                //Play movement sound effect

                //Play leftward movement animation

                //Move Left by grid fixed amount
                Vector3 pos = transform.position;
                pos.x = pos.x - 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextLargeMountainTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextLargeMountainTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextLargeMountainTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextCityTile != null)
        {
            if ((!nextCityTile.occupied) && (movementPoints >= nextCityTile.movementCost) && (transform.position.x > -6.5))
            {
                //Play movement sound effect

                //Play leftward movement animation

                //Move Left by grid fixed amount
                Vector3 pos = transform.position;
                pos.x = pos.x - 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextCityTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextCityTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextCityTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextHeadQuartersTile != null)
        {
            if ((!nextHeadQuartersTile.occupied) && (movementPoints >= nextHeadQuartersTile.movementCost) && (transform.position.x > -6.5))
            {
                //Play movement sound effect

                //Play leftward movement animation

                //Move Left by grid fixed amount
                Vector3 pos = transform.position;
                pos.x = pos.x - 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextHeadQuartersTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextHeadQuartersTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextHeadQuartersTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else
        {
            //Can't move that way - do nothing
        }

    }

    public void moveRight()
    {
        //Get the tile to the right of this space.
        Vector3 targetPosition = transform.position;
        targetPosition.x += 1f;
        targetPosition.y -= 0.1f;
        targetPosition.z = 0f;

        storeTileAtNextRequestedPosition(targetPosition);

        if (nextRiverTile != null)
        {
            if ((!nextRiverTile.occupied) && (movementPoints >= nextRiverTile.movementCost) && (transform.position.x < 7.5))
            {
                //Play movement sound effect

                //Play rightward movement animation

                //Move Right by grid fixed amount
                Vector3 pos = transform.position;
                pos.x = pos.x + 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextRiverTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextRiverTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextRiverTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextGrassTile != null)
        {
            if ((!nextGrassTile.occupied) && (movementPoints >= nextGrassTile.movementCost) && (transform.position.x < 7.5))
            {
                //Play movement sound effect

                //Play rightward movement animation

                //Move Right by grid fixed amount
                Vector3 pos = transform.position;
                pos.x = pos.x + 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextGrassTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextGrassTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextGrassTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextForestTile != null)
        {
            if ((!nextForestTile.occupied) && (movementPoints >= nextForestTile.movementCost) && (transform.position.x < 7.5))
            {
                //Play movement sound effect

                //Play rightward movement animation

                //Move Right by grid fixed amount
                Vector3 pos = transform.position;
                pos.x = pos.x + 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextForestTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextForestTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextForestTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextSmallMountainTile != null)
        {
            if ((!nextSmallMountainTile.occupied) && (movementPoints >= nextSmallMountainTile.movementCost) && (transform.position.x < 7.5))
            {
                //Play movement sound effect

                //Play rightward movement animation

                //Move Right by grid fixed amount
                Vector3 pos = transform.position;
                pos.x = pos.x + 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextSmallMountainTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextSmallMountainTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextSmallMountainTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextLargeMountainTile != null)
        {
            if ((!nextLargeMountainTile.occupied) && (movementPoints >= nextLargeMountainTile.movementCost) && (transform.position.x < 7.5))
            {
                //Play movement sound effect

                //Play rightward movement animation

                //Move Right by grid fixed amount
                Vector3 pos = transform.position;
                pos.x = pos.x + 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextLargeMountainTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextLargeMountainTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextLargeMountainTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextCityTile != null)
        {
            if ((!nextCityTile.occupied) && (movementPoints >= nextCityTile.movementCost) && (transform.position.x < 7.5))
            {
                //Play movement sound effect

                //Play rightward movement animation

                //Move Right by grid fixed amount
                Vector3 pos = transform.position;
                pos.x = pos.x + 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextCityTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextCityTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextCityTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextHeadQuartersTile != null)
        {
            if ((!nextHeadQuartersTile.occupied) && (movementPoints >= nextHeadQuartersTile.movementCost) && (transform.position.x < 7.5))
            {
                //Play movement sound effect

                //Play rightward movement animation

                //Move Right by grid fixed amount
                Vector3 pos = transform.position;
                pos.x = pos.x + 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextHeadQuartersTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextHeadQuartersTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextHeadQuartersTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else
        {
            //Can't move that way - do nothing
        }

    }

    public void moveUp()
    {
        //Get the tile above this space.
        Vector3 targetPosition = transform.position;
        targetPosition.y -= 0.1f; //Visual offset
        targetPosition.y += 1f; //Physical offset
        targetPosition.z = 0f;

        storeTileAtNextRequestedPosition(targetPosition);

        if (nextRiverTile != null)
        {
            if ((!nextRiverTile.occupied) && (movementPoints >= nextRiverTile.movementCost) && (transform.position.y < 4.5))
            {
                //Play movement sound effect

                //Play upward movement animation

                //Move Up by grid fixed amount
                Vector3 pos = transform.position;
                pos.y = pos.y + 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextRiverTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextRiverTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextRiverTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextGrassTile != null)
        {
            if ((!nextGrassTile.occupied) && (movementPoints >= nextGrassTile.movementCost) && (transform.position.y < 4.5))
            {
                //Play movement sound effect

                //Play upward movement animation

                //Move Up by grid fixed amount
                Vector3 pos = transform.position;
                pos.y = pos.y + 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextGrassTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextGrassTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextGrassTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextForestTile != null)
        {
            if ((!nextForestTile.occupied) && (movementPoints >= nextForestTile.movementCost) && (transform.position.y < 4.5))
            {
                //Play movement sound effect

                //Play upward movement animation

                //Move Up by grid fixed amount
                Vector3 pos = transform.position;
                pos.y = pos.y + 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextForestTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextForestTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextForestTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextSmallMountainTile != null)
        {
            if ((!nextSmallMountainTile.occupied) && (movementPoints >= nextSmallMountainTile.movementCost) && (transform.position.y < 4.5))
            {
                //Play movement sound effect

                //Play upward movement animation

                //Move Up by grid fixed amount
                Vector3 pos = transform.position;
                pos.y = pos.y + 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextSmallMountainTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextSmallMountainTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextSmallMountainTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextLargeMountainTile != null)
        {
            if ((!nextLargeMountainTile.occupied) && (movementPoints >= nextLargeMountainTile.movementCost) && (transform.position.y < 4.5))
            {
                //Play movement sound effect

                //Play upward movement animation

                //Move Up by grid fixed amount
                Vector3 pos = transform.position;
                pos.y = pos.y + 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextLargeMountainTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextLargeMountainTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextLargeMountainTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextCityTile != null)
        {
            if ((!nextCityTile.occupied) && (movementPoints >= nextCityTile.movementCost) && (transform.position.y < 4.5))
            {
                //Play movement sound effect

                //Play upward movement animation

                //Move Up by grid fixed amount
                Vector3 pos = transform.position;
                pos.y = pos.y + 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextCityTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextCityTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextCityTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextHeadQuartersTile != null)
        {
            if ((!nextHeadQuartersTile.occupied) && (movementPoints >= nextHeadQuartersTile.movementCost) && (transform.position.y < 4.5))
            {
                //Play movement sound effect

                //Play upward movement animation

                //Move Up by grid fixed amount
                Vector3 pos = transform.position;
                pos.y = pos.y + 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextHeadQuartersTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextHeadQuartersTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextHeadQuartersTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else
        {
            //Can't move that way - do nothing
        }
    }

    public void moveDown()
    {
        //Get the tile below this space.
        Vector3 targetPosition = transform.position;
        targetPosition.y -= 0.1f; //Visual offset
        targetPosition.y -= 1f; //Physical offset
        targetPosition.z = 0f;

        storeTileAtNextRequestedPosition(targetPosition);

        if (nextRiverTile != null)
        {
            if ((!nextRiverTile.occupied) && (movementPoints >= nextRiverTile.movementCost) && (transform.position.y > -4.5))
            {
                //Play movement sound effect

                //Play downward movement animation

                //Move Down by grid fixed amount
                Vector3 pos = transform.position;
                pos.y = pos.y - 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextRiverTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextRiverTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextRiverTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextGrassTile != null)
        {
            if ((!nextGrassTile.occupied) && (movementPoints >= nextGrassTile.movementCost) && (transform.position.y > -4.5))
            {
                //Play movement sound effect

                //Play downward movement animation

                //Move Down by grid fixed amount
                Vector3 pos = transform.position;
                pos.y = pos.y - 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextGrassTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextGrassTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextGrassTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextForestTile != null)
        {
            if ((!nextForestTile.occupied) && (movementPoints >= nextForestTile.movementCost) && (transform.position.y > -4.5))
            {
                //Play movement sound effect

                //Play downward movement animation

                //Move Down by grid fixed amount
                Vector3 pos = transform.position;
                pos.y = pos.y - 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextForestTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextForestTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextForestTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextSmallMountainTile != null)
        {
            if ((!nextSmallMountainTile.occupied) && (movementPoints >= nextSmallMountainTile.movementCost) && (transform.position.y > -4.5))
            {
                //Play movement sound effect

                //Play downward movement animation

                //Move Down by grid fixed amount
                Vector3 pos = transform.position;
                pos.y = pos.y - 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextSmallMountainTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextSmallMountainTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextSmallMountainTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextLargeMountainTile != null)
        {
            if ((!nextLargeMountainTile.occupied) && (movementPoints >= nextLargeMountainTile.movementCost) && (transform.position.y > -4.5))
            {
                //Play movement sound effect

                //Play downward movement animation

                //Move Down by grid fixed amount
                Vector3 pos = transform.position;
                pos.y = pos.y - 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextLargeMountainTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextLargeMountainTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextLargeMountainTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextCityTile != null)
        {
            if ((!nextCityTile.occupied) && (movementPoints >= nextCityTile.movementCost) && (transform.position.y > -4.5))
            {
                //Play movement sound effect

                //Play downward movement animation

                //Move Down by grid fixed amount
                Vector3 pos = transform.position;
                pos.y = pos.y - 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextCityTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextCityTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextCityTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else if (nextHeadQuartersTile != null)
        {
            if ((!nextHeadQuartersTile.occupied) && (movementPoints >= nextHeadQuartersTile.movementCost) && (transform.position.y > -4.5))
            {
                //Play movement sound effect

                //Play downward movement animation

                //Move Down by grid fixed amount
                Vector3 pos = transform.position;
                pos.y = pos.y - 1;
                transform.position = pos;

                //Decrease movementPoints by movementCost
                movementPoints -= nextHeadQuartersTile.movementCost;

                //Update currentDefenseModifier
                currentDefenseModifier = nextHeadQuartersTile.defenseModifier;

                //Set current tile to unoccupied
                setCurrentTileToUnoccupied();

                //Set new tile to occupied
                nextHeadQuartersTile.occupied = true;

                //Make all next tiles the current tiles
                storeTileAtCurrentPosition();
            }
        }
        else
        {
            //Can't move that way - do nothing
        }
    }

    public void wait()
    {
        //Set unit inactive
        active = false;

        animatorToIdleGrey();

        //Replenish MovementPoints
        movementPoints = maxMovementPoints;

        //If the current terrain tile is an allied city or HQ, replenish 2 health, all ammo, all fuel + show suppling UI for a second
        if (currentCityTile != null && currentCityTile.tag == tag)
        {
            int temp = health;
            temp += 2;
            if (temp > 10)
            {
                health = 10;
            }
            else
            {
                health = temp;
            }

            ammoCount = maxAmmoCount;
            fuelLevel = maxFuelLevel;

            //Play Supplying UI for a second?

        }
        else if (currentHeadQuartersTile != null && currentHeadQuartersTile.tag == tag)
        {
            int temp = health;
            temp += 2;
            if (temp > 10)
            {
                health = 10;
            }
            else
            {
                health = temp;
            }

            ammoCount = maxAmmoCount;
            fuelLevel = maxFuelLevel;

            //Play Supplying UI for a second?
        }
        else
        {
            //Reduce FuelLevel and if out of fuel take damage
            if (fuelLevel > 0)
            {
                fuelLevel--;
            }
            else
            {
                //Reduce health by 1
                int temp = health;
                temp -= 1;
                if (temp < 0)
                {
                    health = 0;
                }
                else
                {
                    health = temp;
                }

                //If health has fallen too low, die
                if (health <= 0)
                {
                    die();
                }
            }
        }

    }

    public void fireWeaponOffensive(string direction)
    {
        //Play firing animation
        if (direction == "UP")
        {
            animatorToFiringUp();
        }
        else if (direction == "DOWN")
        {
            animatorToFiringDown();
        }
        else if (direction == "LEFT")
        {
            animatorToFiringLeft();
        }
        else if (direction == "RIGHT")
        {
            animatorToFiringRight();
        }

        //Play firing sound effect

        //Allow firing animation to play out, then finish this method in WaitToEndFireDefensive method
        coroutine = WaitToEndFireOffensive(1.0f);
        StartCoroutine(coroutine);
    }

    private IEnumerator WaitToEndFireOffensive(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        //Stop firing animation
        animatorToIdleActive();

        //Reduce ammo by 1
        if (ammoCount > 0)
        {
            ammoCount--;
        }
        else
        {
            Debug.Log("Error: A Unit Just Attacked When Out of Ammo");
        }

        wait();
    }

    public void fireWeaponDefensive(string direction)
    {
        //Play firing animation
        if (direction == "UP")
        {
            animatorToFiringUp();
        }
        else if (direction == "DOWN")
        {
            animatorToFiringDown();
        }
        else if (direction == "LEFT")
        {
            animatorToFiringLeft();
        }
        else if (direction == "RIGHT")
        {
            animatorToFiringRight();
        }

        //Play firing sound effect

        //Allow firing animation to play out, then finish this method in WaitToEndFireDefensive method
        coroutine = WaitToEndFireDefensive(1.0f);
        StartCoroutine(coroutine);
    }

    private IEnumerator WaitToEndFireDefensive(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        //Stop firing animation
        animatorToIdleActive();
    }

    public void takeDamage(int dmg)
    {
        if (dmg <= 0)
        {
            dmg = 1; //Maybe make there be a minimum damage dealt at 1 for balancing - that would be done here
        }

        //Reduce health by dmg amount
        int temp = health;
        temp -= dmg;
        if (temp < 0)
        {
            health = 0;
        }
        else
        {
            health = temp;
        }

        //If health has fallen too low, die
        if (health <= 0)
        {
            die();
        }

    }

    public void die()
    {
        //Swap unit to alive == false before making UI dissapear so that Update doesn't just make it reappear
        alive = false;

        //Make unit's ui stuff go away
        healthDisappear.dissappear();
        fuelDisappear.dissappear();
        ammoDisappear.dissappear();

        //Play death animation
        animatorToDeath();

        //Play death sound effect

        //Wait until animation and sound effect are done, then finish die() in Wait method
        coroutine = WaitForDeath(1.0f);
        StartCoroutine(coroutine);
    }

    private IEnumerator WaitForDeath(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        //Unit becomes invisible
        dissappear();

        //Unit dies
        alive = false;
        active = false;
        setCurrentTileToUnoccupied();

        transform.position = graveSite;

    }

    public void storeTileAtCurrentPosition()
    {
        currentRiverTile = null;
        currentGrassTile = null;
        currentForestTile = null;
        currentSmallMountainTile = null;
        currentLargeMountainTile = null;
        currentCityTile = null;
        currentHeadQuartersTile = null;

        Vector3 target = transform.position;
        target.y -= 0.1f;

        for (int i = 0; i < centralGameLogic.riverTiles.Length; i++)
        {
            if (centralGameLogic.riverTiles[i].transform.position == target)
            {
                currentRiverTile = centralGameLogic.riverTiles[i];
            }
        }

        if (currentRiverTile != null)
        {
            return;
        }

        for (int i = 0; i < centralGameLogic.grassTiles.Length; i++)
        {
            if (centralGameLogic.grassTiles[i].transform.position == target)
            {
                currentGrassTile = centralGameLogic.grassTiles[i];
            }
        }

        if (currentGrassTile != null)
        {
            return;
        }

        for (int i = 0; i < centralGameLogic.forestTiles.Length; i++)
        {
            if (centralGameLogic.forestTiles[i].transform.position == target)
            {
                currentForestTile = centralGameLogic.forestTiles[i];
            }
        }

        if (currentForestTile != null)
        {
            return;
        }

        for (int i = 0; i < centralGameLogic.smallMountainTiles.Length; i++)
        {
            if (centralGameLogic.smallMountainTiles[i].transform.position == target)
            {
                currentSmallMountainTile = centralGameLogic.smallMountainTiles[i];
            }
        }

        if (currentSmallMountainTile != null)
        {
            return;
        }

        for (int i = 0; i < centralGameLogic.largeMountainTiles.Length; i++)
        {
            if (centralGameLogic.largeMountainTiles[i].transform.position == target)
            {
                currentLargeMountainTile = centralGameLogic.largeMountainTiles[i];
            }
        }

        if (currentLargeMountainTile != null)
        {
            return;
        }

        for (int i = 0; i < centralGameLogic.cityTiles.Length; i++)
        {
            if (centralGameLogic.cityTiles[i].transform.position == target)
            {
                currentCityTile = centralGameLogic.cityTiles[i];
            }
        }

        if (currentCityTile != null)
        {
            return;
        }

        for (int i = 0; i < centralGameLogic.headQuartersTiles.Length; i++)
        {
            if (centralGameLogic.headQuartersTiles[i].transform.position == target)
            {
                currentHeadQuartersTile = centralGameLogic.headQuartersTiles[i];
            }
        }

        if (currentHeadQuartersTile != null)
        {
            return;
        }
    }

    public void storeTileAtNextRequestedPosition(Vector3 target)
    {
        nextRiverTile = null;
        nextGrassTile = null;
        nextForestTile = null;
        nextSmallMountainTile = null;
        nextLargeMountainTile = null;
        nextCityTile = null;
        nextHeadQuartersTile = null;

        for (int i = 0; i < centralGameLogic.riverTiles.Length; i++)
        {
            if (centralGameLogic.riverTiles[i].transform.position == target)
            {
                nextRiverTile = centralGameLogic.riverTiles[i];
            }
        }

        if (nextRiverTile != null)
        {
            return;
        }

        for (int i = 0; i < centralGameLogic.grassTiles.Length; i++)
        {
            if (centralGameLogic.grassTiles[i].transform.position == target)
            {
                nextGrassTile = centralGameLogic.grassTiles[i];
            }
        }

        if (nextGrassTile != null)
        {
            return;
        }

        for (int i = 0; i < centralGameLogic.forestTiles.Length; i++)
        {
            if (centralGameLogic.forestTiles[i].transform.position == target)
            {
                nextForestTile = centralGameLogic.forestTiles[i];
            }
        }

        if (nextForestTile != null)
        {
            return;
        }

        for (int i = 0; i < centralGameLogic.smallMountainTiles.Length; i++)
        {
            if (centralGameLogic.smallMountainTiles[i].transform.position == target)
            {
                nextSmallMountainTile = centralGameLogic.smallMountainTiles[i];
            }
        }

        if (nextSmallMountainTile != null)
        {
            return;
        }

        for (int i = 0; i < centralGameLogic.largeMountainTiles.Length; i++)
        {
            if (centralGameLogic.largeMountainTiles[i].transform.position == target)
            {
                nextLargeMountainTile = centralGameLogic.largeMountainTiles[i];
            }
        }

        if (nextLargeMountainTile != null)
        {
            return;
        }

        for (int i = 0; i < centralGameLogic.cityTiles.Length; i++)
        {
            if (centralGameLogic.cityTiles[i].transform.position == target)
            {
                nextCityTile = centralGameLogic.cityTiles[i];
            }
        }

        if (nextCityTile != null)
        {
            return;
        }

        for (int i = 0; i < centralGameLogic.headQuartersTiles.Length; i++)
        {
            if (centralGameLogic.headQuartersTiles[i].transform.position == target)
            {
                nextHeadQuartersTile = centralGameLogic.headQuartersTiles[i];
            }
        }

        if (nextHeadQuartersTile != null)
        {
            return;
        }
    }

    public void dissappear()
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, 0);
        healthDisappear.dissappear();
        fuelDisappear.dissappear();
        ammoDisappear.dissappear();
    }

    public void reappear()
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, defaultAlpha);
        healthDisappear.reappear();
        fuelDisappear.reappear();
        ammoDisappear.reappear();
    }

    public void setCurrentTileToUnoccupied()
    {
        if (currentRiverTile != null)
        {
            currentRiverTile.occupied = false;
        }
        else if (currentGrassTile != null)
        {
            currentGrassTile.occupied = false;
        }
        else if (currentForestTile != null)
        {
            currentForestTile.occupied = false;
        }
        else if (currentSmallMountainTile != null)
        {
            currentSmallMountainTile.occupied = false;
        }
        else if (currentLargeMountainTile != null)
        {
            currentLargeMountainTile.occupied = false;
        }
        else if (currentCityTile != null)
        {
            currentCityTile.occupied = false;
        }
        else if (currentHeadQuartersTile != null)
        {
            currentHeadQuartersTile.occupied = false;
        }
    }

    public void setCurrentTileToOccupied()
    {
        if (currentRiverTile != null)
        {
            currentRiverTile.occupied = true;
        }
        else if (currentGrassTile != null)
        {
            currentGrassTile.occupied = true;
        }
        else if (currentForestTile != null)
        {
            currentForestTile.occupied = true;
        }
        else if (currentSmallMountainTile != null)
        {
            currentSmallMountainTile.occupied = true;
        }
        else if (currentLargeMountainTile != null)
        {
            currentLargeMountainTile.occupied = true;
        }
        else if (currentCityTile != null)
        {
            currentCityTile.occupied = true;
        }
        else if (currentHeadQuartersTile != null)
        {
            currentHeadQuartersTile.occupied = true;
        }
    }

    public void setDefenseModifierToCurrentTileValue()
    {
        if (currentRiverTile != null)
        {
            currentDefenseModifier = currentRiverTile.defenseModifier;
        }
        else if (currentGrassTile != null)
        {
            currentDefenseModifier = currentGrassTile.defenseModifier;
        }
        else if (currentForestTile != null)
        {
            currentDefenseModifier = currentForestTile.defenseModifier;
        }
        else if (currentSmallMountainTile != null)
        {
            currentDefenseModifier = currentSmallMountainTile.defenseModifier;
        }
        else if (currentLargeMountainTile != null)
        {
            currentDefenseModifier = currentLargeMountainTile.defenseModifier;
        }
        else if (currentCityTile != null)
        {
            currentDefenseModifier = currentCityTile.defenseModifier;
        }
        else if (currentHeadQuartersTile != null)
        {
            currentDefenseModifier = currentHeadQuartersTile.defenseModifier;
        }
    }
}
