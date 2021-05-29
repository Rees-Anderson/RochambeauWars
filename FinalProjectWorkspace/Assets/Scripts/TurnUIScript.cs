using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 5.29.21
 * Game Design Project
 */

public class TurnUIScript : MonoBehaviour
{
    public CentralGameLogic centralGameLogic;
    public SpriteRenderer spriteRenderer;

    public GenericDisappearReappearScript[] thingsToMakeDissappear;

    public Sprite[] blueOrRedFrame;
    public Sprite[] integers;
    public Sprite[] blueOrRedText;

    public Turn1sPlace onesPlace;
    public Turn10sPlace tensPlace;
    public Turn100sPlace hundredsPlace;

    public SpriteRenderer turnTextSprite;

    public SpriteRenderer hundredsPlaceSprite;
    public SpriteRenderer tensPlaceSprite;
    public SpriteRenderer onesPlaceSprite;

    private float r;
    private float g;
    private float b;
    private float defaultAlpha;
    private Vector3 leftSidePosition = new Vector3(-4.6f, 3.815f, 0f);
    private Vector3 rightSidePosition = new Vector3(5.54f, 3.815f, 0f);

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
        if(centralGameLogic.cursor.transform.position.x < 0 && centralGameLogic.cursor.transform.position.y > 0)
        {
            transform.position = rightSidePosition;
        } else if (centralGameLogic.cursor.transform.position.x > 0 && centralGameLogic.cursor.transform.position.y > 0)
        {
            transform.position = leftSidePosition;
        }

        if (centralGameLogic.currentPlayer == "Blue")
        {
            spriteRenderer.sprite = blueOrRedFrame[0];
            turnTextSprite.sprite = blueOrRedText[0];
        }
        else
        {
            spriteRenderer.sprite = blueOrRedFrame[1];
            turnTextSprite.sprite = blueOrRedText[1];
        }

        hundredsPlaceSprite.sprite = integers[centralGameLogic.day / 100];
        tensPlaceSprite.sprite = integers[(centralGameLogic.day % 100) / 10];
        onesPlaceSprite.sprite = integers[((centralGameLogic.day % 100) % 10)];

        //spriteRenderer.sprite = sprites[centralGameLogic.day - 1];
    }

    public void dissappear()
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, 0);
        onesPlace.dissappear();
        tensPlace.dissappear();
        hundredsPlace.dissappear();
        for (int i = 0; i < thingsToMakeDissappear.Length; i++)
        {
            thingsToMakeDissappear[i].dissappear();
        }
    }

    public void reappear()
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, defaultAlpha);
        onesPlace.reappear();
        tensPlace.reappear();
        hundredsPlace.reappear();
        for (int i = 0; i < thingsToMakeDissappear.Length; i++)
        {
            thingsToMakeDissappear[i].reappear();
        }
    }
}
