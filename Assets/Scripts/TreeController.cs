using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour {

    private float speed = 0;
    private float maxSpeed = 8;
    private float acceleration = 1;

    private int screenHeight;
    private GameController controller;

    private void Awake()
    {
        screenHeight = Screen.height;
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
}
