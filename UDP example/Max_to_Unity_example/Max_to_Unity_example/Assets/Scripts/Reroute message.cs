using UnityEngine;
using System.Collections;


public class Reroutemessage
{
    // attribute to be changed.
    private string attribute;
    // New value for attribute.
    private object attributeValue;

    public Reroutemessage() { }


    public string Attribute
    {
        get
        {
            return attribute;
        }

        set
        {
            attribute = value;
        }
    }

    public object AttributeValue
    {
        get
        {
            return attributeValue;
        }

        set
        {
            attributeValue = value;
        }
    }


    public void ConvertOSCMessage(OscMessage maxMessage)
    {
        Attribute = maxMessage.Address;
        AttributeValue = maxMessage.Values[0];

    }

}
