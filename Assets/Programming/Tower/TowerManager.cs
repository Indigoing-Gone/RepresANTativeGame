using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TowerManager : MonoBehaviour
{

    // Singleton
    public static TowerManager Instance;

    // Game
    private bool playing;

    // Goal line
    private LineRenderer goalLine;
    public float width;
    public float alpha;

    // Blocks
    public Transform spawnPoint;
    [SerializeField] private List<BlockController> blocks;
    int blockMask;

    // Completion checking
    public float timeToStable;
    public float countdown;
    private Coroutine countdownRoutine;
    public bool stable;

    private List<GameObject> buildingAreas;

    public TextMeshProUGUI countdownText;

    public bool complete;
    public float speedThreshold;
    public float goalMultiplier;
    public float initialTowerHeight;
    private float heightGoal;

    //Input events
    public static event Action StartingTower;
    public static event Action EndingTower;

    // Bounds (minX, maxX, minY, maxY)
    public Vector4 bounds;

    private void OnEnable()
    {
        Constituant.EnteringTower += OnEnteringTower;
    }

    private void OnDisable()
    {
        Constituant.EnteringTower -= OnEnteringTower;
    }

    private void Awake()
    {
        if (Instance) Destroy(Instance);
        Instance = this;

        BoxCollider2D BBox = GameObject.Find("Bounds").GetComponent<BoxCollider2D>();
        Vector2 boxPos = (Vector2)BBox.transform.position + BBox.offset;
        Vector2 halfs = BBox.size / 2;
        bounds = new Vector4(boxPos.x - halfs.x, boxPos.x + halfs.x,
                             boxPos.y - halfs.y, boxPos.y + halfs.y);
        Destroy(BBox.gameObject);

        blockMask = LayerMask.GetMask("Block");
    }

    // Start is called before the first frame update
    void Start()
    {
        goalLine = GetComponent<LineRenderer>();
        blocks = FindObjectsByType<BlockController>(FindObjectsSortMode.None).ToList();
        buildingAreas = GameObject.FindGameObjectsWithTag("BuildingArea").ToList();
        countdown = timeToStable;
        countdownText.enabled = false;
    }

    private void OnEnteringTower(GameObject[] _blocks)
    {
        AddBlocks(_blocks);
        StartingTower?.Invoke();
        heightGoal = CalculateHeightGoal();
        UpdateGoal();
        playing = true;
    }

    void UpdateCountdownText()
    {
        countdownText.text = (Mathf.Round(countdown * 10.0f) / 10.0f).ToString("0.0");
    }

    IEnumerator FadeTextIn()
    {
        countdownText.enabled = true;
        float step = 0.001f;
        for (float i = 0; i <= 1.0f; i += step)
        {
            countdownText.color = new Color(countdownText.color.r,
                                            countdownText.color.g,
                                            countdownText.color.b, 
                                            i / 1.0f);
            yield return null;
        }
        yield break;
    }

    IEnumerator FadeTextOut()
    {
        float step = 0.002f;
        for (float i = 0; i <= 1.0f; i += step)
        {
            countdownText.color = new Color(countdownText.color.r,
                                            countdownText.color.g,
                                            countdownText.color.b,
                                            1 - i / 1.0f);
            yield return null;
        }
        countdownText.enabled = false;
        yield break;
    }

    // Routine to begin counting down to completion
    IEnumerator CountdownRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        if (!complete) yield break;
        countdown = timeToStable;
        UpdateCountdownText();
        StartCoroutine(FadeTextIn());
        while (countdown > 0)
        {
            yield return new WaitForSeconds(0.1f);
            countdown -= 0.1f;
            UpdateCountdownText();
            if (!complete)
            {
                countdown = timeToStable;
                StartCoroutine(FadeTextOut());
                yield break;
            }
        }
        countdown = 0;
        // Game has been completed
        if (complete)
        {
            stable = true;
            playing = false;
            EndingTower?.Invoke();
        }
        else countdown = timeToStable;
        UpdateCountdownText();
        StartCoroutine(FadeTextOut());
        yield break;
    }

    // Updates building area visuals based on if block placement is valid
    void UpdateBuildingAreas(bool valid)
    {
        Color newCol = valid ? Color.green : Color.red;
        newCol.a = 0.25f;
        foreach (GameObject area in buildingAreas)
        {
            area.GetComponent<SpriteRenderer>().color = newCol;
        }
    }

    // Returns if given block is inside building area
    bool IsInBuildingArea(BlockController block)
    {
        if (block.Clicked) return false;
        if (!block.touchingGround || buildingAreas.Count == 0) return true;

        float blockX = block.transform.position.x;
        foreach (GameObject area in buildingAreas)
        {
            float areaX = area.transform.position.x;
            float areaHalfWidth = area.transform.localScale.x / 2; 
            if (blockX >= areaX - areaHalfWidth && blockX <= areaX + areaHalfWidth)
            {
                return true;
            }
        }
        return false;
    }

    // Returns if all blocks are in the building area
    bool AreBlocksInBuildingArea()
    {
        foreach (BlockController block in blocks)
        {
            if (!IsInBuildingArea(block))
            {
                UpdateBuildingAreas(false);
                return false;
            }
        }
        
        UpdateBuildingAreas(true);
        return true;
    }

    // Returns if the tower has reached the goal
    bool IsComplete()
    {
        if (!AreBlocksInBuildingArea()) return false;

        Vector2 leftBound = new(bounds[0], heightGoal);
        Vector2 rightBound = new(bounds[1], heightGoal);
        if (!Physics2D.Linecast(leftBound, rightBound, blockMask)) return false;

        foreach (BlockController block in blocks)
        {
            if (block.Clicked ||
                block.GetComponent<Rigidbody2D>().linearVelocity.magnitude > speedThreshold)
            {
                return false;
            }
        }

        return true;
    }

    // Calculates the height of the goal line and returns it
    float CalculateHeightGoal()
    {
        float heightAvg = 0;
        int B = blocks.Count;
        foreach (BlockController block in blocks) heightAvg += block.height;
        heightAvg /= B;
        /* 
         * Assuming each block is of height heightAvg, we can calculate
         * the height of a pyramid using B blocks as so:
         * 
         * The total amount of blocks in a pyramid is equal
         * to 1 + 2 + ... + N, which equals N * (N + 1) / 2
         * where N is the number of layers in the pyramid
         * 
         * Setting blocks equal to this, we get
         * N * (N + 1) / 2 = B -> (N^2+N)/2 = B -> N^2+N=2B
         * N^2 + N - 2B = 0
         * 
         * We can then solve for N using the quadratic formula:
         *     -1 + sqrt(1 + 8B)
         * N = -----------------   (plus since we don't want a negative result)
         *            2
         *            
         * We then multiply N (rounded) by heightAvg 
         * to get the total height of the pyramid
        */
        int N = Mathf.RoundToInt((-1 + Mathf.Sqrt(1 + 8*B))/2);
        float height = N * heightAvg;
        return bounds[2] + initialTowerHeight + height * goalMultiplier;
    }
    
    // Updates the goal line
    void UpdateGoal()
    {
        goalLine.SetPosition(0, new Vector3(bounds[0], heightGoal));
        goalLine.SetPosition(1, new Vector3(bounds[1], heightGoal));
        goalLine.startWidth = width;
        goalLine.endWidth = width;
        Color lineCol = complete ? Color.green : Color.red;
        lineCol.a = alpha;
        goalLine.startColor = lineCol;
        goalLine.endColor = lineCol;
    }

    // Used when adding a block to the game
    public void AddBlocks(GameObject[] blocks)
    {
        foreach (GameObject obj in blocks)
        {
            GameObject newBlock = Instantiate(obj);
            newBlock.transform.position = spawnPoint.position;
            BlockController block = newBlock.GetComponent<BlockController>();
            this.blocks.Add(block);
        }
        heightGoal = CalculateHeightGoal();
        UpdateGoal();
    }

    void PlayGame()
    {
        if (complete != IsComplete())
        {
            complete = !complete;
            UpdateGoal();
            if (complete)
            {
                if (countdownRoutine != null) StopCoroutine(countdownRoutine);
                countdownRoutine = StartCoroutine(CountdownRoutine());
            }
            else stable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playing) PlayGame();

        // -- TEMPORARY TEST CODE --
        //if (Time.time % 5 < 0.01f)
        //{
        //    AddBlocks(new GameObject[] { blocks[Random.Range(0, blocks.Count - 1)].gameObject });
        //}
        //blocks = FindObjectsByType<BlockController>(FindObjectsSortMode.None).ToList();
        //heightGoal = CalculateHeightGoal();
        //UpdateGoal();
    }
}
