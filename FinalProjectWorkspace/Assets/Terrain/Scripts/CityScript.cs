using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 6.1.21
 * Game Design Project
 */

public class CityScript : MonoBehaviour
{
    public CentralGameLogic centralGameLogic;

    //public string owner = "Grey";

    public Animator animator;

    public Sprite[] blueRedGreyAppearance;
    public SpriteRenderer theCity;

    public int movementCost = 1;
    public int defenseModifier = 4;
    public bool passableByTank = true;
    public bool capturable = true;

    public bool occupied = false;

    private float r;
    private float g;
    private float b;
    private float defaultAlpha;


    // Start is called before the first frame update
    void Start()
    {
        centralGameLogic = GameObject.FindObjectOfType<CentralGameLogic>();

        //Make logic tile clear
        r = GetComponent<Renderer>().material.color.r;
        g = GetComponent<Renderer>().material.color.g;
        b = GetComponent<Renderer>().material.color.b;
        defaultAlpha = GetComponent<Renderer>().material.color.a;
        GetComponent<Renderer>().material.color = new Color(r, g, b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //If a unit is selected make this logic tile appear, else stay invisible
        if (centralGameLogic.state == "selectedUnit")
        {
            GetComponent<Renderer>().material.color = new Color(r, g, b, defaultAlpha);
        }
        else
        {
            GetComponent<Renderer>().material.color = new Color(r, g, b, 0);
        }

        if (tag == "Red")
        {
            //theCity.sprite = blueRedGreyAppearance[0];
            animator.SetBool("isBlue", false);
            animator.SetBool("isRed", true);
        }
        else if (tag == "Blue")
        {
            //theCity.sprite = blueRedGreyAppearance[1];
            animator.SetBool("isBlue", true);
            animator.SetBool("isRed", false);
        }
        else if (tag == "Grey")
        {
            //theCity.sprite = blueRedGreyAppearance[2];
            animator.SetBool("isBlue", false);
            animator.SetBool("isRed", false);
        }
    }
}
