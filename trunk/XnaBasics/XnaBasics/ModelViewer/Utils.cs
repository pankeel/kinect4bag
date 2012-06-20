using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;

namespace ModelViewer
{
    public static class Utils
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static string FullFilePathOfDragAndDroppedFile (String[] args)
        {

            //
            string fullFilePath = String.Empty;

            //
            if (args.Length > 0)
            {
                string p = args[0];
                string e = Path.GetExtension(p);
                if ((e == ".fbx") || (e == ".FBX") || (e == ".x") || (e == ".X"))
                {
                    //MessageBox.Show(("Got: " + args[0].ToString()));
                    fullFilePath = args[0].ToString();
                }
            }

            //
            return (fullFilePath);

        }

    }
}
