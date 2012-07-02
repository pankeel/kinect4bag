//------------------------------------------------------------------------------
// <copyright file="AvatarAnimator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.XnaBasics
{
    using System;
    using Microsoft.Kinect;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using SkinnedModel;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class is responsible for animating an avatar using a skeleton stream.
    /// </summary>
    [CLSCompliant(true)]
    public class AvatarAnimator : DrawableGameComponent
    {
      
        /// <summary>
        /// This tracks the previous keyboard state.
        /// </summary>
        private KeyboardState previousKeyboard;

        /// <summary>
        /// This tracks the current keyboard state.
        /// </summary>
        private KeyboardState currentKeyboard;


        private bool bSupress = false;

        /// <summary>
        /// This skeleton is the  first tracked skeleton in the frame is used for animation, with constraints and mirroring optionally applied.
        /// </summary>
        private Skeleton skeleton;
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
        private float cameraDistance = 190.0f;

        /// <summary>
        /// This is used to adjust the window size. The height is set automatically from the width using a 4:3 ratio.
        /// </summary>
        private const int WindowedWidth = 800;
        /// <summary>
        /// Draw the avatar only when the player skeleton is detected in the depth image.
        /// </summary>
        private bool drawAvatarOnlyWhenPlayerDetected;

        /// <summary>
        /// Flag for first detection of skeleton.
        /// </summary>
        private bool skeletonDetected;
        /// <summary>
        /// This is used to adjust the fullscreen window size. Only valid resolutions can be set.
        /// </summary>
        private const int FullScreenWidth = 1280;

        /// <summary>
        /// This is used to adjust the fullscreen window size. Only valid resolutions can be set.
        /// </summary>
        private const int FullScreenHeight = 1024;

        /// <summary>
        /// Camera Arc Increment value.
        /// </summary>
        private const float CameraArcIncrement = 0.1f;

        /// <summary>
        /// Camera Arc angle limit value.
        /// </summary>
        private const float CameraArcAngleLimit = 90.0f;

        /// <summary>
        /// Camera Zoom Increment value.
        /// </summary>
        private const float CameraZoomIncrement = 0.25f;

        /// <summary>
        /// Camera Max Distance value.
        /// </summary>
        private const float CameraMaxDistance = 500.0f;

        /// <summary>
        /// Camera Min Distance value.
        /// </summary>
        private const float CameraMinDistance = 10.0f;

        /// <summary>
        /// Camera starting Distance value.
        /// </summary>
        private const float CameraHeight = 40.0f;

        /// <summary>
        /// Camera starting Distance value.
        /// </summary>
        private const float CameraStartingTranslation = 90.0f;
        /// <summary>
        /// Viewing Camera projection matrix.
        /// </summary>
        private Matrix projection;

        /// <summary>
        /// Viewing Camera view matrix.
        /// </summary>
        private Matrix view;

        /// <summary>
        /// The 3D avatar mesh.
        /// </summary>
        private Model currentModel;

        /// <summary>
        /// The avatar relative bone transformation matrices.
        /// </summary>
        private Matrix[] boneTransforms;

        /// <summary>
        /// The avatar "absolute" bone transformation matrices in the world coordinate system.
        /// These are in the warrior ("dude") format
        /// </summary>
        private Matrix[] worldTransforms;

        /// <summary>
        /// The avatar skin transformation matrices.
        /// </summary>
        private Matrix[] skinTransforms;

        /// <summary>
        /// Sets a seated posture when Seated Mode is on.
        /// </summary>
        private bool setSeatedPostureInSeatedMode;

        /// <summary>
        /// Fix the avatar hip center draw height.
        /// </summary>
        private bool fixAvatarHipCenterDrawHeight;

        /// <summary>
        /// Avatar hip center draw height.
        /// </summary>
        private float avatarHipCenterDrawHeight;

        /// <summary>
        /// Adjust Avatar lean when leaning back to reduce lean.
        /// </summary>
        private bool leanAdjust; 


        /// <summary>
        /// Back link to the avatar model bind pose and skeleton hierarchy data.
        /// </summary>
        private SkinningData skinningDataValue;

        /// <summary>
        /// This is the coordinate cross we use to draw the local axes of the model.
        /// </summary>
        private CoordinateCross localAxes;

        /// <summary>
        /// Draws local joint axes inside the 3D avatar mesh if true.
        /// </summary>
        private bool drawLocalAxes;

        /// <summary>
        /// Enables avateering when true.
        /// </summary>
        private bool useKinectAvateering;

        /// <summary>
        /// Compensate the avatar joints for sensor tilt if true.
        /// </summary>
        private bool tiltCompensate;

        /// <summary>
        /// Compensate the avatar joints for sensor height and bring skeleton to floor level if true.
        /// </summary>
        private bool floorOffsetCompensate;

        /// <summary>
        /// Filter to compensate the avatar joints for sensor height and bring skeleton to floor level.
        /// </summary>
        private SkeletonJointsSensorOffsetCorrection sensorOffsetCorrection;

        /// <summary>
        /// Filter to prevent arm-torso self-intersections if true.
        /// </summary>
        private bool selfIntersectionConstraints;

        /// <summary>
        /// The timer for controlling Filter Lerp blends.
        /// </summary>
        private Timer frameTimer;

        /// <summary>
        /// The timer for controlling Filter Lerp blends.
        /// </summary>
        private float lastNuiTime;

        /// <summary>
        /// Filter clipped legs if true.
        /// </summary>
        private bool filterClippedLegs;

        /// <summary>
        /// The filter for clipped legs.
        /// </summary>
        private SkeletonJointsFilterClippedLegs clippedLegs;

        /// <summary>
        /// Mirrors the avatar when true.
        /// </summary>
        private bool mirrorView;

        /// <summary>
        /// Apply joint constraints to joint locations and orientations if true.
        /// </summary>
        private bool boneConstraints;

        /// <summary>
        /// The filter for bone orientations constraints.
        /// </summary>
        private BoneOrientationConstraints boneOrientationConstraints;

        /// <summary>
        /// Draw the Kinect line skeleton using the raw joint positions and joint constraint cones if true.
        /// </summary>
        private bool drawBoneConstraintsSkeleton;

        /// <summary>
        /// The world translation offset for the skeleton drawing in bone constraints.
        /// </summary>
        private Matrix kinectLineSkeletonWorldOffsetMatrix;

        /// <summary>
        /// The filter for joint positions.
        /// </summary>
        private SkeletonJointsPositionDoubleExponentialFilter jointPositionFilter;

        /// <summary>
        /// Filter bone orientations if true.
        /// </summary>
        private bool filterBoneOrientations;

        /// <summary>
        /// The filter for bone orientations.
        /// </summary>
        private BoneOrientationDoubleExponentialFilter boneOrientationFilter;


        /// <summary>
        /// The "Dude" model is defined in centimeters, so re-scale the Kinect translation.
        /// </summary>
        private static readonly Vector3 SkeletonTranslationScaleFactor = new Vector3(40.0f, 40.0f, 40.0f);

        /// <summary>
        /// Store the mapping between the NuiJoint and the Avatar Bone index.
        /// </summary>
        private Dictionary<JointType, int> nuiJointToAvatarBoneIndex;
        /// <summary>
        /// Initializes a new instance of the AvatarAnimator class.
        /// </summary>
        /// <param name="game">The related game object.</param>
        /// <param name="retarget">The avatar mesh re-targeting method to convert from the Kinect skeleton.</param>
        /// 

        private int ModelType = 0;

        private bool isDebug = true;
        private Vector3 debugRotationVector = new Vector3();

        private String _DebugText = "sx";
        private String DebugText
        {
            get
            {
                return _DebugText;
            }
            set
            {
                if (isDebug)
                    _DebugText = value;
            }
        }

        public AvatarAnimator(Game game)
            : base(game)
        {
            
            if (null == game)
            {
                return;
            }

            this.SkeletonDrawn = true;
            this.useKinectAvateering = true;
            this.AvatarHipCenterHeight = 0;

            // Create local axes inside the model to draw at each joint
            this.localAxes = new CoordinateCross(this.Game, 2.0f);
            this.drawLocalAxes = false;
            game.Components.Add(this.localAxes);

            // If we draw the Kinect 3D skeleton in BoneOrientationConstraints, we can offset it from the original 
            // hip center position, so as not to draw over the top of the Avatar. Offset defined in m.
            this.kinectLineSkeletonWorldOffsetMatrix = Matrix.CreateTranslation(40.0f, 0.75f * 40.0f, 0);
            this.drawBoneConstraintsSkeleton = false;

            // Skeleton fixups
            this.frameTimer = new Timer();
            this.lastNuiTime = 0;
            this.FloorClipPlane = new Tuple<float, float, float, float>(0, 0, 0, 0);
            this.clippedLegs = new SkeletonJointsFilterClippedLegs();
            this.sensorOffsetCorrection = new SkeletonJointsSensorOffsetCorrection();
            this.jointPositionFilter = new SkeletonJointsPositionDoubleExponentialFilter();
            this.boneOrientationConstraints = new BoneOrientationConstraints(game);
            this.boneOrientationFilter = new BoneOrientationDoubleExponentialFilter();

            this.filterClippedLegs = true;
            this.tiltCompensate = true;
            this.floorOffsetCompensate = false;
            this.selfIntersectionConstraints = true;
            this.mirrorView = true;
            this.boneConstraints = true;
            this.filterBoneOrientations = true;

            // For many applications we would enable the
            // automatic joint smoothing, however, in this
            // Avateering sample, we perform skeleton joint
            // position corrections, so we will manually
            // filter here after these are complete.

            // Typical smoothing parameters for the joints:
            var jointPositionSmoothParameters = new TransformSmoothParameters
            {
                Smoothing = 0.25f,
                Correction = 0.25f,
                Prediction = 0.75f,
                JitterRadius = 0.1f,
                MaxDeviationRadius = 0.04f
            };

            this.jointPositionFilter.Init(jointPositionSmoothParameters);
            
            // Setup the bone orientation constraint system
            this.boneOrientationConstraints.AddDefaultConstraints();
            game.Components.Add(this.boneOrientationConstraints);

            // Typical smoothing parameters for the bone orientations:
            var boneOrientationSmoothparameters = new TransformSmoothParameters
            {
                Smoothing = 0.5f,
                Correction = 0.8f,
                Prediction = 0.75f,
                JitterRadius = 0.1f,
                MaxDeviationRadius = 0.1f
            };

            this.boneOrientationFilter.Init(boneOrientationSmoothparameters);
        }

        /// <summary>
        /// Gets or sets a value indicating whether This flag ensures we only request a frame once per update call
        /// across the entire application.
        /// </summary>
        public bool SkeletonDrawn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the first tracked skeleton in the frame is used for animation.
        /// </summary>
        public Skeleton RawSkeleton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Store the floor plane to compensate the skeletons for any Kinect tilt.
        /// </summary>
        public System.Tuple<float, float, float, float> FloorClipPlane { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the height of the avatar Hip Center joint off the floor when standing upright.
        /// </summary>
        public float AvatarHipCenterHeight { get; set; }

        /// <summary>
        /// Gets the KinectChooser from the services.
        /// </summary>
        public KinectChooser Chooser
        {
            get
            {
                return (KinectChooser)Game.Services.GetService(typeof(KinectChooser));
            }
        }

        /// <summary>
        /// Gets or sets the Avatar 3D model to animate.
        /// </summary>
        public Model Avatar
        {
            get
            {
                return this.currentModel;
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                this.currentModel = value;

                // Look up our custom skinning information.
                SkinningData skinningData = this.currentModel.Tag as SkinningData;
                if (null == skinningData)
                {
                    throw new InvalidOperationException("This model does not contain a Skinning Data tag.");
                }

                this.skinningDataValue = skinningData;

                // Bone matrices for the "dude" model
                this.boneTransforms = new Matrix[skinningData.BindPose.Count];
                this.worldTransforms = new Matrix[skinningData.BindPose.Count];
                this.skinTransforms = new Matrix[skinningData.BindPose.Count];

                // Initialize bone transforms to the bind pose.
                this.skinningDataValue.BindPose.CopyTo(this.boneTransforms, 0);
                this.UpdateWorldTransforms(Matrix.Identity);
                this.UpdateSkinTransforms();
            }
        }

        /// <summary>
        /// Reset the tracking filters.
        /// </summary>
        public void Reset()
        {
            if (null != this.jointPositionFilter)
            {
                this.jointPositionFilter.Reset();
            }

            if (null != this.boneOrientationFilter)
            {
                this.boneOrientationFilter.Reset();
            }

            if (null != this.sensorOffsetCorrection)
            {
                this.sensorOffsetCorrection.Reset();
            }

            if (null != this.clippedLegs)
            {
                this.clippedLegs.Reset();
            }
        }

        /// <summary>
        /// This method copies a new skeleton locally so we can modify it.
        /// </summary>
        /// <param name="sourceSkeleton">The skeleton to copy.</param>
        public void CopySkeleton(Skeleton sourceSkeleton)
        {
            if (null == sourceSkeleton)
            {
                return;
            }

            if (null == this.skeleton)
            {
                this.skeleton = new Skeleton();
            }

            // Copy the raw Kinect skeleton so we can modify the joint data and apply constraints
            KinectHelper.CopySkeleton(sourceSkeleton, this.skeleton);
        }

        /// <summary>
        /// 
        /// </summary>
        private void HandleSkeleton()
        {
            Skeleton[] SkeletonData = SkeletonStreamRenderer.skeletonData;
            this.FloorClipPlane = SkeletonStreamRenderer.FloorClipPlane;
            // Select the first tracked skeleton we see to avateer
            if (SkeletonData == null)
                return;
            Skeleton rawSkeleton =
                (from s in SkeletonData
                 where s != null && s.TrackingState == SkeletonTrackingState.Tracked
                 select s).FirstOrDefault();

            if (null != this.currentModel && null != rawSkeleton)
            {
                this.CopySkeleton(rawSkeleton);
                

                // Reset the filters if the skeleton was not seen before now
                if (this.skeletonDetected == false)
                {
                    this.Reset();
                }

                this.skeletonDetected = true;
            }
            else
            {
                this.skeletonDetected = false;
            }
        }
        /// <summary>
        /// This method retrieves a new skeleton frame if necessary.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public  override void Update(GameTime gameTime)
        {
            
            if (!this.bSupress)
            {
                this.HandleSkeleton();
                this.UpdateCamera(gameTime);
                this.SkeletonDrawn = false;
            }
            
            // If we have already drawn this skeleton, then we should retrieve a new frame
            // This prevents us from calling the next frame more than once per update
            if (false == this.SkeletonDrawn && null != this.skeleton && this.useKinectAvateering)
            {
                // Copy all bind pose matrices to boneTransforms 
                // Note: most are identity, but the translation is important to describe bone length/the offset between bone drawing positions
                this.skinningDataValue.BindPose.CopyTo(this.boneTransforms, 0);
                this.UpdateWorldTransforms(Matrix.Identity);
                this.UpdateSkinTransforms();

                // If required, we should modify the joint positions before we access the bone orientations, as orientations are calculated
                // on the first access, and then whenever a joint position changes. Hence changing joint positions interleaved with accessing
                // rotations will cause unnecessary additional computation.
                float currentNuiTime = (float)this.frameTimer.AbsoluteTime;
                float deltaNuiTime = currentNuiTime - this.lastNuiTime;

                // Fixup Skeleton to improve avatar appearance.
                if (this.filterClippedLegs && !this.Chooser.SeatedMode && null != this.clippedLegs)
                {
                    this.clippedLegs.FilterSkeleton(this.skeleton, deltaNuiTime);
                }

                if (this.selfIntersectionConstraints)
                {
                    // Constrain the wrist and hand joint positions to not intersect the torso
                    SkeletonJointsSelfIntersectionConstraint.Constrain(this.skeleton);
                }

                if (this.tiltCompensate)
                {
                    // Correct for sensor tilt if we have a valid floor plane or a sensor tilt value from the motor.
                    SkeletonJointsSensorTiltCorrection.CorrectSensorTilt(this.skeleton, this.FloorClipPlane, this.Chooser.Sensor.ElevationAngle);
                }

                if (this.floorOffsetCompensate && 0.0f != this.AvatarHipCenterHeight)
                {
                    // Correct for the sensor height from the floor (moves the skeleton to the floor plane) if we have a valid plane, or feet visible in the image.
                    // Note that by default this will not run unless we have set a non-zero AvatarHipCenterHeight
                    this.sensorOffsetCorrection.CorrectSkeletonOffsetFromFloor(this.skeleton, this.FloorClipPlane, this.AvatarHipCenterHeight);
                }

                if (this.mirrorView)
                {
                    SkeletonJointsMirror.MirrorSkeleton(this.skeleton);
                }

                // Filter the joint positions manually, using a double exponential filter.
                this.jointPositionFilter.UpdateFilter(this.skeleton);

                if (this.boneConstraints && null != this.boneOrientationConstraints)
                {
                    // Constrain the joint positions to approximate range of human motion.
                    this.boneOrientationConstraints.Constrain(this.skeleton, this.mirrorView);
                }

                if (this.filterBoneOrientations && null != this.boneOrientationFilter)
                {
                    // Double Exponential Filtering of the joint orientations.
                    // Note: This updates the joint orientations directly in the skeleton.
                    // It should be performed after all joint position modifications.
                    this.boneOrientationFilter.UpdateFilter(this.skeleton);
                }

                this.RetargetMatrixHierarchyToAvatarMesh(this.skeleton, this.skinningDataValue.BindPose[0], this.boneTransforms);

                // Calculate the Avatar world transforms from the relative bone transforms of Kinect skeleton
                this.UpdateWorldTransforms(Matrix.Identity);

                // Refresh the Avatar SkinTransforms data based on the transforms we just applied
                this.UpdateSkinTransforms();

                this.lastNuiTime = currentNuiTime;
            }

            this.HandleInput();

            //Update Vertex Buffer By Physics
            unsafe
            {
                foreach (ModelMesh mesh in this.currentModel.Meshes)
                {
                    ModelMeshPart mmp = mesh.MeshParts[0];                    
                    byte[] buf = new byte[mmp.NumVertices];

                    mmp.VertexBuffer.GetData<byte>(buf);

                    for (int i = 0; i < mmp.NumVertices; i++)
                    {
                        fixed (byte* bytePtr = &buf[i])
                        {
                            Vector3* pPos = (Vector3*)bytePtr;
                            float a = pPos->X;
                        }
                    }

                }


            }

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            // Draw the actual avatar
            if (this.bSupress)
                return;
            this.UpdateViewingCamera();

            if (null != this.Avatar )
            {
                // Reset the DepthStencilState to avoid the transparent of the 3d model
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                this.Draw(gameTime, Matrix.Identity, this.view, this.projection);
            }
            base.Draw(gameTime);
        }
        /// <summary>
        /// This method draws the skeleton frame data.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        /// <param name="world">The world matrix.</param>
        /// <param name="view">The view matrix.</param>
        /// <param name="projection">The projection matrix.</param>
        public void Draw(GameTime gameTime, Matrix world, Matrix view, Matrix projection)
        {
            if (this.currentModel == null)
                return;
            // Render the 3D model skinned mesh with Skinned Effect.
            // Mesh Draw Method
            foreach (ModelMesh mesh in this.currentModel.Meshes)
            {
                //VertexPositionNormalTexture[] mmp = new VertexPositionNormalTexture[mesh.MeshParts[0].VertexBuffer.VertexCount];
                //mesh.MeshParts[0].VertexBuffer.GetData<VertexPositionNormalTexture>(mmp);
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(this.skinTransforms);

                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }

                mesh.Draw();
            }
            /*GraphicsDevice device = this.Game.GraphicsDevice;
            foreach (ModelMesh mesh in this.currentModel.Meshes)
            {
                double[] mmp = new double[mesh.MeshParts[0].VertexBuffer.VertexCount];
                mesh.MeshParts[0].VertexBuffer.GetData<double>(mmp);

                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    //这里也有个顺序问题，一定要根据每一个ModelMeshPart的每一个EffectPass这样画  
                    foreach (EffectPass pass in mesh.Effects[0].CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        
                        device.VertexDeclaration = part.VertexDeclaration;//顶点声明  
                        device.Indices = mesh.IndexBuffer;//索引缓存  
                        device.Vertices[0].SetSource(mesh.VertexBuffer, part.StreamOffset, part.VertexStride);//设置顶点  
                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.BaseVertex,0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
                        //画顶点，如果要用Shader的话，是不能用mesh.Draw函数来话的，这个函数不支持Shader
                    }
                } 
            }*/
            // Optionally draw local bone transforms with Basic Effect.
            if (this.drawLocalAxes && null != this.localAxes)
            {
                // Disable the depth buffer so we can render the Kinect skeleton inside the model
                Game.GraphicsDevice.DepthStencilState = DepthStencilState.None;

                foreach (Matrix boneWorldTrans in this.worldTransforms)
                {
                    // re-use the coordinate cross instance for each localAxes draw
                    this.localAxes.Draw(gameTime, boneWorldTrans * world, view, projection);
                        

                }

                // Re-enable the depth buffer
                Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }

            // Optionally draw the Kinect 3d line skeleton from the raw joint positions and the bone orientation constraint cones
            if (this.drawBoneConstraintsSkeleton && null != this.boneOrientationConstraints && null != Chooser)
            {
                this.boneOrientationConstraints.Draw(gameTime, skeleton, Chooser.SeatedMode, kinectLineSkeletonWorldOffsetMatrix * world, view, projection);                    
            }

            this.SkeletonDrawn = true;

            base.Draw(gameTime);
        }

        /// <summary>
        /// Handles input for avateering options.
        /// </summary>
        private void HandleInput()
        {
            this.currentKeyboard = Keyboard.GetState();

            // Check for exit.
            if (this.currentKeyboard.IsKeyDown(Keys.Escape))
            {
                this.Game.Exit();
            }

            // Draw avatar when not detected on/off toggle
            if (this.currentKeyboard.IsKeyDown(Keys.V))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.V))
                {
                    this.drawAvatarOnlyWhenPlayerDetected = !this.drawAvatarOnlyWhenPlayerDetected;
                }
            }

            // Seated and near mode on/off toggle
            if (this.currentKeyboard.IsKeyDown(Keys.N))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.N))
                {
                    this.Chooser.SeatedMode = !this.Chooser.SeatedMode;
                    this.skeletonDetected = false;

                    // Set near mode to accompany seated mode
                    this.Chooser.NearMode = this.Chooser.SeatedMode;
                }
            }

            // Fix the avatar hip center draw height on/off toggle
            if (this.currentKeyboard.IsKeyDown(Keys.H))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.H))
                {
                    this.fixAvatarHipCenterDrawHeight = !this.fixAvatarHipCenterDrawHeight;
                }
            }

            // Fix the avatar leaning back too much on/off toggle
            if (this.currentKeyboard.IsKeyDown(Keys.L))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.L))
                {
                    this.leanAdjust = !this.leanAdjust;
                }
            }

            // Reset the avatar filters (also resets camera)
            if (this.currentKeyboard.IsKeyDown(Keys.R))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.R))
                {
                    this.Reset();
                }
            }
            // Mirror Avatar on/off toggle
            if (currentKeyboard.IsKeyDown(Keys.M))
            {
                // If not down last update, key has just been pressed.
                if (!this.previousKeyboard.IsKeyDown(Keys.M))
                {
                    this.mirrorView = !this.mirrorView;
                }
            }

            // Local Axes on/off toggle
            if (currentKeyboard.IsKeyDown(Keys.G))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.G))
                {
                    this.drawLocalAxes = !this.drawLocalAxes;
                }
            }

            // Avateering on/off toggle
            if (currentKeyboard.IsKeyDown(Keys.K))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.K))
                {
                    this.useKinectAvateering = !this.useKinectAvateering;
                }
            }

            // Tilt Compensation on/off toggle
            if (currentKeyboard.IsKeyDown(Keys.T))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.T))
                {
                    this.tiltCompensate = !this.tiltCompensate;
                }
            }

            // Compensate for sensor height, move skeleton to floor on/off toggle
            if (currentKeyboard.IsKeyDown(Keys.O))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.O))
                {
                    this.floorOffsetCompensate = !this.floorOffsetCompensate;
                }
            }

            // Torso self intersection constraints on/off toggle
            if (currentKeyboard.IsKeyDown(Keys.I))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.I))
                {
                    this.selfIntersectionConstraints = !this.selfIntersectionConstraints;
                }
            }

            // Filter Joint Orientation on/off toggle
            if (currentKeyboard.IsKeyDown(Keys.F))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.F))
                {
                    this.filterBoneOrientations = !this.filterBoneOrientations;
                }
            }

            // Constrain Bone orientations on/off toggle
            if (currentKeyboard.IsKeyDown(Keys.C))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.C))
                {
                    this.boneConstraints = !this.boneConstraints;
                }
            }

            // Draw Bones in bone orientation constraints on/off toggle
            if (currentKeyboard.IsKeyDown(Keys.B))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.B))
                {
                    this.drawBoneConstraintsSkeleton = !this.drawBoneConstraintsSkeleton;
                }
            }

            this.previousKeyboard = currentKeyboard;
        }

        /// <summary>
        /// Helper used by the Update method to refresh the WorldTransforms data.
        /// </summary>
        /// <param name="rootTransform">Matrix to modify the Avatar root transform with.</param>
        private void UpdateWorldTransforms(Matrix rootTransform)
        {
            // Root bone of model.
            this.worldTransforms[0] = this.boneTransforms[0] * rootTransform;


            // Child bones in bone hierarchy.
            for (int bone = 1; bone < this.worldTransforms.Length; bone++)
            {
                int parentBone = this.skinningDataValue.SkeletonHierarchy[bone];

                // Every bone world transform is calculated by multiplying it's relative transform by the world transform of it's parent. 
                this.worldTransforms[bone] = this.boneTransforms[bone] * this.worldTransforms[parentBone];
            }
        }

        /// <summary>
        /// Helper used by the Update method to refresh the SkinTransforms data.
        /// </summary>
        private void UpdateSkinTransforms()
        {
            for (int bone = 0; bone < this.skinTransforms.Length; bone++)
            {
                this.skinTransforms[bone] = this.skinningDataValue.InverseBindPose[bone] * this.worldTransforms[bone];
            }
        }

        private void SetJointTransformationByDube(BoneOrientation bone, Skeleton skeleton, Matrix bindRoot, ref Matrix[] boneTransforms)
        {
            // Always look at the skeleton root
            if (bone.StartJoint == JointType.HipCenter && bone.EndJoint == JointType.HipCenter)
            {
                // Unless in seated mode, the hip center is special - it is the root of the NuiSkeleton and describes the skeleton orientation in the world
                // (camera) coordinate system. All other bones/joint orientations in the hierarchy have hip center as one of their parents.
                // However, if in seated mode, the shoulder center then holds the skeleton orientation in the world (camera) coordinate system.
                bindRoot.Translation = Vector3.Zero;
                Matrix invBindRoot = Matrix.Invert(bindRoot);

                Matrix hipOrientation = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // ensure pure rotation, as we set world translation from the Kinect camera below
                Matrix hipCenter = boneTransforms[1];
                hipCenter.Translation = Vector3.Zero;
                Matrix invPelvis = Matrix.Invert(hipCenter);

                Matrix combined = (invBindRoot * hipOrientation) * invPelvis;

                this.ReplaceBoneMatrix(JointType.HipCenter, combined, true, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.ShoulderCenter)
            {
                // This contains an absolute rotation if we are in seated mode, or the hip center is not tracked, as the HipCenter will be identity
                if (this.Chooser.SeatedMode || (this.Chooser.SeatedMode == false && skeleton.Joints[JointType.HipCenter].TrackingState == JointTrackingState.NotTracked))
                {
                    bindRoot.Translation = Vector3.Zero;
                    Matrix invBindRoot = Matrix.Invert(bindRoot);

                    Matrix hipOrientation = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                    // ensure pure rotation, as we set world translation from the Kinect camera
                    Matrix hipCenter = boneTransforms[1];
                    hipCenter.Translation = Vector3.Zero;
                    Matrix invPelvis = Matrix.Invert(hipCenter);

                    Matrix combined = (invBindRoot * hipOrientation) * invPelvis;

                    this.ReplaceBoneMatrix(JointType.HipCenter, combined, true, ref boneTransforms);
                }
            }
            else if (bone.EndJoint == JointType.Spine)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // The Dude appears to lean back too far compared to a real person, so here we adjust this lean.
                CorrectBackwardsLean(skeleton, ref tempMat);

                // Also add a small constant adjustment rotation to correct for the hip center to spine bone being at a rear-tilted angle in the Kinect skeleton.
                // The dude should now look more straight ahead when avateering
                Matrix adjustment = Matrix.CreateRotationX(MathHelper.ToRadians(20));  // 20 degree rotation around the local Kinect x axis for the spine bone.
                tempMat *= adjustment;

                // Kinect = +X left, +Y up, +Z forward in body coordinate system
                // Avatar = +Z left, +X up, +Y forward
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.Z, kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                // Set the corresponding matrix in the avatar using the translation table we specified.
                // Note for the spine and shoulder center rotations, we could also try to spread the angle
                // over all the Avatar skeleton spine joints, causing a more curved back, rather than apply
                // it all to one joint, as we do here.
                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.Head)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // Add a small adjustment rotation to correct for the avatar skeleton head bones being defined pointing looking slightly down, not vertical.
                // The dude should now look more straight ahead when avateering
                Matrix adjustment = Matrix.CreateRotationX(MathHelper.ToRadians(-30));  // -30 degree rotation around the local Kinect x axis for the head bone.
                tempMat *= adjustment;

                // Kinect = +X left, +Y up, +Z forward in body coordinate system
                // Avatar = +Z left, +X up, +Y forward
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.Z, kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                // Set the corresponding matrix in the avatar using the translation table we specified
                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.ElbowLeft || bone.EndJoint == JointType.WristLeft)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                if (bone.EndJoint == JointType.ElbowLeft)
                {
                    // Add a small adjustment rotation to correct for the avatar skeleton shoulder/upper arm bones.
                    // The dude should now be able to have arms correctly down at his sides when avateering
                    Matrix adjustment = Matrix.CreateRotationZ(MathHelper.ToRadians(-15));  // -15 degree rotation around the local Kinect z axis for the upper arm bone.
                    tempMat *= adjustment;
                }

                // Kinect = +Y along arm, +X down, -Z backward in body coordinate system
                // Avatar = +X along arm, -Z down, +Y backwards
                // Avatar = +X along arm, -Y down, +Z backwards
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, -kinectRotation.Z, -kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system

                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                //Debug according to the keyboard input
                if (bone.EndJoint == JointType.ElbowLeft && this.isDebug)
                {
                    tempMat = Matrix.Identity
                        * Matrix.CreateRotationX(this.debugRotationVector.X)
                        * Matrix.CreateRotationY(this.debugRotationVector.Y)
                        * Matrix.CreateRotationZ(this.debugRotationVector.Z);
                }


                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.HandLeft)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // Add a small adjustment rotation to correct for the avatar skeleton wist/hand bone.
                // The dude should now have the palm of his hands toward his body when arms are straight down
                Matrix adjustment = Matrix.CreateRotationY(MathHelper.ToRadians(-90));  // -90 degree rotation around the local Kinect y axis for the wrist-hand bone.
                tempMat *= adjustment;

                // Kinect = +Y along arm, +X down, +Z forward in body coordinate system
                // Avatar = +X along arm, +Y down, +Z backwards
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.X, -kinectRotation.Z, kinectRotation.W);
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.ElbowRight || bone.EndJoint == JointType.WristRight)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                if (bone.EndJoint == JointType.ElbowRight)
                {
                    // Add a small adjustment rotation to correct for the avatar skeleton shoulder/upper arm bones.
                    // The dude should now be able to have arms correctly down at his sides when avateering
                    Matrix adjustment = Matrix.CreateRotationZ(MathHelper.ToRadians(15));  // 15 degree rotation around the local Kinect  z axis for the upper arm bone.
                    tempMat *= adjustment;
                }

                // Kinect = +Y along arm, +X up, +Z forward in body coordinate system
                // Avatar = +X along arm, +Y back, +Z down
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, -kinectRotation.Z, -kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.HandRight)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // Add a small adjustment rotation to correct for the avatar skeleton wist/hand bone.
                // The dude should now have the palm of his hands toward his body when arms are straight down
                Matrix adjustment = Matrix.CreateRotationY(MathHelper.ToRadians(90));  // -90 degree rotation around the local Kinect y axis for the wrist-hand bone.
                tempMat *= adjustment;

                // Kinect = +Y along arm, +X up, +Z forward in body coordinate system
                // Avatar = +X along arm, +Y down, +Z forwards
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, -kinectRotation.X, kinectRotation.Z, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.KneeLeft)
            {
                // Combine the two joint rotations from the hip and knee
                Matrix hipLeft = KinectHelper.Matrix4ToXNAMatrix(skeleton.BoneOrientations[JointType.HipLeft].HierarchicalRotation.Matrix);
                Matrix kneeLeft = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);
                Matrix combined = kneeLeft * hipLeft;

                this.SetLegMatrix(bone.EndJoint, combined, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.AnkleLeft || bone.EndJoint == JointType.AnkleRight)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);
                this.SetLegMatrix(bone.EndJoint, tempMat, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.KneeRight)
            {
                // Combine the two joint rotations from the hip and knee
                Matrix hipRight = KinectHelper.Matrix4ToXNAMatrix(skeleton.BoneOrientations[JointType.HipRight].HierarchicalRotation.Matrix);
                Matrix kneeRight = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);
                Matrix combined = kneeRight * hipRight;

                this.SetLegMatrix(bone.EndJoint, combined, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.FootLeft || bone.EndJoint == JointType.FootRight)
            {
                // Only set this if we actually have a good track on this and the parent
                if (skeleton.Joints[bone.EndJoint].TrackingState == JointTrackingState.Tracked && skeleton.Joints[skeleton.BoneOrientations[bone.EndJoint].StartJoint].TrackingState == JointTrackingState.Tracked)
                {
                    Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                    // Add a small adjustment rotation to correct for the avatar skeleton foot bones being defined pointing down at 45 degrees, not horizontal
                    Matrix adjustment = Matrix.CreateRotationX(MathHelper.ToRadians(-45));
                    tempMat *= adjustment;

                    // Kinect = +Y along foot (fwd), +Z up, +X right in body coordinate system
                    // Avatar = +X along foot (fwd), +Y up, +Z right
                    Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat); // XYZ
                    Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.Z, kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                    tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                    this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
                }
            }
        }

        private void SetJointTransformationByPeople(BoneOrientation bone, Skeleton skeleton, Matrix bindRoot, ref Matrix[] boneTransforms)
        {
            // Always look at the skeleton root
            if (bone.StartJoint == JointType.HipCenter && bone.EndJoint == JointType.HipCenter)
            {

                // Unless in seated mode, the hip center is special - it is the root of the NuiSkeleton and describes the skeleton orientation in the world
                // (camera) coordinate system. All other bones/joint orientations in the hierarchy have hip center as one of their parents.
                // However, if in seated mode, the shoulder center then holds the skeleton orientation in the world (camera) coordinate system.
                bindRoot.Translation = Vector3.Zero;
                Matrix invBindRoot = Matrix.Invert(bindRoot);

                Matrix hipOrientation = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);
                Vector3 hipRotation = RotationHelper.GetInstance().MatrixToEulerAngleVector3(hipOrientation);
                //DebugText += String.Format("hipCenter:(x={0},y={1},z={2})\n", hipRotation.X, hipRotation.Y, hipRotation.Z);
                // ensure pure rotation, as we set world translation from the Kinect camera below

                int hipCenterIndex = 0;
                //this.nuiJointToAvatarBoneIndex.TryGetValue(JointType.HipCenter, out hipCenterIndex);
                Matrix hipCenter = boneTransforms[hipCenterIndex];

                hipCenter.Translation = Vector3.Zero;
                Matrix invPelvis = Matrix.Invert(hipCenter);


                Matrix combined = (invBindRoot * hipOrientation) * invPelvis;

                //this.ReplaceBoneMatrix(JointType.HipCenter, combined, true, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.ShoulderCenter)
            {
                // This contains an absolute rotation if we are in seated mode, or the hip center is not tracked, as the HipCenter will be identity
                if (this.Chooser.SeatedMode || (this.Chooser.SeatedMode == false && skeleton.Joints[JointType.HipCenter].TrackingState == JointTrackingState.NotTracked))
                {
                    bindRoot.Translation = Vector3.Zero;
                    Matrix invBindRoot = Matrix.Invert(bindRoot);

                    Matrix hipOrientation = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                    // ensure pure rotation, as we set world translation from the Kinect camera
                    Matrix hipCenter = boneTransforms[0];
                    hipCenter.Translation = Vector3.Zero;
                    Matrix invPelvis = Matrix.Invert(hipCenter);

                    Matrix combined = (invBindRoot * hipOrientation) * invPelvis;

                    this.ReplaceBoneMatrix(JointType.HipCenter, combined, true, ref boneTransforms);
                }
            }
            else if (bone.EndJoint == JointType.Spine)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // The Dude appears to lean back too far compared to a real person, so here we adjust this lean.
                CorrectBackwardsLean(skeleton, ref tempMat);

                Vector3 rotationVector = RotationHelper.GetInstance().MatrixToEulerAngleVector3(tempMat);
                // Kinect = +X left, +Y up, +Z forward in body coordinate system
                // Avatar = +Z left, +X up, +Y forward
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.X, kinectRotation.Y, kinectRotation.Z, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                // Set the corresponding matrix in the avatar using the translation table we specified.
                // Note for the spine and shoulder center rotations, we could also try to spread the angle
                // over all the Avatar skeleton spine joints, causing a more curved back, rather than apply
                // it all to one joint, as we do here.
                //this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.Head)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);
                // Kinect = +X left, +Y up, +Z forward in body coordinate system
                // Avatar = +Z left, +X up, +Y forward
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.X, -kinectRotation.Y, kinectRotation.Z, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                // Set the corresponding matrix in the avatar using the translation table we specified
                //this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            // Mirror View for the People Model's Left Arm, the real people's Right Arm
            else if (bone.EndJoint == JointType.ElbowLeft || bone.EndJoint == JointType.WristLeft)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);


                // The Dude appears to lean back too far compared to a real person, so here we adjust this lean.
                CorrectBackwardsLean(skeleton, ref tempMat);

                Vector3 rotationVector = RotationHelper.GetInstance().MatrixToEulerAngleVector3(tempMat);
                //DebugText += String.Format("Joint{0}:(x={1},y={2},z={3})\n", bone.EndJoint, rotationVector.X, rotationVector.Y, rotationVector.Z);
                // Kinect = +Y along arm, +X down, -Z backward in body coordinate system
                // Avatar = +X along arm, -Z down, +Y backwards
                // People = +X along arm, -Y down, +Z backwards
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ

                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, -kinectRotation.X, kinectRotation.Z, kinectRotation.W); // transform from Kinect to avatar coordinate system

                tempMat = Matrix.CreateFromQuaternion(avatarRotation);
                //if (bone.EndJoint == JointType.ElbowLeft)
                //{
                //    tempMat =tempMat*
                //        Matrix.CreateRotationX(0.2f) *
                //        Matrix.CreateRotationY(-0.2f) *
                //        Matrix.CreateRotationZ(-0.7f);
                //}
                //Debug according to the keyboard input
                if (bone.EndJoint == JointType.ElbowLeft && this.isDebug)
                {
                    tempMat = tempMat
                        * Matrix.CreateRotationX(this.debugRotationVector.X)
                        * Matrix.CreateRotationY(this.debugRotationVector.Y)
                        * Matrix.CreateRotationZ(this.debugRotationVector.Z);
                }
                // Set the corresponding matrix in the avatar using the translation table we specified.
                // Note for the spine and shoulder center rotations, we could also try to spread the angle
                // over all the Avatar skeleton spine joints, causing a more curved back, rather than apply
                // it all to one joint, as we do here.
                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.HandLeft)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);


                // Kinect = +Y along arm, +X down, +Z forward in body coordinate system
                // Avatar = +X along arm, +Y down, +Z backwards
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(-kinectRotation.Y, kinectRotation.Z, kinectRotation.X, kinectRotation.W);
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.ElbowRight || bone.EndJoint == JointType.WristRight)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);
                // The Dude appears to lean back too far compared to a real person, so here we adjust this lean.
                CorrectBackwardsLean(skeleton, ref tempMat);
                // Kinect = +X left, +Y up, +Z forward in body coordinate system
                // Avatar = +Z left, +X up, +Y forward
                // People = +X left, +Y up, +z forward
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(-kinectRotation.Y, kinectRotation.X, kinectRotation.Z, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                // Set the corresponding matrix in the avatar using the translation table we specified.
                // Note for the spine and shoulder center rotations, we could also try to spread the angle
                // over all the Avatar skeleton spine joints, causing a more curved back, rather than apply
                // it all to one joint, as we do here.
                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.KneeLeft)
            {
                // Combine the two joint rotations from the hip and knee
                Matrix hipLeft = KinectHelper.Matrix4ToXNAMatrix(skeleton.BoneOrientations[JointType.HipLeft].HierarchicalRotation.Matrix);
                Vector3 rotationVector = RotationHelper.GetInstance().MatrixToEulerAngleVector3(hipLeft);
                Matrix kneeLeft = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);
                Matrix legRotation = kneeLeft * hipLeft * Matrix.CreateRotationZ(rotationVector.Z);

                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(legRotation);  // XYZ
                Quaternion avatarRotation = new Quaternion(-kinectRotation.X, kinectRotation.Y, kinectRotation.Z, kinectRotation.W); // transform from Kinect to avatar coordinate system
                rotationVector = RotationHelper.GetInstance().QuaternionToEulerAngleVector3(kinectRotation);
                legRotation = Matrix.CreateFromQuaternion(avatarRotation);

                this.ReplaceBoneMatrix(bone.EndJoint, legRotation, false, ref boneTransforms);

            }
            else if (bone.EndJoint == JointType.AnkleLeft || bone.EndJoint == JointType.AnkleRight)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);
                Matrix legRotation = tempMat;
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(legRotation);  // XYZ
                Quaternion avatarRotation = new Quaternion(-kinectRotation.X, kinectRotation.Y, kinectRotation.Z, kinectRotation.W); // transform from Kinect to avatar coordinate system
                Vector3 rotationVector = RotationHelper.GetInstance().QuaternionToEulerAngleVector3(kinectRotation);
                legRotation = Matrix.CreateFromQuaternion(avatarRotation);

                this.ReplaceBoneMatrix(bone.EndJoint, legRotation, false, ref boneTransforms);

            }
            else if (bone.EndJoint == JointType.KneeRight)
            {
                // Combine the two joint rotations from the hip and knee
                Matrix hipRight = KinectHelper.Matrix4ToXNAMatrix(skeleton.BoneOrientations[JointType.HipRight].HierarchicalRotation.Matrix);
                Matrix kneeRight = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);
                Vector3 rotationVector = RotationHelper.GetInstance().MatrixToEulerAngleVector3(hipRight);
                Matrix legRotation = kneeRight * hipRight * Matrix.CreateRotationZ(rotationVector.Z);

                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(legRotation);  // XYZ
                Quaternion avatarRotation = new Quaternion(-kinectRotation.X, kinectRotation.Y, kinectRotation.Z, kinectRotation.W); // transform from Kinect to avatar coordinate system
                rotationVector = RotationHelper.GetInstance().QuaternionToEulerAngleVector3(kinectRotation);
                legRotation = Matrix.CreateFromQuaternion(avatarRotation);

                this.ReplaceBoneMatrix(bone.EndJoint, legRotation, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.FootLeft || bone.EndJoint == JointType.FootRight)
            {
                // Only set this if we actually have a good track on this and the parent
                if (skeleton.Joints[bone.EndJoint].TrackingState == JointTrackingState.Tracked && skeleton.Joints[skeleton.BoneOrientations[bone.EndJoint].StartJoint].TrackingState == JointTrackingState.Tracked)
                {
                    Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                    // Kinect = +Y along foot (fwd), +Z up, +X right in body coordinate system
                    // Avatar = +X along foot (fwd), +Y up, +Z right
                    Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat); // XYZ
                    Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.Z, kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                    tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                    this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
                }
            }

        }

        /// <summary>
        /// 3D avatar models typically have varying bone structures and joint orientations, depending on how they are built.
        /// Here we adapt the calculated hierarchical relative rotation matrices to work with our avatar and set these into the 
        /// boneTransforms array. This array is then later converted to world transforms and then skinning transforms for the
        /// XNA skinning processor to draw the mesh.
        /// The "Dude.fbx" model defines more bones/joints (57 in total) and in different locations and orientations to the 
        /// Nui Skeleton. Many of the bones/joints have no direct equivalent - e.g. with Kinect we cannot currently recover 
        /// the fingers pose. Bones are defined relative to each other, hence unknown bones will be left as identity relative
        /// transformation in the boneTransforms array, causing them to take their parent's orientation in the world coordinate system.
        /// </summary>
        /// <param name="skeleton">The Kinect skeleton.</param>
        /// <param name="bindRoot">The bind root matrix of the avatar mesh.</param>
        /// <param name="boneTransforms">The avatar mesh rotation matrices.</param>
        private void RetargetMatrixHierarchyToAvatarMesh(Skeleton skeleton, Matrix bindRoot, Matrix[] boneTransforms)
        {
            if (null == skeleton)
            {
                return;
            }

            // Set the bone orientation data in the avatar mesh
            foreach (BoneOrientation bone in skeleton.BoneOrientations)
            {
                // If any of the joints/bones are not tracked, skip them
                // Note that if we run filters on the raw skeleton data, which fix tracking problems,
                // We should set the tracking state from NotTracked to Inferred.
                if (skeleton.Joints[bone.EndJoint].TrackingState == JointTrackingState.NotTracked)
                {
                    continue;
                }

                if (this.ModelType == 0 || this.ModelType == 1)
                    this.SetJointTransformationByPeople(bone, skeleton, bindRoot, ref boneTransforms);
                else if (this.ModelType == 2)
                    this.SetJointTransformationByDube(bone, skeleton, bindRoot, ref boneTransforms);

            }

            // If seated mode is on, sit the avatar down
            if (this.Chooser.SeatedMode && this.setSeatedPostureInSeatedMode)
            {
                this.SetSeatedPosture(ref boneTransforms);
            }

            // Set the world position of the avatar
            this.SetAvatarRootWorldPosition(skeleton, ref boneTransforms);
        }

        /// <summary>
        /// Set the bone transform in the avatar mesh.
        /// </summary>
        /// <param name="bone">Nui Joint/bone orientation</param>
        /// <param name="skeleton">The Kinect skeleton.</param>
        /// <param name="bindRoot">The bind root matrix of the avatar mesh.</param>
        /// <param name="boneTransforms">The avatar mesh rotation matrices.</param>
        private void SetJointTransformation(BoneOrientation bone, Skeleton skeleton, Matrix bindRoot, ref Matrix[] boneTransforms)
        {
            // Always look at the skeleton root
            if (bone.StartJoint == JointType.HipCenter && bone.EndJoint == JointType.HipCenter)
            {
                // Unless in seated mode, the hip center is special - it is the root of the NuiSkeleton and describes the skeleton orientation in the world
                // (camera) coordinate system. All other bones/joint orientations in the hierarchy have hip center as one of their parents.
                // However, if in seated mode, the shoulder center then holds the skeleton orientation in the world (camera) coordinate system.
                bindRoot.Translation = Vector3.Zero;
                Matrix invBindRoot = Matrix.Invert(bindRoot);

                Matrix hipOrientation = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // ensure pure rotation, as we set world translation from the Kinect camera below
                Matrix hipCenter = boneTransforms[1];
                hipCenter.Translation = Vector3.Zero;
                Matrix invPelvis = Matrix.Invert(hipCenter);

                Matrix combined = (invBindRoot * hipOrientation) * invPelvis;

                this.ReplaceBoneMatrix(JointType.HipCenter, combined, true, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.ShoulderCenter)
            {
                // This contains an absolute rotation if we are in seated mode, or the hip center is not tracked, as the HipCenter will be identity
                if (this.Chooser.SeatedMode || (this.Chooser.SeatedMode == false && skeleton.Joints[JointType.HipCenter].TrackingState == JointTrackingState.NotTracked))
                {
                    bindRoot.Translation = Vector3.Zero;
                    Matrix invBindRoot = Matrix.Invert(bindRoot);

                    Matrix hipOrientation = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                    // ensure pure rotation, as we set world translation from the Kinect camera
                    Matrix hipCenter = boneTransforms[1];
                    hipCenter.Translation = Vector3.Zero;
                    Matrix invPelvis = Matrix.Invert(hipCenter);

                    Matrix combined = (invBindRoot * hipOrientation) * invPelvis;

                    this.ReplaceBoneMatrix(JointType.HipCenter, combined, true, ref boneTransforms);
                }
            }
            else if (bone.EndJoint == JointType.Spine)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // The Dude appears to lean back too far compared to a real person, so here we adjust this lean.
                CorrectBackwardsLean(skeleton, ref tempMat);

                // Also add a small constant adjustment rotation to correct for the hip center to spine bone being at a rear-tilted angle in the Kinect skeleton.
                // The dude should now look more straight ahead when avateering
                Matrix adjustment = Matrix.CreateRotationX(MathHelper.ToRadians(20));  // 20 degree rotation around the local Kinect x axis for the spine bone.
                tempMat *= adjustment;

                // Kinect = +X left, +Y up, +Z forward in body coordinate system
                // Avatar = +Z left, +X up, +Y forward
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.Z, kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                // Set the corresponding matrix in the avatar using the translation table we specified.
                // Note for the spine and shoulder center rotations, we could also try to spread the angle
                // over all the Avatar skeleton spine joints, causing a more curved back, rather than apply
                // it all to one joint, as we do here.
                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.Head)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // Add a small adjustment rotation to correct for the avatar skeleton head bones being defined pointing looking slightly down, not vertical.
                // The dude should now look more straight ahead when avateering
                Matrix adjustment = Matrix.CreateRotationX(MathHelper.ToRadians(-30));  // -30 degree rotation around the local Kinect x axis for the head bone.
                tempMat *= adjustment;

                // Kinect = +X left, +Y up, +Z forward in body coordinate system
                // Avatar = +Z left, +X up, +Y forward
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.Z, kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                // Set the corresponding matrix in the avatar using the translation table we specified
                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.ElbowLeft || bone.EndJoint == JointType.WristLeft)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                if (bone.EndJoint == JointType.ElbowLeft)
                {
                    // Add a small adjustment rotation to correct for the avatar skeleton shoulder/upper arm bones.
                    // The dude should now be able to have arms correctly down at his sides when avateering
                    Matrix adjustment = Matrix.CreateRotationZ(MathHelper.ToRadians(-15));  // -15 degree rotation around the local Kinect z axis for the upper arm bone.
                    tempMat *= adjustment;
                }

                // Kinect = +Y along arm, +X down, +Z forward in body coordinate system
                // Avatar = +X along arm, +Y down, +Z backwards
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, -kinectRotation.Z, -kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.HandLeft)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // Add a small adjustment rotation to correct for the avatar skeleton wist/hand bone.
                // The dude should now have the palm of his hands toward his body when arms are straight down
                Matrix adjustment = Matrix.CreateRotationY(MathHelper.ToRadians(-90));  // -90 degree rotation around the local Kinect y axis for the wrist-hand bone.
                tempMat *= adjustment;

                // Kinect = +Y along arm, +X down, +Z forward in body coordinate system
                // Avatar = +X along arm, +Y down, +Z backwards
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.X, -kinectRotation.Z, kinectRotation.W);
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.ElbowRight || bone.EndJoint == JointType.WristRight)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                if (bone.EndJoint == JointType.ElbowRight)
                {
                    // Add a small adjustment rotation to correct for the avatar skeleton shoulder/upper arm bones.
                    // The dude should now be able to have arms correctly down at his sides when avateering
                    Matrix adjustment = Matrix.CreateRotationZ(MathHelper.ToRadians(15));  // 15 degree rotation around the local Kinect  z axis for the upper arm bone.
                    tempMat *= adjustment;
                }

                // Kinect = +Y along arm, +X up, +Z forward in body coordinate system
                // Avatar = +X along arm, +Y back, +Z down
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, -kinectRotation.Z, -kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.HandRight)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                // Add a small adjustment rotation to correct for the avatar skeleton wist/hand bone.
                // The dude should now have the palm of his hands toward his body when arms are straight down
                Matrix adjustment = Matrix.CreateRotationY(MathHelper.ToRadians(90));  // -90 degree rotation around the local Kinect y axis for the wrist-hand bone.
                tempMat *= adjustment;

                // Kinect = +Y along arm, +X up, +Z forward in body coordinate system
                // Avatar = +X along arm, +Y down, +Z forwards
                Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat);    // XYZ
                Quaternion avatarRotation = new Quaternion(kinectRotation.Y, -kinectRotation.X, kinectRotation.Z, kinectRotation.W); // transform from Kinect to avatar coordinate system
                tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.KneeLeft)
            {
                // Combine the two joint rotations from the hip and knee
                Matrix hipLeft = KinectHelper.Matrix4ToXNAMatrix(skeleton.BoneOrientations[JointType.HipLeft].HierarchicalRotation.Matrix);
                Matrix kneeLeft = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);
                Matrix combined = kneeLeft * hipLeft;

                this.SetLegMatrix(bone.EndJoint, combined, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.AnkleLeft || bone.EndJoint == JointType.AnkleRight)
            {
                Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);
                this.SetLegMatrix(bone.EndJoint, tempMat, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.KneeRight)
            {
                // Combine the two joint rotations from the hip and knee
                Matrix hipRight = KinectHelper.Matrix4ToXNAMatrix(skeleton.BoneOrientations[JointType.HipRight].HierarchicalRotation.Matrix);
                Matrix kneeRight = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);
                Matrix combined = kneeRight * hipRight;

                this.SetLegMatrix(bone.EndJoint, combined, ref boneTransforms);
            }
            else if (bone.EndJoint == JointType.FootLeft || bone.EndJoint == JointType.FootRight)
            {
                // Only set this if we actually have a good track on this and the parent
                if (skeleton.Joints[bone.EndJoint].TrackingState == JointTrackingState.Tracked && skeleton.Joints[skeleton.BoneOrientations[bone.EndJoint].StartJoint].TrackingState == JointTrackingState.Tracked)
                {
                    Matrix tempMat = KinectHelper.Matrix4ToXNAMatrix(bone.HierarchicalRotation.Matrix);

                    // Add a small adjustment rotation to correct for the avatar skeleton foot bones being defined pointing down at 45 degrees, not horizontal
                    Matrix adjustment = Matrix.CreateRotationX(MathHelper.ToRadians(-45));
                    tempMat *= adjustment;

                    // Kinect = +Y along foot (fwd), +Z up, +X right in body coordinate system
                    // Avatar = +X along foot (fwd), +Y up, +Z right
                    Quaternion kinectRotation = KinectHelper.DecomposeMatRot(tempMat); // XYZ
                    Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.Z, kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
                    tempMat = Matrix.CreateFromQuaternion(avatarRotation);

                    this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
                }
            }
        }

        /// <summary>
        /// Correct the spine rotation when leaning back to reduce lean.
        /// </summary>
        /// <param name="skeleton">The Kinect skeleton.</param>
        /// <param name="spineMat">The spine orientation.</param>
        private void CorrectBackwardsLean(Skeleton skeleton, ref Matrix spineMat)
        {
            Matrix hipOrientation = KinectHelper.Matrix4ToXNAMatrix(skeleton.BoneOrientations[JointType.HipCenter].HierarchicalRotation.Matrix);

            Vector3 hipZ = new Vector3(hipOrientation.M31, hipOrientation.M32, hipOrientation.M33);   // Z (forward) vector
            Vector3 boneY = new Vector3(spineMat.M21, spineMat.M22, spineMat.M23);   // Y (up) vector

            hipZ *= -1;
            hipZ.Normalize();
            boneY.Normalize();

            // Dot product the hip center forward vector with our spine bone up vector.
            float cosAngle = Vector3.Dot(hipZ, boneY);

            // If it's negative (i.e. greater than 90), we are leaning back, so reduce this lean.
            if (cosAngle < 0 && this.leanAdjust)
            {
                float angle = (float)Math.Acos(cosAngle);
                float correction = (angle / 2) * -(cosAngle / 2);
                Matrix leanAdjustment = Matrix.CreateRotationX(correction);  // reduce the lean by up to half, scaled by how far back we are leaning
                spineMat *= leanAdjustment;
            }
        }

        /// <summary>
        /// Helper used for leg bones.
        /// </summary>
        /// <param name="joint">Nui Joint index</param>
        /// <param name="legRotation">Matrix containing a leg joint rotation.</param>
        /// <param name="boneTransforms">The avatar mesh rotation matrices.</param>
        private void SetLegMatrix(JointType joint, Matrix legRotation, ref Matrix[] boneTransforms)
        {
            // Kinect = +Y along leg (down), +Z fwd, +X right in body coordinate system
            // Avatar = +X along leg (down), +Y fwd, +Z right
            Quaternion kinectRotation = KinectHelper.DecomposeMatRot(legRotation);  // XYZ
            Quaternion avatarRotation = new Quaternion(kinectRotation.Y, kinectRotation.Z, kinectRotation.X, kinectRotation.W); // transform from Kinect to avatar coordinate system
            legRotation = Matrix.CreateFromQuaternion(avatarRotation);

            this.ReplaceBoneMatrix(joint, legRotation, false, ref boneTransforms);
        }

        /// <summary>
        /// Set the avatar root position in world coordinates.
        /// </summary>
        /// <param name="skeleton">The Kinect skeleton.</param>
        /// <param name="boneTransforms">The avatar mesh rotation matrices.</param>
        private void SetAvatarRootWorldPosition(Skeleton skeleton, ref Matrix[] boneTransforms)
        {
            // Get XNA world position of skeleton.
            Matrix worldTransform = this.GetModelWorldTranslation(skeleton.Joints, this.Chooser.NearMode);

            // set root translation
            boneTransforms[0].Translation = worldTransform.Translation;
            //boneTransforms[0].Translation = new Vector3(0,0,0);
        }

        /// <summary>
        /// This function configures the mapping between the Nui Skeleton bones/joints and the Avatar bones/joints
        /// </summary>
        protected void BuildJointHierarchy()
        {
            // "Dude.fbx" bone index definitions
            // These are described as the "bone" that the transformation affects.
            // The rotation values are stored at the start joint before the bone (i.e. at the shared joint with the end of the parent bone).
            // 0 = root node
            // 1 = pelvis
            // 2 = spine
            // 3 = spine1
            // 4 = spine2
            // 5 = spine3
            // 6 = neck
            // 7 = head
            // 8-11 = eyes
            // 12 = Left clavicle (joint between spine and shoulder)
            // 13 = Left upper arm (joint at left shoulder)
            // 14 = Left forearm
            // 15 = Left hand
            // 16-30 = Left hand finger bones
            // 31 = Right clavicle (joint between spine and shoulder)
            // 32 = Right upper arm (joint at left shoulder)
            // 33 = Right forearm
            // 34 = Right hand
            // 35-49 = Right hand finger bones
            // 50 = Left Thigh
            // 51 = Left Knee
            // 52 = Left Ankle
            // 53 = Left Ball
            // 54 = Right Thigh
            // 55 = Right Knee
            // 56 = Right Ankle
            // 57 = Right Ball

            // For the Kinect NuiSkeleton, the joint at the end of the bone describes the rotation to get there, 
            // and the root orientation is in HipCenter. This is different to the Avatar skeleton described above.
            if (null == this.nuiJointToAvatarBoneIndex)
            {
                this.nuiJointToAvatarBoneIndex = new Dictionary<JointType, int>();
            }

            if (this.ModelType == 0 || this.ModelType == 1)
            {
                this.nuiJointToAvatarBoneIndex.Add(JointType.HipCenter, 1);
                this.nuiJointToAvatarBoneIndex.Add(JointType.Spine, 4);
                this.nuiJointToAvatarBoneIndex.Add(JointType.ShoulderCenter, 5);
                this.nuiJointToAvatarBoneIndex.Add(JointType.Head, 7);
                this.nuiJointToAvatarBoneIndex.Add(JointType.ElbowLeft, 9);
                this.nuiJointToAvatarBoneIndex.Add(JointType.WristLeft, 10);
                this.nuiJointToAvatarBoneIndex.Add(JointType.HandLeft, 11);
                this.nuiJointToAvatarBoneIndex.Add(JointType.ElbowRight, 13);
                this.nuiJointToAvatarBoneIndex.Add(JointType.WristRight, 14);
                this.nuiJointToAvatarBoneIndex.Add(JointType.HandRight, 15);
                this.nuiJointToAvatarBoneIndex.Add(JointType.KneeLeft, 16);
                this.nuiJointToAvatarBoneIndex.Add(JointType.AnkleLeft, 17);
                this.nuiJointToAvatarBoneIndex.Add(JointType.FootLeft, 18);
                this.nuiJointToAvatarBoneIndex.Add(JointType.KneeRight, 20);
                this.nuiJointToAvatarBoneIndex.Add(JointType.AnkleRight, 21);
                this.nuiJointToAvatarBoneIndex.Add(JointType.FootRight, 22);
            }

            else if (this.ModelType == 2)
            {
                this.nuiJointToAvatarBoneIndex.Add(JointType.HipCenter, 1);
                this.nuiJointToAvatarBoneIndex.Add(JointType.Spine, 4);
                this.nuiJointToAvatarBoneIndex.Add(JointType.ShoulderCenter, 6);
                this.nuiJointToAvatarBoneIndex.Add(JointType.Head, 7);
                this.nuiJointToAvatarBoneIndex.Add(JointType.ElbowLeft, 13);
                this.nuiJointToAvatarBoneIndex.Add(JointType.WristLeft, 14);
                this.nuiJointToAvatarBoneIndex.Add(JointType.HandLeft, 15);
                this.nuiJointToAvatarBoneIndex.Add(JointType.ElbowRight, 32);
                this.nuiJointToAvatarBoneIndex.Add(JointType.WristRight, 33);
                this.nuiJointToAvatarBoneIndex.Add(JointType.HandRight, 34);
                this.nuiJointToAvatarBoneIndex.Add(JointType.KneeLeft, 50);
                this.nuiJointToAvatarBoneIndex.Add(JointType.AnkleLeft, 51);
                this.nuiJointToAvatarBoneIndex.Add(JointType.FootLeft, 52);
                this.nuiJointToAvatarBoneIndex.Add(JointType.KneeRight, 54);
                this.nuiJointToAvatarBoneIndex.Add(JointType.AnkleRight, 55);
                this.nuiJointToAvatarBoneIndex.Add(JointType.FootRight, 56);

            }
            // Note: the actual hip center joint in the Avatar mesh has a root node (index 0) as well, which we ignore here for rotation.

        }


        /// <summary>
        /// This function sets the mapping between the Nui Skeleton bones/joints and the Avatar bones/joints
        /// </summary>
        /// <param name="joint">Nui Joint index</param>
        /// <param name="boneMatrix">Matrix to set in joint/bone.</param>
        /// <param name="replaceTranslationInExistingBoneMatrix">set Boolean true to replace the translation in the original bone matrix with the one passed in boneMatrix (i.e. at root), false keeps the original (default).</param>
        /// <param name="boneTransforms">The avatar mesh rotation matrices.</param>
        private void ReplaceBoneMatrix(JointType joint, Matrix boneMatrix, bool replaceTranslationInExistingBoneMatrix, ref Matrix[] boneTransforms)
        {
            int meshJointId;
            bool success = this.nuiJointToAvatarBoneIndex.TryGetValue(joint, out meshJointId);

            if (success)
            {
                Vector3 offsetTranslation = boneTransforms[meshJointId].Translation;
                boneTransforms[meshJointId] = boneMatrix;

                if (replaceTranslationInExistingBoneMatrix == false)
                {
                    // overwrite any new boneMatrix translation with the original one
                    boneTransforms[meshJointId].Translation = offsetTranslation;   // re-set the translation
                }
            }
        }

        /// <summary>
        /// Helper used to get the world translation for the root.
        /// </summary>
        /// <param name="joints">Nui Joint collection.</param>
        /// <param name="seatedMode">Boolean true if seated mode.</param>
        /// <returns>Returns a Matrix containing the translation.</returns>
        private Matrix GetModelWorldTranslation(JointCollection joints, bool seatedMode)
        {
            Vector3 transVec = Vector3.Zero;

            if (seatedMode && joints[JointType.ShoulderCenter].TrackingState != JointTrackingState.NotTracked)
            {
                transVec = KinectHelper.SkeletonPointToVector3(joints[JointType.ShoulderCenter].Position);
            }
            else
            {
                if (joints[JointType.HipCenter].TrackingState != JointTrackingState.NotTracked)
                {
                    transVec = KinectHelper.SkeletonPointToVector3(joints[JointType.HipCenter].Position);
                }
                else if (joints[JointType.ShoulderCenter].TrackingState != JointTrackingState.NotTracked)
                {
                    // finally try shoulder center if this is tracked while hip center is not
                    transVec = KinectHelper.SkeletonPointToVector3(joints[JointType.ShoulderCenter].Position);
                }
            }

            if (this.fixAvatarHipCenterDrawHeight)
            {
                transVec.Y = this.avatarHipCenterDrawHeight;
            }

            // Here we scale the translation, as the "Dude" avatar mesh is defined in centimeters, and the Kinect skeleton joint positions in meters.
            return Matrix.CreateTranslation(transVec * SkeletonTranslationScaleFactor);
        }

        /// <summary>
        /// Sets the Avatar in a seated posture - useful for seated mode.
        /// </summary>
        /// <param name="boneTransforms">The relative bone transforms of the avatar mesh.</param>
        private void SetSeatedPosture(ref Matrix[] boneTransforms)
        {
            // In the Kinect coordinate system, we first rotate from the local avatar 
            // root orientation with +Y up to +Y down for the leg bones (180 around Z)
            // then pull the knees up for a seated posture.
            Matrix rot180 = Matrix.CreateRotationZ(MathHelper.ToRadians(180));
            Matrix rot90 = Matrix.CreateRotationX(MathHelper.ToRadians(90));
            Matrix rotMinus90 = Matrix.CreateRotationX(MathHelper.ToRadians(-90));
            Matrix combinedHipRotation = rot90 * rot180;

            this.SetLegMatrix(JointType.KneeLeft, combinedHipRotation, ref boneTransforms);
            this.SetLegMatrix(JointType.KneeRight, combinedHipRotation, ref boneTransforms);
            this.SetLegMatrix(JointType.AnkleLeft, rotMinus90, ref boneTransforms);
            this.SetLegMatrix(JointType.AnkleRight, rotMinus90, ref boneTransforms);
        }

        /// <summary>
        /// Create the viewing camera.
        /// </summary>
        protected void UpdateViewingCamera()
        {
            GraphicsDevice device = this.Game.GraphicsDevice;

            // Compute camera matrices.
            this.view = Matrix.CreateTranslation(0, -CameraHeight, 0) *
                          Matrix.CreateRotationY(MathHelper.ToRadians(this.cameraRotation)) *
                          Matrix.CreateRotationX(MathHelper.ToRadians(this.cameraArc)) *
                          Matrix.CreateLookAt(
                                                new Vector3(0, 0, -this.cameraDistance),
                                                new Vector3(0, 0, 0),
                                                Vector3.Up);

            // Kinect vertical FOV in degrees
            float nominalVerticalFieldOfView = 45.6f;

            if (null != this.Chooser && null != this.Chooser.Sensor && this.Chooser.Sensor.IsRunning && KinectStatus.Connected == this.Chooser.Sensor.Status)
            {
                nominalVerticalFieldOfView = this.Chooser.Sensor.DepthStream.NominalVerticalFieldOfView;
            }

            this.projection = Matrix.CreatePerspectiveFieldOfView(
                                                                (nominalVerticalFieldOfView * (float)Math.PI / 180.0f),
                                                                device.Viewport.AspectRatio,
                                                                1,
                                                                10000);
        }

        protected override void LoadContent()
        {


            // Load the model.
            this.ModelType = 1;
            Model avatar = null;
            if (this.ModelType == 0)
            {
                avatar = this.Game.Content.Load<Model>("people01");
            }
            else if (this.ModelType == 1)
            {
                avatar = this.Game.Content.Load<Model>("yifu_bone2");
            }
            else if (this.ModelType == 2)
            {
                avatar = this.Game.Content.Load<Model>("dude");
            }
            // Load the model.
             

            // Add the model to the avatar animator
            this.Avatar = avatar;

            // Set the Nui joint to model mapping for this avatar
            this.BuildJointHierarchy();
            base.LoadContent();
        }


        /// <summary>
        /// Handles camera input.
        /// </summary>
        /// <param name="gameTime">The gametime.</param>
        private void UpdateCamera(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Check for input to rotate the camera up and down around the model.
            if (this.currentKeyboard.IsKeyDown(Keys.Up) ||
                this.currentKeyboard.IsKeyDown(Keys.W))
            {
                this.cameraArc += time * CameraArcIncrement;
            }

            if (this.currentKeyboard.IsKeyDown(Keys.Down) ||
                this.currentKeyboard.IsKeyDown(Keys.S))
            {
                this.cameraArc -= time * CameraArcIncrement;
            }

            // Limit the arc movement.
            if (this.cameraArc > CameraArcAngleLimit)
            {
                this.cameraArc = CameraArcAngleLimit;
            }
            else if (this.cameraArc < -CameraArcAngleLimit)
            {
                this.cameraArc = -CameraArcAngleLimit;
            }

            // Check for input to rotate the camera around the model.
            if (this.currentKeyboard.IsKeyDown(Keys.Right) ||
                this.currentKeyboard.IsKeyDown(Keys.D))
            {
                this.cameraRotation += time * CameraArcIncrement;
            }

            if (this.currentKeyboard.IsKeyDown(Keys.Left) ||
                this.currentKeyboard.IsKeyDown(Keys.A))
            {
                this.cameraRotation -= time * CameraArcIncrement;
            }

            // Check for input to zoom camera in and out.
            if (this.currentKeyboard.IsKeyDown(Keys.Z))
            {
                this.cameraDistance += time * CameraZoomIncrement;
            }

            if (this.currentKeyboard.IsKeyDown(Keys.X))
            {
                this.cameraDistance -= time * CameraZoomIncrement;
            }

            // Limit the camera distance from the origin.
            if (this.cameraDistance > CameraMaxDistance)
            {
                this.cameraDistance = CameraMaxDistance;
            }
            else if (this.cameraDistance < CameraMinDistance)
            {
                this.cameraDistance = CameraMinDistance;
            }

            if (this.currentKeyboard.IsKeyDown(Keys.R))
            {
                this.cameraArc = 0;
                this.cameraRotation = 0;
                this.cameraDistance = CameraStartingTranslation;
            }
        }
    }
}
