using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Rees Anderson
 * 5.15.21
 * Game Design Project
 */

public class CursorScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void moveLeft()
    {
        Vector3 pos = transform.position;
        pos.x = pos.x - 1;
        transform.position = pos;
    }

    public void moveRight()
    {
        Vector3 pos = transform.position;
        pos.x = pos.x + 1;
        transform.position = pos;
    }

    public void moveUp()
    {
        Vector3 pos = transform.position;
        pos.y = pos.y + 1;
        transform.position = pos;
    }

    public void moveDown()
    {
        Vector3 pos = transform.position;
        pos.y = pos.y - 1;
        transform.position = pos;
    }
}
