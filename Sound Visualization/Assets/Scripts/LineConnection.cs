using UnityEngine;
using System.Collections;

public class LineConnection : MonoBehaviour
{
    // Iss it currently connected to anything?
    public bool isConnected = true;
    private string visualConnection;
    private string audioConnection;

    public string VisualConnection
    {
        get
        {
            return visualConnection;
        }

        set
        {
            visualConnection = value;
        }
    }

    public string AudioConnection
    {
        get
        {
            return audioConnection;
        }

        set
        {
            audioConnection = value;
        }
    }
}
