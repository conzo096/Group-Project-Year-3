/* Line Controller class by Conner Weatherston. Last edited: 07/03/2017
 * This class controls the creation, deletion and searching of line connections between parameters.
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LineController : MonoBehaviour
{

    // List of all created lines.
    private ArrayList lineCollection = new ArrayList();

    // List of all positions audio parameter buttons are at.
    public ArrayList audioPos;
    public ArrayList meshPos;

    // Rename these two variables.
    // What visual effect has been selected.
    public GameObject sm;
    // What audio parameter has been selected.
    public GameObject sa;


    int lineCounter =0;

    public void Start()
    {
        // Initial variables.
        audioPos = new ArrayList();
        meshPos = new ArrayList();
        // Add all the UI features into correct lists.
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("VisualButton"))
        {
            meshPos.Add(obj);
        }
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("AudioButton"))
        {
            audioPos.Add(obj);
        }
    }


    public void Update()
    {

        // Check if two options have been selected, if so, create a line and reset parameters.
        if (sm != null && sa != null)
        {
            DrawLineConnection(sa.transform.position, sm.transform.position);
            // This resets the event system to prevent the second option being automatically selected.
            GameObject myEventSystem = GameObject.Find("EventSystem");
            myEventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);
            sm = null;
            sa = null;
        }

        // Detect if any of the lines have been selected by the user, if so destroy the connection.
        LineSelectedCheck();

    }

    // Check if any of the line connections have been selected.
    private void LineSelectedCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, 1000))
            {
                // Need to find out how to get parent of collider.
                if (hit.collider.tag == "LineConnection")
                {
                    Debug.Log("well done, you managed to press the line. ");
                    RemoveLine(hit.collider.gameObject.GetComponent<LineConnection>());
                    Destroy(hit.collider.gameObject, 2);
                }
            }
        }
    }


    // Constantly check if the user has selected any audio or visual parameters.
    public void OnGUI()
    {
        foreach (GameObject obj in meshPos)
        {
            if (EventSystem.current.currentSelectedGameObject == obj)
            {
                sm = obj;
            }
        }
        foreach (GameObject obj in audioPos)
        {
            if (EventSystem.current.currentSelectedGameObject == obj)
            {
                sa = obj;
            }
        }

    }


    // Create a new Game object which contains all the information for the line connection.
    public void DrawLineConnection(Vector3 start, Vector3 end)
    {
        // Initalise new object.
        GameObject lineObj = new GameObject();
        // Set the layer to UI.
        lineObj.layer = 5;
        lineObj.tag = "LineConnection";
        lineObj.name = "LineConnection(" + lineCounter + ")";
 
        // Create visual line between two connected parameters.
        lineObj.AddComponent<LineRenderer>();
        LineRenderer lineObjRend = lineObj.GetComponent<LineRenderer>();
        lineObjRend.SetPosition(0, start);
        lineObjRend.SetPosition(1, end);
        // LineCounter is just to keep track of Line game objects.
        lineCounter++;

        // Add a collider to the line. - Need to readjust formaula.
        lineObj.AddComponent<BoxCollider>(); 
        BoxCollider col = lineObj.GetComponent<BoxCollider>();
        float lineLength = Vector3.Distance(start, end); // length of line
        col.size = new Vector3(lineLength, 3f, 2f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
        Vector3 midPoint = (start + end) / 2;
        col.transform.position = midPoint; // setting position of collider object

        // Following lines calculate the angle between startPos and endPos
        float angle = (Mathf.Abs(start.y - end.y) / Mathf.Abs(start.x - end.x));
        if ((start.y < end.y && start.x > end.x) || (end.y < start.y && end.x > start.x))
        {
            angle *= -1;
        }
        angle = Mathf.Rad2Deg * Mathf.Atan(angle);
        col.transform.Rotate(0, 0, angle);


        // Add the line connection class to the line.
        lineObj.AddComponent<LineConnection>();
        LineConnection newLine = lineObj.GetComponent<LineConnection>();
        // Set the connection parameters. - This needs more detail as it will not work with Jacks UI. 

        newLine.VisualConnection = ConvertVisualText(sm.GetComponentInChildren<Text>().text);
        newLine.AudioConnection = ConvertAudioText(sa.GetComponentInChildren<Text>().text);
        // Set the parent of the line to the canvas.
        lineObj.transform.SetParent(GameObject.Find("Canvas").transform);

        // Add this line to the array of connections.
        if (!CheckLineMatch(newLine))
            lineCollection.Add(newLine);
        else
            Destroy(lineObj);
    }

    // CONVERTION METHODS ARE NOT FULLY IMPLEMENTED.

    // Converts message from the UI to valid one for the max-unity message.
    private string ConvertAudioText(string s)
    {
        // Check for audio parameters.
        if (s.ToLower().Contains("pitch"))
            return "/Pitch";
        if (s.ToLower().Contains("volume"))
            return "/Volume";
        if (s.ToLower().Contains("amplitude"))
            return "/Amplitude";

        return "Need to convert";
    }

    // Converts message from the UI to valid one for the max-unity message. - DOUBLE CHECK THESE VALUES WITH THE OBJECT MANAGER.
    private string ConvertVisualText(string s)
    {
        // Check for audio parameters.
        if (s.ToLower().Contains("scale"))
        {
            // Check if only x,y,z or all.
            return "/Scale";
        }

        if (s.ToLower().Contains("rotation"))
        {
            // Check if only x,y,z or all.
            return "/Rotation";
        }
        if (s.ToLower().Contains("Translation"))
        {
            // Check if only x,y,z or all.
            return "/Translate";
        }
        return "Need to convert";
    }

    // Remove the line from the scene/ Detach connection. - DOUBLE CHECK THESE VALUES WITH THE OBJECT MANAGER.
    public bool RemoveLine(LineConnection line)
    {
        if (lineCollection.Contains(line))
        {
            lineCollection.Remove(line);
            return true;
        }
        return false;
    }

    // Check if this line is already been created- This can be improved by changing data structure. Rename this method.
    public bool CheckLineMatch(LineConnection line)
    {
        foreach (LineConnection obj in lineCollection)
        {
            if (obj.VisualConnection == line.VisualConnection
                && obj.AudioConnection == line.AudioConnection)
                return true;
        }
        return false;
    }

    // Find the corrosponding visual effect for the audio parameter
    public string FindMeshParameter(string par)
    {
        foreach (LineConnection line in lineCollection)
        {
            Debug.Log(line.AudioConnection);
            if (line.AudioConnection == par)
            {
                Debug.Log(line.VisualConnection);
                return line.VisualConnection;
            }
        }
        return "Not found";
    }
}
