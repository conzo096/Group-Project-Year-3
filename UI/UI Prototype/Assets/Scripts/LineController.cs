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

    public bool selectedMesh;
    public GameObject sm;
    public bool selectedAudio;
    public GameObject sa;
    linecreation lc;


    public void Start()
    {
        lc = new linecreation();
        audioPos = new ArrayList();
        meshPos = new ArrayList();
        selectedAudio = false;
        selectedMesh = false;
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
       // Debug.Log(Camera.main.ScreenPointToRay(Input.mousePosition).origin);
        if (selectedMesh && selectedAudio)
        {
           // lc.DrawGuiLine(sa.transform.position, sm.transform.position);
            string[] combination = new string[2];
            combination[0] = sa.GetComponentInChildren<Text>().text;
            combination[1] = sm.GetComponentInChildren<Text>().text;
            if(!lineCollection.Contains(combination))
                lineCollection.Add(combination);
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
                Debug.Log(EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text);
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
    }

    public void AddNewLine(string[] pair)
    {
        string[] newLine = new string[2];
        newLine = pair;
        lineCollection.Add(newLine);
    }

    public  bool RemoveLine(string[] connection)
    {
        if (lineCollection.Contains(connection))
        {
            lineCollection.Remove(connection);
            return true;
        }
        return false;
    }

    public string FindMeshParameter(string par)
    {
        foreach (string[] line in lineCollection)
        {
            Debug.Log(par + " | " + line[0]);
            if (line[0] == par)
            {
                Debug.Log("line 1 " + line[1]);
                return line[1];
            }
        }
        return "Not found";
    }
}
