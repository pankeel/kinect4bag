//------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.IO;

[assembly: CLSCompliant(true)]

namespace Microsoft.Samples.Kinect.Avateering
{
    /// <summary>
    /// The base XNA program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// This method starts the game cycle.
        /// </summary>
        public static void Main()
        {

            //CGePhysX test = new CGePhysX();
            //test.OnInit();
            //string filePath = Path.GetFullPath("..\\..\\..\\Media\\skirt2.obj");

            //test.createCloth(filePath);

            //PxVec3_Vector a = test.ClothVerticesWrapper;
            //foreach (PxVec3Wrapper pv3 in a)
            //{
            //    Console.WriteLine(String.Format("(%s,%s,%s)"), pv3.x, pv3.y, pv3.z);
            //}

            //ModelViewer.App app = new ModelViewer.App();
            //app.Run();
            //using (XnaBasics.AvateeringXNA game = new XnaBasics.AvateeringXNA())
            
            //using (JiggleGame.JiggleGame game = new JiggleGame.JiggleGame())
            using (XnaBasics.VirtualTryonGame game = new XnaBasics.VirtualTryonGame())
            {
                game.Run();
            }
        }

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        //[System.STAThreadAttribute()]
        //[System.Diagnostics.DebuggerNonUserCodeAttribute()]
        //public static void Main()
        //{
        //    ModelViewer.App app = new ModelViewer.App();
        //    app.Run();
        //}
    }
}