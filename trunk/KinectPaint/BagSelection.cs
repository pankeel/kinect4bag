using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Kinect.Samples.KinectPaint
{

    /// <summary>
    /// Contains information about a particular kind of bag
    /// </summary>
    public class BagSelection
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="icon">URI of the icon representing the brush</param>
        /// <param name="iconSelected">URI of the icon representing the brush when it is selected</param>
        /// <param name="brush">The type of brush</param>
        /// <param name="friendlyName">The user-friendly name of the brush</param>
        public BagSelection(Uri icon, Uri iconSelected, string friendlyName)
        {
            Icon = icon;
            IconSelected = iconSelected;
            FriendlyName = friendlyName;
        }

        #region Properties

        /// <summary>
        /// URI of the icon representing the brush
        /// </summary>
        public Uri Icon { get; private set; }

        /// <summary>
        /// URI of the icon representing the brush when the tool is selected
        /// </summary>
        public Uri IconSelected { get; private set; }

        /// <summary>
        /// The user-friendly name of the brush
        /// </summary>
        public string FriendlyName { get; private set; }

        #endregion
    }
}
