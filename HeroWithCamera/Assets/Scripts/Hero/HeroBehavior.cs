using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroBehavior : MonoBehaviour 
{
    public GameObject chaseCam = null;
    public GameManager theManager = null;
    public bool beingChased = false;

    public EggSpawnSystem mEggSystem = null;
    private const float kHeroRotateSpeed = 90f/2f; // 90-degrees in 2 seconds
    private const float kHeroSpeed = 20f;  // 20-units in a second
    private float mHeroSpeed = kHeroSpeed;
    public GameObject heroCam = null;

    private bool mMouseDrive = true;
    //  Hero state
    private int mHeroTouchedEnemy = 0;
    private void TouchedEnemy() { mHeroTouchedEnemy++; }
    public string GetHeroState() { return "HERO: Drive(" + (mMouseDrive?"Mouse":"Key") + 
                                          ") TouchedEnemy(" + mHeroTouchedEnemy + ")   " 
                                            + mEggSystem.EggSystemStatus(); }

    private void Awake()
    {
        // Actually since Hero spwans eggs, this can be done in the Start() function, but, 
        // just to show this can also be done here.
        Debug.Assert(mEggSystem != null);
        EggBehavior.InitializeEggSystem(mEggSystem);
    }

    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update () 
    {
        UpdateMotion();
        ProcessEggSpwan();
        MoveCamera();

        
        if (beingChased)
        {
            chaseCam.SetActive(true);
            //theManager.readyToChase = false;
            theManager.setChaseCamSize();
        }

        if (!beingChased)
        {
            theManager.resetChaseCam();
            chaseCam.SetActive(false);
            //theManager.readyToChase = true;
        }
        
    }

    public void MoveCamera()
    {
        Vector3 pos = transform.position;
        pos.z = -10;
        heroCam.transform.position = pos;
    }

    private int EggsOnScreen() { return mEggSystem.GetEggCount();  }

    private void UpdateMotion()
    {
        if (Input.GetKeyDown(KeyCode.M))
            mMouseDrive = !mMouseDrive;
            
        // Only support rotation
        transform.Rotate(Vector3.forward, -1f * Input.GetAxis("Horizontal") *
                                    (kHeroRotateSpeed * Time.smoothDeltaTime));
        if (mMouseDrive)
        {
            Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0f;
            transform.position = p;
        } else
        {
            mHeroSpeed += Input.GetAxis("Vertical");
            transform.position += transform.up * (mHeroSpeed * Time.smoothDeltaTime);
        }
    }

    private void ProcessEggSpwan()
    {
        if (mEggSystem.CanSpawn())
        {
            if (Input.GetKey("space") || Input.GetKey(KeyCode.Mouse0))
                mEggSystem.SpawnAnEgg(transform.position, transform.up);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hero touched");
        if (collision.gameObject.name == "Enemy(Clone)")
        {
            TouchedEnemy();

            theManager.readyToChase = false;
            //chaseCam.SetActive(true);
        }
    }
}