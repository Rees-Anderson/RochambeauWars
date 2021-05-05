using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WayPoint : MonoBehaviour
{
    public Text waypointCamText = null;

    private Vector3 mInitPosition = Vector3.zero;
    private int mHitCount = 0;
    private const int kHitLimit = 3;
    private const float kRepositionRange = 15f; // +- this value
    private Color mNormalColor = Color.white;
    public GameObject waypointCam = null;
    public GameManager theManager = null;
    
    // Start is called before the first frame update
    void Start()
    {
        waypointCam.SetActive(false);
        mInitPosition = transform.position;
    }


    private void Reposition() 
    {
        Vector3 p = mInitPosition;
        p += new Vector3(Random.Range(-kRepositionRange, kRepositionRange),
                         Random.Range(-kRepositionRange, kRepositionRange),
                         0f);
        transform.position = p;
        GetComponent<SpriteRenderer>().color = mNormalColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Egg(Clone)")
        {
            mHitCount++;
            Color c = mNormalColor * (float)(kHitLimit - mHitCount + 1) / (float)(kHitLimit + 1);
            GetComponent<SpriteRenderer>().color = c;
            
            if (mHitCount > kHitLimit)
            {
                mHitCount = 0;
                Reposition();
            }
            else
            {
                if (theManager.readyToShake)
                {
                    //Focus camera on this waypoint
                    Vector3 pos = transform.position;
                    pos.z = -10;
                    waypointCam.transform.position = pos;
                }
                
                if (mHitCount == 1)
                {
                    CallShake(1, 1);
                    
                }
                if (mHitCount == 2)
                {
                    CallShake(2, 2);
                    
                }
                if (mHitCount == 3)
                {
                    CallShake(3, 3);
                    
                }
                
                //Turn Camera off

            }
        }
    }

    public void CallShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        if (theManager.readyToShake)
        {
            waypointCam.SetActive(true);
            waypointCamText.text = "Waypoint Cam: On";
            theManager.readyToShake = false;
        }
        Vector3 originalPos = transform.position;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range((originalPos.x - (1f * magnitude)), (originalPos.x + (1f * magnitude)));
            float y = Random.Range((originalPos.y - (1f * magnitude)), (originalPos.y + (1f * magnitude)));

            transform.position = new Vector3(x, y, originalPos.z);
            //originalPos = transform.position;

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPos;

        waypointCam.SetActive(false);
        waypointCamText.text = "Waypoint Cam: Off";
        theManager.readyToShake = true;
    }
}
