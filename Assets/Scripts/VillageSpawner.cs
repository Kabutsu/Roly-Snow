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

    bool stopped = false;

    private void Awake()
    {
        //find out the size of the largest house, and so the minimum distance each house can be from each other
        foreach(GameObject house in housePrefabs)
        {
            if (house.GetComponent<SpriteRenderer>().bounds.size.x > horizontalMin)
                horizontalMin = house.GetComponent<SpriteRenderer>().bounds.size.x;

            if (house.GetComponent<SpriteRenderer>().bounds.size.y > verticalMin)
                verticalMin = house.GetComponent<SpriteRenderer>().bounds.size.y;
        }

        horizontalMin += 0.5f;
        verticalMin += 0.5f;

        //find boundaries of the screen
        float vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        screenMin = horzExtent - 35.275f / 2.0f;
        screenMax = 35.275f / 2.0f - horzExtent;

        //find minimum & maximum placements of houses
        houseMin = screenMin + horizontalMin;
        houseMax = screenMax - horizontalMin;

        bool idealWidthFound = false;
        numberOfHousesAcross = 1;

        //find how far apart each house should be to look pleasing
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
        trees.Stop(); //pause the spawning of trees

        //randomise the height of the village
        villageHeight = Random.Range(4, 8);
        coordinates = new float[villageHeight, numberOfHousesAcross + 1];
        bool[,] houses = new bool[villageHeight, numberOfHousesAcross + 1];
        float[,] likelihood = new float[villageHeight, numberOfHousesAcross + 1];
        
        //reset the arrays, which may have been used in an earlier village
        for (int i = 0; i < villageHeight; i++)
        {
            float currentHouseX = houseMin;
            for (int j = 0; j <= numberOfHousesAcross; j++)
            {
                coordinates[i, j] = currentHouseX;
                currentHouseX += horizontalMin;

                houses[i, j] = false;
                likelihood[i, j] = 0f;
            }
        }

        //randomly place the first few houses
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
            //generate likelihoods: houses are 20% more likely to be above/below another house, and 10% more likely to be next to another house
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

            //using these probablilites, generate the next houses
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
        if (!stopped)
        {
            GameObject newHouse = Instantiate(housePrefabs[Random.Range(0, housePrefabs.Length)], new Vector3(xPos, gameObject.transform.position.y), gameObject.transform.rotation);
            controller.AddTree(newHouse.GetComponent<TreeController>());
            lastHousePlaced = newHouse;
        }
    }

    //return the vertical distance between the last house placed and the spawner
    private float LastHouseDistance()
    {
        try
        {
            return lastHousePlaced.transform.position.y - gameObject.transform.position.y;
        } catch (System.NullReferenceException)
        {
            return -1f;
        } catch (MissingReferenceException)
        {
            return -1f;
        }
    }

    //physically place the houses
    private IEnumerator PlaceHouses(bool[,] houses)
    {
        yield return new WaitForSeconds(0.5f);
        
        //for each row of houses...
        for(int i = 0; i < houses.GetLength(0); i++)
        {
            //randomly shift the houses across and up a bit on each row to make it look a bit nicer
            float xShift = Random.Range(0, 3f * (horizontalMin / 4f));
            if (Random.Range(0, 2) == 0) xShift = 0 - xShift;

            float yShift = Random.Range((verticalMin / 4f), 3f * (verticalMin / 4f));

            //add houses where they should be
            for (int j = 0; j < houses.GetLength(1); j++)
            {
                if (houses[i, j] == true)
                {
                    AddHouse(coordinates[i, j] + xShift);
                }
            }

            //wait until the last house placed is appropriately far away
            yield return new WaitUntil(() => (LastHouseDistance() >= verticalMin - yShift || LastHouseDistance() == -1f));
        }

        yield return new WaitForSeconds(0.25f);

        if(!controller.GameIsOver()) trees.Resume(); //resume spawning trees
    }

    public void Stop()
    {
        stopped = true;
    }

    public void Resume()
    {
        stopped = false;
    }
}
