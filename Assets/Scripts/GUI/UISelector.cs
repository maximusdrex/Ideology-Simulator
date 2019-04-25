using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISelector : MonoBehaviour
{
    PlayerManager pm;
    List<IInteractableObj> ObjectList;
    void Start()
    {
        pm = Camera.main.GetComponent<PlayerManager>();
    }

    public void Init(List<IInteractableObj> objs)
    {
        ObjectList = objs;
        Dropdown dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<string> options = new List<string>();
        options.Add("Select an option: ");
        foreach (IInteractableObj obj in ObjectList)
        {
            options.Add(obj.GetName());
        }
        dropdown.value = -1;
        dropdown.AddOptions(options);
        dropdown.onValueChanged.AddListener(onSelected);
        Debug.Log("Finished init");
    }

    public void onSelected(int arg)
    {
        Debug.Log("started selected: " + arg.ToString());
        Dropdown dropdown = GetComponent<Dropdown>();
        if (dropdown.value >= 0)
        {
            Debug.Log(ObjectList[dropdown.value -1].GetName());
            pm.setGUI(ObjectList[dropdown.value -1].GetUI());
            Destroy(transform.gameObject);
        }
    }
}
