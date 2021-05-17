using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Author: Rees Anderson
 * 5.16.21
 * Game Design Project
 */

public class CentralGameLogic : MonoBehaviour
{
    public CursorScript cursor;
    public string state;
    public string currentPlayer = "Blue";
    public int day = 1;

    public TerrainUIScript terrainUI;
    public TurnUIScript turnUI;
    public UnitUIScript unitUI;
    public EndTurnUIScript endTurnUI;
    public MovRemUI movementRemainingUI;
    public WaitMenuScript waitMenuUI;

    public RiverScript currentRiverTile; //null if not on a river tile
    public GrassScript currentGrassTile; //null if not on a grass tile
    public ForestScript currentForestTile; //null if not on a forest tile
    public SmallMountainScript currentSmallMountainTile; //null if not on a small mountain tile
    public LargeMountainScript currentLargeMountainTile; //null if not on a large mountain tile
    public CityScript currentCityTile; //null if not on a city tile
    public HeadQuartersScript currentHeadQuartersTile; //null if not on a headquarters tile

    public InfantryScript currentInfantry; //null if not on an infantry unit
    public AntiTankScript currentAntiTank; //null if not on an AT unit
    public TankScript currentTank; //null if not on a tank unit

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
        storeTileAtCursorPosition();
        storeUnitAtCursorPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F4)) //Add controller support later
        {
            Application.Quit();
        }

        if (state == "default")
        {
            //Hide all menus but the turn counter, terrain UI, and unit UI
            endTurnUI.dissappear();
            movementRemainingUI.dissappear();
            waitMenuUI.dissappear();

            //Make the default menus reappear
            turnUI.reappear();
            unitUI.allowedToReappear = true;
            unitUI.reappear();
            terrainUI.reappear();
            cursor.reappear();

            //Can move cursor around with WASD (Control Stick on controller)
            if (Input.GetKeyDown(KeyCode.W)) //Add controller support later
            {
                cursor.moveUp();
                storeTileAtCursorPosition();
                storeUnitAtCursorPosition();
            }

            if (Input.GetKeyDown(KeyCode.A)) //Add controller support later
            {
                cursor.moveLeft();
                storeTileAtCursorPosition();
                storeUnitAtCursorPosition();
            }

            if (Input.GetKeyDown(KeyCode.S)) //Add controller support later
            {
                cursor.moveDown();
                storeTileAtCursorPosition();
                storeUnitAtCursorPosition();
            }

            if (Input.GetKeyDown(KeyCode.D)) //Add controller support later
            {
                cursor.moveRight();
                storeTileAtCursorPosition();
                storeUnitAtCursorPosition();
            }

            //Hitting E (A on controller)
            //On unoccupied tile sends controller into selectedUnoccupied state
            //On occupied tile with unit of same color sends controller into selectedUnit state
            //On occupied tile with unit of different color plays an error sound
            if (Input.GetKeyDown(KeyCode.E)) //Add controller support later
            {
                if (!isCurrentTileOccupied())
                {
                    state = "selectedUnoccupied";
                } 
                else
                {
                    if (currentInfantry != null)
                    {
                        if (currentInfantry.tag == currentPlayer && currentInfantry.active)
                        {
                            state = "selectedUnit";
                        }
                        else
                        {
                            //Play error sound
                        }
                    } 
                    else if (currentAntiTank != null)
                    {
                        if (currentAntiTank.tag == currentPlayer && currentAntiTank.active)
                        {
                            state = "selectedUnit";
                        }
                        else
                        {
                            //Play error sound
                        }
                    } 
                    else if (currentTank != null)
                    {
                        if (currentTank.tag == currentPlayer && currentTank.active)
                        {
                            state = "selectedUnit";
                        }
                        else
                        {
                            //Play error sound
                        }
                    }
                }
            }

            //Hitting F does nothing

        } else if (state == "selectedUnoccupied")
        {
            //Hides Cursor and defaultUI
            turnUI.dissappear();
            terrainUI.dissappear();
            unitUI.allowedToReappear = false;
            unitUI.dissappear();
            cursor.dissappear();

            //Pulls up a menu in the top right where you can manually end your turn (Pos 1), restart (Pos 2), return to main menu (Pos 3), quit (Pos 4)
            endTurnUI.reappear();

            //Pressing W (Up on controller) when at Pos 1 does nothing, at Pos 2 moves hand to Pos 1, at Pos 3 moves hand to Pos 2, etc
            if (Input.GetKeyDown(KeyCode.W) && endTurnUI.menuArrow.currentPosition > 0) //Add controller support later
            {
                endTurnUI.menuArrow.currentPosition--;
            }

            //Pressing S (Down on controller) when at Pos 1 moves to Pos 2, Pos 2 moves to Pos 3, Pos 3 does nothing, etc
            if (Input.GetKeyDown(KeyCode.S) && endTurnUI.menuArrow.currentPosition < 3) //Add controller support later
            {
                endTurnUI.menuArrow.currentPosition++;
            }

            //Pressing E (A on controller)
            if (Input.GetKeyDown(KeyCode.E)) //Add controller support later
            {
                if (endTurnUI.menuArrow.currentPosition == 0)
                {
                    //EndTurn
                    endTurn();
                    state = "default";
                } 
                else if (endTurnUI.menuArrow.currentPosition == 1)
                {
                    //Reload Level
                    SceneManager.LoadScene("Map-01");
                } 
                else if (endTurnUI.menuArrow.currentPosition == 2)
                {
                    //Go to main menu (main menu does not currently exist)
                } 
                else if (endTurnUI.menuArrow.currentPosition == 3)
                {
                    //Quit game
                    Application.Quit();
                }
            }

            //Pressing F (B on controller)
            if (Input.GetKeyDown(KeyCode.F)) //Add controller support later
            {
                state = "default";
            }

        } else if (state == "selectedUnit")
        {
            //Hides Cursor and defaultUI
            turnUI.dissappear();
            terrainUI.dissappear();
            unitUI.allowedToReappear = false;
            unitUI.dissappear();
            cursor.dissappear();

            //terrainUI should auto pop up due to its own listener

            //Find Selected Unit and set as selected
            if (currentInfantry != null)
            {
                currentInfantry.selected = true;
            }
            else if (currentAntiTank != null)
            {
                currentAntiTank.selected = true;
            }
            else if (currentTank != null)
            {
                currentTank.selected = true;
            }

            //Store unit's original position - if allowing for F button - TODO

            //In the top left show selected unit's movement points remaining
            movementRemainingUI.reappear();

            //Pressing E (A on controller)
            //Hide top left movement points window
            //Send into determineStrikeCapability state
            if (Input.GetKeyDown(KeyCode.E)) //Add controller support later
            {
                //Update Cursor Pos
                Vector3 whereToMoveCursor;
                if (currentInfantry != null)
                {
                    whereToMoveCursor = currentInfantry.transform.position;
                    whereToMoveCursor.y -= 0.1f;
                }
                else if (currentAntiTank != null)
                {
                    whereToMoveCursor = currentAntiTank.transform.position;
                    whereToMoveCursor.y -= 0.1f;
                }
                else if (currentTank != null)
                {
                    whereToMoveCursor = currentTank.transform.position;
                    whereToMoveCursor.y -= 0.1f;
                } else
                {
                    whereToMoveCursor = new Vector3(-1.5f, -1.5f, 0);
                }
                cursor.transform.position = whereToMoveCursor;

                //Hide top left movement points window
                movementRemainingUI.dissappear();

                //Find and store all valid targets in a list

                //If valid targets is empty
                //Send into wait only state

                //Else
                //Send into attackOrWait state

                //This would normally split into either wait or attackORwait, but attacking isn't implemented so you can only wait :)
                state = "onlyWait";
            }

            //Pressing F (B on controller) - TODO
            //Return unit to original position
            //Restore unit's max movement points
            //Set unit as not selected
            //Hide terrain movement costs
            //Hide top left movement points window
            //Unhide defaultUI
            //Return state to default


        } else if (state == "onlyWait")
        {
            //Draw wait only window in the top right
            waitMenuUI.reappear();

            //Pressing E (A on controller)
            //Set Unit as not selected
            //Call unit's wait method
            //Stop drawing wait only window
            //Return to default state
            if (Input.GetKeyDown(KeyCode.E)) //Add controller support later
            {
                if (currentInfantry != null)
                {
                    currentInfantry.selected = false;
                    currentInfantry.wait();
                }
                else if (currentAntiTank != null)
                {
                    currentAntiTank.selected = false;
                    currentAntiTank.wait();
                }
                else if (currentTank != null)
                {
                    currentTank.selected = false;
                    currentTank.wait();
                }

                state = "default";
            }

            //Should there be a Pressing F option, to keep moving? - TODO

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

    public void endTurn()
    {
        if (currentPlayer == "Blue")
        {
            currentPlayer = "Red";
        }
        else
        {
            currentPlayer = "Blue";
        }

        day++;

        for (int i = 0; i < blueTanks.Length; i++)
        {
            blueTanks[i].readyUnitForNewTurn();
        }

        for (int i = 0; i < blueInfantry.Length; i++)
        {
            blueInfantry[i].readyUnitForNewTurn();
        }

        for (int i = 0; i < blueAntiTanks.Length; i++)
        {
            blueAntiTanks[i].readyUnitForNewTurn();
        }

        for (int i = 0; i < redAntiTanks.Length; i++)
        {
            redAntiTanks[i].readyUnitForNewTurn();
        }

        for (int i = 0; i < redInfantry.Length; i++)
        {
            redInfantry[i].readyUnitForNewTurn();
        }

        for (int i = 0; i < redTanks.Length; i++)
        {
            redTanks[i].readyUnitForNewTurn();
        }

        //Maybe show a visual turn change animation here?

    }

    public void storeTileAtCursorPosition()
    {
        currentRiverTile = null;
        currentGrassTile = null;
        currentForestTile = null;
        currentSmallMountainTile = null;
        currentLargeMountainTile = null;
        currentCityTile = null;
        currentHeadQuartersTile = null;

        for (int i = 0; i < riverTiles.Length; i++)
        {
            if (riverTiles[i].transform.position == cursor.transform.position)
            {
                currentRiverTile = riverTiles[i];
            }
        }

        if (currentRiverTile != null)
        {
            return;
        }

        for (int i = 0; i < grassTiles.Length; i++)
        {
            if (grassTiles[i].transform.position == cursor.transform.position)
            {
                currentGrassTile = grassTiles[i];
            }
        }

        if (currentGrassTile != null)
        {
            return;
        }

        for (int i = 0; i < forestTiles.Length; i++)
        {
            if (forestTiles[i].transform.position == cursor.transform.position)
            {
                currentForestTile = forestTiles[i];
            }
        }

        if (currentForestTile != null)
        {
            return;
        }

        for (int i = 0; i < smallMountainTiles.Length; i++)
        {
            if (smallMountainTiles[i].transform.position == cursor.transform.position)
            {
                currentSmallMountainTile = smallMountainTiles[i];
            }
        }

        if (currentSmallMountainTile != null)
        {
            return;
        }

        for (int i = 0; i < largeMountainTiles.Length; i++)
        {
            if (largeMountainTiles[i].transform.position == cursor.transform.position)
            {
                currentLargeMountainTile = largeMountainTiles[i];
            }
        }

        if (currentLargeMountainTile != null)
        {
            return;
        }

        for (int i = 0; i < cityTiles.Length; i++)
        {
            if (cityTiles[i].transform.position == cursor.transform.position)
            {
                currentCityTile = cityTiles[i];
            }
        }

        if (currentCityTile != null)
        {
            return;
        }

        for (int i = 0; i < headQuartersTiles.Length; i++)
        {
            if (headQuartersTiles[i].transform.position == cursor.transform.position)
            {
                currentHeadQuartersTile = headQuartersTiles[i];
            }
        }

        if (currentHeadQuartersTile != null)
        {
            return;
        }
    }

    public void storeUnitAtCursorPosition()
    {
        currentAntiTank = null;
        currentTank = null;
        currentInfantry = null;

        //Set up actual position to compare due to units being offset by a y value of +0.1
        Vector3 positionToCompare = cursor.transform.position;
        positionToCompare.y += 0.1f;

        for (int i = 0; i < blueTanks.Length; i++)
        {
            if (blueTanks[i].transform.position == positionToCompare)
            {
                currentTank = blueTanks[i];
            }
        }

        if (currentTank != null)
        {
            return;
        }

        for (int i = 0; i < blueInfantry.Length; i++)
        {
            if (blueInfantry[i].transform.position == positionToCompare)
            {
                currentInfantry = blueInfantry[i];
            }
        }

        if (currentInfantry != null)
        {
            return;
        }

        for (int i = 0; i < blueAntiTanks.Length; i++)
        {
            if (blueAntiTanks[i].transform.position == positionToCompare)
            {
                currentAntiTank = blueAntiTanks[i];
            }
        }

        if (currentAntiTank != null)
        {
            return;
        }

        for (int i = 0; i < redTanks.Length; i++)
        {
            if (redTanks[i].transform.position == positionToCompare)
            {
                currentTank = redTanks[i];
            }
        }

        if (currentTank != null)
        {
            return;
        }

        for (int i = 0; i < redInfantry.Length; i++)
        {
            if (redInfantry[i].transform.position == positionToCompare)
            {
                currentInfantry = redInfantry[i];
            }
        }

        if (currentInfantry != null)
        {
            return;
        }

        for (int i = 0; i < redAntiTanks.Length; i++)
        {
            if (redAntiTanks[i].transform.position == positionToCompare)
            {
                currentAntiTank = redAntiTanks[i];
            }
        }

        if (currentAntiTank != null)
        {
            return;
        }
    }

    public bool isCurrentTileOccupied()
    {
        return (currentInfantry != null || currentAntiTank != null || currentTank != null);
    }
}
