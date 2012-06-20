using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ModelViewer
{
    public class ArcBallCamera
    {
        public const float min_distance = 1f;
        public const float max_distance = 50000f;
        public const float near_plane = 0.1f;
        public const float far_plane = 100000f;
        public const float min_fov = MathHelper.PiOver4;
        public const float max_fov = MathHelper.Pi * 0.8f; // cant have a fov of Pi...

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        public float Pitch { get; set; }
        public float Yaw { get; set; }
        public float Distance { get; set; }

        // in case we resize window
        public float AspectRatio { get; set; }

        // control can change this
        public float FieldOfView { get; set; }

        private Vector3 cameraPosition;

        private Vector3 cameraTarget;
        public Vector3 CameraTarget
        {
            get { return cameraTarget; }
            set { cameraTarget = value; }
        }

        public ArcBallCamera()
        {
            Pitch = 1f;
            Yaw = 1f;
            AspectRatio = 0;
            FieldOfView = 0;
        }

        public void Update()
        {
            // dont wrap around the model on top
            // also make sure it doesnt equal Pi otherwise it looks weird
            Pitch = MathHelper.Clamp(Pitch, 0.001f, MathHelper.Pi * 0.99f);

            Distance = MathHelper.Clamp(Distance, min_distance, max_distance);

            cameraPosition = new Vector3(0f, Distance, 0f);
            cameraPosition = Vector3.Transform(cameraPosition, Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, 0f));

            Projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, near_plane, far_plane);           
            //View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            View = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
        }

    }
}
