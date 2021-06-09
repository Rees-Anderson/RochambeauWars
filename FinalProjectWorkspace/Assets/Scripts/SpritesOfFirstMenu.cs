using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 6.8.21
 * Game Design Project
 */

public class SpritesOfFirstMenu : MonoBehaviour
{
    public MainMenuLogic MainMenuLogic;

    public GenericDisappearReappearScript[] thingsToMakeDissappear;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (MainMenuLogic.state == "Default")
        {
            reappear();
        }
        else
        {
            dissappear();
        }
    }

    public void dissappear()
    {
        for (int i = 0; i < thingsToMakeDissappear.Length; i++)
        {
            thingsToMakeDissappear[i].dissappear();
        }
    }

    public void reappear()
    {
        for (int i = 0; i < thingsToMakeDissappear.Length; i++)
        {
            thingsToMakeDissappear[i].reappear();
        }
    }
}
