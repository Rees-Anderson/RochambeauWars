using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Author: Rees Anderson
 * 6.8.21
 * Game Design Project
 */

public class CentralGameLogic : MonoBehaviour
{
    public CursorScript cursor;
    public string state;
    public string currentPlayer = "Red";
    public int day = 1;

    public HeadQuartersScript redHQ;
    public HeadQuartersScript blueHQ;

    public TerrainUIScript terrainUI;
    public TurnUIScript turnUI;
    public UnitUIScript unitUI;
    public EndTurnUIScript endTurnUI;
    public MovRemUI movementRemainingUI;
    public WaitMenuScript waitMenuUI;
    public AttackOrWaitMenuScript attackOrWaitUI;
    public VictoryUIScript victoryUI;
    public CaptureOrWaitScript captureOrWaitUI;
    public CaptFireWaitScript captFireWaitUI;

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

    public int defenderDisadvantage = 2;
    public int attackBonus = 2;
    public int typeMatchBonus = 3;
    public int typeMatchPenalty = 3;

    //Beginning of Movement Stored Variables
    Vector3 unitPositionBeforeMoving;
    Vector3 cursorPositionBeforeMoving;

    //Helps attack state determine if it came from captureAttackWait or just AttackWait
    string justCameFrom = "";

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

        storeTileAtCursorPosition(); //Added 5.31.21 - Is it okay to constantly update the tile stored, I don't think anything really interacts with this other than the terrain UI

        if (state == "default")
        {
            //If all Red Units are dead OR The Red HQ has been captured enter Victory State
            //If all Blue Units are dead OR The Blue HQ has been captured enter Victory State
            if (allRedUnitsDead() || allBlueUnitsDead() || redHQ.tag == "Blue" || blueHQ.tag == "Red")
            {
                state = "victory";
            }

            //Auto End Turn Functionality
            if (allCurrentPlayerUnitsMoved())
            {
                endTurn();
            }

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
                //storeTileAtCursorPosition(); //Removed 5.31.21 - Redundant?
                storeUnitAtCursorPosition();
            }

            if (Input.GetKeyDown(KeyCode.A)) //Add controller support later
            {
                cursor.moveLeft();
                //storeTileAtCursorPosition(); //Removed 5.31.21 - Redundant?
                storeUnitAtCursorPosition();
            }

            if (Input.GetKeyDown(KeyCode.S)) //Add controller support later
            {
                cursor.moveDown();
                //storeTileAtCursorPosition(); //Removed 5.31.21 - Redundant?
                storeUnitAtCursorPosition();
            }

            if (Input.GetKeyDown(KeyCode.D)) //Add controller support later
            {
                cursor.moveRight();
                //storeTileAtCursorPosition(); //Removed 5.31.21 - Redundant?
                storeUnitAtCursorPosition();
            }

            //Hitting Return
            //On unoccupied tile sends controller into selectedUnoccupied state
            //On occupied tile with unit of same color sends controller into selectedUnit state
            //On occupied tile with unit of different color plays an error sound
            if (Input.GetKeyDown(KeyCode.K))
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
                            currentInfantry.startRunningBecauseSelected();
                            storeBeginningOfSelectedUnitInfo();
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
                            currentAntiTank.startRunningBecauseSelected();
                            storeBeginningOfSelectedUnitInfo();
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
                            currentTank.startRunningBecauseSelected();
                            storeBeginningOfSelectedUnitInfo();
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

