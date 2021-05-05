using UnityEngine;
using System.Collections;

public partial class EnemyBehavior : MonoBehaviour 
{
    public GameObject chaseCam = null;
    public GameManager theManager = null;
    private HeroBehavior hero = null;
    public bool theOneTrueEnemy = false; //Part of the system for focusing the chase camera (only one enemy can be the focus)

    //ANTOINETTE'S VARIABLES
    //GameController gc;
    GameObject[] waypoints;
    public Rigidbody2D rb;
    public float speed = 5f;
    float waypointRotationSpeed = 5f;
    float offset = 90f;
    GameObject waypoint;
    Vector3 targetDirection;
    Vector3 newDirection;
    Vector3 currentPos;
    Vector3 heroPos;
    float angle;
    int index = 0;
    public SpriteRenderer SpriteRenderer;
    public Sprite eggSprite;
    public Sprite stunnedSprite; //ADDED
    float rotationSpeed = 90f / 60f;
    int sizeFrames = 60;
    float scaleRate = 2f / 60f;
    int rotationFrames = 60;
    int frameTick = 0;
    int hitCounter = 0;
    bool stunned = false;
    bool isEgg = false;
    bool aggro = false;
    bool touched = false;
    public EnemyState state = EnemyState.defaultState;
    // Start is called before the first frame update
    public enum EnemyState
    {
        defaultState,
        ccwState,
        cwState,
        chaseState,
        growState,
        shrinkState,
        stunnedState,
        uselessState
    };

    // All instances of Enemy shares this one WayPoint and EnemySystem
    static private WayPointSystem sWayPoints = null;
    static private EnemySpawnSystem sEnemySystem = null;
    static public void InitializeEnemySystem(EnemySpawnSystem s, WayPointSystem w) { sEnemySystem = s; sWayPoints = w; }

    private const float kSpeed = 5f;
    private int mWayPointIndex = 0;

    private const float kTurnRate = 0.03f/60f;
    //private int mNumHit = 0;
    private const int kHitsToDestroy = 4;
    private const float kEnemyEnergyLost = 0.8f;
		
	// Use this for initialization
	void Start () {
        mWayPointIndex = sWayPoints.GetInitWayIndex();

        //ANTOINETTE
        waypoints = sWayPoints.mWayPoints;

        //REES 
        hero = GameObject.FindObjectOfType<HeroBehavior>();
        theManager = GameObject.FindObjectOfType<GameManager>();
        chaseCam = GameObject.FindWithTag("ChaseCam");
        /*
        theManager = GameObject.FindObjectOfType<GameManager>();
        chaseCam.SetActive(false);
        */
    }
	
	// Update is called once per frame
	void Update () {
        /*
       sWayPoints.CheckNextWayPoint(transform.position, ref mWayPointIndex);
       PointAtPosition(sWayPoints.WayPoint(mWayPointIndex), kTurnRate);
       transform.position += (kSpeed * Time.smoothDeltaTime) * transform.up;
        */

        //ANTOINETTE 
        heroPos = hero.transform.position;
        UpdateFSM();

        //REES
        /*
        if (hero.beingChased && !theManager.readyToChase)
        {
            theManager.setChaseCamSize(transform.position);
        }

        /*
        if (theManager.readyToChase == false && state == EnemyState.chaseState) //I believe this should only be true for the 1st chasing enemy
        {
            theManager.setChaseCamSize(transform.position);
        }
        */
    }

    private void PointAtPosition(Vector3 p, float r)
    {
        Vector3 v = p - transform.position;
        transform.up = Vector3.LerpUnclamped(transform.up, v, r);
    }

    /*
    #region Trigger into chase or die
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("Emeny OnTriggerEnter");
        TriggerCheck(collision.gameObject);
    }

    private void TriggerCheck(GameObject g)
    {
        if (g.name == "Hero")
        {
            ThisEnemyIsHit();

        } else if (g.name == "Egg(Clone)")
        {
            mNumHit++;
            if (mNumHit < kHitsToDestroy)
            {
                Color c = GetComponent<Renderer>().material.color;
                c.a = c.a * kEnemyEnergyLost;
                GetComponent<Renderer>().material.color = c;
            } else
            {
                ThisEnemyIsHit();
            }
        }
    }
    

    private void ThisEnemyIsHit()
    {
        sEnemySystem.OneEnemyDestroyed();
        Destroy(gameObject);
    }
    #endregion
    */

    //ANTOINETTE'S STUFF
    
