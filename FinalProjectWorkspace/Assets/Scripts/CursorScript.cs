using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 5.16.21
 * Game Design Project
 */

public class CursorScript : MonoBehaviour
{
    private float r;
    private float g;
    private float b;
    private float defaultAlpha;

    public Vector3 defaultPos = new Vector3(-1.5f, -1.5f, 0);

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
        
    }

    public void goToDefaultPosition()
    {
        transform.position = defaultPos;
    }

    public void moveLeft()
    {
        if (transform.position.x > -6.5)
        {
            Vector3 pos = transform.position;
            pos.x = pos.x - 1;
            transform.position = pos;
        }
    }

    public void moveRight()
    {
        if (transform.position.x < 7.5)
        {
            Vector3 pos = transform.position;
            pos.x = pos.x + 1;
            transform.position = pos;
        }
    }

    public void moveUp()
    {
        if (transform.position.y < 4.5)
        {
            Vector3 pos = transform.position;
            pos.y = pos.y + 1;
            transform.position = pos;
        }
    }

    public void moveDown()
    {
        if (transform.position.y > -4.5)
        {
            Vector3 pos = transform.position;
            pos.y = pos.y - 1;
            transform.position = pos;
        }
    }

    public void dissappear()
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, 0);
    }

    public void reappear()
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, defaultAlpha);
    }
}
