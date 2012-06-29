//------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;

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
            //ModelViewer.App app = new ModelViewer.App();
            //app.Run();
            //using (XnaBasics.AvateeringXNA game = new XnaBasics.AvateeringXNA())
            using (XnaBasics.XnaBasics game = new XnaBasics.XnaBasics())
            //using (JiggleGame.JiggleGame game = new JiggleGame.JiggleGame())
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