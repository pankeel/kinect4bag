using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Kinect;

    /// <summary>
    /// A very basic game component to track common values.
    /// </summary>
    public class Object3D : DrawableGameComponent
    {
        /// <summary>
        /// The Skeleton tracked From the Kinect 
        /// </summary>
        protected Skeleton trackedSkeleton = null;
        /// <summary>
        /// The 3D Object mesh Model
        /// </summary>
        public Model Model3DAvatar
        {
            get;
            set;
        }

        private Texture2D _TargetTexture;
        public Texture2D TargetTexture
        {
            get
            {
                return _TargetTexture;
            }
            set
            {
                _TargetTexture = value;
            }
        }

        protected DateTime lastChangeTicks;

        /// <summary>
        /// Gets the KinectChooser from the services.
        /// </summary>
        public KinectChooser Chooser
        {
            get
            {
                return (KinectChooser)this.Game.Services.GetService(typeof(KinectChooser));
            }
        }


        private Matrix worldMatrix;

        private Matrix viewMatrix;

        private Matrix projectionMatrix;

        /// <summary>
        /// The back buffer where the depth frame is scaled as requested by the Size.
        /// </summary>
        public RenderTarget2D backBuffer;

        /// <summary>
        /// Whether or not the back buffer needs updating.
        /// </summary>
        private bool needToRedrawBackBuffer = true;
        /// <summary>
        /// The last frames skeleton data.
        /// </summary>
        protected static Skeleton[] skeletonData;

        /// <summary>
        /// This flag ensures only request a frame once per update call
        /// across the entire application.
        /// </summary>
        protected bool skeletonDrawn = true;

        /// <summary>
        /// The scale factor of the displayed bag 3d model
        /// </summary>
        protected float scaleFactor;
        /// <summary>
        /// The display Position (x,y,z) in Kinect Coordinate
        /// </summary>
        protected Vector3 showPosition;
        /// <summary>
        /// Initializes a new instance of the Object2D class.
        /// </summary>
        /// <param name="game">The related game object.</param>
        public Object3D(Game game)
            : base(game)
        {
            lastChangeTicks = DateTime.Now;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected virtual Matrix CreateWorldMatrix(Skeleton skeleton)
        {
            return Matrix.Identity;
        }

        protected virtual Matrix CreateViewMatrix(Skeleton skeleton)
        {
            return Matrix.Identity;
        }

        protected virtual Matrix CreateProjectionMatrix(Skeleton skeleton)
        {
            return Matrix.Identity;
        }

        private static int testFlag = 0;
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            // Render the 3D model skinned mesh with Skinned Effect.
            Matrix world = this.CreateWorldMatrix(trackedSkeleton),
                view = this.CreateViewMatrix(trackedSkeleton),
                projection = this.CreateProjectionMatrix(trackedSkeleton);
            if (this.Model3DAvatar != null)
            {
                foreach (ModelMesh mesh in this.Model3DAvatar.Meshes)
                {
                    
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        //effect.SetBoneTransforms(this.skinTransforms);
                      if (TargetTexture != null && effect.Texture != TargetTexture)
                        {
                            effect.Texture = TargetTexture;
                            //if (testFlag %2 == 0)
                            //    effect.Texture = this.Game.Content.Load<Texture2D>("Textures\\tex0");
                            //else
                            //    effect.Texture = this.Game.Content.Load<Texture2D>("Textures\\tex1");
                            
                            //TargetTexture = effect.Texture;
                            testFlag++;
                        }
                        effect.World = world;
                        effect.View = view;
                        effect.Projection = projection;
                        effect.EnableDefaultLighting();

                        effect.SpecularColor = new Vector3(0.25f);
                        effect.SpecularPower = 16;
                    }

                    mesh.Draw();
                }
            }
        }

        /// <summary>
        /// This method retrieves a new skeleton frame if necessary.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // If we have already drawn this skeleton, then we should retrieve a new frame
            // This prevents us from calling the next frame more than once per update
            skeletonData = SkeletonStreamRenderer.skeletonData;

            this.trackedSkeleton = null;
            if (skeletonData != null)
            {
                foreach (var skeleton in skeletonData)
                {
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        this.trackedSkeleton = skeleton;
                    }
                }

                this.needToRedrawBackBuffer = true;
            }

        }
    }
}
