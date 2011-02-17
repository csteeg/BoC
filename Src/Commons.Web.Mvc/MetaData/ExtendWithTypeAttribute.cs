using System;

namespace BoC.Web.Mvc.MetaData
{
    public class ExtendWithTypeAttribute : Attribute
    {
        private readonly Type with;

        public ExtendWithTypeAttribute(Type with)
        {
            this.with = with;
        }

        public Type With
        {
            get { return with; }
        }
    }
}