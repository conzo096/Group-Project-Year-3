using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace NodeEditor
{
    [Serializable]
    public class VisualObject
    {
        public string identifier;

        public Dictionary<PropertyInfo, Component> propertyInfo = new Dictionary<PropertyInfo, Component>();
        public Dictionary<FieldInfo, Component> fieldInfo = new Dictionary<FieldInfo, Component>();

        public VisualObject(string identifier)
        {
            this.identifier = identifier;
        }

        // Override Equals operator
        public override bool Equals(object other)
        {
            var item = other as VisualObject;

            return this.identifier.Equals(item.identifier);
        }

        // Iverrude GetHashCode operator
        public override int GetHashCode()
        {
            return identifier.GetHashCode();
        }
    }
}
