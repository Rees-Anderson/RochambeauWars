using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 6.4.21
 * Game Design Project
 */

public class BattleForcastScript : MonoBehaviour
{
    public CentralGameLogic centralGameLogic;

    public GenericDisappearReappearScript[] thingsToMakeDissappear;

    private float r;
    private float g;
    private float b;
    private float defaultAlpha;

    private Vector3 leftSidePosition = new Vector3(-3.25f, -0.5f, 0f);
    private Vector3 rightSidePosition = new Vector3(4.25f, -0.5f, 0f);

    public SpriteRenderer hundredsPlace;
    public SpriteRenderer tensPlace;
    public SpriteRenderer onesPlace;

    public Sprite[] integers;

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Renderer>().material.color.r;
        g = GetComponent<Renderer>().material.color.g;
        b = GetComponent<Renderer>().material.color.b;
        defaultAlpha = GetComponent<Renderer>().material.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if (centralGameLogic.cursor.transform.position.x < 0)
        {
            transform.position = rightSidePosition;
        }
        else if (centralGameLogic.cursor.transform.position.x > 0)
        {
            transform.position = leftSidePosition;
        }

        if ((centralGameLogic.state == "attack") && (centralGameLogic.attackingInfantry != null || centralGameLogic.attackingAntiTank != null || centralGameLogic.attackingTank != null) && (centralGameLogic.currentInfantry != null || centralGameLogic.currentAntiTank != null || centralGameLogic.currentTank != null))
        {
            if (centralGameLogic.attackingInfantry != null)
            {
                float damageToDealToDefender = 0;
                damageToDealToDefender += Mathf.Ceil(((float)centralGameLogic.attackingInfantry.health) / 2); //Add Base Attack : Ceiling of (Health / 2)
                damageToDealToDefender += centralGameLogic.attackBonus; //Add amount for attack bonus
                if (centralGameLogic.currentInfantry != null)
                {
                    damageToDealToDefender -= centralGameLogic.currentInfantry.currentDefenseModifier; //Subtract defense modifier

                    //Added since units can now do a minimum of 1 damage - 6.4.21
                    if (damageToDealToDefender <= 0)
                    {
                        damageToDealToDefender = 1;
                    }

                    int numberToDisplay = (int)((damageToDealToDefender / centralGameLogic.currentInfantry.health) * 100);
                    if (numberToDisplay < 0)
                    {
                        numberToDisplay = 0;
                    }
                    hundredsPlace.sprite = integers[numberToDisplay / 100];
                    tensPlace.sprite = integers[(numberToDisplay % 100) / 10];
                    onesPlace.sprite = integers[((numberToDisplay % 100) % 10)];
                }
                else if (centralGameLogic.currentAntiTank != null)
                {
                    damageToDealToDefender -= centralGameLogic.currentAntiTank.currentDefenseModifier; //Subtract defense modifier
                    damageToDealToDefender += centralGameLogic.typeMatchBonus; //Add type match bonus

                    //Added since units can now do a minimum of 1 damage - 6.4.21
                    if (damageToDealToDefender <= 0)
                    {
                        damageToDealToDefender = 1;
                    }

                    int numberToDisplay = (int)((damageToDealToDefender / centralGameLogic.currentAntiTank.health) * 100);
                    if (numberToDisplay < 0)
                    {
                        numberToDisplay = 0;
                    }
                    hundredsPlace.sprite = integers[numberToDisplay / 100];
                    tensPlace.sprite = integers[(numberToDisplay % 100) / 10];
                    onesPlace.sprite = integers[((numberToDisplay % 100) % 10)];
                }
                else if (centralGameLogic.currentTank != null)
                {
                    damageToDealToDefender -= centralGameLogic.currentTank.currentDefenseModifier; //Subtract defense modifier
                    damageToDealToDefender -= centralGameLogic.typeMatchPenalty; //Subtract poor type match penalty

                    //Added since units can now do a minimum of 1 damage - 6.4.21
                    if (damageToDealToDefender <= 0)
                    {
                        damageToDealToDefender = 1;
                    }

                    int numberToDisplay = (int)((damageToDealToDefender / centralGameLogic.currentTank.health) * 100);
                    if (numberToDisplay < 0)
                    {
                        numberToDisplay = 0;
                    }
                    hundredsPlace.sprite = integers[numberToDisplay / 100];
                    tensPlace.sprite = integers[(numberToDisplay % 100) / 10];
                    onesPlace.sprite = integers[((numberToDisplay % 100) % 10)];
                }
                else
                {
                    Debug.Log("Critical Error - Flow should not be here");
                }
            }
            else if (centralGameLogic.attackingAntiTank != null)
            {
                float damageToDealToDefender = 0;
                damageToDealToDefender += Mathf.Ceil(((float)centralGameLogic.attackingAntiTank.health) / 2); //Add Base Attack : Ceiling of (Health / 2)
                damageToDealToDefender += centralGameLogic.attackBonus; //Add amount for attack bonus
                if (centralGameLogic.currentInfantry != null)
                {
                    damageToDealToDefender -= centralGameLogic.currentInfantry.currentDefenseModifier; //Subtract defense modifier
                    damageToDealToDefender -= centralGameLogic.typeMatchPenalty; //Subtract poor type match penalty

                    //Added since units can now do a minimum of 1 damage - 6.4.21
                    if (damageToDealToDefender <= 0)
                    {
                        damageToDealToDefender = 1;
                    }

                    int numberToDisplay = (int)((damageToDealToDefender / centralGameLogic.currentInfantry.health) * 100);
                    if (numberToDisplay < 0)
                    {
                        numberToDisplay = 0;
                    }
                    hundredsPlace.sprite = integers[numberToDisplay / 100];
                    tensPlace.sprite = integers[(numberToDisplay % 100) / 10];
                    onesPlace.sprite = integers[((numberToDisplay % 100) % 10)];
                }
                else if (centralGameLogic.currentAntiTank != null)
                {
                    damageToDealToDefender -= centralGameLogic.currentAntiTank.currentDefenseModifier; //Subtract defense modifier

                    //Added since units can now do a minimum of 1 damage - 6.4.21
                    if (damageToDealToDefender <= 0)
                    {
                        damageToDealToDefender = 1;
                    }

                    int numberToDisplay = (int)((damageToDealToDefender / centralGameLogic.currentAntiTank.health) * 100);
                    if (numberToDisplay < 0)
                    {
                        numberToDisplay = 0;
                    }
                    hundredsPlace.sprite = integers[numberToDisplay / 100];
                    tensPlace.sprite = integers[(numberToDisplay % 100) / 10];
                    onesPlace.sprite = integers[((numberToDisplay % 100) % 10)];
                }
                else if (centralGameLogic.currentTank != null)
                {
                    damageToDealToDefender -= centralGameLogic.currentTank.currentDefenseModifier; //Subtract defense modifier
                    damageToDealToDefender += centralGameLogic.typeMatchBonus; //Add type match bonus

                    //Added since units can now do a minimum of 1 damage - 6.4.21
                    if (damageToDealToDefender <= 0)
                    {
                        damageToDealToDefender = 1;
                    }

                    int numberToDisplay = (int)((damageToDealToDefender / centralGameLogic.currentTank.health) * 100);
                    if (numberToDisplay < 0)
                    {
                        numberToDisplay = 0;
                    }
                    hundredsPlace.sprite = integers[numberToDisplay / 100];
                    tensPlace.sprite = integers[(numberToDisplay % 100) / 10];
                    onesPlace.sprite = integers[((numberToDisplay % 100) % 10)];
                }
                else
                {
                    Debug.Log("Critical Error - Flow should not be here");
                }
            }
            else if (centralGameLogic.attackingTank != null)
            {
                float damageToDealToDefender = 0;
                damageToDealToDefender += Mathf.Ceil(((float)centralGameLogic.attackingTank.health) / 2); //Add Base Attack : Ceiling of (Health / 2)
                damageToDealToDefender += centralGameLogic.attackBonus; //Add amount for attack bonus
                if (centralGameLogic.currentInfantry != null)
                {
                    damageToDealToDefender -= centralGameLogic.currentInfantry.currentDefenseModifier; //Subtract defense modifier
                    damageToDealToDefender += centralGameLogic.typeMatchBonus; //Add type match bonus

                    //Added since units can now do a minimum of 1 damage - 6.4.21
                    if (damageToDealToDefender <= 0)
                    {
                        damageToDealToDefender = 1;
                    }

                    int numberToDisplay = (int)((damageToDealToDefender / centralGameLogic.currentInfantry.health) * 100);
                    if (numberToDisplay < 0)
                    {
                        numberToDisplay = 0;
                    }
                    hundredsPlace.sprite = integers[numberToDisplay / 100];
                    tensPlace.sprite = integers[(numberToDisplay % 100) / 10];
                    onesPlace.sprite = integers[((numberToDisplay % 100) % 10)];
                }
                else if (centralGameLogic.currentAntiTank != null)
                {
                    damageToDealToDefender -= centralGameLogic.currentAntiTank.currentDefenseModifier; //Subtract defense modifier
                    damageToDealToDefender -= centralGameLogic.typeMatchPenalty; //Subtract poor type match penalty

                    //Added since units can now do a minimum of 1 damage - 6.4.21
                    if (damageToDealToDefender <= 0)
                    {
                        damageToDealToDefender = 1;
                    }

                    int numberToDisplay = (int)((damageToDealToDefender / centralGameLogic.currentAntiTank.health) * 100);
                    if (numberToDisplay < 0)
                    {
                        numberToDisplay = 0;
                    }
                    hundredsPlace.sprite = integers[numberToDisplay / 100];
                    tensPlace.sprite = integers[(numberToDisplay % 100) / 10];
                    onesPlace.sprite = integers[((numberToDisplay % 100) % 10)];
                }
                else if (centralGameLogic.currentTank != null)
                {
                    damageToDealToDefender -= centralGameLogic.currentTank.currentDefenseModifier; //Subtract defense modifier

                    //Added since units can now do a minimum of 1 damage - 6.4.21
                    if (damageToDealToDefender <= 0)
                    {
                        damageToDealToDefender = 1;
                    }

                    int numberToDisplay = (int)((damageToDealToDefender / centralGameLogic.currentTank.health) * 100);
                    if (numberToDisplay < 0)
                    {
                        numberToDisplay = 0;
                    }
                    hundredsPlace.sprite = integers[numberToDisplay / 100];
                    tensPlace.sprite = integers[(numberToDisplay % 100) / 10];
                    onesPlace.sprite = integers[((numberToDisplay % 100) % 10)];
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
            reappear();
        }
        else
        {
            dissappear();
        }
    }

    public void dissappear()
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, 0);
        for (int i = 0; i < thingsToMakeDissappear.Length; i++)
        {
            thingsToMakeDissappear[i].dissappear();
        }
    }

    public void reappear()
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, defaultAlpha);
        for (int i = 0; i < thingsToMakeDissappear.Length; i++)
        {
            thingsToMakeDissappear[i].reappear();
        }
    }
}
