using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    private List<TreeController> trees;
    public TreeSpawner spawner;
    public SnowballController snowball;

    [SerializeField]
    private int[] levelBoundaries = new int[5];
    [SerializeField]
    private float[] treeSpeedValues = new float[5];
    [SerializeField]
    private float[] snowballSpeedValues = new float[5];
    [SerializeField]
    private float[] snowballAccelerationValues = new float[5];
    [SerializeField]
    private float[] snowballMomentumValues = new float[5];
    [SerializeField]
    private float[] snowballSizeValues = new float[5];
    
    private int currentLevel = -1;

    private float score = 0;
    private float boundaryScore = 0;
    private float scoreSpeed = 0f;
    private float acceleration = 0.025f;
    private float scoreMaxIncrement;

    public UnityEngine.UI.Text scoreText;

    private bool gameOver = false;

    // Use this for initialization
    void Start () {
        trees = new List<TreeController>();
        IncrementLevel();
	}
	
	// Update is called once per frame
	void Update () {
        if (scoreSpeed < scoreMaxIncrement) scoreSpeed += acceleration;

        score += (scoreSpeed * Time.deltaTime);
        boundaryScore += (scoreSpeed * Time.deltaTime);

        if (!gameOver && currentLevel < 4 && boundaryScore > levelBoundaries[currentLevel]) IncrementLevel();

        scoreText.text = Mathf.RoundToInt(score).ToString();
    }

    public void AddTree(TreeController tree)
    {
        tree.SetMaxSpeed(treeSpeedValues[currentLevel]);
        trees.Add(tree);
    }

    public void RemoveTree(TreeController tree)
    {
        trees.Remove(tree);
        Destroy(tree.gameObject);
    }

    private void IncrementLevel()
    {
        currentLevel++;
        SetLevels(false);
    }

    public void PlayerHitTree()
    {
        currentLevel--;

        if(currentLevel < 0)
        {
            GameOver();
        } else
        {
            spawner.PauseTreesSpawningFor(currentLevel);
            SetLevels(true);
        }
    }

    private void SetLevels(bool slowDown)
    {
        if(slowDown) scoreSpeed = 0.15f;
        scoreMaxIncrement = treeSpeedValues[currentLevel];
        boundaryScore = 0;

        foreach (TreeController tree in trees)
        {
            tree.SetMaxSpeed(treeSpeedValues[currentLevel]);
            if(slowDown) tree.SlowDown();
        }

        snowball.SetMaxSpeed(snowballSpeedValues[currentLevel]);
        snowball.SetAcceleration(snowballAccelerationValues[currentLevel]);
        snowball.SetMomentum(snowballMomentumValues[currentLevel]);
        snowball.SetSize(snowballSizeValues[currentLevel]);

        if (currentLevel < 2)
        {
            if (slowDown)
            {
                spawner.DecreaseSpawnRate();
            }
            else
            {
                spawner.IncreaseSpawnRate();
            }
        }
    }

    public void GameOver()
    {
        gameOver = true;
        foreach (TreeController tree in trees) tree.enabled = false;
        spawner.enabled = false;
        snowball.enabled = false;
    }
}
