using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 5.16.21
 * Game Design Project
 */

public class MovRemUI : MonoBehaviour
{
    public CentralGameLogic centralGameLogic;
    public SpriteRenderer movementRemainingInt;

    public GenericDisappearReappearScript[] thingsToMakeDissappear;

    public Sprite[] integers;

    private float r;
    private float g;
    private float b;
    private float defaultAlpha;

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
        if (centralGameLogic.currentInfantry != null)
        {
            movementRemainingInt.sprite = integers[centralGameLogic.currentInfantry.movementPoints];

        } else if (centralGameLogic.currentAntiTank != null)
        {
            movementRemainingInt.sprite = integers[centralGameLogic.currentAntiTank.movementPoints];

        } else if (centralGameLogic.currentTank != null)
        {
            movementRemainingInt.sprite = integers[centralGameLogic.currentTank.movementPoints];
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
