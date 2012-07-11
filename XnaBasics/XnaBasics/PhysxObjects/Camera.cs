using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace JiggleGame
{
    /// <summary>
    /// First person camera component for the demos, rotated by mouse.
    /// </summary>
    public class Camera : GameComponent
    {
        private Matrix view;
        private Matrix projection;

        private Vector3 position = new Vector3(0,0,0);
        private Vector2 angles = Vector2.Zero;
        public Vector2 Angles
        {
            get { return angles; }
            set { angles = value; }
        }
        private int widthOver2;
        private int heightOver2;

        private float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
        private float aspectRatio;
        private float nearPlaneDistance = 0.1f;
        private float farPlaneDistance = 10000.0f;


        /// <summary>
        /// Initializes new camera component.
        /// </summary>
        /// <param name="game">Game to which attach this camera.</param>
        public Camera(Game game)
            : base(game)
        {
            widthOver2 = game.Window.ClientBounds.Width / 2;
            heightOver2 = game.Window.ClientBounds.Height / 2;
            aspectRatio = (float)game.Window.ClientBounds.Width / (float)game.Window.ClientBounds.Height;
            UpdateProjection();
            //Mouse.SetPosition(widthOver2, heightOver2);
        }

        /// <summary>
        /// Gets camera view matrix.
        /// </summary>
        public Matrix View { get { return view; } set { view = value; } }
        /// <summary>
        /// Gets or sets camera projection matrix.
        /// </summary>
        public Matrix Projection { get { return projection; } set { projection = value; } }
        /// <summary>
        /// Gets camera view matrix multiplied by projection matrix.
        /// </summary>
        public Matrix ViewProjection { get { return view * projection; } }

        /// <summary>
        /// Gets or sets camera position.
        /// </summary>
        public Vector3 Position { get { return position; } set { position = value; } }

        /// <summary>
        /// Gets or sets camera field of view.
        /// </summary>
        public float FieldOfView { get { return fieldOfView; } set { fieldOfView = value; UpdateProjection(); } }
        /// <summary>
        /// Gets or sets camera aspect ratio.
        /// </summary>
        public float AspectRatio { get { return aspectRatio; } set { aspectRatio = value; UpdateProjection(); } }
        /// <summary>
        /// Gets or sets camera near plane distance.
        /// </summary>
        public float NearPlaneDistance { get { return nearPlaneDistance; } set { nearPlaneDistance = value; UpdateProjection(); } }
        /// <summary>
        /// Gets or sets camera far plane distance.
        /// </summary>
        public float FarPlaneDistance { get { return farPlaneDistance; } set { farPlaneDistance = value; UpdateProjection(); } }

        /// <summary>
        /// Gets or sets camera's target.
        /// </summary>
        public Vector3 Target
        {
            get
            {
                Matrix cameraRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);
                return position + Vector3.Transform(Vector3.Forward, cameraRotation);
            }
            set
            {
                Vector3 forward = Vector3.Normalize(position - value);
                Vector3 right = Vector3.Normalize(Vector3.Cross(forward, Vector3.Up));
                Vector3 up = Vector3.Normalize(Vector3.Cross(right, forward));

                Matrix test = Matrix.Identity;
                test.Forward = forward;
                test.Right = right;
                test.Up = up;
                angles.X = -(float)Math.Asin(test.M32);
                angles.Y = -(float)Math.Asin(test.M13);
            }
        }

        /// <summary>
        /// Updates camera with input and updates view matrix.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                double elapsedTime = (double)gameTime.ElapsedGameTime.Ticks / (double)TimeSpan.TicksPerSecond;
                ProcessInput((float)elapsedTime * 50.0f);
                UpdateView();

                base.Update(gameTime);
            }

        }

        /// <summary>
        /// Handle the mouse position for arcball
        /// change camera view
        /// </summary>
        /// <param name="amountOfMovement"></param>
        private void ProcessInput(float amountOfMovement)
        {

        }

        private void UpdateProjection()
        {
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        private void UpdateView()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);
            Vector3 targetPos = position + Vector3.Transform(Vector3.Backward, cameraRotation);

            Vector3 upVector = Vector3.Transform(Vector3.Up, cameraRotation);

            view = Matrix.CreateLookAt(position, targetPos, upVector);
        }
    }
}
