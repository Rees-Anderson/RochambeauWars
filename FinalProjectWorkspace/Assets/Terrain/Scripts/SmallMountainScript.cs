using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 5.14.21
 * Game Design Project
 */

public class SmallMountainScript : MonoBehaviour
{
    public CentralGameLogic centralGameLogic;

    public const int movementCost = 4;
    public const int defenseModifier = 3;
    public const bool passableByTank = true;
    public const bool capturable = false;

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
    }
}