//------------------------------------------------------------------------------
// <copyright file="XnaBasicsGame.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------


// E_Layer
public enum E_Layer
{
    UI = 0,

    // etc ...

    Count,
};
namespace Microsoft.Samples.Kinect.XnaBasics
{
    using Microsoft.Kinect;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System;
    using Microsoft.Xna.Framework.GamerServices;
    using JiggleGame.PhysicObjects;
    using JigLibX.Physics;

    /// <summary>
    /// The main Xna game implementation.
    /// </summary>
    public class XnaBasics : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// This is used to adjust the window size.
        /// </summary>
        private int Width = 1024;
        private int Height = 600;
        //private const int selection_panel_w = 200;
        //private const int selection_panel_h = Width / 4 * 3;

        /// <summary>
        /// The 3D avatar mesh animator.
        /// </summary>
        private AvatarAnimator animator;

        private ClothRender clothRender;
        public ClothRender ClothModelRender
        {
            get { return clothRender; }
        }
        SimpleGUI ui;
        public static Vector2 LeftHand, RightHand;

        /// <summary>
        /// This controls the transition time for the resize animation.
        /// </summary>
        private const double TransitionDuration = 1.0;

        /// <summary>
        /// The graphics device manager provided by Xna.
        /// </summary>
        private readonly GraphicsDeviceManager graphics;
        private UiLayer						UiLayer;
        public bool IsRunningSlowly;

        /// <summary>
        /// Viewing Camera arc.
        /// </summary>
        private float cameraArc = 0;

        /// <summary>
        /// Viewing Camera current rotation.
        /// The virtual camera starts where Kinect is looking i.e. looking along the Z axis, with +X left, +Y up, +Z forward
        /// </summary>
        private float cameraRotation = 0;

        /// <summary>
        /// Viewing Camera distance from origin.
        /// The "Dude" model is defined in centimeters, hence all the units we use here are cm.
        /// </summary>
        private float cameraDistance = 90.0f;
        /// <summary>
        /// Viewing Camera view matrix.
        /// </summary>
        private Matrix view;

        /// <summary>
        /// Viewing Camera projection matrix.
        /// </summary>
        private Matrix projection;
        /// <summary>
        /// This control selects a sensor, and displays a notice if one is
        /// not connected.
        /// </summary>
        private readonly KinectChooser chooser;

        /// <summary>
        /// This manages the rendering of the color stream.
        /// </summary>
        public readonly ColorStreamRenderer colorStream;

        /// <summary>
        /// This manages the rendering of the depth stream.
        /// </summary>
        private readonly DepthStreamRenderer depthStream;

        /// <summary>
        /// This is the location of the color stream when minimized.
        /// </summary>
        private readonly Vector2 colorSmallPosition;

        /// <summary>
        /// This is the location of the depth stream when minimized;
        /// </summary>
        private readonly Vector2 depthSmallPosition;

        /// <summary>
        /// This is the minimized size for both streams.
        /// </summary>
        private readonly Vector2 minSize;

        /// <summary>
        /// This is the viewport of the streams.
        /// </summary>
        private readonly Rectangle viewPortRectangle;


        /// <summary>
        /// This is the SpriteBatch used for rendering the header/footer.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// This tracks the state to indicate which stream has focus.
        /// </summary>
        private bool colorHasFocus = true;

        /// <summary>
        /// This tracks the current transition time.
        /// 0                   = Color Stream Full Focus
        /// TransitionDuration  = Depth Stream Full Focus
        /// </summary>
        private double transition;

        /// <summary>
        /// This is the font for the footer.
        /// </summary>
        private SpriteFont font;

        /// <summary>
        /// Ragdoll Model Loading Object
        /// </summary>
        private Model BoxModel;
        private Model SphereModel;
        private Model CapsuleModel;
        private PhysicsSystem physicSystem;

