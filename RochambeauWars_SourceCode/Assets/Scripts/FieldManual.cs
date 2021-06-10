using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 6.8.21
 * Game Design Project
 */

public class FieldManual : MonoBehaviour
{
    public MainMenuLogic MainMenuLogic;

    public GenericDisappearReappearScript[] thingsToMakeDissappear;

    public Sprite[] manualPages;

    public SpriteRenderer page;

    public int currentPage;

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
        if (MainMenuLogic.state == "Field Manual")
        {
            reappear();
        }
        else
        {
            dissappear();
        }

        page.sprite = manualPages[currentPage];
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
