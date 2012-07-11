using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    using JiggleGame;

    /// <summary>
    /// The main Xna game implementation.
    /// </summary>

    public class VirtualTryonGame : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// This is used to adjust the window size.
        /// </summary>
        private int Width = 800;
        private int Height = 600;

        /// <summary>
        /// The 3D avatar mesh animator.
        /// </summary>
        private AvatarAnimator animator;

        private ClothRender clothRender;
        public ClothRender ClothModelRender
        {
            get { return clothRender; }
        }

        /// <summary>
        /// This controls the transition time for the resize animation.
        /// </summary>
        private const double TransitionDuration = 1.0;

        /// <summary>
        /// The graphics device manager provided by Xna.
        /// </summary>
        private readonly GraphicsDeviceManager graphics;
        private UiLayer UiLayer;
        public bool IsRunningSlowly;


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
        /// This is the location of the color stream when minimized.
        /// </summary>
        private readonly Vector2 colorSmallPosition;

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

        private Camera camera;
        /// <summary>
        /// 
        /// </summary>
        private readonly SkeletonStreamRenderer skeletonStream;
        /// <summary>
        /// Initializes a new instance of the XnaBasics class.
        /// </summary>
        public VirtualTryonGame()
        {
            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;
            this.Window.Title = "Virtual Tryon System";
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
            
            Content.RootDirectory = "Content";

            // The Kinect sensor will use 640x480 for both streams
            // To make your app handle multiple Kinects and other scenarios,
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
            this.chooser = new KinectChooser(this, ColorImageFormat.RgbResolution640x480Fps30, DepthImageFormat.Resolution640x480Fps30);
            this.Services.AddService(typeof(KinectChooser), this.chooser);


            // Default size is the full viewport
            this.colorStream = new ColorStreamRenderer(this);

            this.colorSmallPosition = new Vector2(15, 10);


            this.Components.Add(this.chooser);

            this.clothRender = new ClothRender(this);
            //this.Components.Add(this.clothRender);

            this.skeletonStream = new SkeletonStreamRenderer(this,null);
            this.Components.Add(this.skeletonStream);
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

            
            base.LoadContent();

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
            //this.Components.Add(this.animator);

            // Add XUI Component
            _G.Game = this;

            // add core components
            Components.Add(new GamerServicesComponent(this));

            // add layers
            UiLayer = new UiLayer(this);
            _G.UI = UiLayer;

            // add other components
            _G.GameInput = new GameInput((int)E_GameButton.Count, (int)E_GameAxis.Count);
            GameControls.Setup(); // initialise mappings

            camera = new Camera(this);
            camera.Position = Vector3.Forward * 12;
            
            this.Services.AddService(typeof(Camera), this.camera);

            List<Camera> kinectCamera = new List<Camera>();
            kinectCamera.Add(new Camera(this));
            kinectCamera[0].Position = Vector3.Zero;
            kinectCamera[0].View = Matrix.CreateLookAt(
                   new Vector3(0.0f, 0.0f, 0.0f),
                   new Vector3(0.0f, 0.0f, 1.0f),
                   Vector3.Down);
            this.Services.AddService(typeof(List<Camera>), kinectCamera);

            this.Components.Add(camera);
            
            base.Initialize();

            

        }

        private void CreateScene9()
        {
            RagdollObject rgd;

            rgd = new RagdollObject(this, CapsuleModel, SphereModel, BoxModel, RagdollObject.RagdollType.Complex, 1.0f);
            rgd.Position = new Vector3(1 * 2, -14, 10 + 1 * 2);
            rgd.PutToSleep();

        }

        /// <summary>
        /// Handles input for avateering options.
        /// </summary>
        private void HandleKeyBoard()
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

        }

        /// <summary>
        /// This method updates the game state. Including monitoring
        /// keyboard state and the transitions.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        protected override void Update(GameTime gameTime)
        {
            this.HandleKeyBoard();

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

            // Animate the stream positions and sizes
            this.colorStream.Position = Vector2.SmoothStep(
                new Vector2(this.viewPortRectangle.X, this.viewPortRectangle.Y),
                this.colorSmallPosition,
                (float)(this.transition / TransitionDuration));

            this.colorStream.Size = Vector2.SmoothStep(
                new Vector2(this.viewPortRectangle.Width, this.viewPortRectangle.Height),
                this.minSize,
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


            float frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // TODO - other stuff here ...

            // render ui
            UiLayer.Render(frameTime);

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
