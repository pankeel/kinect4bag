using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public enum E_Gesture
{
    HorizonalLeftToRight = 0,
    HorizonalRightToLeft,
    VerticalUpToDown,
    VerticalDownToUp,
    Count,
};

namespace XUI.Input
{
    public class Kinect : Device
    {
        public override void Update(float frameTime)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool ButtonDown(int button)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool ButtonJustPressed(int button)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool ButtonJustReleased(int button)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override float ButtonValue(int button)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
