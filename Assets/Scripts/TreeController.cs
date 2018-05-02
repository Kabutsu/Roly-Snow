using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour {

    private float speed = 0f;
    private float maxSpeed = 2.5f;
    private float acceleration = 0.025f;

    private float screenHeight;
    private GameController controller;

    private void Awake()
    {
        float vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;
        screenHeight = 25f / 2.0f - vertExtent;
    }

    // Use this for initialization
    void Start ()
    {
        controller = GameObject.Find("Game Controller").GetComponent<GameController>();
    }
	
	// Update is called once per frame
	void Update () {
        if(speed < maxSpeed) speed += acceleration;

        float moveAmount = speed * Time.deltaTime;
        gameObject.transform.Translate(new Vector3(0, moveAmount));

        if (gameObject.transform.position.y > screenHeight) controller.RemoveTree(this);
    }

    public void SlowDown()
    {
        speed = 0.15f;
    }

    public void SetMaxSpeed(float to)
    {
        maxSpeed = to;
    }
}
