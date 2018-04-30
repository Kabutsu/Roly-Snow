using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    private List<TreeController> trees;

	// Use this for initialization
	void Start () {
        trees = new List<TreeController>();
        Debug.Log("width:= " + Screen.width + ", height:= " + Screen.height);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddTree(TreeController tree)
    {
        trees.Add(tree);
    }

    public void RemoveTree(TreeController tree)
    {
        trees.Remove(tree);
        Destroy(tree.gameObject);
    }
}
