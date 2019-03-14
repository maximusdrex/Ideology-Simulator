using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour
{
    public Vector3 lastPosition;
    public bool hasMoved;
    public int magnitude;
    
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = this.transform.position;
        hasMoved = true;
        magnitude = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(this.transform.position.x - lastPosition.x) > 10)
        {
            lastPosition = this.transform.position;
            hasMoved = true;
        }
        else
        {
            hasMoved = false;
        }
    }
}
