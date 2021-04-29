using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
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
    }
    
	void Update () {
        EchoGameState(); // always do this

        if (Input.GetKey(KeyCode.Q))
            Application.Quit();
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