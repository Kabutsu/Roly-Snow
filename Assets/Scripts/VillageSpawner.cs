using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageSpawner : MonoBehaviour {

    public GameController controller;
    public TreeSpawner trees;
    public GameObject[] housePrefabs;
    private GameObject lastHousePlaced;

    private float verticalMin = 0f;
    private float horizontalMin = 0f;

    private int villageHeight;
    private int numberOfHousesAcross;
    private float[,] coordinates;

    private float screenMin;
    private float screenMax;
    private float houseMin;
    private float houseMax;

    private void Awake()
    {
        foreach(GameObject house in housePrefabs)
        {
            if (house.GetComponent<SpriteRenderer>().bounds.size.x > horizontalMin)
                horizontalMin = house.GetComponent<SpriteRenderer>().bounds.size.x;

            if (house.GetComponent<SpriteRenderer>().bounds.size.y > verticalMin)
                verticalMin = house.GetComponent<SpriteRenderer>().bounds.size.y;
        }

        horizontalMin += 0.5f;
        verticalMin += 0.5f;

        Debug.Log("horoMin:= " + horizontalMin + ", vertMin:= " + verticalMin);

        float vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        screenMin = horzExtent - 35.275f / 2.0f;
        screenMax = 35.275f / 2.0f - horzExtent;

        houseMin = screenMin + horizontalMin;
        houseMax = screenMax - horizontalMin;

        Debug.Log("houseMin:= " + houseMin + ", houseMax:= " + houseMax);

        bool idealWidthFound = false;
        numberOfHousesAcross = 1;

        do
        {
            idealWidthFound = (((houseMax + Mathf.Abs(houseMin)) / numberOfHousesAcross <= horizontalMin) ? true : false);

            if (!idealWidthFound) numberOfHousesAcross++;
        } while (!idealWidthFound);
        
        numberOfHousesAcross--;
        horizontalMin = (houseMax + Mathf.Abs(houseMin)) / numberOfHousesAcross;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnVillage()
    {
        trees.Stop();

        villageHeight = Random.Range(4, 8);
        coordinates = new float[villageHeight, numberOfHousesAcross + 1];
        bool[,] houses = new bool[villageHeight, numberOfHousesAcross + 1];
        float[,] likelihood = new float[villageHeight, numberOfHousesAcross + 1];
        
        for (int i = 0; i < villageHeight; i++)
        {
            float currentHouseX = houseMin;
            for (int j = 0; j <= numberOfHousesAcross; j++)
            {
                coordinates[i, j] = currentHouseX;
                currentHouseX += horizontalMin;

                houses[i, j] = false;
                likelihood[i, j] = 0f;

                Debug.Log("coordinates[" + i + "," + j + "]:= " + coordinates[i, j]);
            }
        }

        int numberOfStartingHouses = Random.Range(Mathf.RoundToInt(villageHeight / 4f) + 1, Mathf.RoundToInt(villageHeight / 4f) + 4);

        for (int i = 0; i < numberOfStartingHouses; i++)
        {
            int randomX = 0;
            int randomY = 0;
            do
            {
                randomX = Random.Range(0, houses.GetLength(0) - 1);
                randomY = Random.Range(0, houses.GetLength(1) - 1);
            } while (houses[randomX, randomY] == true);
            houses[randomX, randomY] = true;
        }

        //iteravely generate where the houses are
        for(int pass = 0; pass < Random.Range(1, 5); pass++)
        {

            for (int i = 0; i < houses.GetLength(0); i++)
            {
                for (int j = 0; j < houses.GetLength(1); j++)
                {
                    if (houses[i, j] == true)
                    {
                        if (i != 0 && houses[i - 1, j] != true) likelihood[i - 1, j] += 0.2f;
                        if (i != houses.GetLength(0) - 1 && houses[i + 1, j] != true) likelihood[i + 1, j] += 0.2f;

                        if (j != 0 && houses[i, j - 1] != true) likelihood[i, j - 1] += 0.1f;
                        if (j != houses.GetLength(1) - 1 && houses[i, j + 1] != true) likelihood[i, j + 1] += 0.1f;
                    }
                }
            }

            for (int i = 0; i < houses.GetLength(0); i++)
            {
                for (int j = 0; j < houses.GetLength(1); j++)
                {
                    if (likelihood[i, j] > 0f && Random.Range(0f, 1f) <= likelihood[i, j])
                    {
                        houses[i, j] = true;
                        likelihood[i, j] = 0f;
                    }
                }
            }
        }

        //place the houses
        StartCoroutine(PlaceHouses(houses));
    }

    private void AddHouse(float xPos)
    {
        GameObject newHouse = Instantiate(housePrefabs[Random.Range(0, housePrefabs.Length)], new Vector3(xPos, gameObject.transform.position.y), gameObject.transform.rotation);
        controller.AddTree(newHouse.GetComponent<TreeController>());
        lastHousePlaced = newHouse;
    }

    private float LastHouseDistance()
    {
        try
        {
            return lastHousePlaced.transform.position.y - gameObject.transform.position.y;
        } catch (System.NullReferenceException)
        {
            return -1f;
        }
    }

    private IEnumerator PlaceHouses(bool[,] houses)
    {
        yield return new WaitForSeconds(0.5f);
        
        for(int i = 0; i < houses.GetLength(0); i++)
        {
            float xShift = Random.Range(0, 3f * (horizontalMin / 4f));
            if (Random.Range(0, 2) == 0) xShift = 0 - xShift;

            float yShift = Random.Range((verticalMin / 4f), 3f * (verticalMin / 4f));

            for (int j = 0; j < houses.GetLength(1); j++)
            {
                if (houses[i, j] == true)
                {
                    AddHouse(coordinates[i, j] + xShift);
                }
            }

            yield return new WaitUntil(() => (LastHouseDistance() >= verticalMin - yShift || LastHouseDistance() == -1f));
        }

        yield return new WaitForSeconds(0.25f);

        trees.Resume();
    }
}