    void UpdateFSM()
    {
        switch (state)
        {
            case EnemyState.defaultState:
                Patrol();
                break;
            case EnemyState.ccwState:
                CCWState();
                break;
            case EnemyState.cwState:
                CWState();
                break;
            case EnemyState.chaseState:
                ChaseState();
                break;
            case EnemyState.growState:
                GrowState();
                break;
            case EnemyState.shrinkState:
                ShrinkState();
                break;
            case EnemyState.stunnedState:
                StunnedState();
                break;
            case EnemyState.uselessState:
                UselessState();
                break;
        }
    }
    /*
     * Moves the gameObject towards a waypoint sequentially using the waypoints array
     */
    private void Patrol()
    {
        currentPos = transform.position;
        /*
        if (Vector3.Distance(transform.position, heroPos) <= 40f)
        {
            state = EnemyState.chaseState;
        }
        */

        if (sWayPoints.mPointsInSequence)
        {
            MoveTowardsWaypointSequentially();
        }
        else
        {
            MoveTowardsRandomWaypoint();
        }
    }
    /*
     * Moves the object towards a randomly chosen waypoint once bool randPoints is activated.
     */
    void MoveTowardsRandomWaypoint()
    {
        waypoint = waypoints[index];
        transform.position = Vector3.MoveTowards(transform.position,
        waypoint.transform.position, speed * Time.deltaTime);
        RotateTowardsPoint(waypoint.transform.position);

        if (Vector3.Distance(transform.position, waypoint.transform.position) <= 25f)
        {
            index = Random.Range(0, 5);
        }
    }
    /*
     * Rotates the object towards the target point.
     * @param Vector3 target, the position to rotate towards
     */
    void RotateTowardsPoint(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.Normalize();
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle + offset, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, waypointRotationSpeed * Time.deltaTime);

    }
    void MoveTowardsWaypointSequentially()
    {
        waypoint = waypoints[index];
        transform.position = Vector3.MoveTowards(transform.position,
        waypoint.transform.position, speed * Time.deltaTime);
        RotateTowardsPoint(waypoint.transform.position);

        if (Vector3.Distance(transform.position, waypoint.transform.position) <= 25f)
        {
            index++;
        }
        if (index == waypoints.Length)
        {
            index = 0;
        }
    }
    /*
     * Rotates counter clockwise (90 deg per sec)
     */
    private void CCWState()
    {
        //color changes to red when entering this state, changes to white on shrink state
        SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, Color.red, 1f);
        touched = true;
        if (frameTick > rotationFrames)
        {
            //go to cw state
            state = EnemyState.cwState;
            //reset frame tick
            frameTick = 0;
        }
        else
        {
            //increment the frame tick
            frameTick++;
            //begin rotation
            Vector3 angle = transform.rotation.eulerAngles;
            angle.z += rotationSpeed;
            transform.rotation = Quaternion.Euler(0, 0, angle.z);
        }
    }
    /*
     * rotate 90 deg clockwise 
     */
    private void CWState()
    {
        if (frameTick > rotationFrames)
        {
            state = EnemyState.growState;
            frameTick = 0;
        }
        else
        {
            //increment the frame tick
            frameTick++;
            //begin rotation
            Vector3 angle = transform.rotation.eulerAngles;
            angle.z -= rotationSpeed;
            transform.rotation = Quaternion.Euler(0, 0, angle.z);
        }
        state = EnemyState.chaseState; // ADDED
    }
    private void ChaseState()
    {
        if (!theManager.isThereATrueEnemy())
        {
            //Mark as one true enemy
            theManager.setTrueEnemy(this.GetInstanceID());
        }
        aggro = true;
        transform.position = Vector3.MoveTowards(transform.position,
        heroPos, speed * Time.deltaTime);
        RotateTowardsPoint(heroPos);
        if (Vector3.Distance(transform.position, heroPos) > 40f)
        {
            aggro = false;
            touched = false;
            if (theManager.getOneTrueEnemy().GetInstanceID() == this.GetInstanceID())
            {
                theManager.clearTrueEnemy();
            }
            state = EnemyState.growState;
        }
    }
    private void GrowState()
    {
        if (frameTick > sizeFrames)
        {
            frameTick = 0;
            state = EnemyState.shrinkState;
        }
        frameTick++;
        float scale = transform.localScale.x;
        scale += scaleRate;
        transform.localScale = new Vector3(scale, scale, 1);
    }
    private void ShrinkState()
    {
        SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, Color.white, 1f);
        if (frameTick > sizeFrames)
        {
            frameTick = 0;
            state = EnemyState.defaultState;
        }
        frameTick++;
        float scale = transform.localScale.x;
        scale -= scaleRate;
        transform.localScale = new Vector3(scale, scale, 1);
    }
    private void StunnedState()
    {
        

        //SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, Color.magenta, 1f);
        SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, Color.white, 1f);
        Spin();
        SpriteRenderer.sprite = stunnedSprite;

        if (theManager.getOneTrueEnemy().GetInstanceID() == this.GetInstanceID())
        {
            theManager.clearTrueEnemy();
        }
    }
    private void UselessState()
    {
        SpriteRenderer.color = Color.Lerp(SpriteRenderer.color, Color.white, 1f);
        SpriteRenderer.sprite = eggSprite;
    }
    /*
     * On collision enter ccw state
     * @param collider, a Collider2D object
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        rb.velocity = Vector3.zero;
        if (collision.tag == "Hero" && !isEgg && !touched)
        {
            state = EnemyState.ccwState;
            frameTick = 0;
        }
        if (collision.tag == "Hero" && aggro && touched)
        {
            //gc.numEnemies--;
            sEnemySystem.OneEnemyDestroyed(); //Pisan
            Destroy(gameObject);
        }
        if (collision.tag == "Egg")
        {
            hitCounter++;
            if (hitCounter == 1)
            {
                stunned = true;
                float magnitude = 8f;
                Vector3 force = transform.position - collision.gameObject.transform.up;
                force.Normalize();
                rb.AddForce(force * magnitude);
                state = EnemyState.stunnedState;
            }
            else if (hitCounter == 2 && stunned)
            {
                float magnitude = 8f;
                Vector3 force = transform.position - heroPos;
                force.Normalize();
                rb.AddForce(force * magnitude);
                gameObject.tag = "Useless";
                isEgg = true;
                state = EnemyState.uselessState;
            }
            else if (hitCounter == 3 && isEgg)
            {
                //gc.numEnemies--;
                sEnemySystem.OneEnemyDestroyed(); //Pisan
                Destroy(gameObject);
            }

        }
    }
    /*
     * Spin while stunned, stop when hit with egg and go to useless state
     */
    private void Spin()
    {
        Vector3 angle = transform.rotation.eulerAngles;
        angle.z -= rotationSpeed;
        transform.rotation = Quaternion.Euler(0, 0, angle.z);
    }
}
