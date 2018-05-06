using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballController : MonoBehaviour {

    private float velocity = 0;
    private float maxSpeed = 4f;
    private float acceleration = 0.25f;
    private float momentum = 8f;
    
    [SerializeField]
    private float screenLimit = 8.6f;

    public GameController controller;

    public GameObject snowflake;
    private const int MIN_SNOWFLAKES = 4;
    private const int MAX_SNOWFLAKES = 8;
    private float previousVelocity = 0f;
    
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

        if(transform.position.x < -screenLimit-0.25)
        {
            transform.position = new Vector3(screenLimit, transform.position.y);
            
        } else if (transform.position.x > screenLimit+0.25)
        {
            transform.position = new Vector3(-screenLimit, transform.position.y);
          
        } else if (velocity != 0)
        {
            if(velocity > 0)
            {
                velocity = ( ((velocity - acceleration / momentum) < 0) ? 0 : (velocity - acceleration / momentum));
            } else
            {
                velocity = ( ((velocity + acceleration / momentum) > 0) ? 0 : (velocity + acceleration / momentum));
            }
        }

        if(previousVelocity > 0 && velocity < 0)
        {
            
        } else if (previousVelocity < 0 && velocity > 0)
        {
            
        }

        previousVelocity = velocity;
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

    public IEnumerator MoveDownScreen()
    {
        velocity = 0f;
        for (float t = 0; t<1; t+= Time.deltaTime / 1.75f)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, Mathf.Lerp(4.05f, 2f, t));
            yield return null;
        }
        
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, 2f);
        yield return null;
    }

    //private IEnumerator BrushSnow(float leftmostStart, float rightmostStart, float minAngle, float maxAngle)
    //{
    //    int noSnowflakes = Random.Range(MIN_SNOWFLAKES, MAX_SNOWFLAKES + 1);

    //    for(int i = 0; i < noSnowflakes; i++)
    //    {
    //        GameObject newSnowflake = Instantiate(snowflake, new Vector3(Random.Range(gameObject.transform.position.x + leftmostStart, gameObject.transform.position.x + rightmostStart), gameObject.transform.position.y), gameObject.transform.rotation);
    //        float scale = Random.Range(0.33f, 1f);
    //        newSnowflake.transform.localScale = new Vector3(scale, scale);
    //        float angle = Random.Range(minAngle, maxAngle);
    //        StartCoroutine(MoveSnow(angle, 1.33f - scale));
    //        yield return null;
    //    }
    //}

    //private IEnumerator MoveSnow(float angle, float lifetime)
    //{

    //}
}
