﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TreeSpawner : MonoBehaviour {

    private float minSpawnRate = 1;
    private float maxSpawnRate = 1;
    private int treesSpawned = 0;
    private int i;

    private float minFrequency = 0.25f;
    private float maxFrequency = 1f;

    public GameObject treePrefab;
    private float randomiser;
    private float ripple = 0.5f;

    private float screenMin;
    private float screenMax;
    private Transform trans;
    public GameController controller;

    private bool paused = false;

    public GameObject lastTreePlaced;

    private void Awake()
    {
        float vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        
        screenMin = horzExtent - 35.275f / 2.0f;
        screenMax = 35.275f / 2.0f - horzExtent;

        trans = gameObject.transform;
    }

    // Use this for initialization
    void Start () {
        randomiser = Random.Range(1f, 100f);
        SpawnTrees();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    private void AddTree(float xPos)
    {
        GameObject newTree = Instantiate(treePrefab, new Vector3(xPos, trans.position.y), trans.rotation);
        controller.AddTree(newTree.GetComponent<TreeController>());
        lastTreePlaced = newTree;
    }

    private void SpawnTrees()
    {
        float noise = Mathf.PerlinNoise(randomiser, ripple * Time.time);
        float delay = Mathf.Lerp(minFrequency, maxFrequency, noise);

        treesSpawned = Mathf.RoundToInt(Mathf.Lerp(minSpawnRate, maxSpawnRate, noise));

        for (i = 0; i <= treesSpawned; i++)
        {
            if (!paused)
            {
                AddTree(Random.Range(screenMin, screenMax));
            }
        }

        if(!paused) Invoke("SpawnTrees", delay);
    }

    public void PauseTreesSpawningFor(float seconds)
    {
        paused = true;
        StartCoroutine(Resume(seconds));
    }

    private IEnumerator Resume(float inSeconds)
    {
        yield return new WaitForSeconds(inSeconds);

        SpawnTrees();
    }

    public void IncreaseSpawnRate()
    {
        maxSpawnRate++;
        maxFrequency -= 0.1f;
    }

    public void DecreaseSpawnRate()
    {
        maxSpawnRate--;
        maxFrequency += 0.1f;
    }

    public void Stop()
    {
        paused = true;
    }

    public bool Stopped()
    {
        return paused;
    }

    public void Resume()
    {
        paused = false;
        SpawnTrees();
    }

    public void SpawnPath(List<Dictionary<float, float>> pathValues)
    {
        Stop();
        StartCoroutine(PlacePath(pathValues));
    }

    private IEnumerator PlacePath(List<Dictionary<float, float>> pathValues)
    {
        yield return new WaitForSeconds(1.5f);

        foreach (Dictionary<float, float> pathValue in pathValues)
        {
            if (!controller.GameIsOver())
            {
                float distanceFromLeft = pathValue.Keys.ToArray().First();
                float width = pathValue.Values.ToArray().First();

                float horoTreeSplit = Random.Range(0.3f, 0.5f);
                float vertTreeSplit = Random.Range(0.55f, 0.65f);

                for (float i = screenMin; i <= (screenMin + distanceFromLeft); i += horoTreeSplit)
                    AddTree(i);

                for (float i = (screenMin + distanceFromLeft + width); i <= screenMax + horoTreeSplit; i += horoTreeSplit)
                    AddTree(i);
                
                yield return new WaitUntil(() => LastTreeDistance() >= vertTreeSplit);
            }
        }

        yield return new WaitForSeconds(1f);

        if (!controller.GameIsOver()) controller.StateComplete();
    }

    private float LastTreeDistance()
    {
        return lastTreePlaced.transform.position.y - gameObject.transform.position.y;
    }
}
