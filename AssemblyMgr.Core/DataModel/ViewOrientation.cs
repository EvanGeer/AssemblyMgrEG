using System;
using System.Collections.Generic;
using System.Text;

namespace AssemblyMgr.Core.DataModel
{
    public enum Orientation
    {
        None = 0,
        
        // plan view orientations
        Left = 5,
        Right = 6,
        Front = 7,
        Back = 8,

        // view cube orientations
        TopFrontRight = 9,
        TopFrontLeft = 10,
        TopBackRight = 11,
        TopBackLeft = 12,
    }

    public enum Quadrant
    {
        TopRight = 1, 
        TopLeft = 2, 
        BottomLeft = 3,
        BottomRight = 4, 
    }
}
