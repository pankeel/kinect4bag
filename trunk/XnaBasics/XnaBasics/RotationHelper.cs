using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    public class RotationHelper
    {

        // Returns Euler angles that point from one point to another
        public Vector3 AngleTo(Vector3 from, Vector3 location)
        {
            Vector3 angle = new Vector3();
            Vector3 v3 = Vector3.Normalize(location - from);

            angle.X = (float)Math.Asin(v3.Y);
            angle.Y = (float)Math.Atan2((double)-v3.X, (double)-v3.Z);

            return angle;
        }

        // Converts a Quaternion to Euler angles (X = Yaw, Y = Pitch, Z = Roll)
        public Vector3 QuaternionToEulerAngleVector3(Quaternion rotation)
        {
            Vector3 rotationaxes = new Vector3();
            Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);

            rotationaxes = AngleTo(new Vector3(), forward);

            if (rotationaxes.X == MathHelper.PiOver2)
            {
                rotationaxes.Y = (float)Math.Atan2((double)up.X, (double)up.Z);
                rotationaxes.Z = 0;
            }
            else if (rotationaxes.X == -MathHelper.PiOver2)
            {
                rotationaxes.Y = (float)Math.Atan2((double)-up.X, (double)-up.Z);
                rotationaxes.Z = 0;
            }
            else
            {
                up = Vector3.Transform(up, Matrix.CreateRotationY(-rotationaxes.Y));
                up = Vector3.Transform(up, Matrix.CreateRotationX(-rotationaxes.X));

                rotationaxes.Z = (float)Math.Atan2((double)-up.Z, (double)up.Y);
            }

            return rotationaxes;
        }

        // Converts a Rotation Matrix to a quaternion, then into a Vector3 containing
        // Euler angles (X: Pitch, Y: Yaw, Z: Roll)
        public Vector3 MatrixToEulerAngleVector3(Matrix Rotation)
        {
            Vector3 translation, scale;
            Quaternion rotation;

            Rotation.Decompose(out scale, out rotation, out translation);

            Vector3 eulerVec = QuaternionToEulerAngleVector3(rotation);

            return eulerVec;
        }

        // Converts a rotation vector into a rotation matrix
        public Matrix Vector3ToMatrix(Vector3 Rotation)
        {
            return Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
        }

        public Vector3 RadiansToDegrees(Vector3 Vector)
        {
            return new Vector3(
                MathHelper.ToDegrees(Vector.X),
                MathHelper.ToDegrees(Vector.Y),
                MathHelper.ToDegrees(Vector.Z));
        }

        public Vector3 DegreesToRadians(Vector3 Vector)
        {
            return new Vector3(
                MathHelper.ToRadians(Vector.X),
                MathHelper.ToRadians(Vector.Y),
                MathHelper.ToRadians(Vector.Z));
        }



        private static RotationHelper Instance;

        public static RotationHelper GetInstance()
        {
            if (Instance == null)
                Instance = new RotationHelper();
            return Instance;
        }
    }
}
