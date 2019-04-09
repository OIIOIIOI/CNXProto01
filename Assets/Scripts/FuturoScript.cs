using System;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class FuturoScript : MonoBehaviour
{
    
    static string COMBO_AMBIENT = "A";
    static string COMBO_SHOOT = "B";
    static string COMBO_AFTERMATH = "C";

    public enum Message4D
    {
        Ambient,
        Shoot,
        Aftermath,
    }

    protected List<string> buffer = new List<string>();
    protected string lastConfirmed;
    private SerialPort stream;

    public void Activate4D (Message4D msg, bool overwrite = true)
    {
        //Debug.Log("Activate "+msg);

        if (overwrite)
            buffer.Clear();

        switch (msg)
        {
            case Message4D.Ambient:
                buffer.Add(COMBO_AMBIENT);
                break;

            case Message4D.Shoot:
                buffer.Add(COMBO_SHOOT);
                break;

            case Message4D.Aftermath:
                buffer.Add(COMBO_AFTERMATH);
                break;
        }
    }

    void Awake ()
    {
        //Open("COM10");
        foreach (string port in SerialPort.GetPortNames())
        {
            try
            {
                Open(port);
            }
            catch (Exception e)
            {
                //Debug.Log("Exception");
            }
        }
    }

    void Start ()
    {
        Activate4D(Message4D.Ambient);
    }

    void Update ()
    {
        // If nothing in the queue, return
        if (buffer.Count == 0)
            return;

        // If last confirmed is the same as the next command in the queue, skip it
        if (lastConfirmed == buffer[0])
        {
            buffer.RemoveAt(0);
            if (buffer.Count == 0)
                return;
        }
        // If not, write to arduino
        else
            Write(buffer[0]);

        // Read from arduino
        string data = Read();
        // If received data is the same as the next one in the queue,
        // store it as the last received command and remove it from the queue
        if (data == buffer[0])
        {
            lastConfirmed = data;
            buffer.RemoveAt(0);
        }
    }

    void Open (string autoport)
    {
        stream = new SerialPort("\\\\.\\" + autoport, 115200);
        stream.ReadTimeout = 50;
        try
        {
            stream.Open();
        }
        catch (Exception e)
        {
            //Debug.Log("Exception");
        }
    }

    void Write (string command)
    {
        stream.Write(command + "\n");
        stream.BaseStream.Flush();
    }

    string Read ()
    {
        try {
            return stream.ReadLine();
        }
        catch (TimeoutException) {
            return null;
        }
    }

}
