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

    // What visual effect has been selected.
    public bool selectedMesh;
    public GameObject sm;
    // What audio parameter has been selected.
    public bool selectedAudio;
    public GameObject sa;

    // Line selected.
    public bool selectedLine;

    int lineCounter =0;

    public void Start()
    {
        // Initial variables.
        audioPos = new ArrayList();
        meshPos = new ArrayList();
        selectedAudio = false;
        selectedMesh = false;
        selectedLine = false;
        // Add all the UI features into correct lists.
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MeshButton"))
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
        //Debug.Log(Camera.main.ScreenPointToRay(Input.mousePosition).origin);
        // If a visual and audio property has been selected, create a line and add to list of lines.
        if (selectedMesh && selectedAudio)
        {
            DrawGuiLine(sa.transform.position, sm.transform.position);
            selectedAudio = false;
            selectedMesh = false;
        }
    }

    public void OnGUI()
    {
        foreach (GameObject obj in meshPos)
        {
            if (EventSystem.current.currentSelectedGameObject == obj && selectedMesh == false)
            {
                selectedMesh = true;
                sm = obj;
            }
        }
        foreach (GameObject obj in audioPos)
        {
            if (EventSystem.current.currentSelectedGameObject == obj && selectedAudio == false)
            {
                selectedAudio = true;
                sa = obj;
            }
        }

       
        foreach (GameObject obj in lineCollection)
        {
            if (EventSystem.current.currentSelectedGameObject == obj && selectedLine == false)
            {
                selectedLine = true;
                Debug.Log("well done, you managed to press the line. ");

                RemoveLine(obj);
                
                Destroy(obj, 2);
                break;
            }
        }
    }


    // Draws the line, just in the incorrect place.
    public void DrawGuiLine(Vector3 start, Vector3 end)
    {

        GameObject lineObj = new GameObject();
        lineObj.layer = 5;
        lineObj.tag = "LineConnection";
        lineObj.name = "LineConnection(" + lineCounter + ")";
        lineObj.AddComponent<Image>();
        lineObj.AddComponent<Button>();

        lineObj.transform.SetParent(GameObject.Find("Panel").transform);

        RectTransform rt = lineObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(1, Vector3.Distance(start, end));
        rt.anchoredPosition = new Vector2(0, 0);
        Vector3 pos = Vector3.Lerp(start, end, 0.5f);
        rt.transform.position = pos;
        rt.transform.localPosition = new Vector3(0, 0, 0);


        // This part draws a line that cannot be interacted with, this was for testing purposes.
        lineObj.AddComponent<LineRenderer>();
        LineRenderer lineObjRend = lineObj.GetComponent<LineRenderer>();
        lineObjRend.enabled = false;
        lineObjRend.SetPosition(0, start);
        lineObjRend.SetPosition(1, end);
        lineCounter++;

        lineObj.AddComponent<LineConnection>();
        LineConnection newLine = lineObj.GetComponent<LineConnection>();
        newLine.VisualConnection = sm.GetComponentInChildren<Text>().text;
        newLine.AudioConnection = sa.GetComponentInChildren<Text>().text;
        lineCollection.Add(lineObj);
 

    }


    public void AddNewLine(string[] pair)
    {
        string[] newLine = new string[2];
        newLine = pair;
        lineCollection.Add(newLine);
    }

    public  bool RemoveLine(GameObject line)
    {
        if (lineCollection.Contains(line))
        {
            lineCollection.Remove(line);
            return true;
        }
        return false;
    }

    public string FindMeshParameter(string par)
    {
        Debug.Log(lineCollection.Count);
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
