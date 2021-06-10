using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 5.31.21
 * Game Design Project
 */

public class TerrainUIScript : MonoBehaviour
{
    public CentralGameLogic centralGameLogic;

    public Sprite[] defenseSprites;
    public Sprite[] movementSprites;
    public Sprite[] terrainTypeSprites;
    public Sprite[] terrainTypeVisualSprites;

    public SpriteRenderer terrainText;
    public SpriteRenderer terrainSprite;
    public SpriteRenderer DefenseValue;
    public SpriteRenderer MovementCost;

    public GenericDisappearReappearScript text;
    public GenericDisappearReappearScript visual;
    public GenericDisappearReappearScript defense;
    public GenericDisappearReappearScript movement;

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

        updateTerrainImageAndText();
    }

    public void dissappear()
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, 0);
        text.dissappear();
        visual.dissappear();
        defense.dissappear();
        movement.dissappear();
    }

    public void reappear()
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, defaultAlpha);
        text.reappear();
        visual.reappear();
        defense.reappear();
        movement.reappear();
    }

    public void updateTerrainImageAndText()
    {
        if (centralGameLogic.currentRiverTile != null)
        {
            terrainText.sprite = terrainTypeSprites[0];
            terrainSprite.sprite = terrainTypeVisualSprites[0];
            DefenseValue.sprite = defenseSprites[centralGameLogic.currentRiverTile.defenseModifier];
            MovementCost.sprite = movementSprites[centralGameLogic.currentRiverTile.movementCost - 1];

        } else if (centralGameLogic.currentGrassTile != null)
        {
            if (centralGameLogic.currentGrassTile.tag == "Bridge")
            {
                terrainText.sprite = terrainTypeSprites[8];
                terrainSprite.sprite = terrainTypeVisualSprites[9];
                DefenseValue.sprite = defenseSprites[centralGameLogic.currentGrassTile.defenseModifier];
                MovementCost.sprite = movementSprites[centralGameLogic.currentGrassTile.movementCost - 1];
            }
            else
            {
                terrainText.sprite = terrainTypeSprites[1];
                terrainSprite.sprite = terrainTypeVisualSprites[1];
                DefenseValue.sprite = defenseSprites[centralGameLogic.currentGrassTile.defenseModifier];
                MovementCost.sprite = movementSprites[centralGameLogic.currentGrassTile.movementCost - 1];
            }

        } else if (centralGameLogic.currentForestTile != null)
        {
            terrainText.sprite = terrainTypeSprites[2];
            terrainSprite.sprite = terrainTypeVisualSprites[2];
            DefenseValue.sprite = defenseSprites[centralGameLogic.currentForestTile.defenseModifier];
            MovementCost.sprite = movementSprites[centralGameLogic.currentForestTile.movementCost - 1];

        } else if (centralGameLogic.currentSmallMountainTile != null)
        {
            terrainText.sprite = terrainTypeSprites[3];
            terrainSprite.sprite = terrainTypeVisualSprites[3];
            DefenseValue.sprite = defenseSprites[centralGameLogic.currentSmallMountainTile.defenseModifier];
            MovementCost.sprite = movementSprites[centralGameLogic.currentSmallMountainTile.movementCost - 1];

        } else if (centralGameLogic.currentLargeMountainTile != null)
        {
            terrainText.sprite = terrainTypeSprites[4];
            terrainSprite.sprite = terrainTypeVisualSprites[4];
            DefenseValue.sprite = defenseSprites[centralGameLogic.currentLargeMountainTile.defenseModifier];
            MovementCost.sprite = movementSprites[centralGameLogic.currentLargeMountainTile.movementCost - 1];

        } else if (centralGameLogic.currentCityTile != null)
        {
            if (centralGameLogic.currentCityTile.tag == "Red")
            {
                terrainText.sprite = terrainTypeSprites[5];
                terrainSprite.sprite = terrainTypeVisualSprites[5];
                DefenseValue.sprite = defenseSprites[centralGameLogic.currentCityTile.defenseModifier];
                MovementCost.sprite = movementSprites[centralGameLogic.currentCityTile.movementCost - 1];
            }
            else if (centralGameLogic.currentCityTile.tag == "Blue")
            {
                terrainText.sprite = terrainTypeSprites[5];
                terrainSprite.sprite = terrainTypeVisualSprites[6];
                DefenseValue.sprite = defenseSprites[centralGameLogic.currentCityTile.defenseModifier];
                MovementCost.sprite = movementSprites[centralGameLogic.currentCityTile.movementCost - 1];
            }
            else
            {
                terrainText.sprite = terrainTypeSprites[5];
                terrainSprite.sprite = terrainTypeVisualSprites[11];
                DefenseValue.sprite = defenseSprites[centralGameLogic.currentCityTile.defenseModifier];
                MovementCost.sprite = movementSprites[centralGameLogic.currentCityTile.movementCost - 1];
            }

        } else if (centralGameLogic.currentHeadQuartersTile != null)
        {
            if (centralGameLogic.currentHeadQuartersTile.tag == "Red")
            {
                terrainText.sprite = terrainTypeSprites[6];
                terrainSprite.sprite = terrainTypeVisualSprites[7];
                DefenseValue.sprite = defenseSprites[centralGameLogic.currentHeadQuartersTile.defenseModifier];
                MovementCost.sprite = movementSprites[centralGameLogic.currentHeadQuartersTile.movementCost - 1];
            }
            else if (centralGameLogic.currentHeadQuartersTile.tag == "Blue")
            {
                terrainText.sprite = terrainTypeSprites[6];
                terrainSprite.sprite = terrainTypeVisualSprites[8];
                DefenseValue.sprite = defenseSprites[centralGameLogic.currentHeadQuartersTile.defenseModifier];
                MovementCost.sprite = movementSprites[centralGameLogic.currentHeadQuartersTile.movementCost - 1];
            }
            else if (centralGameLogic.currentHeadQuartersTile.name == "HeadQuarters")
            {
                terrainText.sprite = terrainTypeSprites[6];
                terrainSprite.sprite = terrainTypeVisualSprites[12];
                DefenseValue.sprite = defenseSprites[centralGameLogic.currentHeadQuartersTile.defenseModifier];
                MovementCost.sprite = movementSprites[centralGameLogic.currentHeadQuartersTile.movementCost - 1];
            }
            else
            {
                terrainText.sprite = terrainTypeSprites[6];
                terrainSprite.sprite = terrainTypeVisualSprites[13];
                DefenseValue.sprite = defenseSprites[centralGameLogic.currentHeadQuartersTile.defenseModifier];
                MovementCost.sprite = movementSprites[centralGameLogic.currentHeadQuartersTile.movementCost - 1];
            }

        } else
        {
            terrainText.sprite = terrainTypeSprites[7];
            terrainSprite.sprite = terrainTypeVisualSprites[10];
            DefenseValue.sprite = defenseSprites[0];
            MovementCost.sprite = movementSprites[0];
        }
    }
}
