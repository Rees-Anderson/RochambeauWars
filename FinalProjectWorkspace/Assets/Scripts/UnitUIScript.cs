using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 5.30.21
 * Game Design Project
 */

public class UnitUIScript : MonoBehaviour
{
    public CentralGameLogic centralGameLogic;

    public bool allowedToReappear = true;

    public Sprite[] smallIntegers;
    public Sprite[] unitVisualSprites;
    public Sprite[] unitTextSprites;

    public SpriteRenderer unitText;
    public SpriteRenderer unitSprite;

    public SpriteRenderer health10sPlace;
    public SpriteRenderer health1sPlace;
    public SpriteRenderer ammo10sPlace;
    public SpriteRenderer ammo1sPlace;
    public SpriteRenderer fuel10sPlace;
    public SpriteRenderer fuel1sPlace;

    public GenericDisappearReappearScript text;
    public GenericDisappearReappearScript visual;
    public GenericDisappearReappearScript healthTens;
    public GenericDisappearReappearScript healthOnes;
    public GenericDisappearReappearScript ammoTens;
    public GenericDisappearReappearScript ammoOnes;
    public GenericDisappearReappearScript fuelTens;
    public GenericDisappearReappearScript fuelOnes;
    public GenericDisappearReappearScript heart;
    public GenericDisappearReappearScript bullet;
    public GenericDisappearReappearScript fuel;
    public GenericDisappearReappearScript dash1;
    public GenericDisappearReappearScript dash2;
    public GenericDisappearReappearScript dash3;

    private float r;
    private float g;
    private float b;
    private float defaultAlpha;
    private Vector3 leftSidePosition = new Vector3(-3.25f, -3.0f, 0f);
    private Vector3 rightSidePosition = new Vector3(4.25f, -3.0f, 0f);

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

        updateUnitImageAndText();
    }

    public void dissappear()
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, 0);
        text.dissappear();
        visual.dissappear();
        healthTens.dissappear();
        healthOnes.dissappear();
        ammoTens.dissappear();
        ammoOnes.dissappear();
        fuelTens.dissappear();
        fuelOnes.dissappear();
        heart.dissappear();
        bullet.dissappear();
        fuel.dissappear();
        dash1.dissappear();
        dash2.dissappear();
        dash3.dissappear();
    }

    public void reappear()
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, defaultAlpha);
        text.reappear();
        visual.reappear();
        healthTens.reappear();
        healthOnes.reappear();
        ammoTens.reappear();
        ammoOnes.reappear();
        fuelTens.reappear();
        fuelOnes.reappear();
        heart.reappear();
        bullet.reappear();
        fuel.reappear();
        dash1.reappear();
        dash2.reappear();
        dash3.reappear();
    }

    public void updateUnitImageAndText()
    {
        if (centralGameLogic.currentInfantry != null)
        {
            if (centralGameLogic.currentInfantry.tag == "Red")
            {
                unitSprite.sprite = unitVisualSprites[0];
            }
            else
            {
                unitSprite.sprite = unitVisualSprites[3];
            }

            health10sPlace.sprite = smallIntegers[(centralGameLogic.currentInfantry.health / 10)];
            health1sPlace.sprite = smallIntegers[(centralGameLogic.currentInfantry.health % 10)];

            ammo10sPlace.sprite = smallIntegers[(centralGameLogic.currentInfantry.ammoCount / 10)];
            ammo1sPlace.sprite = smallIntegers[(centralGameLogic.currentInfantry.ammoCount % 10)];

            fuel10sPlace.sprite = smallIntegers[(centralGameLogic.currentInfantry.fuelLevel / 10)];
            fuel1sPlace.sprite = smallIntegers[(centralGameLogic.currentInfantry.fuelLevel % 10)];

            unitText.sprite = unitTextSprites[0];

            if (allowedToReappear)
            {
                reappear();
            }
        }
        else if (centralGameLogic.currentAntiTank != null)
        {
            if (centralGameLogic.currentAntiTank.tag == "Red")
            {
                unitSprite.sprite = unitVisualSprites[1];
            }
            else
            {
                unitSprite.sprite = unitVisualSprites[4];
            }

            health10sPlace.sprite = smallIntegers[(centralGameLogic.currentAntiTank.health / 10)];
            health1sPlace.sprite = smallIntegers[(centralGameLogic.currentAntiTank.health % 10)];

            ammo10sPlace.sprite = smallIntegers[(centralGameLogic.currentAntiTank.ammoCount / 10)];
            ammo1sPlace.sprite = smallIntegers[(centralGameLogic.currentAntiTank.ammoCount % 10)];

            fuel10sPlace.sprite = smallIntegers[(centralGameLogic.currentAntiTank.fuelLevel / 10)];
            fuel1sPlace.sprite = smallIntegers[(centralGameLogic.currentAntiTank.fuelLevel % 10)];

            unitText.sprite = unitTextSprites[1];

            if (allowedToReappear)
            {
                reappear();
            }
        }
        else if (centralGameLogic.currentTank != null)
        {
            if (centralGameLogic.currentTank.tag == "Red")
            {
                unitSprite.sprite = unitVisualSprites[2];
            }
            else
            {
                unitSprite.sprite = unitVisualSprites[5];
            }

            health10sPlace.sprite = smallIntegers[(centralGameLogic.currentTank.health / 10)];
            health1sPlace.sprite = smallIntegers[(centralGameLogic.currentTank.health % 10)];

            ammo10sPlace.sprite = smallIntegers[(centralGameLogic.currentTank.ammoCount / 10)];
            ammo1sPlace.sprite = smallIntegers[(centralGameLogic.currentTank.ammoCount % 10)];

            fuel10sPlace.sprite = smallIntegers[(centralGameLogic.currentTank.fuelLevel / 10)];
            fuel1sPlace.sprite = smallIntegers[(centralGameLogic.currentTank.fuelLevel % 10)];

            unitText.sprite = unitTextSprites[2];

            if (allowedToReappear)
            {
                reappear();
            }
        }
        else
        {
            dissappear();
        }
    }
}
