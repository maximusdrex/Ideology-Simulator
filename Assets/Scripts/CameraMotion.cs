using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour
{
    public Vector3 lastPosition;
    public bool hasMoved;
    public int magnitude;
    public float panSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = this.transform.position;
        hasMoved = true;
        magnitude = 1;
        panSpeed = 20;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = this.transform.position;
        if(Mathf.Abs(this.transform.position.x - lastPosition.x) > magnitude)
        {
            lastPosition = this.transform.position;
            hasMoved = true;
        }
        else
        {
            hasMoved = false;
        }
        if(Input.GetKey("w"))
        {
           pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("q") && pos.y > 1.7f)
        {
            pos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("e") && pos.y < 7f)
        {
            pos.y += panSpeed * Time.deltaTime;
        }
        transform.position = pos;
    }
}
