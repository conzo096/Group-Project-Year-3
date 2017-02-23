using UnityEngine;
using System.Collections;

public class LineController : MonoBehaviour
{

    private ArrayList lineCollection = new ArrayList();

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
            if (line[0] == par)
            {
                string t = line[1] + "in findmeshpara";
                Debug.Log(t);
                return line[1];
            }
        return "Not found";
    }
}
