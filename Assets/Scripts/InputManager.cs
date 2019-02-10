using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public static InputManager Instance
    {
        get
        {
            if (instance != null)
                return instance;

            instance = FindObjectOfType<InputManager>();

            if (instance != null)
                return instance;

            Create();

            return instance;
        }
    }

    protected static InputManager instance;

    public static InputManager Create()
    {
        GameObject go = new GameObject("InputManager");
        instance = go.AddComponent<InputManager>();

        return instance;
    }

    public enum Axis
    {
        Horizontal,
        Vertical,
    }

    public enum Button
    {
        Fire1,
        Jump,
    }

    protected string[] axisNames;
    protected string[] buttonNames;

    protected Dictionary<string, float> axis;
    protected Dictionary<string, bool> pressed;
    protected Dictionary<string, bool> held;
    protected Dictionary<string, bool> released;

    private void Awake ()
    {
        // List enums
        axisNames = System.Enum.GetNames(typeof(Axis));
        buttonNames = System.Enum.GetNames(typeof(Button));

        // Init axis
        axis = new Dictionary<string, float>();
        // Populate axis
        for (int i = 0; i < axisNames.Length; i++)
            axis.Add(axisNames[i], 0f);

        // Init buttons
        pressed = new Dictionary<string, bool>();
        held = new Dictionary<string, bool>();
        released = new Dictionary<string, bool>();
        // Populate buttons
        for (int i = 0; i < buttonNames.Length; i++)
        {
            pressed.Add(buttonNames[i], false);
            held.Add(buttonNames[i], false);
            released.Add(buttonNames[i], false);
        }
    }

    private void Update ()
    {
        // Update axis
        for (int i = 0; i < axisNames.Length; i++)
        {
            name = axisNames[i];
            axis[name] = Input.GetAxis(name);
        }

        // Update buttons
        for (int i = 0; i < buttonNames.Length; i++)
        {
            name = buttonNames[i];
            pressed[name] = Input.GetButtonDown(name);
            held[name] = Input.GetButton(name);
            released[name] = Input.GetButtonUp(name);
        }
    }

    public float GetAxis (Axis name) { return axis[name.ToString()]; }

    public bool IsPressed (Button name) { return pressed[name.ToString()]; }
    public bool IsHeld (Button name) { return held[name.ToString()]; }
    public bool IsReleased (Button name) { return released[name.ToString()]; }

    public void Test ()
    {

    }

}
