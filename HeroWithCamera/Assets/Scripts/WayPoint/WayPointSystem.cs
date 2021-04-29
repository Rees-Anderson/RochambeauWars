using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointSystem : MonoBehaviour
{
    private string[] kWayPointNames = {
            "WayPointA", "WayPointB", "WayPointC",
            "WayPointD", "WayPointE", "WayPointF"};
    private GameObject[] mWayPoints;
    private const int kNumWayPoints = 6;
    private const float kWayPointTouchDistance = 25f;
    private bool mPointsInSequence = true;

    // Start is called before the first frame update
    void Awake()
    {
        mWayPoints = new GameObject[kWayPointNames.Length];
        int i = 0;
        foreach (string s in kWayPointNames)
        {
            mWayPoints[i] = GameObject.Find(kWayPointNames[i]);
            Debug.Assert(mWayPoints[i] != null);
            i++;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleVisibility();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleSequenceOrder();
        }
    }

    private void ToggleVisibility()
    {
        foreach (GameObject g in mWayPoints)
            g.SetActive(!g.activeSelf);
    }

    private void ToggleSequenceOrder()
    {
        mPointsInSequence = !mPointsInSequence;
    }
    public bool WayPointInSequence() { return mPointsInSequence; }

    public void CheckNextWayPoint(Vector3 p, ref int index)
    {
        if (Vector3.Distance(p, mWayPoints[index].transform.position) < kWayPointTouchDistance)
        {
            if (mPointsInSequence)
            {
                index++;
                if (index >= kNumWayPoints)
                    index = 0;
            } else
            {
                index = Random.Range(0, 5);
            }
        }
    }

    public int GetInitWayIndex()
    {
        return Random.Range(0, mWayPoints.Length);
    }

    // Update is called once per frame
    public Vector3 WayPoint(int index) 
    {
        return mWayPoints[index].transform.position;
    }

    public string GetWayPointState() { return "  WAYPOINTS:(" + (WayPointInSequence() ? "Sequence" : "Random") + ")"; }
}
