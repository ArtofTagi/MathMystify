using UnityEngine;
using System.Collections;

public class CloudControl : MonoBehaviour {

    public float add = .5f;
    public RectTransform rectTransform;

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(rectTransform.position.x - 200> Screen.width || rectTransform.position.x + 200 < 0) {
            add = add * -1;
        }
        rectTransform.position = new Vector3(rectTransform.position.x + add, rectTransform.position.y, rectTransform.position.z);
    }
}
