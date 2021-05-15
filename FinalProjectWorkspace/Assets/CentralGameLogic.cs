using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 5.14.21
 * Game Design Project
 */

public class CentralGameLogic : MonoBehaviour
{
    public GameObject cursor;
    public string state;
    public string currentPlayer = "Blue";

    public RiverScript[] riverTiles;
    public GrassScript[] grassTiles;
    public ForestScript[] forestTiles;
    public SmallMountainScript[] smallMountainTiles;
    public LargeMountainScript[] largeMountainTiles;
    public CityScript[] cityTiles;
    public HeadQuartersScript[] headQuartersTiles;

    public TankScript[] blueTanks;
    public InfantryScript[] blueInfantry;
    public AntiTankScript[] blueAntiTanks;

    public TankScript[] redTanks;
    public InfantryScript[] redInfantry;
    public AntiTankScript[] redAntiTanks;

    // Start is called before the first frame update
    void Start()
    {
        state = "default";
    }

    // Update is called once per frame
    void Update()
    {
        if (state == "default")
        {
            //If Cursor is on right side of screen have terrain/unit UI move to left side, else terrain/unit UI is on the right

            //If hovering over an occupied tile show the details of the unit and tile in the UI, else only show tile info

            //Show what day it is and whose turn in the top right

            //Can move cursor around with WASD (Control Stick on controller)

            //Hitting E (A on controller)
                //On unoccupied tile sends controller into selectedUnoccupied state
                //On occupied tile with unit of same color sends controller into selectedUnit state
                //On occupied tile with unit of different color plays an error sound

            //Hitting F does nothing

        } else if (state == "selectedUnoccupied")
        {
            //Hides Cursor and defaultUI

            //Pulls up a menu in the top right where you can manually end your turn (Pos 1), restart (Pos 2), or return to main menu (Pos 3)

            //Pressing W (Up on controller) when at Pos 1 does nothing, at Pos 2 moves hand to Pos 1, at Pos 3 moves hand to Pos 2

            //Pressing S (Down on controller) when at Pos 1 moves to Pos 2, Pos 2 moves to Pos 3, Pos 3 does nothing

            //Pressing E (A on controller)
                //When at Pos 1 - Ends your turn - swaps current player to Red if currently Blue, and vice versa
                //When at Pos 2 - Restarts Map
                //When at Pos 3 - Returns to Main Menu

            //Pressing F (B on controller)
                //Hides Menu in top right
                //Returns state to default

        } else if (state == "selectedUnit")
        {
            //Hide defaultUI

            //Find Selected Unit and set as selected

            //Store unit's original position

            //In the top left show selected unit's movement points remaining

            //Show terrain movement costs on all tiles

            //Pressing W (Up on controller) - calls the selected unit's and cursor's move up method

            //Pressing A (Left on controller) - calls the selected unit's and cursor's move left method

            //Pressing S (Down on controller) - calls the selected unit's and cursor's move down method

            //Pressing D (Right on controller) - calls the selected unit's and cursor's move right method

            //Pressing E (A on controller)
                //Hide terrain movement costs
                //Hide top left movement points window
                //Send into determineStrikeCapability state

            //Pressing F (B on controller)
                //Return unit to original position
                //Restore unit's max movement points
                //Set unit as not selected
                //Hide terrain movement costs
                //Hide top left movement points window
                //Unhide defaultUI
                //Return state to default


        } else if (state == "determineStrikeCapability")
        {
            //Find and store all valid targets in a list

            //If valid targets is empty
                //Send into wait only state

            //Else
                //Send into attackOrWait state

        } else if (state == "onlyWait")
        {
            //Draw wait only window in the top right

            //Pressing E (A on controller)
                //Set Unit as not selected
                //Call unit's wait method
                //Stop drawing wait only window
                //Return to default state

        } else if (state == "attackOrWait")
        {
            //Draw attack or wait window in the top right - attack Pos 1, wait Pos 2

            //Pressing W (Up on controller) when at Pos 1 does nothing, at Pos 2 moves hand to Pos 1

            //Pressing S (Down on controller) when at Pos 1 moves to Pos 2, Pos 2 does nothing

            //Pressing E (A on controller)
                //When at Pos 1 - attack state
                    //Set Unit as not selected
                    //Stop drawing attack or wait window
                    //Enter attack state
                //When at Pos 2 - wait
                    //Set Unit as not selected
                    //Call unit's wait method
                    //Stop drawing attack or wait window
                    //Return to default state
        } else if (state == "attack")
        {
            //Find and store all valid targets in a list
            //Change Cursor to a crosshair
            //Snap cursor to the position of the first target
            //Start drawing terrain and unit UI
            //int indexToLookAt = 0;

            //Calculate damage to be done to current target and show estimated damage above the terrain/unit UI

            //Pressing A (Left on controller) - cycles to the previous unit (cycles around if at 0 index), move cursor, redo calculations

            //Pressing D (Right on controller) - cycles to the next unit (cycles around if at end index), move cursor, redo calculations

            //Pressing E (A on controller)
                //Do damage to both units
                //Stop drawing current UI stuff
                //Return to default state

        }
    }
}
