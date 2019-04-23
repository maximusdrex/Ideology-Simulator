using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnClick()
    {
        GameObject GUI = (GameObject)Resources.Load("TechCanvas");
        Camera.main.GetComponent<PlayerManager>().setGUI(GUI);
    }
}