        public JiggleGame.Camera GameCamera
        {
            get
            {
                return camera;
            }
        }
        private JiggleGame.Camera camera;
        /// <summary>
        /// Initializes a new instance of the XnaBasics class.
        /// </summary>
        public XnaBasics()
        {
            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;
            this.Window.Title = "Xna Basics";
            this.Window.AllowUserResizing = true;
            Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            // This sets the width to the desired width
            // It also forces a 4:3 ratio for height
            // Adds 110 for header/footer
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = Width;
            //this.graphics.PreferredBackBufferHeight = ((Width / 4) * 3) + 110;
            this.graphics.PreferredBackBufferHeight = Height;
            this.graphics.PreparingDeviceSettings += this.GraphicsDevicePreparingDeviceSettings;
            this.graphics.SynchronizeWithVerticalRetrace = true;
            //this.viewPortRectangle = new Rectangle(10, 80, Width - 20, ((Width - 2) / 4) * 3);
            this.viewPortRectangle = new Rectangle(0, 0, Width, Height);
            this.graphics.IsFullScreen = false;
            this.IsMouseVisible = false;

            physicSystem = new PhysicsSystem();
            camera = new JiggleGame.Camera(this);


            Content.RootDirectory = "Content";

            // The Kinect sensor will use 640x480 for both streams
            // To make your app handle multiple Kinects and other scenarios,
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
            this.chooser = new KinectChooser(this, ColorImageFormat.RgbResolution640x480Fps30, DepthImageFormat.Resolution640x480Fps30);
            this.Services.AddService(typeof(KinectChooser), this.chooser);



            // Default size is the full viewport
            this.colorStream = new ColorStreamRenderer(this);

            // Calculate the minimized size and location
            this.depthStream = new DepthStreamRenderer(this);
            this.depthStream.Size = new Vector2(this.viewPortRectangle.Width / 4, this.viewPortRectangle.Height / 4);
            this.depthStream.Position = new Vector2(Width - this.depthStream.Size.X - 15, 10);

            // Store the values so we can animate them later
            this.minSize = this.depthStream.Size;
            this.depthSmallPosition = this.depthStream.Position;
            this.colorSmallPosition = new Vector2(15, 10);


            this.Components.Add(this.chooser);

            LeftHand = new Vector2();
            RightHand = new Vector2();

            this.clothRender = new ClothRender(this);
            this.Components.Add(this.clothRender);
        }

        /// <summary>
        /// Loads the Xna related content.
        /// </summary>
        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.Services.AddService(typeof(SpriteBatch), this.spriteBatch);

            this.font = Content.Load<SpriteFont>("Segoe16");

            UiLayer.Startup(Content);

            BoxModel = Content.Load<Model>("box");
            SphereModel = Content.Load<Model>("sphere");
            CapsuleModel = Content.Load<Model>("capsule");

            camera.Position = Vector3.Down * 12 + Vector3.Backward * 30.0f;
            UpdateViewingCamera();

            base.LoadContent();

            

            this.ui.ObjectRender = this.colorStream.BodyModelRender;

