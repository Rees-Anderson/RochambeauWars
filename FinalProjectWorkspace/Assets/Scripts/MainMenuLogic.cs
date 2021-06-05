using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 6.5.21
 * Game Design Project
 */

public class MainMenuLogic : MonoBehaviour
{
    public string state;

    public MenuChoicesUI menuChoicesUI;

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
                    menuChoicesUI.menuArrow.currentPosition = 0;
                    state = "Single Player Selector";

                }
                else if (menuChoicesUI.menuArrow.currentPosition == 1)
                {
                    //Go to Mulitplayer Selector
                    menuChoicesUI.menuArrow.currentPosition = 0;
                    state = "Multiplayer Selector";
                }
                else if (menuChoicesUI.menuArrow.currentPosition == 2)
                {
                    //Go to Field Manual
                    menuChoicesUI.menuArrow.currentPosition = 0;
                    state = "Field Manual";
                }
                else if (menuChoicesUI.menuArrow.currentPosition == 3)
                {
                    //Go to Credits
                    menuChoicesUI.menuArrow.currentPosition = 0;
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

        }
        else if (state == "Multiplayer Selector")
        {

        }
        else if (state == "Field Manual")
        {

        }
        else if (state == "Credits")
        {

        }
    }
}
