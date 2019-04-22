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
    }

    public enum Buttons
    {
        Fire,
        Jump,
    }

    public enum Controllers
    {
        Keyboard,
        Xbox360,
        Arcade
    }

    public Controllers controllerChoice = Controllers.Keyboard;
    string controllerPrefix;

    protected string[] playersNames;
    protected string[] axisNames;
    protected string[] buttonsNames;

    protected Dictionary<string, float> axis;
    protected Dictionary<string, bool> pressed;
    protected Dictionary<string, bool> held;
    protected Dictionary<string, bool> released;

    protected List<string> axisKeys;
    protected List<string> buttonsKeys;

    private void Awake ()
    {
        // Get chosen controller
        controllerPrefix = controllerChoice.ToString();

        // List enums
        axisNames = System.Enum.GetNames(typeof(Axis));
        buttonsNames = System.Enum.GetNames(typeof(Buttons));
        playersNames = System.Enum.GetNames(typeof(Player.Players));

        // Init axis
        axis = new Dictionary<string, float>();
        // Init buttons
        pressed = new Dictionary<string, bool>();
        held = new Dictionary<string, bool>();
        released = new Dictionary<string, bool>();

        // For each player, store all axis
        for (int p = 0; p < playersNames.Length; p++)
        {
            // Populate axis
            for (int a = 0; a < axisNames.Length; a++)
                axis.Add(playersNames[p] + axisNames[a], 0f);

            // Populate buttons
            for (int b = 0; b < buttonsNames.Length; b++)
            {
                pressed.Add(playersNames[p] + buttonsNames[b], false);
                held.Add(playersNames[p] + buttonsNames[b], false);
                released.Add(playersNames[p] + buttonsNames[b], false);
            }
        }

        // Store keys for iteration in update
        axisKeys = new List<string>(axis.Keys);
        buttonsKeys = new List<string>(pressed.Keys);
    }

    private void Update ()
    {
        // Update axis
        foreach (string k in axisKeys)
        {
            axis[k] = Input.GetAxis(k + controllerPrefix);
        }

        // Update buttons
        foreach (string k in buttonsKeys)
        {
            pressed[k] = Input.GetButtonDown(k + controllerPrefix);
            held[k] = Input.GetButton(k + controllerPrefix);
            released[k] = Input.GetButtonUp(k + controllerPrefix);
        }
    }

    public float GetAxis (string name) { return axis[name]; }
    public bool IsPressed (string name) { return pressed[name]; }
    public bool IsHeld (string name) { return held[name]; }
    public bool IsReleased (string name) { return released[name]; }

}
