using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 5.15.21
 * Game Design Project
 */

public class TerrainUIScript : MonoBehaviour
{
    public CentralGameLogic centralGameLogic;

    public Sprite[] defenseSprites;
    public Sprite[] movementSprites;
    public Sprite[] terrainTypeSprites;
    public Sprite[] terrainTypeVisualSprites;

    public SpriteRenderer terrainSprite;
    public SpriteRenderer DefenseValue;
    public SpriteRenderer MovementCost;

    private float r;
    private float g;
    private float b;
    private float defaultAlpha;
    private Vector3 leftSidePosition = new Vector3(-5.5f, -3.0f, 0f);
    private Vector3 rightSidePosition = new Vector3(6.5f, -3.0f, 0f);

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
