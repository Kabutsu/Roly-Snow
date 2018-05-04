using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathSpawner : MonoBehaviour {

    List<Dictionary<float, float>> pathValues;

    public float MIN_PATH_WIDTH = 4f;
    public float MAX_PATH_WIDTH = 8f;

    public float MIN_PATH_LENGTH = 18f;
    public float MAX_PATH_LENGTH = 54f;

    private float randomiser;
    private float perlinFrequency = 0.5f;
    
    private float lastCentrePoint = 0f;

    private float screenMin;
    private float screenMax;

    public TreeSpawner spawner;

    // Use this for initialization
    void Start () {
        randomiser = Random.Range(1f, 100f);

        float vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        screenMin = horzExtent - 35.275f / 2.0f;
        screenMax = 35.275f / 2.0f - horzExtent;

        pathValues = new List<Dictionary<float, float>>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnPath()
    {
        pathValues = new List<Dictionary<float, float>>();

        float minCentrePoint = screenMin + (MAX_PATH_WIDTH / 2);
        float maxCentrePoint = screenMax - (MAX_PATH_WIDTH / 2);

        lastCentrePoint = Random.Range(minCentrePoint, maxCentrePoint);
        float pathLength = Random.Range(MIN_PATH_LENGTH, MAX_PATH_LENGTH);

        for(float yPos = 0f; yPos <= pathLength; yPos += 0.65f)
        {
            float noise = Mathf.PerlinNoise(randomiser, perlinFrequency * Time.time);

            float width = Mathf.Lerp(MIN_PATH_WIDTH, MAX_PATH_WIDTH, noise);
            float newCentrePoint;

            do
            {
                newCentrePoint = Mathf.Lerp(lastCentrePoint, (Random.Range(0, 2) == 0 ? minCentrePoint : maxCentrePoint), noise);
            }
            while (Impossible(newCentrePoint, lastCentrePoint));

            lastCentrePoint = newCentrePoint;
            
            pathValues.Add(new Dictionary<float, float>() { { (lastCentrePoint - (width / 2)) + screenMax, width } });

            Debug.Log("Centre:= " + lastCentrePoint + ", Width:= " + width + ", DistanceFromLeft:= " + pathValues.Last().Keys.ToArray().First());
        }

        spawner.SpawnPath(pathValues);
    }

    private bool Impossible(float proposedCentre, float lastCentre)
    {
        if (proposedCentre > lastCentre)
        {
            if (proposedCentre - lastCentre > MIN_PATH_WIDTH / 2f) return true;
        }
        else if (proposedCentre < lastCentre)
        {
            if (lastCentre - proposedCentre > MIN_PATH_WIDTH / 2f) return true;
        }

        return false;
    }
}
