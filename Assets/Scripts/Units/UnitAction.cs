using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAction : MonoBehaviour
{
    public Unit parent;

    public void Use()
    {
        if(parent != null)
        {
            parent.doAction();
        }
    }

    public void setText(string text)
    {
        GetComponentInChildren<UnityEngine.UI.Text>().text = text;
    }
}
