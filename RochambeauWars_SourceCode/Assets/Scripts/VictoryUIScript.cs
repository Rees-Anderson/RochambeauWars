using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 5.30.21
 * Game Design Project
 */

public class VictoryUIScript : MonoBehaviour
{
    public CentralGameLogic centralGameLogic;
    public SpriteRenderer frame;
    public SpriteRenderer text;

    public Sprite[] blueOrRedFrame;
    public Sprite[] blueOrRedText;

    public VictoryUICursorScript menuArrow;

    public GenericDisappearReappearScript[] thingsToMakeDissappear;

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
        if (centralGameLogic.state == "victory")
        {
            if (centralGameLogic.allRedUnitsDead() || centralGameLogic.redHQ.tag == "Blue")
            {
                frame.sprite = blueOrRedFrame[0];
                text.sprite = blueOrRedText[0];
            }
            else
            {
                frame.sprite = blueOrRedFrame[1];
                text.sprite = blueOrRedText[1];
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
        menuArrow.dissappear();
        for (int i = 0; i < thingsToMakeDissappear.Length; i++)
        {
            thingsToMakeDissappear[i].dissappear();
        }
    }

    public void reappear()
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, defaultAlpha);
        menuArrow.reappear();
        for (int i = 0; i < thingsToMakeDissappear.Length; i++)
        {
            thingsToMakeDissappear[i].reappear();
        }
    }
}
