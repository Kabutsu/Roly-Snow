using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    private List<TreeController> trees;
    public TreeSpawner spawner;
    public SnowballController snowball;
    public PathSpawner paths;
    public VillageSpawner villages;

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

    private int[] originalBoundaries;
    
    private int currentLevel = -1;

    private float score = 0;
    private float boundaryScore = 0;
    private float scoreSpeed = 0f;
    private float acceleration = 0.025f;
    private float scoreMaxIncrement;

    public UnityEngine.UI.Text titleText;

    public UnityEngine.UI.Text scoreTitleText;
    public UnityEngine.UI.Text scoreText;

    public UnityEngine.UI.Text textHints;

    public UnityEngine.UI.Image[] heartImages = new UnityEngine.UI.Image[3];
    public Sprite[] heartImageTypes = new Sprite[2];

    public GameObject startButton;
    public GameObject restartButton;
    public GameObject aboutButton;

    public GameObject infoPanel;
    private bool infoOpen = false;

    public string[] encouragements;

    private int lives = 3;
    private bool gameOver = false;

    // Use this for initialization
    void Start () {
        originalBoundaries = new int[levelBoundaries.Length];
        for (int i = 0; i < levelBoundaries.Length; i++) originalBoundaries[i] = levelBoundaries[i];

        trees = new List<TreeController>();
        IncrementLevel();

        scoreTitleText.enabled = false;
        scoreText.enabled = false;
        textHints.enabled = false;
        foreach (UnityEngine.UI.Image heart in heartImages) heart.enabled = false;

        gameOver = true;

        restartButton.SetActive(false);
        infoPanel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (!gameOver)
        {
            if (Input.GetKeyDown(KeyCode.D)) villages.SpawnVillage();
            if (Input.GetKeyDown(KeyCode.E)) paths.SpawnPath();

            if (scoreSpeed < scoreMaxIncrement) scoreSpeed += acceleration;

            score += (scoreSpeed * Time.deltaTime);
            boundaryScore += (scoreSpeed * Time.deltaTime);

            switch (scoreText.text)
            {
                case "250":
                    StartCoroutine(ShowTextHint("You're doing great!", true));
                    break;
                case "500":
                    StartCoroutine(ShowTextHint("You're on fire!", true));
                    break;
                case "1000":
                    StartCoroutine(ShowTextHint("Incredible!", true));
                    break;
                case "1500":
                    StartCoroutine(ShowTextHint("Unbelievable!", true));
                    break;
                case "2000":
                    StartCoroutine(ShowTextHint("Spectacular!", true));
                    break;
                case "2500":
                    StartCoroutine(ShowTextHint("I don't believe what I'm seeing!", true));
                    break;
                case "5000":
                    StartCoroutine(ShowTextHint("Some say it's photoshopped, but I don't believe them!", true));
                    break;
                case "10000":
                    StartCoroutine(ShowTextHint("Ok this is getting ridiculous now...", true));
                    break;
                case "20000":
                    StartCoroutine(ShowTextHint("That's it, I'm off. This is too amazing to withstand.", true));
                    break;
            }

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
        currentLevel = (currentLevel == 0 ? currentLevel : currentLevel--);
        lives--;
        if (lives >= 0 && lives <= 2) heartImages[lives].sprite = heartImageTypes[1];

        if (lives == 0)
        {
            GameOver();
        } else
        {
            SetLevels(true);
        }
    }

    private void SetLevels(bool slowDown)
    {
        if (slowDown)
        {
            scoreSpeed = 0.15f;
            if(lives == 2 && currentLevel < 2)
            {
                StartCoroutine(ShowTextHint("That's taken a bit of your speed away!", true));
                for (int i = 0; i < 5; i++) levelBoundaries[i] += 15;
            } else if (lives == 1 && currentLevel < 3)
            {
                StartCoroutine(ShowTextHint("We'll keep things a bit slower for you!", true));
                for (int i = 0; i < 5; i++) levelBoundaries[i] += (25 * (4 - i));
            } else
            {
                StartCoroutine(ShowTextHint("That'll slow you down a bit!", true));
            }
        } else
        {
            StartCoroutine(ShowTextHint(encouragements[UnityEngine.Random.Range(0, encouragements.Length)], true));
        }
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

    private IEnumerator ShowTextHint(string message, bool fadeAway)
    {
        textHints.fontSize = 0;
        textHints.enabled = true;
        textHints.text = message;
        textHints.color = new Color(0.4f, 0.73f, 1f, 0f);

        for (float t = 0; t < 1; t += Time.deltaTime / 0.25f)
        {
            if (t <= 0.5f) textHints.color = new Color(0.4f, 0.73f, 1f, t * 2);
            textHints.fontSize = Mathf.RoundToInt(34f * t);
            yield return null;
        }

        textHints.fontSize = 34;
        textHints.color = new Color(0.4f, 0.73f, 1f, 1f);

        if (fadeAway)
        {
            yield return new WaitForSeconds(3.5f);

            if (!gameOver)
            {
                for (float t = 1; t > 0; t -= Time.deltaTime / 1f)
                {
                    textHints.color = new Color(0.4f, 0.73f, 1f, t);
                    yield return null;
                }

                textHints.enabled = false;
            }
        }

        yield return null;
    }

    public void OpenAbout()
    {
        if(!infoOpen)
        {
            infoOpen = true;
            StartCoroutine(OpenAboutMenu());
        }
    }

    public void CloseAbout()
    {
        if(infoOpen)
        {
            infoOpen = false;
            StartCoroutine(CloseAboutMenu());
        }
    }

    private IEnumerator OpenAboutMenu()
    {
        infoPanel.SetActive(true);
        RectTransform infoBox = infoPanel.GetComponent<RectTransform>();
        infoBox.localScale = new Vector3(0, 0);

        for(float t = 0; t <= 1; t += Time.deltaTime / 0.3f)
        {
            infoBox.localScale = new Vector3(t * 0.8f, t * 0.2f);
            yield return null;
        }
    }

    private IEnumerator CloseAboutMenu()
    {
        RectTransform infoBox = infoPanel.GetComponent<RectTransform>();

        for (float t = 1; t <= 0; t -= Time.deltaTime / 0.3f)
        {
            infoBox.localScale = new Vector3(t * 0.8f, t * 0.2f);
            yield return null;
        }

        infoPanel.SetActive(false);
        yield return null;
    }

    public bool GameIsOver()
    {
        return gameOver;
    }

    public void GameOver()
    {
        gameOver = true;
        spawner.Stop();
        villages.Stop();
        foreach (TreeController tree in trees) tree.Stop();
        snowball.enabled = false;

        this.StopAllCoroutines();
        StartCoroutine(ShowTextHint("Game Over", false));

        restartButton.SetActive(true);
    }

    public void GameStart()
    {
        GameObject snowballObj = snowball.gameObject;
        snowballObj.GetComponent<SpriteRenderer>().enabled = true;
        if (infoOpen) CloseAbout();
        StartCoroutine(StartAnimation(snowballObj));
    }

    private IEnumerator StartAnimation(GameObject snowballObj)
    {
        UnityEngine.UI.Image startButtonImg = startButton.GetComponent<UnityEngine.UI.Image>();
        UnityEngine.UI.Image aboutButtonImg = aboutButton.GetComponent<UnityEngine.UI.Image>();

        for (float t = 0; t < 1; t += Time.deltaTime / 2f)
        {
            titleText.color = new Color(1, 1, 1, 1 - t);
            snowballObj.transform.position = new Vector3(Mathf.Lerp(-3.9f, 0f, t), Mathf.Lerp(13.17f, 7f, t));
            snowballObj.transform.localScale = new Vector3(Mathf.Lerp(0.25f, 1f, t), Mathf.Lerp(0.25f, 1f, t));
            startButtonImg.color = new Color(1, 1, 1, 1-t);
            aboutButtonImg.color = new Color(1, 1, 1, 1-t);
            yield return null;
        }

        titleText.enabled = false;
        startButton.SetActive(false);
        aboutButton.SetActive(false);

        scoreTitleText.enabled = true;
        scoreTitleText.color = new Color(0, 0, 0, 0);
        scoreText.enabled = true;
        scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, 0);

        foreach(UnityEngine.UI.Image heart in heartImages)
        {
            heart.enabled = true;
            heart.color = new Color(0, 0, 0, 0);
        }

        Camera mainCamera = Camera.main;

        SpriteRenderer leftArrow = snowball.leftArrow.GetComponent<SpriteRenderer>();
        SpriteRenderer rightArrow = snowball.rightArrow.GetComponent<SpriteRenderer>();
        leftArrow.enabled = true;
        leftArrow.color = new Color(0.82f, 0.82f, 0.82f, 0f);
        rightArrow.enabled = true;
        rightArrow.color = new Color(0.82f, 0.82f, 0.82f, 0f);

        yield return null;

        for(float t = 0; t < 1; t += Time.deltaTime / 1.5f)
        {
            scoreTitleText.color = new Color(0, 0, 0, t);
            scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, t);
            foreach (UnityEngine.UI.Image heart in heartImages) heart.color = new Color(0, 0, 0, t);
            mainCamera.transform.position = new Vector3(0, Mathf.Lerp(10f, 0f, t), -10);
            snowballObj.transform.position = new Vector3(0, Mathf.Lerp(7f, 4.05f, t));
            if(t <= (1f/3f))
            {
                leftArrow.color = new Color(0.82f, 0.82f, 0.82f, t * 3f);
                rightArrow.color = new Color(0.82f, 0.82f, 0.82f, t * 3f);
            }
            yield return null;
        }

        scoreTitleText.color = Color.black;
        scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, 1);

        foreach (UnityEngine.UI.Image heart in heartImages) heart.color = new Color(0, 0, 0, 1);

        gameOver = false;
        snowball.enabled = true;
        spawner.enabled = true;

        yield return null;
    }

    public void RestartGame()
    {
        lives = 3;
        currentLevel = -1;

        for (int i = 0; i < originalBoundaries.Length; i++) levelBoundaries[i] = originalBoundaries[i];

        foreach (TreeController tree in trees.ToArray()) RemoveTree(tree);

        Camera.main.transform.position = new Vector3(0, 10, -10);
        snowball.transform.position = new Vector3(-3.9f, 13.17f, 0f);
        snowball.transform.localScale = new Vector3(0.25f, 0.25f);

        restartButton.SetActive(false);
        scoreText.text = "0";
        score = 0;
        boundaryScore = 0;
        scoreSpeed = 0f;
        acceleration = 0.025f;
        IncrementLevel();

        scoreText.enabled = false;
        scoreTitleText.enabled = false;
        textHints.enabled = false;
        foreach(UnityEngine.UI.Image heart in heartImages)
        {
            heart.sprite = heartImageTypes[0];
            heart.enabled = false;
        }

        snowball.enabled = false;
        spawner.enabled = false;

        StartCoroutine(RestartAnimation(snowball.gameObject));
    }

    private IEnumerator RestartAnimation(GameObject snowballObj)
    {
        for (float t = 0; t < 1; t += Time.deltaTime / 1.5f)
        {
            snowballObj.transform.position = new Vector3(Mathf.Lerp(-3.9f, 0f, t), Mathf.Lerp(13.17f, 7f, t));
            snowballObj.transform.localScale = new Vector3(Mathf.Lerp(0.25f, 1f, t), Mathf.Lerp(0.25f, 1f, t));
            yield return null;
        }

        scoreTitleText.enabled = true;
        scoreTitleText.color = new Color(0, 0, 0, 0);
        scoreText.enabled = true;
        scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, 0);

        foreach(UnityEngine.UI.Image heart in heartImages)
        {
            heart.enabled = true;
            heart.color = new Color(0, 0, 0, 0);
        }

        Camera mainCamera = Camera.main;

        SpriteRenderer leftArrow = snowball.leftArrow.GetComponent<SpriteRenderer>();
        SpriteRenderer rightArrow = snowball.rightArrow.GetComponent<SpriteRenderer>();
        leftArrow.enabled = true;
        leftArrow.color = new Color(0.82f, 0.82f, 0.82f, 0f);
        rightArrow.enabled = true;
        rightArrow.color = new Color(0.82f, 0.82f, 0.82f, 0f);

        yield return null;

        for (float t = 0; t < 1; t += Time.deltaTime / 1.5f)
        {
            scoreTitleText.color = new Color(0, 0, 0, t);
            scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, t);
            foreach (UnityEngine.UI.Image heart in heartImages) heart.color = new Color(0, 0, 0, t);
            mainCamera.transform.position = new Vector3(0, Mathf.Lerp(10f, 0f, t), -10);
            snowballObj.transform.position = new Vector3(0, Mathf.Lerp(7f, 4.05f, t));
            if (t <= (1f / 3f))
            {
                leftArrow.color = new Color(0.82f, 0.82f, 0.82f, t * 3f);
                rightArrow.color = new Color(0.82f, 0.82f, 0.82f, t * 3f);
            }
            yield return null;
        }

        scoreTitleText.color = Color.black;
        scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, 1);

        foreach (UnityEngine.UI.Image heart in heartImages) heart.color = new Color(0, 0, 0, 1);

        gameOver = false;
        snowball.enabled = true;
        StartCoroutine(snowball.MoveDownScreen());
        spawner.enabled = true;
        gameOver = false;

        yield return new WaitForSeconds(1f);

        spawner.Resume();
        yield return null;
    }
}
