using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour {

    private float minSpawnRate = 3;
    private float maxSpawnRate = 6;
    private float frequency = 3;
    private int treesSpawned = 0;
    private int i;

    public GameObject treePrefab;
    private float randomiser;

    private int screenWidth;
    private Transform trans;
    public GameController controller;

    private void Awake()
    {
        screenWidth = Screen.width;
        trans = gameObject.transform;
    }

    // Use this for initialization
    void Start () {
        randomiser = Random.Range(1f, 100f);
	}
	
	// Update is called once per frame
	void Update () {
        float noise = Mathf.PerlinNoise(randomiser, frequency * Time.time);
        treesSpawned = Mathf.RoundToInt(Mathf.Lerp(minSpawnRate, maxSpawnRate, noise));

        for(i = 0; i <= treesSpawned; i++)
        {
            GameObject newTree = Instantiate(treePrefab, new Vector3(Random.Range(0, screenWidth), trans.position.y), trans.rotation);
            controller.AddTree(newTree.GetComponent<TreeController>());
        }
	}
}
