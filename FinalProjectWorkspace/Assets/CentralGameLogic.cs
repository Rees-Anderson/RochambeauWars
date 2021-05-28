using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Author: Rees Anderson
 * 5.28.21
 * Game Design Project
 */

public class CentralGameLogic : MonoBehaviour
{
    public CursorScript cursor;
    public string state;
    public string currentPlayer = "Red";
    public int day = 1;

    public TerrainUIScript terrainUI;
    public TurnUIScript turnUI;
    public UnitUIScript unitUI;
    public EndTurnUIScript endTurnUI;
    public MovRemUI movementRemainingUI;
    public WaitMenuScript waitMenuUI;
    public AttackOrWaitMenuScript attackOrWaitUI;

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

    public InfantryScript attackingInfantry; //null if not an infantry unit
    public AntiTankScript attackingAntiTank; //null if not an AT unit
    public TankScript attackingTank; //null if not a tank unit

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

    Vector3 tempCursorPosition;
    
    public List<Vector3> directions = new List<Vector3>();
    public int directionIterator = 0;

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
            attackOrWaitUI.dissappear();

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

                //storeTileAtCursorPosition(); //Added 5.28.21
                //storeUnitAtCursorPosition(); //Added 5.28.21

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
            attackOrWaitUI.dissappear();
            waitMenuUI.dissappear();
            movementRemainingUI.dissappear();

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
                    endTurnUI.menuArrow.currentPosition = 0;
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
                endTurnUI.menuArrow.currentPosition = 0;
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
            attackOrWaitUI.dissappear();
            waitMenuUI.dissappear();
            endTurnUI.dissappear();

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
                }
                else
                {
                    whereToMoveCursor = new Vector3(-1.5f, -1.5f, 0);
                }
                cursor.transform.position = whereToMoveCursor;

                //Hide top left movement points window
                movementRemainingUI.dissappear();

                if (currentInfantry != null)
                {
                    currentInfantry.selected = false;
                }
                else if (currentAntiTank != null)
                {
                    currentAntiTank.selected = false;
                }
                else if (currentTank != null)
                {
                    currentTank.selected = false;
                }

                //If there is at least one valid target, send to attack or wait, else only wait
                if (atLeastOneValidTargetFromCurrent())
                {
                    state = "attackOrWait";
                }
                else
                {
                    state = "onlyWait";
                }
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
            //Hide other menus
            turnUI.dissappear();
            terrainUI.dissappear();
            unitUI.allowedToReappear = false;
            unitUI.dissappear();
            cursor.dissappear();
            attackOrWaitUI.dissappear();
            movementRemainingUI.dissappear();
            endTurnUI.dissappear();

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
                    currentInfantry.wait();
                }
                else if (currentAntiTank != null)
                {
                    currentAntiTank.wait();
                }
                else if (currentTank != null)
                {
                    currentTank.wait();
                }

                state = "default";
            }

            //Should there be a Pressing F option, to keep moving? - TODO

        } else if (state == "attackOrWait")
        {
            //Hide other menus
            turnUI.dissappear();
            terrainUI.dissappear();
            unitUI.allowedToReappear = false;
            unitUI.dissappear();
            cursor.dissappear();
            waitMenuUI.dissappear();
            movementRemainingUI.dissappear();
            endTurnUI.dissappear();

            //Draw attack or wait window in the top right - attack Pos 1, wait Pos 2
            attackOrWaitUI.reappear();

            //Pressing W (Up on controller) when at Pos 1 does nothing, at Pos 2 moves hand to Pos 1
            if (Input.GetKeyDown(KeyCode.W) && attackOrWaitUI.menuArrow.currentPosition > 0) //Add controller support later
            {
                attackOrWaitUI.menuArrow.currentPosition--;
            }

            //Pressing S (Down on controller) when at Pos 1 moves to Pos 2, Pos 2 does nothing
            if (Input.GetKeyDown(KeyCode.S) && attackOrWaitUI.menuArrow.currentPosition < 1) //Add controller support later
            {
                attackOrWaitUI.menuArrow.currentPosition++;
            }

            //Pressing E (A on controller)
            if (Input.GetKeyDown(KeyCode.E)) //Add controller support later
            {
                if (attackOrWaitUI.menuArrow.currentPosition == 0)
                {
                    Vector3 north;
                    Vector3 south;
                    Vector3 east;
                    Vector3 west;

                    if (currentInfantry != null)
                    {
                        attackingInfantry = currentInfantry;
                        tempCursorPosition = cursor.transform.position;

                        north = attackingInfantry.transform.position;
                        south = attackingInfantry.transform.position;
                        east = attackingInfantry.transform.position;
                        west = attackingInfantry.transform.position;
                        north.y += 1;
                        south.y -= 1;
                        east.x += 1;
                        west.x -= 1;

                        directions.Add(west);
                        directions.Add(north);
                        directions.Add(east);
                        directions.Add(south);

                        directions = removeFriendliesFromTargets(directions, attackingInfantry.tag);
                    }
                    else if (currentAntiTank != null)
                    {
                        attackingAntiTank = currentAntiTank;
                        tempCursorPosition = cursor.transform.position;

                        north = attackingAntiTank.transform.position;
                        south = attackingAntiTank.transform.position;
                        east = attackingAntiTank.transform.position;
                        west = attackingAntiTank.transform.position;
                        north.y += 1;
                        south.y -= 1;
                        east.x += 1;
                        west.x -= 1;

                        directions.Add(west);
                        directions.Add(north);
                        directions.Add(east);
                        directions.Add(south);

                        directions = removeFriendliesFromTargets(directions, attackingAntiTank.tag);
                    }
                    else if (currentTank != null)
                    {
                        attackingTank = currentTank;
                        tempCursorPosition = cursor.transform.position;

                        north = attackingTank.transform.position;
                        south = attackingTank.transform.position;
                        east = attackingTank.transform.position;
                        west = attackingTank.transform.position;
                        north.y += 1;
                        south.y -= 1;
                        east.x += 1;
                        west.x -= 1;

                        directions.Add(west);
                        directions.Add(north);
                        directions.Add(east);
                        directions.Add(south);

                        directions = removeFriendliesFromTargets(directions, attackingTank.tag);
                    }
                    storeUnitAtCursorPosition();

                    attackOrWaitUI.menuArrow.currentPosition = 0;
                    
                    state = "attack";
                }
                else if (attackOrWaitUI.menuArrow.currentPosition == 1)
                {
                    if (currentInfantry != null)
                    {
                        currentInfantry.wait();
                    }
                    else if (currentAntiTank != null)
                    {
                        currentAntiTank.wait();
                    }
                    else if (currentTank != null)
                    {
                        currentTank.wait();
                    }
                    attackOrWaitUI.menuArrow.currentPosition = 0;
                    state = "default";
                }
            }

        } else if (state == "attack")
        {
            //Hide Other Menu Elements
            waitMenuUI.dissappear();
            movementRemainingUI.dissappear();
            endTurnUI.dissappear();
            attackOrWaitUI.dissappear();

            //Certain UI Elements Appear
            unitUI.allowedToReappear = true;
            unitUI.reappear();
            terrainUI.reappear();

            //Change Cursor to a crosshair and appear
            cursor.reappear();

            //Calculate damage to be done to current target and show estimated damage above the terrain/unit UI - much of the damage calculation done in the E button press section should likely be in a method that can be recalled so that the forcast UI can be constantly updated

            //Pressing A (Left on controller) - cycles to the previous unit (cycles around if at 0 index), move cursor, redo calculations
            if (Input.GetKeyDown(KeyCode.A)) //Add controller support later
            {
                if (directionIterator > 0)
                {
                    directionIterator--;
                }
                else
                {
                    directionIterator = directions.Count - 1;
                }
                
                Vector3 target = directions[directionIterator];
                target.y -= 0.1f;
                cursor.transform.position = target;
                storeUnitAtCursorPosition();
            }

            //Pressing D (Right on controller) - cycles to the next unit (cycles around if at end index), move cursor, redo calculations
            if (Input.GetKeyDown(KeyCode.D)) //Add controller support later
            {
                if (directionIterator < directions.Count - 1)
                {
                    directionIterator++;
                }
                else
                {
                    directionIterator = 0;
                }

                Vector3 target = directions[directionIterator];
                target.y -= 0.1f;
                cursor.transform.position = target;
                storeUnitAtCursorPosition();
            }

            //Pressing E (A on controller)
            //Do damage to both units
            //Return to default state
            if (Input.GetKeyDown(KeyCode.E)) //Add controller support later
            {
                if (attackingInfantry != null)
                {
                    int damageToDealToDefender = 0;
                    damageToDealToDefender += (int) Mathf.Ceil(((float) attackingInfantry.health) / 2); //Add Base Attack : Ceiling of (Health / 2)
                    damageToDealToDefender += 2; //Add amount for attack bonus
                    if (currentInfantry != null)
                    {
                        damageToDealToDefender -= currentInfantry.currentDefenseModifier; //Subtract defense modifier

                        int damageToDealToAttacker = 0;
                        damageToDealToAttacker += (int) Mathf.Ceil(((float)currentInfantry.health) / 2); //Add Base Attack : Ceiling of (Health / 2)
                        damageToDealToAttacker -= 2; //Sub amount for defense penalty
                        damageToDealToAttacker -= attackingInfantry.currentDefenseModifier; //Sub defense modifier

                        attackingInfantry.fireWeaponOffensive();
                        currentInfantry.fireWeaponDefensive();
                        attackingInfantry.takeDamage(damageToDealToAttacker);
                        currentInfantry.takeDamage(damageToDealToDefender);

                        //Change cursor back to normal


                        //Retore cursor position
                        cursor.transform.position = tempCursorPosition;

                        attackingInfantry.wait();
                        storeUnitAtCursorPosition(); //Disabled due to odd attacking self bug

                        directions.Clear(); //Added 5.28.21
                        attackingInfantry = null; //Added 5.28.21
                        attackingAntiTank = null; //Added 5.28.21
                        attackingTank = null; //Added 5.28.21

                        state = "default";
                    } 
                    else if (currentAntiTank != null)
                    {
                        damageToDealToDefender -= currentAntiTank.currentDefenseModifier; //Subtract defense modifier
                        damageToDealToDefender += 3; //Add type match bonus

                        int damageToDealToAttacker = 0;
                        damageToDealToAttacker += (int) Mathf.Ceil(((float)currentAntiTank.health) / 2);
                        damageToDealToAttacker -= 2;
                        damageToDealToAttacker -= attackingInfantry.currentDefenseModifier;

                        attackingInfantry.fireWeaponOffensive();
                        currentAntiTank.fireWeaponDefensive();
                        attackingInfantry.takeDamage(damageToDealToAttacker);
                        currentAntiTank.takeDamage(damageToDealToDefender);

                        //Change cursor back to normal


                        //Retore cursor position
                        cursor.transform.position = tempCursorPosition;

                        attackingInfantry.wait();
                        storeUnitAtCursorPosition(); //Disabled due to odd attacking self bug

                        directions.Clear(); //Added 5.28.21
                        attackingInfantry = null; //Added 5.28.21
                        attackingAntiTank = null; //Added 5.28.21
                        attackingTank = null; //Added 5.28.21

                        state = "default";
                    }
                    else if (currentTank != null)
                    {
                        damageToDealToDefender -= currentTank.currentDefenseModifier; //Subtract defense modifier

                        int damageToDealToAttacker = 0;
                        damageToDealToAttacker += (int)Mathf.Ceil(((float)currentTank.health) / 2);
                        damageToDealToAttacker -= 2;
                        damageToDealToAttacker -= attackingInfantry.currentDefenseModifier;

                        attackingInfantry.fireWeaponOffensive();
                        currentTank.fireWeaponDefensive();
                        attackingInfantry.takeDamage(damageToDealToAttacker);
                        currentTank.takeDamage(damageToDealToDefender);

                        //Change cursor back to normal


                        //Retore cursor position
                        cursor.transform.position = tempCursorPosition;

                        attackingInfantry.wait();
                        storeUnitAtCursorPosition(); //Disabled due to odd attacking self bug

                        directions.Clear(); //Added 5.28.21
                        attackingInfantry = null; //Added 5.28.21
                        attackingAntiTank = null; //Added 5.28.21
                        attackingTank = null; //Added 5.28.21

                        state = "default";
                    }
                    else
                    {
                        Debug.Log("Critical Error - Flow should not be here");
                    }
                } 
                else if (attackingAntiTank != null)
                {

                }
                else if (attackingTank != null)
                {

                }
                else
                {
                    Debug.Log("Critical Error - Flow should not be here");
                }
            }

        }
    }

    public void endTurn()
    {
        if (currentPlayer == "Blue")
        {
            currentPlayer = "Red";
            day++;
        }
        else
        {
            currentPlayer = "Blue";
        }

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
            if (blueTanks[i].transform.position == positionToCompare && blueTanks[i].alive == true)
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
            if (blueInfantry[i].transform.position == positionToCompare && blueInfantry[i].alive == true)
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
            if (blueAntiTanks[i].transform.position == positionToCompare && blueAntiTanks[i].alive == true)
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
            if (redTanks[i].transform.position == positionToCompare && redTanks[i].alive == true)
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
            if (redInfantry[i].transform.position == positionToCompare && redInfantry[i].alive == true)
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
            if (redAntiTanks[i].transform.position == positionToCompare && redAntiTanks[i].alive == true)
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

    public List<Vector3> removeFriendliesFromTargets(List<Vector3> toReduce, string tagToRemove)
    {
        List<Vector3> temp = new List<Vector3>();

        InfantryScript potentialInfantryTarget;
        TankScript potentialTankTarget;
        AntiTankScript potentialAntiTankTarget;

        for (int i = 0; i < toReduce.Count; i++)
        {
            potentialInfantryTarget = findInfantryAtLocation(toReduce[i]);
            potentialTankTarget = findTankAtLocation(toReduce[i]);
            potentialAntiTankTarget = findAntiTankAtLocation(toReduce[i]);

            if (potentialInfantryTarget != null && potentialInfantryTarget.tag != tagToRemove && potentialInfantryTarget.alive)
            {
                temp.Add(toReduce[i]);
            }
            else if (potentialTankTarget != null && potentialTankTarget.tag != tagToRemove && potentialTankTarget.alive)
            {
                temp.Add(toReduce[i]);
            }
            else if (potentialAntiTankTarget != null && potentialAntiTankTarget.tag != tagToRemove && potentialAntiTankTarget.alive)
            {
                temp.Add(toReduce[i]);
            }
        }

        Vector3 target = temp[0];
        target.y -= 0.1f;
        cursor.transform.position = target;
        return temp;
    }

    public bool atLeastOneValidTargetFromCurrent()
    {
        //Find and store all valid targets in a list
        if (currentInfantry != null)
        {
            //Find all valid target locations
            Vector3 north = currentInfantry.transform.position;
            north.y += 1;

            Vector3 south = currentInfantry.transform.position;
            south.y -= 1;

            Vector3 east = currentInfantry.transform.position;
            east.x += 1;

            Vector3 west = currentInfantry.transform.position;
            west.x -= 1;

            //If currently red's turn look for blue targets, else vice versa
            if (currentInfantry.tag == "Red")
            {
                for (int i = 0; i < blueInfantry.Length; i++)
                {
                    if (blueInfantry[i].alive && (blueInfantry[i].transform.position == north || blueInfantry[i].transform.position == south || blueInfantry[i].transform.position == west || blueInfantry[i].transform.position == east))
                    {
                        return true;
                    }
                }

                for (int i = 0; i < blueTanks.Length; i++)
                {
                    if (blueTanks[i].alive && (blueTanks[i].transform.position == north || blueTanks[i].transform.position == south || blueTanks[i].transform.position == west || blueTanks[i].transform.position == east))
                    {
                        return true;
                    }
                }

                for (int i = 0; i < blueAntiTanks.Length; i++)
                {
                    if (blueAntiTanks[i].alive && (blueAntiTanks[i].transform.position == north || blueAntiTanks[i].transform.position == south || blueAntiTanks[i].transform.position == west || blueAntiTanks[i].transform.position == east))
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < redInfantry.Length; i++)
                {
                    if (redInfantry[i].alive && (redInfantry[i].transform.position == north || redInfantry[i].transform.position == south || redInfantry[i].transform.position == west || redInfantry[i].transform.position == east))
                    {
                        return true;
                    }
                }

                for (int i = 0; i < redTanks.Length; i++)
                {
                    if (redTanks[i].alive && (redTanks[i].transform.position == north || redTanks[i].transform.position == south || redTanks[i].transform.position == west || redTanks[i].transform.position == east))
                    {
                        return true;
                    }
                }

                for (int i = 0; i < redAntiTanks.Length; i++)
                {
                    if (redAntiTanks[i].alive && (redAntiTanks[i].transform.position == north || redAntiTanks[i].transform.position == south || redAntiTanks[i].transform.position == west || redAntiTanks[i].transform.position == east))
                    {
                        return true;
                    }
                }
            }
        }
        else if (currentAntiTank != null)
        {
            //Find all valid target locations
            Vector3 north = currentAntiTank.transform.position;
            north.y += 1;

            Vector3 south = currentAntiTank.transform.position;
            south.y -= 1;

            Vector3 east = currentAntiTank.transform.position;
            east.x += 1;

            Vector3 west = currentAntiTank.transform.position;
            west.x -= 1;

            if (currentAntiTank.tag == "Red")
            {
                for (int i = 0; i < blueInfantry.Length; i++)
                {
                    if (blueInfantry[i].alive && (blueInfantry[i].transform.position == north || blueInfantry[i].transform.position == south || blueInfantry[i].transform.position == west || blueInfantry[i].transform.position == east))
                    {
                        return true;
                    }
                }

                for (int i = 0; i < blueTanks.Length; i++)
                {
                    if (blueTanks[i].alive && (blueTanks[i].transform.position == north || blueTanks[i].transform.position == south || blueTanks[i].transform.position == west || blueTanks[i].transform.position == east))
                    {
                        return true;
                    }
                }

                for (int i = 0; i < blueAntiTanks.Length; i++)
                {
                    if (blueAntiTanks[i].alive && (blueAntiTanks[i].transform.position == north || blueAntiTanks[i].transform.position == south || blueAntiTanks[i].transform.position == west || blueAntiTanks[i].transform.position == east))
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < redInfantry.Length; i++)
                {
                    if (redInfantry[i].alive && (redInfantry[i].transform.position == north || redInfantry[i].transform.position == south || redInfantry[i].transform.position == west || redInfantry[i].transform.position == east))
                    {
                        return true;
                    }
                }

                for (int i = 0; i < redTanks.Length; i++)
                {
                    if (redTanks[i].alive && (redTanks[i].transform.position == north || redTanks[i].transform.position == south || redTanks[i].transform.position == west || redTanks[i].transform.position == east))
                    {
                        return true;
                    }
                }

                for (int i = 0; i < redAntiTanks.Length; i++)
                {
                    if (redAntiTanks[i].alive && (redAntiTanks[i].transform.position == north || redAntiTanks[i].transform.position == south || redAntiTanks[i].transform.position == west || redAntiTanks[i].transform.position == east))
                    {
                        return true;
                    }
                }
            }
        }
        else if (currentTank != null)
        {
            //Find all valid target locations
            Vector3 north = currentTank.transform.position;
            north.y += 1;

            Vector3 south = currentTank.transform.position;
            south.y -= 1;

            Vector3 east = currentTank.transform.position;
            east.x += 1;

            Vector3 west = currentTank.transform.position;
            west.x -= 1;

            if (currentTank.tag == "Red")
            {
                for (int i = 0; i < blueInfantry.Length; i++)
                {
                    if (blueInfantry[i].alive && (blueInfantry[i].transform.position == north || blueInfantry[i].transform.position == south || blueInfantry[i].transform.position == west || blueInfantry[i].transform.position == east))
                    {
                        return true;
                    }
                }

                for (int i = 0; i < blueTanks.Length; i++)
                {
                    if (blueTanks[i].alive && (blueTanks[i].transform.position == north || blueTanks[i].transform.position == south || blueTanks[i].transform.position == west || blueTanks[i].transform.position == east))
                    {
                        return true;
                    }
                }

                for (int i = 0; i < blueAntiTanks.Length; i++)
                {
                    if (blueAntiTanks[i].alive && (blueAntiTanks[i].transform.position == north || blueAntiTanks[i].transform.position == south || blueAntiTanks[i].transform.position == west || blueAntiTanks[i].transform.position == east))
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < redInfantry.Length; i++)
                {
                    if (redInfantry[i].alive && (redInfantry[i].transform.position == north || redInfantry[i].transform.position == south || redInfantry[i].transform.position == west || redInfantry[i].transform.position == east))
                    {
                        return true;
                    }
                }

                for (int i = 0; i < redTanks.Length; i++)
                {
                    if (redTanks[i].alive && (redTanks[i].transform.position == north || redTanks[i].transform.position == south || redTanks[i].transform.position == west || redTanks[i].transform.position == east))
                    {
                        return true;
                    }
                }

                for (int i = 0; i < redAntiTanks.Length; i++)
                {
                    if (redAntiTanks[i].alive && (redAntiTanks[i].transform.position == north || redAntiTanks[i].transform.position == south || redAntiTanks[i].transform.position == west || redAntiTanks[i].transform.position == east))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public InfantryScript findInfantryAtLocation(Vector3 target)
    {
        for (int i = 0; i < blueInfantry.Length; i++)
        {
            if (blueInfantry[i].transform.position == target)
            {
                return blueInfantry[i];
            }
        }

        for (int i = 0; i < redInfantry.Length; i++)
        {
            if (redInfantry[i].transform.position == target)
            {
                return redInfantry[i];
            }
        }
        return null;
    }

    public TankScript findTankAtLocation(Vector3 target)
    {
        for (int i = 0; i < blueTanks.Length; i++)
        {
            if (blueTanks[i].transform.position == target)
            {
                return blueTanks[i];
            }
        }

        for (int i = 0; i < redTanks.Length; i++)
        {
            if (redTanks[i].transform.position == target)
            {
                return redTanks[i];
            }
        }
        return null;
    }

    public AntiTankScript findAntiTankAtLocation(Vector3 target)
    {
        for (int i = 0; i < blueAntiTanks.Length; i++)
        {
            if (blueAntiTanks[i].transform.position == target)
            {
                return blueAntiTanks[i];
            }
        }

        for (int i = 0; i < redAntiTanks.Length; i++)
        {
            if (redAntiTanks[i].transform.position == target)
            {
                return redAntiTanks[i];
            }
        }
        return null;
    }

    public InfantryScript findBlueInfantryAtLocation(Vector3 target)
    {
        for (int i = 0; i < blueInfantry.Length; i++)
        {
            if (blueInfantry[i].transform.position == target)
            {
                return blueInfantry[i];
            }
        }
        return null;
    }

    public InfantryScript findRedInfantryAtLocation(Vector3 target)
    {
        for (int i = 0; i < redInfantry.Length; i++)
        {
            if (redInfantry[i].transform.position == target)
            {
                return redInfantry[i];
            }
        }
        return null;
    }

    public TankScript findBlueTankAtLocation(Vector3 target)
    {
        for (int i = 0; i < blueTanks.Length; i++)
        {
            if (blueTanks[i].transform.position == target)
            {
                return blueTanks[i];
            }
        }
        return null;
    }

    public TankScript findRedTankAtLocation(Vector3 target)
    {
        for (int i = 0; i < redTanks.Length; i++)
        {
            if (redTanks[i].transform.position == target)
            {
                return redTanks[i];
            }
        }
        return null;
    }

    public AntiTankScript findBlueAntiTankAtLocation(Vector3 target)
    {
        for (int i = 0; i < blueAntiTanks.Length; i++)
        {
            if (blueAntiTanks[i].transform.position == target)
            {
                return blueAntiTanks[i];
            }
        }
        return null;
    }

    public AntiTankScript findRedAntiTankAtLocation(Vector3 target)
    {
        for (int i = 0; i < redAntiTanks.Length; i++)
        {
            if (redAntiTanks[i].transform.position == target)
            {
                return redAntiTanks[i];
            }
        }
        return null;
    }
}
