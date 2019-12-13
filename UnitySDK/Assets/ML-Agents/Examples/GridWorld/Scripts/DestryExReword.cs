using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestryExReword : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider collider)
    {
        //if (collider.gameObject.name == "exReword")
        Destroy(this.gameObject);
        Debug.Log("Destroy：exReword");
    }
}
