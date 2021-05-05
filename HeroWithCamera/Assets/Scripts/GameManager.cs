using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
    public Text waypointHiddenText = null;
    public Text waypointSettingText = null;
    public Text enemyCountText = null;


    public bool readyToShake = true;
    public bool readyToChase = true;
    public Camera chaseCam = null;
    public HeroBehavior hero = null;
    EnemyBehavior[] allEnemies;

    public void updateEnemyCount(int count)
    {
        enemyCountText.text = "Enemies on Screen: " + count;
    }

    public void updateWaypointVisText(bool visible)
    {
        if (visible)
        {
            waypointHiddenText.text = "Waypoints: Visible";
        }
        else
        {
            waypointHiddenText.text = "Waypoints: Hidden";
        }
    }

    public void updateWaypointOrderText(bool ordered)
    {
        if (ordered)
        {
            waypointSettingText.text = "Waypoints: Ordered";
        }
        else
        {
            waypointSettingText.text = "Waypoints: Random";
        }
    }

    public void setChaseCamSize()
    {
        //Vector3 enemyPos = getOneTrueEnemy().transform.position;
        Vector3 temp = chaseCam.transform.position;
        temp.x = (hero.transform.position.x + getOneTrueEnemy().transform.position.x) * 0.5f;
        temp.y = (hero.transform.position.y + getOneTrueEnemy().transform.position.y) * 0.5f;
        chaseCam.orthographicSize = (hero.transform.position - getOneTrueEnemy().transform.position).magnitude;
        chaseCam.transform.position = temp;
    }

    public void resetChaseCam()
    {
        chaseCam.orthographicSize = 8;
    }

    public void setTrueEnemy(int id)
    {
        readyToChase = false;
        hero.beingChased = true;
        foreach (EnemyBehavior focus in allEnemies)
        {
            if (focus.GetInstanceID() == id)
            {
                focus.theOneTrueEnemy = true;
            } else
            {
                focus.theOneTrueEnemy = false;
            }
        }
    }

    public void clearTrueEnemy()
    {
        readyToChase = true;
        hero.beingChased = false;
        foreach (EnemyBehavior focus in allEnemies)
        {
            focus.theOneTrueEnemy = false;
        }
    }

    public EnemyBehavior getOneTrueEnemy()
    {
        foreach (EnemyBehavior focus in allEnemies)
        {
            if (focus.theOneTrueEnemy == true)
            {
                return focus;
            }
        }
        Debug.Log("Error: couldn't find true enemy!");
        return null;
    }

    public bool isThereATrueEnemy()
    {
        foreach (EnemyBehavior focus in allEnemies)
        {
            if (focus.theOneTrueEnemy == true)
            {
                return true;
            }
        }
        return false;
    }

    //PISAN'S STUFF BEGINS
    public static GameManager sTheGlobalBehavior = null;

    public Text mGameStateEcho = null;  // Defined in UnityEngine.UI
    public HeroBehavior mHero = null;
    public WayPointSystem mWayPoints = null;
    private EnemySpawnSystem mEnemySystem = null;

    private CameraSupport mMainCamera;
    
    // Use this for initialization
    void Start () {
        GameManager.sTheGlobalBehavior = this;  // Singleton pattern

        // This must occur before EnemySystem's Start();
        Debug.Assert(mWayPoints != null);
        Debug.Assert(mHero != null);

        mMainCamera = Camera.main.GetComponent<CameraSupport>();
        Debug.Assert(mMainCamera != null);

        Bounds b = mMainCamera.GetWorldBound();

        mEnemySystem = new EnemySpawnSystem(b.min, b.max);
        // Make sure all enemy sees the same EnemySystem and WayPointSystem
        EnemyBehavior.InitializeEnemySystem(mEnemySystem, mWayPoints);
        mEnemySystem.GenerateEnemy();  // Can only create enemies when WayPoint is initialized in EnemyBehavior

        //REES
        allEnemies = GameObject.FindObjectsOfType<EnemyBehavior>();
    }
    
	void Update () {
        EchoGameState(); // always do this

        if (Input.GetKey(KeyCode.Q))
            Application.Quit();

        Debug.Log("Current One True Enemy: " + getOneTrueEnemy().GetInstanceID());
    }


    #region Bound Support
    public CameraSupport.WorldBoundStatus CollideWorldBound(Bounds b) { return mMainCamera.CollideWorldBound(b); }
    #endregion 

    private void EchoGameState()
    {
        mGameStateEcho.text =  mWayPoints.GetWayPointState() + "  " + 
                               mHero.GetHeroState() + "  " + 
                               mEnemySystem.GetEnemyState();
    }
}