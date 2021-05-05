using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroBehavior : MonoBehaviour 
{
    public Text chaseCamText = null;
    public Text numEggsText = null;
    public Text enemiesDestroyedText = null;
    public Text timesHitText = null;
    public Text controlTypeText = null;

    public int numEggs = 0;
    public int enemiesDestroyed = 0;
    public int timesHit = 0;

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
            chaseCamText.text = "Enemy Cam: On";
            //theManager.readyToChase = false;
            theManager.setChaseCamSize();
        }

        if (!beingChased)
        {
            theManager.resetChaseCam();
            chaseCam.SetActive(false);
            chaseCamText.text = "Enemy Cam: Off";
            //theManager.readyToChase = true;
        }
        
    }

    public void MoveCamera()
    {
        Vector3 pos = transform.position;
        pos.z = -10;
        heroCam.transform.position = Vector3.Lerp(heroCam.transform.position, pos, 0.125f);
        //heroCam.transform.position = pos;
    }

    public int EggsOnScreen() { return mEggSystem.GetEggCount();  }

    private void UpdateMotion()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            mMouseDrive = !mMouseDrive;
            if (mMouseDrive)
            {
                controlTypeText.text = "Hero Control: Mouse";
            }
            else
            {
                controlTypeText.text = "Hero Control: Keyboard";
            }
        }
            
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
            {
                mEggSystem.SpawnAnEgg(transform.position, transform.up);
                incrementEggsOnScreen();
                CallShake(1, 1);
            }
        }
    }

    public void incrementTimesHit()
    {
        timesHit++;
        timesHitText.text = "Hero Hit: " + timesHit + " Times";
    }

    public void incrementEnemiesDestroyed()
    {
        enemiesDestroyed++;
        enemiesDestroyedText.text = "Enemies Destroyed: " + enemiesDestroyed;
    }

    public void incrementEggsOnScreen()
    {
        numEggs++;
        numEggsText.text = "Eggs on Screen: " + numEggs;
    }

    public void decrementEggsOnScreen()
    {
        numEggs--;
        numEggsText.text = "Eggs on Screen: " + numEggs;
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

    public void CallShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = heroCam.transform.position;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            originalPos = heroCam.transform.position;
            float x = Random.Range((originalPos.x - (1f * magnitude)), (originalPos.x + (1f * magnitude)));
            float y = Random.Range((originalPos.y - (1f * magnitude)), (originalPos.y + (1f * magnitude)));

            heroCam.transform.position = new Vector3(x, y, originalPos.z);
            //originalPos = transform.position;

            elapsed += Time.deltaTime;

            yield return null;
        }

        heroCam.transform.position = originalPos;

    }
}