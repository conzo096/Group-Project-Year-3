/* Used as a replacement of PropertyInfo, of the Reflection class. */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NodeEditor
{
    public class CustomPropertyInfo
    {
        public PropertyInfo propertyInfo;
        public string parent;

        public CustomPropertyInfo(PropertyInfo propertyInfo, string parent)
        {
            this.propertyInfo = propertyInfo;
            this.parent = parent;
        }

        // Override Equals operator
        public override bool Equals(object other)
        {
            var item = other as CustomPropertyInfo;

            return this.propertyInfo == item.propertyInfo && this.parent == item.parent;
        }

        // Override GetHashCode operator
        public override int GetHashCode()
        {
            return propertyInfo.GetHashCode();
        }
    }
}
