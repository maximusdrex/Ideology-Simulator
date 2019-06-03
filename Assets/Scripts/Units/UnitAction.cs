using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAction : MonoBehaviour
{
    public Unit parent;

    void Start()
    {
        parent = Camera.main.GetComponent<PlayerManager>().rememberedUnit;
    }

    public void Use()
    {
        Debug.Log("Use");
        if(parent != null)
        {
            Debug.Log("Action!");
            parent.doAction();
        }
    }

    public void setText(string text)
    {
        GetComponentInChildren<UnityEngine.UI.Text>().text = text;
    }
}