            //Pressing Return
            if (Input.GetKeyDown(KeyCode.K))
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
                    //SceneManager.LoadScene("Map-01"); -- Replaced by the line below that will work on any level.
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                } 
                else if (endTurnUI.menuArrow.currentPosition == 2)
                {
                    //Go to main menu
                    SceneManager.LoadScene("Main-Menu");
                } 
                else if (endTurnUI.menuArrow.currentPosition == 3)
                {
                    //Quit game
                    Application.Quit();
                }
            }

            //Pressing Backspace
            if (Input.GetKeyDown(KeyCode.L))
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

            //In the top left show selected unit's movement points remaining
            movementRemainingUI.reappear();

            //Pressing K
            if (Input.GetKeyDown(KeyCode.K))
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

                bool hasAmmoToAttack = false;
                bool inCapturableCityOrHQ = false;

                if (currentInfantry != null)
                {
                    currentInfantry.selected = false;
                    if (currentInfantry.ammoCount > 0)
                    {
                        hasAmmoToAttack = true;
                    }
                    if ((currentInfantry.currentCityTile != null && currentInfantry.currentCityTile.tag != currentInfantry.tag) || (currentInfantry.currentHeadQuartersTile != null && currentInfantry.currentHeadQuartersTile.tag != currentInfantry.tag))
                    {
                        inCapturableCityOrHQ = true;
                    }
                }
                else if (currentAntiTank != null)
                {
                    currentAntiTank.selected = false;
                    if (currentAntiTank.ammoCount > 0)
                    {
                        hasAmmoToAttack = true;
                    }
                    if ((currentAntiTank.currentCityTile != null && currentAntiTank.currentCityTile.tag != currentAntiTank.tag) || (currentAntiTank.currentHeadQuartersTile != null && currentAntiTank.currentHeadQuartersTile.tag != currentAntiTank.tag))
                    {
                        inCapturableCityOrHQ = true;
                    }
                }
                else if (currentTank != null)
                {
                    currentTank.selected = false;
                    if (currentTank.ammoCount > 0)
                    {
                        hasAmmoToAttack = true;
                    }
                    
                    //inCapturableCityOrHQ is not changed since tanks cannot capture cities or hqs
                }

                //Flow of control for what menu to pull up next
                if (inCapturableCityOrHQ && atLeastOneValidTargetFromCurrent() && hasAmmoToAttack)
                {
                    state = "captureOrAttackOrWait";
                }
                else if (atLeastOneValidTargetFromCurrent() && hasAmmoToAttack)
                {
                    state = "attackOrWait";
                }
                else if (inCapturableCityOrHQ)
                {
                    state = "captureOrWait";
                }
                else
                {
                    state = "onlyWait";
                }
            }

            //Pressing L
            if (Input.GetKeyDown(KeyCode.L))
            {
                revertToBeforeMovingUnit();
                state = "default";
            }

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

            //Pressing Return
            //Set Unit as not selected
            //Call unit's wait method
            //Stop drawing wait only window
            //Return to default state
            if (Input.GetKeyDown(KeyCode.K))
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

            //Pressing L
            if (Input.GetKeyDown(KeyCode.L))
            {
                state = "selectedUnit";
            }

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

            //Pressing Return
            if (Input.GetKeyDown(KeyCode.K))
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

                    //Change attacking unit's run animation to face defender
                    attackingUnitFaceDefender();

                    attackOrWaitUI.menuArrow.currentPosition = 0;
                    justCameFrom = "attackOrWait";
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

            //Pressing L
            if (Input.GetKeyDown(KeyCode.L))
            {
                attackOrWaitUI.menuArrow.currentPosition = 0;
                state = "selectedUnit";
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

                //Update Battle Forcast - Automatic

                //Change attacking unit's run animation to face defender
                attackingUnitFaceDefender();
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

                //Update Battle Forecast - Automatic

                //Change attacking unit's run animation to face defender
                attackingUnitFaceDefender();
            }

            //Pressing Return
            //Do damage to both units
            //Return to default state
            if (Input.GetKeyDown(KeyCode.K))
            {
                string attackerDirection = directionforAttackerToFire();
                string defenderDirection = directionforDefenderToFire(attackerDirection);

                if (attackingInfantry != null)
                {

                    int damageToDealToDefender = 0;
                    damageToDealToDefender += (int) Mathf.Ceil(((float) attackingInfantry.health) / 2); //Add Base Attack : Ceiling of (Health / 2)
                    damageToDealToDefender += attackBonus; //Add amount for attack bonus
                    if (currentInfantry != null)
                    {
                        damageToDealToDefender -= currentInfantry.currentDefenseModifier; //Subtract defense modifier

                        int damageToDealToAttacker = 0;
                        damageToDealToAttacker += (int) Mathf.Ceil(((float)currentInfantry.health) / 2); //Add Base Attack : Ceiling of (Health / 2)
                        damageToDealToAttacker -= defenderDisadvantage; //Sub amount for defense penalty
                        damageToDealToAttacker -= attackingInfantry.currentDefenseModifier; //Sub defense modifier

                        attackingInfantry.fireWeaponOffensive(attackerDirection);
                        currentInfantry.fireWeaponDefensive(defenderDirection);
                        attackingInfantry.takeDamage(damageToDealToAttacker);
                        currentInfantry.takeDamage(damageToDealToDefender);

                        //Change cursor back to normal


                        //Retore cursor position
                        cursor.transform.position = tempCursorPosition;

                        //attackingInfantry.wait(); Removed to prevent this from stopping the firing animation, wait is now called in fireWeaponOffensive for attackers!
                        storeUnitAtCursorPosition(); //Disabled due to odd attacking self bug

                        directions.Clear(); //Added 5.28.21
                        attackingInfantry = null; //Added 5.28.21
                        attackingAntiTank = null; //Added 5.28.21
                        attackingTank = null; //Added 5.28.21

                        directionIterator = 0; //Added 6.8.21

                        state = "default";
                    } 
                    else if (currentAntiTank != null)
                    {
                        damageToDealToDefender -= currentAntiTank.currentDefenseModifier; //Subtract defense modifier
                        damageToDealToDefender += typeMatchBonus; //Add good type match bonus

                        int damageToDealToAttacker = 0;
                        damageToDealToAttacker += (int) Mathf.Ceil(((float)currentAntiTank.health) / 2);
                        damageToDealToAttacker -= defenderDisadvantage;
                        damageToDealToAttacker -= attackingInfantry.currentDefenseModifier;

                        attackingInfantry.fireWeaponOffensive(attackerDirection);
                        currentAntiTank.fireWeaponDefensive(defenderDirection);
                        attackingInfantry.takeDamage(damageToDealToAttacker);
                        currentAntiTank.takeDamage(damageToDealToDefender);

                        //Change cursor back to normal


                        //Retore cursor position
                        cursor.transform.position = tempCursorPosition;

                        //attackingInfantry.wait(); Removed to prevent this from stopping the firing animation, wait is now called in fireWeaponOffensive for attackers!
                        storeUnitAtCursorPosition(); //Disabled due to odd attacking self bug

                        directions.Clear(); //Added 5.28.21
                        attackingInfantry = null; //Added 5.28.21
                        attackingAntiTank = null; //Added 5.28.21
                        attackingTank = null; //Added 5.28.21

                        directionIterator = 0; //Added 6.8.21

                        state = "default";
                    }
                    else if (currentTank != null)
                    {
                        damageToDealToDefender -= currentTank.currentDefenseModifier; //Subtract defense modifier
                        damageToDealToDefender -= typeMatchPenalty; //Subtract poor type match penalty

                        int damageToDealToAttacker = 0;
                        damageToDealToAttacker += (int)Mathf.Ceil(((float)currentTank.health) / 2);
                        damageToDealToAttacker -= defenderDisadvantage;
                        damageToDealToAttacker -= attackingInfantry.currentDefenseModifier;
                        damageToDealToAttacker += typeMatchBonus; //Add good type match bonus

                        attackingInfantry.fireWeaponOffensive(attackerDirection);
                        currentTank.fireWeaponDefensive(defenderDirection);
                        attackingInfantry.takeDamage(damageToDealToAttacker);
                        currentTank.takeDamage(damageToDealToDefender);

                        //Change cursor back to normal


                        //Retore cursor position
                        cursor.transform.position = tempCursorPosition;

                        //attackingInfantry.wait(); Removed to prevent this from stopping the firing animation, wait is now called in fireWeaponOffensive for attackers!
                        storeUnitAtCursorPosition(); //Disabled due to odd attacking self bug

                        directions.Clear(); //Added 5.28.21
                        attackingInfantry = null; //Added 5.28.21
                        attackingAntiTank = null; //Added 5.28.21
                        attackingTank = null; //Added 5.28.21

                        directionIterator = 0; //Added 6.8.21

                        state = "default";
                    }
                    else
                    {
                        Debug.Log("Critical Error - Flow should not be here");
                    }
                } 
                else if (attackingAntiTank != null)
                {
                    int damageToDealToDefender = 0;
                    damageToDealToDefender += (int)Mathf.Ceil(((float)attackingAntiTank.health) / 2); //Add Base Attack : Ceiling of (Health / 2)
                    damageToDealToDefender += attackBonus; //Add amount for attack bonus
                    if (currentInfantry != null)
                    {
                        damageToDealToDefender -= currentInfantry.currentDefenseModifier; //Subtract defense modifier
                        damageToDealToDefender -= typeMatchPenalty; //Subtract poor type match penalty

                        int damageToDealToAttacker = 0;
                        damageToDealToAttacker += (int)Mathf.Ceil(((float)currentInfantry.health) / 2); //Add Base Attack : Ceiling of (Health / 2)
                        damageToDealToAttacker -= defenderDisadvantage; //Sub amount for defense penalty
                        damageToDealToAttacker -= attackingAntiTank.currentDefenseModifier; //Sub defense modifier
                        damageToDealToAttacker += typeMatchBonus; //Add good type match bonus

                        attackingAntiTank.fireWeaponOffensive(attackerDirection);
                        currentInfantry.fireWeaponDefensive(defenderDirection);
                        attackingAntiTank.takeDamage(damageToDealToAttacker);
                        currentInfantry.takeDamage(damageToDealToDefender);

                        //Change cursor back to normal


                        //Retore cursor position
                        cursor.transform.position = tempCursorPosition;

                        //attackingAntiTank.wait(); Removed to prevent this from stopping the firing animation, wait is now called in fireWeaponOffensive for attackers!
                        storeUnitAtCursorPosition(); //Disabled due to odd attacking self bug

                        directions.Clear(); //Added 5.28.21
                        attackingInfantry = null; //Added 5.28.21
                        attackingAntiTank = null; //Added 5.28.21
                        attackingTank = null; //Added 5.28.21

                        directionIterator = 0; //Added 6.8.21

                        state = "default";
                    }
                    else if (currentAntiTank != null)
                    {
                        damageToDealToDefender -= currentAntiTank.currentDefenseModifier; //Subtract defense modifier

                        int damageToDealToAttacker = 0;
                        damageToDealToAttacker += (int)Mathf.Ceil(((float)currentAntiTank.health) / 2);
                        damageToDealToAttacker -= defenderDisadvantage;
                        damageToDealToAttacker -= attackingAntiTank.currentDefenseModifier;

                        attackingAntiTank.fireWeaponOffensive(attackerDirection);
                        currentAntiTank.fireWeaponDefensive(defenderDirection);
                        attackingAntiTank.takeDamage(damageToDealToAttacker);
                        currentAntiTank.takeDamage(damageToDealToDefender);

                        //Change cursor back to normal


                        //Retore cursor position
                        cursor.transform.position = tempCursorPosition;

                        //attackingAntiTank.wait(); Removed to prevent this from stopping the firing animation, wait is now called in fireWeaponOffensive for attackers!
                        storeUnitAtCursorPosition(); //Disabled due to odd attacking self bug

                        directions.Clear(); //Added 5.28.21
                        attackingInfantry = null; //Added 5.28.21
                        attackingAntiTank = null; //Added 5.28.21
                        attackingTank = null; //Added 5.28.21

                        directionIterator = 0; //Added 6.8.21

                        state = "default";
                    }
                    else if (currentTank != null)
                    {
                        damageToDealToDefender -= currentTank.currentDefenseModifier; //Subtract defense modifier
                        damageToDealToDefender += typeMatchBonus; //Add type match bonus

                        int damageToDealToAttacker = 0;
                        damageToDealToAttacker += (int)Mathf.Ceil(((float)currentTank.health) / 2);
                        damageToDealToAttacker -= defenderDisadvantage;
                        damageToDealToAttacker -= attackingAntiTank.currentDefenseModifier;

                        attackingAntiTank.fireWeaponOffensive(attackerDirection);
                        currentTank.fireWeaponDefensive(defenderDirection);
                        attackingAntiTank.takeDamage(damageToDealToAttacker);
                        currentTank.takeDamage(damageToDealToDefender);

                        //Change cursor back to normal


                        //Retore cursor position
                        cursor.transform.position = tempCursorPosition;

                        //attackingAntiTank.wait(); Removed to prevent this from stopping the firing animation, wait is now called in fireWeaponOffensive for attackers!
                        storeUnitAtCursorPosition(); //Disabled due to odd attacking self bug

                        directions.Clear(); //Added 5.28.21
                        attackingInfantry = null; //Added 5.28.21
                        attackingAntiTank = null; //Added 5.28.21
                        attackingTank = null; //Added 5.28.21

                        directionIterator = 0; //Added 6.8.21

                        state = "default";
                    }
                    else
                    {
                        Debug.Log("Critical Error - Flow should not be here");
                    }
                }
                else if (attackingTank != null)
                {
                    int damageToDealToDefender = 0;
                    damageToDealToDefender += (int)Mathf.Ceil(((float)attackingTank.health) / 2); //Add Base Attack : Ceiling of (Health / 2)
                    damageToDealToDefender += attackBonus; //Add amount for attack bonus
                    if (currentInfantry != null)
                    {
                        damageToDealToDefender -= currentInfantry.currentDefenseModifier; //Subtract defense modifier
                        damageToDealToDefender += typeMatchBonus; //Add type match bonus

                        int damageToDealToAttacker = 0;
                        damageToDealToAttacker += (int)Mathf.Ceil(((float)currentInfantry.health) / 2); //Add Base Attack : Ceiling of (Health / 2)
                        damageToDealToAttacker -= defenderDisadvantage; //Sub amount for defense penalty
                        damageToDealToAttacker -= attackingTank.currentDefenseModifier; //Sub defense modifier

                        attackingTank.fireWeaponOffensive(attackerDirection);
                        currentInfantry.fireWeaponDefensive(defenderDirection);
                        attackingTank.takeDamage(damageToDealToAttacker);
                        currentInfantry.takeDamage(damageToDealToDefender);

                        //Change cursor back to normal


                        //Retore cursor position
                        cursor.transform.position = tempCursorPosition;

                        //attackingTank.wait(); Removed to prevent this from stopping the firing animation, wait is now called in fireWeaponOffensive for attackers!
                        storeUnitAtCursorPosition(); //Disabled due to odd attacking self bug

                        directions.Clear(); //Added 5.28.21
                        attackingInfantry = null; //Added 5.28.21
                        attackingAntiTank = null; //Added 5.28.21
                        attackingTank = null; //Added 5.28.21

                        directionIterator = 0; //Added 6.8.21

                        state = "default";
                    }
                    else if (currentAntiTank != null)
                    {
                        damageToDealToDefender -= currentAntiTank.currentDefenseModifier; //Subtract defense modifier
                        damageToDealToDefender -= typeMatchPenalty; //Subtract poor type match penalty

                        int damageToDealToAttacker = 0;
                        damageToDealToAttacker += (int)Mathf.Ceil(((float)currentAntiTank.health) / 2);
                        damageToDealToAttacker -= defenderDisadvantage;
                        damageToDealToAttacker -= attackingTank.currentDefenseModifier;
                        damageToDealToAttacker += typeMatchBonus; //Add good type match bonus

                        attackingTank.fireWeaponOffensive(attackerDirection);
                        currentAntiTank.fireWeaponDefensive(defenderDirection);
                        attackingTank.takeDamage(damageToDealToAttacker);
                        currentAntiTank.takeDamage(damageToDealToDefender);

                        //Change cursor back to normal


                        //Retore cursor position
                        cursor.transform.position = tempCursorPosition;

                        //attackingTank.wait(); Removed to prevent this from stopping the firing animation, wait is now called in fireWeaponOffensive for attackers!
                        storeUnitAtCursorPosition(); //Disabled due to odd attacking self bug

                        directions.Clear(); //Added 5.28.21
                        attackingInfantry = null; //Added 5.28.21
                        attackingAntiTank = null; //Added 5.28.21
                        attackingTank = null; //Added 5.28.21

                        directionIterator = 0; //Added 6.8.21

                        state = "default";
                    }
                    else if (currentTank != null)
                    {
                        damageToDealToDefender -= currentTank.currentDefenseModifier; //Subtract defense modifier

                        int damageToDealToAttacker = 0;
                        damageToDealToAttacker += (int)Mathf.Ceil(((float)currentTank.health) / 2);
                        damageToDealToAttacker -= defenderDisadvantage;
                        damageToDealToAttacker -= attackingTank.currentDefenseModifier;

                        attackingTank.fireWeaponOffensive(attackerDirection);
                        currentTank.fireWeaponDefensive(defenderDirection);
                        attackingTank.takeDamage(damageToDealToAttacker);
                        currentTank.takeDamage(damageToDealToDefender);

                        //Change cursor back to normal


                        //Retore cursor position
                        cursor.transform.position = tempCursorPosition;

                        //attackingTank.wait(); Removed to prevent this from stopping the firing animation, wait is now called in fireWeaponOffensive for attackers!
                        storeUnitAtCursorPosition(); //Disabled due to odd attacking self bug

                        directions.Clear(); //Added 5.28.21
                        attackingInfantry = null; //Added 5.28.21
                        attackingAntiTank = null; //Added 5.28.21
                        attackingTank = null; //Added 5.28.21

                        directionIterator = 0; //Added 6.8.21

                        state = "default";
                    }
                    else
                    {
                        Debug.Log("Critical Error - Flow should not be here");
                    }
                }
                else
                {
                    Debug.Log("Critical Error - Flow should not be here");
                }
            }

            //Pressing L
            if (Input.GetKeyDown(KeyCode.L))
            {
                revertToBeforeFiring();

                if (justCameFrom == "captureOrAttackOrWait")
                {
                    state = "captureOrAttackOrWait";
                }
                else
                {
                    state = "attackOrWait";
                }
            }

        }
        else if (state == "victory")
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

            //Victory Menu Should Pop up on its own

            //Pressing W (Up on controller) when at Pos 1 does nothing, at Pos 2 moves to Pos 1
            if (Input.GetKeyDown(KeyCode.W) && victoryUI.menuArrow.currentPosition > 0) //Add controller support later
            {
                victoryUI.menuArrow.currentPosition--;
            }

            //Pressing S (Down on controller) when at Pos 1 moves to Pos 2, Pos 2 does nothing
            if (Input.GetKeyDown(KeyCode.S) && victoryUI.menuArrow.currentPosition < 1) //Add controller support later
            {
                victoryUI.menuArrow.currentPosition++;
            }

            //Pressing Return
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (victoryUI.menuArrow.currentPosition == 0)
                {
                    //Reload Scene
                    //SceneManager.LoadScene("Map-01"); -- Replaced by the line below that will work on any level.
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                else if (victoryUI.menuArrow.currentPosition == 1)
                {
                    //Go to main menu
                    SceneManager.LoadScene("Main-Menu");
                }
            }
        }
        else if (state == "captureOrWait")
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

            //CaptureOrWaitUI should pop up on its own

            //Pressing W (Up on controller) when at Pos 1 does nothing, at Pos 2 moves hand to Pos 1
            if (Input.GetKeyDown(KeyCode.W) && captureOrWaitUI.menuArrow.currentPosition > 0) //Add controller support later
            {
                captureOrWaitUI.menuArrow.currentPosition--;
            }

            //Pressing S (Down on controller) when at Pos 1 moves to Pos 2, Pos 2 does nothing
            if (Input.GetKeyDown(KeyCode.S) && captureOrWaitUI.menuArrow.currentPosition < 1) //Add controller support later
            {
                captureOrWaitUI.menuArrow.currentPosition++;
            }

            //Pressing Return
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (captureOrWaitUI.menuArrow.currentPosition == 0)
                {
                    //Do some capturing logic
                    performCaptureLogic();

                    //Then Wait
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
                    captureOrWaitUI.menuArrow.currentPosition = 0;
                    state = "default";

                }
                else if (captureOrWaitUI.menuArrow.currentPosition == 1)
                {
                    //Wait
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
                    captureOrWaitUI.menuArrow.currentPosition = 0;
                    state = "default";
                }
            }

            //Pressing L
            if (Input.GetKeyDown(KeyCode.L))
            {
                captureOrWaitUI.menuArrow.currentPosition = 0;
                state = "selectedUnit";
            }

        }
        else if (state == "captureOrAttackOrWait")
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

            //CaptFireWaitUI should pop up on its own

            //Pressing W (Up on controller) when at Pos 1 does nothing, at Pos 2 moves hand to Pos 1
            if (Input.GetKeyDown(KeyCode.W) && captFireWaitUI.menuArrow.currentPosition > 0) //Add controller support later
            {
                captFireWaitUI.menuArrow.currentPosition--;
            }

            //Pressing S (Down on controller) when at Pos 1 moves to Pos 2, Pos 2 to 3, Pos 3 does nothing
            if (Input.GetKeyDown(KeyCode.S) && captFireWaitUI.menuArrow.currentPosition < 2) //Add controller support later
            {
                captFireWaitUI.menuArrow.currentPosition++;
            }

            //Pressing Return
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (captFireWaitUI.menuArrow.currentPosition == 0)
                {
                    //Do some capturing logic
                    performCaptureLogic();

                    //Then Wait
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
                    captFireWaitUI.menuArrow.currentPosition = 0;
                    state = "default";

                }
                else if (captFireWaitUI.menuArrow.currentPosition == 1)
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

                    //Change attacking unit's run animation to face defender
                    attackingUnitFaceDefender();

                    captFireWaitUI.menuArrow.currentPosition = 0;
                    justCameFrom = "captureOrAttackOrWait";
                    state = "attack";
                }
                else if (captFireWaitUI.menuArrow.currentPosition == 2)
                {
                    //Wait
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
                    captFireWaitUI.menuArrow.currentPosition = 0;
                    state = "default";
                }
            }

            //Pressing L
            if (Input.GetKeyDown(KeyCode.L))
            {
                captFireWaitUI.menuArrow.currentPosition = 0;
                state = "selectedUnit";
            }
        }
    }

    public bool allCurrentPlayerUnitsMoved()
    {
        if (currentPlayer == "Red")
        {
            for (int i = 0; i < redAntiTanks.Length; i++)
            {
                if (redAntiTanks[i].active)
                {
                    return false;
                }
            }

            for (int i = 0; i < redInfantry.Length; i++)
            {
                if (redInfantry[i].active)
                {
                    return false;
                }
            }

            for (int i = 0; i < redTanks.Length; i++)
            {
                if (redTanks[i].active)
                {
                    return false;
                }
                
            }
        }
        else
        {
            for (int i = 0; i < blueTanks.Length; i++)
            {
                if (blueTanks[i].active)
                {
                    return false;
                }
            }

            for (int i = 0; i < blueInfantry.Length; i++)
            {
                if (blueInfantry[i].active)
                {
                    return false;
                }
            }

            for (int i = 0; i < blueAntiTanks.Length; i++)
            {
                if (blueAntiTanks[i].active)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public string directionforDefenderToFire(string attackerDirection)
    {
        if (attackerDirection == "UP")
        {
            return "DOWN";
        }
        else if (attackerDirection == "DOWN")
        {
            return "UP";
        }
        else if (attackerDirection == "LEFT")
        {
            return "RIGHT";
        }
        else if (attackerDirection == "RIGHT")
        {
            return "LEFT";
        }
        else
        {
            Debug.Log("Error: Invalid direction recieved" + attackerDirection);
            return "ERROR";
        }
    }

    public string directionforAttackerToFire()
    {
        //Change attacking unit's run animation to face defender - used when selecting target
        if (attackingInfantry != null)
        {
            if (cursor.transform.position.y > attackingInfantry.transform.position.y)
            {
                return "UP";
            }
            else if (cursor.transform.position.x > attackingInfantry.transform.position.x)
            {
                return "RIGHT";
            }
            else if (cursor.transform.position.x < attackingInfantry.transform.position.x)
            {
                return "LEFT";
            }
            else
            {
                return "DOWN";
            }
        }
        else if (attackingAntiTank != null)
        {
            if (cursor.transform.position.y > attackingAntiTank.transform.position.y)
            {
                return "UP";
            }
            else if (cursor.transform.position.x > attackingAntiTank.transform.position.x)
            {
                return "RIGHT";
            }
            else if (cursor.transform.position.x < attackingAntiTank.transform.position.x)
            {
                return "LEFT";
            }
            else
            {
                return "DOWN";
            }
        }
        else if (attackingTank != null)
        {
            if (cursor.transform.position.y > attackingTank.transform.position.y)
            {
                return "UP";
            }
            else if (cursor.transform.position.x > attackingTank.transform.position.x)
            {
                return "RIGHT";
            }
            else if (cursor.transform.position.x < attackingTank.transform.position.x)
            {
                return "LEFT";
            }
            else
            {
                return "DOWN";
            }
        }
        else
        {
            Debug.Log("Error: Faulty Code in directionForAttackerToFire method");
            return "ERROR";
        }
    }

    public void attackingUnitFaceDefender()
    {
        //Change attacking unit's run animation to face defender - used when selecting target
        if (attackingInfantry != null)
        {
            if (cursor.transform.position.y > attackingInfantry.transform.position.y)
            {
                attackingInfantry.animatorToRunningUp();
            }
            else if (cursor.transform.position.x > attackingInfantry.transform.position.x)
            {
                attackingInfantry.animatorToRunningRight();
            }
            else if (cursor.transform.position.x < attackingInfantry.transform.position.x)
            {
                attackingInfantry.animatorToRunningLeft();
            }
            else
            {
                attackingInfantry.animatorToRunningDown();
            }
        }
        else if (attackingAntiTank != null)
        {
            if (cursor.transform.position.y > attackingAntiTank.transform.position.y)
            {
                attackingAntiTank.animatorToRunningUp();
            }
            else if (cursor.transform.position.x > attackingAntiTank.transform.position.x)
            {
                attackingAntiTank.animatorToRunningRight();
            }
            else if (cursor.transform.position.x < attackingAntiTank.transform.position.x)
            {
                attackingAntiTank.animatorToRunningLeft();
            }
            else
            {
                attackingAntiTank.animatorToRunningDown();
            }
        }
        else if (attackingTank != null)
        {
            if (cursor.transform.position.y > attackingTank.transform.position.y)
            {
                attackingTank.animatorToRunningUp();
            }
            else if (cursor.transform.position.x > attackingTank.transform.position.x)
            {
                attackingTank.animatorToRunningRight();
            }
            else if (cursor.transform.position.x < attackingTank.transform.position.x)
            {
                attackingTank.animatorToRunningLeft();
            }
            else
            {
                attackingTank.animatorToRunningDown();
            }
        }
    }

    public void performCaptureLogic()
    {
        if (currentInfantry != null)
        {
            if (currentInfantry.currentCityTile != null)
            {
                if (currentInfantry.currentCityTile.tag == "Red" && currentInfantry.tag == "Blue")
                {
                    currentInfantry.currentCityTile.tag = "Grey";
                }
                else if (currentInfantry.currentCityTile.tag == "Blue" && currentInfantry.tag == "Red")
                {
                    currentInfantry.currentCityTile.tag = "Grey";
                }
                else
                {
                    currentInfantry.currentCityTile.tag = currentInfantry.tag;
                }
            }
            else if (currentInfantry.currentHeadQuartersTile != null)
            {
                if (currentInfantry.currentHeadQuartersTile.tag == "Red" && currentInfantry.tag == "Blue")
                {
                    currentInfantry.currentHeadQuartersTile.tag = "Grey";
                }
                else if (currentInfantry.currentHeadQuartersTile.tag == "Blue" && currentInfantry.tag == "Red")
                {
                    currentInfantry.currentHeadQuartersTile.tag = "Grey";
                }
                else
                {
                    currentInfantry.currentHeadQuartersTile.tag = currentInfantry.tag;
                }
            }
        }
        else if (currentAntiTank != null)
        {
            if (currentAntiTank.currentCityTile != null)
            {
                if (currentAntiTank.currentCityTile.tag == "Red" && currentAntiTank.tag == "Blue")
                {
                    currentAntiTank.currentCityTile.tag = "Grey";
                }
                else if (currentAntiTank.currentCityTile.tag == "Blue" && currentAntiTank.tag == "Red")
                {
                    currentAntiTank.currentCityTile.tag = "Grey";
                }
                else
                {
                    currentAntiTank.currentCityTile.tag = currentAntiTank.tag;
                }
            }
            else if (currentAntiTank.currentHeadQuartersTile != null)
            {
                if (currentAntiTank.currentHeadQuartersTile.tag == "Red" && currentAntiTank.tag == "Blue")
                {
                    currentAntiTank.currentHeadQuartersTile.tag = "Grey";
                }
                else if (currentAntiTank.currentHeadQuartersTile.tag == "Blue" && currentAntiTank.tag == "Red")
                {
                    currentAntiTank.currentHeadQuartersTile.tag = "Grey";
                }
                else
                {
                    currentAntiTank.currentHeadQuartersTile.tag = currentAntiTank.tag;
                }
            }
        }

        //Tanks are not included since tanks cannot capture
    }

    public void endTurn()
    {
        //Ready All Units for a new turn, make all unmoved units of the current player wait (To make them consume fuel)
        for (int i = 0; i < blueTanks.Length; i++)
        {
            if (currentPlayer == "Blue" && blueTanks[i].active)
            {
                blueTanks[i].wait();
            }
            blueTanks[i].readyUnitForNewTurn();
        }

        for (int i = 0; i < blueInfantry.Length; i++)
        {
            if (currentPlayer == "Blue" && blueInfantry[i].active)
            {
                blueInfantry[i].wait();
            }
            blueInfantry[i].readyUnitForNewTurn();
        }

        for (int i = 0; i < blueAntiTanks.Length; i++)
        {
            if (currentPlayer == "Blue" && blueAntiTanks[i].active)
            {
                blueAntiTanks[i].wait();
            }
            blueAntiTanks[i].readyUnitForNewTurn();
        }

        for (int i = 0; i < redAntiTanks.Length; i++)
        {
            if (currentPlayer == "Red" && redAntiTanks[i].active)
            {
                redAntiTanks[i].wait();
            }
            redAntiTanks[i].readyUnitForNewTurn();
        }

        for (int i = 0; i < redInfantry.Length; i++)
        {
            if (currentPlayer == "Red" && redInfantry[i].active)
            {
                redInfantry[i].wait();
            }
            redInfantry[i].readyUnitForNewTurn();
        }

        for (int i = 0; i < redTanks.Length; i++)
        {
            if (currentPlayer == "Red" && redTanks[i].active)
            {
                redTanks[i].wait();
            }
            redTanks[i].readyUnitForNewTurn();
        }

        //Change Current Player and Increment Day (increment only if it was just blue's turn)
        if (currentPlayer == "Blue")
        {
            currentPlayer = "Red";

            int temp = day;
            temp++;
            if (temp > 999)
            {
                day = 0;
            }
            else
            {
                day = temp;
            }

        }
        else
        {
            currentPlayer = "Blue";
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

    public bool allRedUnitsDead()
    {
        for (int i = 0; i < redInfantry.Length; i++)
        {
            if (redInfantry[i].alive)
            {
                return false;
            }
        }

        for (int i = 0; i < redAntiTanks.Length; i++)
        {
            if (redAntiTanks[i].alive)
            {
                return false;
            }
        }

        for (int i = 0; i < redTanks.Length; i++)
        {
            if (redTanks[i].alive)
            {
                return false;
            }
        }

        return true;
    }

    public bool allBlueUnitsDead()
    {
        for (int i = 0; i < blueInfantry.Length; i++)
        {
            if (blueInfantry[i].alive)
            {
                return false;
            }
        }

        for (int i = 0; i < blueAntiTanks.Length; i++)
        {
            if (blueAntiTanks[i].alive)
            {
                return false;
            }
        }

        for (int i = 0; i < blueTanks.Length; i++)
        {
            if (blueTanks[i].alive)
            {
                return false;
            }
        }

        return true;
    }

    public void storeBeginningOfSelectedUnitInfo()
    {
        if (currentInfantry != null)
        {
            unitPositionBeforeMoving = currentInfantry.transform.position;
            cursorPositionBeforeMoving = cursor.transform.position;
        }
        else if (currentAntiTank != null)
        {
            unitPositionBeforeMoving = currentAntiTank.transform.position;
            cursorPositionBeforeMoving = cursor.transform.position;
        }
        else if (currentTank != null)
        {
            unitPositionBeforeMoving = currentTank.transform.position;
            cursorPositionBeforeMoving = cursor.transform.position;
        }
    }

    public void revertToBeforeMovingUnit()
    {
        if (currentInfantry != null)
        {
            currentInfantry.animatorToIdleActive();
            currentInfantry.selected = false;
            currentInfantry.movementPoints = currentInfantry.maxMovementPoints;
            currentInfantry.setCurrentTileToUnoccupied();
            currentInfantry.transform.position = unitPositionBeforeMoving;
            currentInfantry.storeTileAtCurrentPosition();
            currentInfantry.setCurrentTileToOccupied();
            currentInfantry.setDefenseModifierToCurrentTileValue();
            cursor.transform.position = cursorPositionBeforeMoving;
        }
        else if (currentAntiTank != null)
        {
            currentAntiTank.animatorToIdleActive();
            currentAntiTank.selected = false;
            currentAntiTank.movementPoints = currentAntiTank.maxMovementPoints;
            currentAntiTank.setCurrentTileToUnoccupied();
            currentAntiTank.transform.position = unitPositionBeforeMoving;
            currentAntiTank.storeTileAtCurrentPosition();
            currentAntiTank.setCurrentTileToOccupied();
            currentAntiTank.setDefenseModifierToCurrentTileValue();
            cursor.transform.position = cursorPositionBeforeMoving;
        }
        else if (currentTank != null)
        {
            currentTank.animatorToIdleActive();
            currentTank.selected = false;
            currentTank.movementPoints = currentTank.maxMovementPoints;
            currentTank.setCurrentTileToUnoccupied();
            currentTank.transform.position = unitPositionBeforeMoving;
            currentTank.storeTileAtCurrentPosition();
            currentTank.setCurrentTileToOccupied();
            currentTank.setDefenseModifierToCurrentTileValue();
            cursor.transform.position = cursorPositionBeforeMoving;
        }
    }

    public void revertToBeforeFiring()
    {
        if (attackingInfantry != null)
        {
            directionIterator = 0;
            cursor.transform.position = tempCursorPosition;

            storeUnitAtCursorPosition();

            directions.Clear();
            attackingInfantry = null;
            attackingAntiTank = null;
            attackingTank = null;

            currentInfantry.startRunningBecauseSelected();
        }
        else if (attackingAntiTank != null)
        {
            directionIterator = 0;
            cursor.transform.position = tempCursorPosition;

            storeUnitAtCursorPosition();

            directions.Clear();
            attackingInfantry = null;
            attackingAntiTank = null;
            attackingTank = null;

            currentAntiTank.startRunningBecauseSelected();
        }
        else if (attackingTank != null)
        {
            directionIterator = 0;
            cursor.transform.position = tempCursorPosition;

            storeUnitAtCursorPosition();

            directions.Clear();
            attackingInfantry = null;
            attackingAntiTank = null;
            attackingTank = null;

            currentTank.startRunningBecauseSelected();
        }
    }
}
