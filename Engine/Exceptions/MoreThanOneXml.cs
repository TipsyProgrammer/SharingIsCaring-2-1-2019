using System;

namespace Engine.Exceptions
{
    public class MoreThanOneXml : Exception
    {
        public MoreThanOneXml(string message) : base (message)
        {
                
        }
    }
}