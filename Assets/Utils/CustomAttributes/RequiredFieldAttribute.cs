using System;
using UnityEngine;

namespace CustomAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RequiredFieldAttribute : PropertyAttribute
    {
        public WarningType warningType;

        public RequiredFieldAttribute(WarningType warningType = WarningType.Error)
        {
            this.warningType = warningType;
        }
    }

    public enum WarningType
    {
        Error,
        Warning
    }
}
