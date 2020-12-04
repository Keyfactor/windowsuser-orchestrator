using System;

namespace Keyfactor.AnyAgent.WindowsUser
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JobAttribute : Attribute
    {
        private string jobClass { get; set; }

        public JobAttribute(string jobClass)
        {            
            this.jobClass = jobClass;
        }

        public virtual string JobClass
        {
            get { return jobClass; }
        }
    }
}
