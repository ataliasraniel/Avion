using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_Controller : MonoBehaviour
{
    public bool showConsole;
    private string stringToEdit;

    private void Update()
    {
        OnToggleDebug();
    }
    public void OnToggleDebug()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            showConsole = !showConsole;
        }
    }
    private void OnGUI()
    {
        if (!showConsole) { return; }
        float y = 0;
        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        stringToEdit = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), stringToEdit);

    }
}
