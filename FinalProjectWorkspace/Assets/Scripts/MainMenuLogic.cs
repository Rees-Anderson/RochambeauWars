using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Author: Rees Anderson
 * 6.9.21
 * Game Design Project
 */

public class MainMenuLogic : MonoBehaviour
{
    public string state;

    public MenuChoicesUI menuChoicesUI;
    public MapSelector1P singlePlayerMapSelector;
    public MapSelectorMulti multiplayerMapSelector;
    public FieldManual fieldManual;

    // Start is called before the first frame update
    void Start()
    {
        state = "Default";
    }

    // Update is called once per frame
    void Update()
    {
        if (state == "Default")
        {
            //Menu Choices UI should pop up on its own

            //Pressing W - move cursor up one if possible
            if (Input.GetKeyDown(KeyCode.W) && menuChoicesUI.menuArrow.currentPosition > 0)
            {
                menuChoicesUI.menuArrow.currentPosition--;
            }

            //Pressing S - move cursor down one if possible
            if (Input.GetKeyDown(KeyCode.S) && menuChoicesUI.menuArrow.currentPosition < 4)
            {
                menuChoicesUI.menuArrow.currentPosition++;
            }

            //Pressing K - choose menu option
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (menuChoicesUI.menuArrow.currentPosition == 0)
                {
                    //Go to Single Player Selector
                    //menuChoicesUI.menuArrow.currentPosition = 0;
                    state = "Single Player Selector";

                }
                else if (menuChoicesUI.menuArrow.currentPosition == 1)
                {
                    //Go to Mulitplayer Selector
                    //menuChoicesUI.menuArrow.currentPosition = 0;
                    state = "Multiplayer Selector";
                }
                else if (menuChoicesUI.menuArrow.currentPosition == 2)
                {
                    //Go to Field Manual
                    //menuChoicesUI.menuArrow.currentPosition = 0;
                    state = "Field Manual";
                }
                else if (menuChoicesUI.menuArrow.currentPosition == 3)
                {
                    //Go to Credits
                    //menuChoicesUI.menuArrow.currentPosition = 0;
                    state = "Credits";
                }
                else if (menuChoicesUI.menuArrow.currentPosition == 4)
                {
                    //Quit the Game
                    Application.Quit();
                }
            }
        }
        else if (state == "Single Player Selector")
        {
            //Menu Choices UI should pop up on its own

            //Pressing W - move cursor up one if possible
            if (Input.GetKeyDown(KeyCode.W) && singlePlayerMapSelector.menuArrow.currentPosition > 0)
            {
                singlePlayerMapSelector.menuArrow.currentPosition--;
            }

            //Pressing S - move cursor down one if possible
            if (Input.GetKeyDown(KeyCode.S) && singlePlayerMapSelector.menuArrow.currentPosition < 2)
            {
                singlePlayerMapSelector.menuArrow.currentPosition++;
            }

            //Pressing K - choose menu option
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (singlePlayerMapSelector.menuArrow.currentPosition == 0)
                {
                    //Load Single Player Map-01
                }
                else if (singlePlayerMapSelector.menuArrow.currentPosition == 1)
                {
                    //Load Single Player Map-02
                }
                else if (singlePlayerMapSelector.menuArrow.currentPosition == 2)
                {
                    //Load Single Player Map-03
                }
            }

            //Pressing L - return to previous screen
            if (Input.GetKeyDown(KeyCode.L))
            {
                singlePlayerMapSelector.menuArrow.currentPosition = 0;
                state = "Default";
            }

        }
        else if (state == "Multiplayer Selector")
        {
            //Menu Choices UI should pop up on its own

            //Pressing W - move cursor up one if possible
            if (Input.GetKeyDown(KeyCode.W) && multiplayerMapSelector.menuArrow.currentPosition > 0)
            {
                multiplayerMapSelector.menuArrow.currentPosition--;
            }

            //Pressing S - move cursor down one if possible
            if (Input.GetKeyDown(KeyCode.S) && multiplayerMapSelector.menuArrow.currentPosition < 2)
            {
                multiplayerMapSelector.menuArrow.currentPosition++;
            }

            //Pressing K - choose menu option
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (multiplayerMapSelector.menuArrow.currentPosition == 0)
                {
                    //Load Multiplayer Map-01
                    SceneManager.LoadScene("Map-01");
                }
                else if (multiplayerMapSelector.menuArrow.currentPosition == 1)
                {
                    //Load Multiplayer Map-02
                    SceneManager.LoadScene("Map-02");
                }
                else if (multiplayerMapSelector.menuArrow.currentPosition == 2)
                {
                    //Load Multiplayer Map-03
                    SceneManager.LoadScene("Map-03");
                }
            }

            //Pressing L - return to previous screen
            if (Input.GetKeyDown(KeyCode.L))
            {
                multiplayerMapSelector.menuArrow.currentPosition = 0;
                state = "Default";
            }
        }
        else if (state == "Field Manual")
        {
            //Pressing A - go to previous page if possible
            if (Input.GetKeyDown(KeyCode.A) && fieldManual.currentPage > 0)
            {
                fieldManual.currentPage--;
            }

            //Pressing D - go to next page if possible
            if (Input.GetKeyDown(KeyCode.D) && fieldManual.currentPage < 13)
            {
                fieldManual.currentPage++;
            }

            //Pressing L - return to previous screen
            if (Input.GetKeyDown(KeyCode.L))
            {
                fieldManual.currentPage = 0;
                state = "Default";
            }
        }
        else if (state == "Credits")
        {
            //Pressing L - return to previous screen
            if (Input.GetKeyDown(KeyCode.L))
            {
                state = "Default";
            }
        }
    }
}
