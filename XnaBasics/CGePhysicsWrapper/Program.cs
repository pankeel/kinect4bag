using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CGePhysicsWrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            // ----- Object creation -----

            Console.WriteLine("Creating some objects:");
            CGePhysX test = new CGePhysX();
            test.OnInit();
            string filePath = Path.GetFullPath("..\\Media\\skirt2.obj");

            test.createCloth(filePath);


            Console.WriteLine("Goodbye");
        }
    }
}
