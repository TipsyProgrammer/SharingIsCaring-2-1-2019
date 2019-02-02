using System;
using Engine;

namespace Svc
{
    class Program
    {
        public void Main(string[] args)
        {
            try
            {
                Worker worker = new Worker();
                worker.DoWork();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}