            UiLayer.SideBars.ObjectRender = this.colorStream.BodyModelRender;
        }

        /// <summary>
        /// Initializes class and components
        /// </summary>
        protected override void Initialize()
        {           
            this.Components.Add(this.colorStream);


            // Create the avatar animator
            this.animator = new AvatarAnimator(this);
            this.Components.Add(this.animator);

            

            this.ui = new SimpleGUI(this);
            this.ui.DrawOrder = 1000;
            //Components.Add(this.ui);

            // Add XUI Component
            //_G.Game = this;

            // add core components
            Components.Add(new GamerServicesComponent(this));

            // add layers
            //UiLayer = new UiLayer(this);
            _G.UI = UiLayer;

            // add other components
            _G.GameInput = new GameInput((int)E_GameButton.Count, (int)E_GameAxis.Count);
            GameControls.Setup(); // initialise mappings


            base.Initialize();

            this.Components.Add(camera);

        }

        private void CreateScene9()
        {
            RagdollObject rgd;

            

            rgd = new RagdollObject(this, CapsuleModel, SphereModel, BoxModel, RagdollObject.RagdollType.Complex, 1.0f);
            rgd.Position = new Vector3(1 * 2, -14, 10 + 1 * 2);
            rgd.PutToSleep();

        }


        /// <summary>
        /// This method updates the game state. Including monitoring
        /// keyboard state and the transitions.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        protected override void Update(GameTime gameTime)
        {
#if !RELEASE
            Input input = _G.GameInput.GetInput(0);

            if (input.ButtonJustPressed((int)E_UiButton.Quit))
                this.Exit();
            if (input.ButtonJustPressed((int)E_UiButton.Space))
                this.colorHasFocus = !this.colorHasFocus;
            //if (input.ButtonJustPressed((int)E_UiButton.D9))
            //    CreateScene9();
#endif


            // Animate the transition value
            if (this.colorHasFocus)
            {
                this.transition -= gameTime.ElapsedGameTime.TotalSeconds;
                if (this.transition < 0)
                {
                    this.transition = 0;
                }
            }
            else
            {
                this.transition += gameTime.ElapsedGameTime.TotalSeconds;
                if (this.transition > TransitionDuration)
                {
                    this.transition = TransitionDuration;
                }
            }

            //this.bagAnimator.ViewPortRectangle = this.viewPortRectangle;

            // Animate the stream positions and sizes
            this.colorStream.Position = Vector2.SmoothStep(
                new Vector2(this.viewPortRectangle.X, this.viewPortRectangle.Y),
                this.colorSmallPosition,
                (float)(this.transition / TransitionDuration));

            this.colorStream.Size = Vector2.SmoothStep(
                new Vector2(this.viewPortRectangle.Width, this.viewPortRectangle.Height),
                this.minSize,
                (float)(this.transition / TransitionDuration));

            

            this.depthStream.Position = Vector2.SmoothStep(
                this.depthSmallPosition,
                new Vector2(this.viewPortRectangle.X, this.viewPortRectangle.Y),
                (float)(this.transition / TransitionDuration));
            this.depthStream.Size = Vector2.SmoothStep(
                this.minSize,
                new Vector2(this.viewPortRectangle.Width, this.viewPortRectangle.Height),
                (float)(this.transition / TransitionDuration));


            // update ui
            float frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            UiLayer.Update(frameTime);

            IsRunningSlowly = gameTime.IsRunningSlowly;

            // update input
            _G.GameInput.Update(frameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This method renders the current state.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen
            GraphicsDevice.Clear(Color.White);

            base.Draw(gameTime);


            // Render the streams with respect to focus
            if (this.colorHasFocus)
            {
                this.colorStream.DrawOrder = 1;
                this.depthStream.DrawOrder = 2;
            }
            else
            {
                this.colorStream.DrawOrder = 2;
                this.depthStream.DrawOrder = 1;
            }

            float frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // TODO - other stuff here ...

            // render ui
            UiLayer.Render(frameTime);

        }


        /// <summary>
        /// Create the viewing camera.
        /// </summary>
        protected void UpdateViewingCamera()
        {
            GraphicsDevice device = this.graphics.GraphicsDevice;

            // Compute camera matrices.
            this.view = Matrix.CreateTranslation(0, -40.0f, 0) *
                          Matrix.CreateRotationY(MathHelper.ToRadians(this.cameraRotation)) *
                          Matrix.CreateRotationX(MathHelper.ToRadians(this.cameraArc)) *
                          Matrix.CreateLookAt(
                                                new Vector3(0, 0, -this.cameraDistance),
                                                new Vector3(0, 0, 0),
                                                Vector3.Up);

            // Kinect vertical FOV in degrees
            float nominalVerticalFieldOfView = 45.6f;

            if (null != this.chooser && null != this.chooser.Sensor && this.chooser.Sensor.IsRunning && KinectStatus.Connected == this.chooser.Sensor.Status)
            {
                nominalVerticalFieldOfView = this.chooser.Sensor.DepthStream.NominalVerticalFieldOfView;
                
            }

            this.projection = Matrix.CreatePerspectiveFieldOfView(
                                                                (nominalVerticalFieldOfView * (float)Math.PI / 180.0f),
                                                                device.Viewport.AspectRatio,
                                                                1,
                                                                10000);
        }
        /// <summary>
        /// This method ensures that we can render to the back buffer without
        /// losing the data we already had in our previous back buffer.  This
        /// is necessary for the SkeletonStreamRenderer.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event args.</param>
        private void GraphicsDevicePreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            // This is necessary because we are rendering to back buffer/render targets and we need to preserve the data
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }
    }
}
