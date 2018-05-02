using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballController : MonoBehaviour {

    private float velocity = 0;
    private float maxSpeed = 4f;
    private float acceleration = 0.75f;
    private float momentum = 8f;
    
    [SerializeField]
    private float screenLimit = 8.6f;

    public GameController controller;

	// Use this for initialization
	void Start () {
        StartCoroutine(MoveDownScreen());
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if(velocity > 0 - maxSpeed)
            {
                velocity = (velocity > 0 ? (velocity - acceleration * 2) : (velocity - acceleration));
            }
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            if(velocity < maxSpeed)
            {
                velocity = (velocity < 0 ? (velocity + acceleration * 2) : (velocity + acceleration));
            }
        }

        float moveAmount = velocity * Time.deltaTime;
        transform.Translate(new Vector3(moveAmount, 0));

        if(transform.position.x < -screenLimit)
        {
            transform.position = new Vector3(-screenLimit, transform.position.y);
            velocity = 0;
        } else if (transform.position.x > screenLimit)
        {
            transform.position = new Vector3(screenLimit, transform.position.y);
            velocity = 0;
        } else if (velocity != 0)
        {
            if(velocity > 0)
            {
                velocity = (velocity - acceleration / momentum);
            } else
            {
                velocity = (velocity + acceleration / momentum);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Tree")) controller.PlayerHitTree();
    }

    public void SetMomentum(float to)
    {
        momentum = to;
    }

    public void SetAcceleration(float to)
    {
        acceleration = to;
    }

    public void SetMaxSpeed(float to)
    {
        maxSpeed = to;
    }

    public void SetSize(float to)
    {
        StartCoroutine(SetSizeSmoothly(gameObject.transform.localScale.x, to));
    }

    private IEnumerator SetSizeSmoothly(float from, float to)
    {
        for(float t = 0; t<1; t+= Time.deltaTime/1)
        {
            float scaleNow = Mathf.Lerp(from, to, t);
            gameObject.transform.localScale = new Vector3(scaleNow, scaleNow);
            yield return null;
        }

        gameObject.transform.localScale = new Vector3(to, to);
        yield return null;
    }

    private IEnumerator MoveDownScreen()
    {
        for (float t = 0; t<1; t+= Time.deltaTime/4f)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, Mathf.Lerp(4.05f, 2f, t));
            yield return null;
        }
        
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, 2f);
        yield return null;
    }
}
