using System;
using UnityEngine;

namespace NodeEditor
{
    // This class is used to store the mouse position and the name of the callback request.
    public class CallBackObject
    {
        public string callBackName;
        public Vector2 mousePosition;

        public CallBackObject() { }

        public CallBackObject(string name, Vector2 pos)
        {
            callBackName = name;
            mousePosition = pos;
        }
    }
}
