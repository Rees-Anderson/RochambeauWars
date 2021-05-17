using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCursorScript : MonoBehaviour
{
    private float r;
    private float g;
    private float b;
    private float defaultAlpha;

    public int currentPosition = 0;
    public Vector3[] positions = { new Vector3(-1.6f, 1.375f, 0f), new Vector3(-1.6f, 0.5f, 0f), new Vector3(-1.6f, -0.375f, 0f), new Vector3(-1.6f, -1.25f, 0f) };

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
        transform.position = positions[currentPosition];
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
