using System;
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

    public UnityEngine.UI.Text titleText;
    public UnityEngine.UI.Text scoreTitleText;
    public UnityEngine.UI.Text scoreText;

    private bool gameOver = false;

    // Use this for initialization
    void Start () {
        trees = new List<TreeController>();
        IncrementLevel();

        scoreTitleText.enabled = false;
        scoreText.enabled = false;
        gameOver = true;

        GameStart();
	}
	
	// Update is called once per frame
	void Update () {
        if (!gameOver)
        {
            if (scoreSpeed < scoreMaxIncrement) scoreSpeed += acceleration;

            score += (scoreSpeed * Time.deltaTime);
            boundaryScore += (scoreSpeed * Time.deltaTime);

            if (!gameOver && currentLevel < 4 && boundaryScore > levelBoundaries[currentLevel]) IncrementLevel();

            scoreText.text = Mathf.RoundToInt(score).ToString();
        }
    }

    public void AddTree(TreeController tree)
    {
        try
        {
            tree.SetMaxSpeed(treeSpeedValues[currentLevel]);
            trees.Add(tree);
        } catch (IndexOutOfRangeException)
        {
            trees.Add(tree);
            GameOver();
        }
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
        if(!gameOver) snowball.SetSize(snowballSizeValues[currentLevel]);

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
        foreach (TreeController tree in trees) tree.Stop();
        spawner.Stop();
        snowball.enabled = false;
    }

    public void GameStart()
    {
        GameObject snowballObj = snowball.gameObject;
        snowballObj.GetComponent<SpriteRenderer>().enabled = true;
        StartCoroutine(StartAnimation(snowballObj));
    }

    private IEnumerator StartAnimation(GameObject snowballObj)
    {
        for (float t = 0; t < 1; t += Time.deltaTime / 3f)
        {
            titleText.color = new Color(1, 1, 1, 1 - t);
            snowballObj.transform.position = new Vector3(Mathf.Lerp(-3.9f, 0f, t), Mathf.Lerp(13.17f, 7f, t));
            //snowballObj.transform.localScale = new Vector3(Mathf.Lerp(0.25f, 1f, t), Mathf.Lerp(0.25f, 1f, t));
            yield return null;
        }

        titleText.enabled = false;

        scoreTitleText.enabled = true;
        scoreTitleText.color = new Color(0, 0, 0, 0);
        scoreText.enabled = true;
        scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, 0);

        Camera mainCamera = Camera.main;

        yield return null;

        for(float t = 0; t < 1; t += Time.deltaTime / 3f)
        {
            scoreTitleText.color = new Color(0, 0, 0, t);
            scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, t);
            mainCamera.transform.position = new Vector3(0, Mathf.Lerp(10f, 0f, t), -10);
            snowballObj.transform.position = new Vector3(0, Mathf.Lerp(7f, 4.05f, t));
            yield return null;
        }

        scoreTitleText.color = Color.black;
        scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, 1);

        gameOver = false;
        snowball.enabled = true;
        spawner.enabled = true;

        yield return null;
    }
}
