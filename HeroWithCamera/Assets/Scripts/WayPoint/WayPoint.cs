using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    private Vector3 mInitPosition = Vector3.zero;
    private int mHitCount = 0;
    private const int kHitLimit = 3;
    private const float kRepositionRange = 15f; // +- this value
    private Color mNormalColor = Color.white;
    public GameObject waypointCam = null;
    
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
                //Focus camera on this waypoint
                Vector3 pos = transform.position;
                pos.z = -10;
                waypointCam.transform.position = pos;

                
                if (mHitCount == 1)
                {
                    CallShake(1);
                    
                }
                if (mHitCount == 2)
                {
                    CallShake(2);
                    
                }
                if (mHitCount == 3)
                {
                    CallShake(3);
                    
                }
                
                //Turn Camera off

            }
        }
    }

    public void CallShake(float duration)
    {
        StartCoroutine(Shake(duration));
    }

    public IEnumerator Shake(float duration)
    {
        waypointCam.SetActive(true);
        Vector3 originalPos = transform.position;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range((originalPos.x - 1f), (originalPos.x + 1f));
            float y = Random.Range((originalPos.y - 1f), (originalPos.y + 1f));

            transform.position = new Vector3(x, y, originalPos.z);
            //originalPos = transform.position;

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPos;
        waypointCam.SetActive(false);
    }
}
