using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnUIScript : MonoBehaviour
{
    public CentralGameLogic centralGameLogic;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    private float r;
    private float g;
    private float b;
    private float defaultAlpha;
    private Vector3 leftSidePosition = new Vector3(-4.75f, 3.75f, 0f);
    private Vector3 rightSidePosition = new Vector3(5.75f, 3.75f, 0f);

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

        spriteRenderer.sprite = sprites[centralGameLogic.day - 1];
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
