using JiggleGame;
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
        private bool bSupress = false;

        /// <summary>
        /// This skeleton is the  first tracked skeleton in the frame is used for animation, with constraints and mirroring optionally applied.
        /// </summary>
        private Skeleton skeleton;
        /// <summary>
        /// Draw the avatar only when the player skeleton is detected in the depth image.
        /// </summary>
        private bool drawAvatarOnlyWhenPlayerDetected;

        /// <summary>
        /// Flag for first detection of skeleton.
        /// </summary>
        private bool skeletonDetected;



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
        /// Gets or sets a value indicating whether This flag ensures we only request a frame once per update call
        /// across the entire application.
        /// </summary>
        public bool SkeletonDrawn { get; set; }

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

        public Camera GameCamera
        {
            get
            {
                return (Camera)Game.Services.GetService(typeof(Camera));
            }
        }


        private SkeletonTransformHelper skeletonTransformHelper;

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

            this.skeletonTransformHelper = new SkeletonTransformHelper(game);
            game.Components.Add(this.skeletonTransformHelper);

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
        /// The 3D avatar mesh.
        /// </summary>
        private Model currentModel;
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
                skeletonTransformHelper.ResetTransform(skinningData);
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
                this.SkeletonDrawn = false;
            }
            
            // If we have already drawn this skeleton, then we should retrieve a new frame
            // This prevents us from calling the next frame more than once per update
            if (false == this.SkeletonDrawn && null != this.skeleton && this.useKinectAvateering)
            {
                // Copy all bind pose matrices to boneTransforms 
                // Note: most are identity, but the translation is important to describe bone length/the offset between bone drawing positions
                this.skeletonTransformHelper.RefreshBindPose();
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

                this.skeletonTransformHelper.RetargetMatrixHierarchy(this.skeleton);
                

                this.lastNuiTime = currentNuiTime;
            }

            this.HandleKeyBoard();

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
            if (!this.skeletonDetected)
                return;

            if (null != this.Avatar )
            {
                // Reset the DepthStencilState to avoid the transparent of the 3d model
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                Matrix view = GameCamera.View, projection = GameCamera.Projection;
                this.Draw(gameTime, Matrix.Identity, view, projection);
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
            if (this.currentModel == null )
                return;
            // Render the 3D model skinned mesh with Skinned Effect.
            // Mesh Draw Method
            foreach (ModelMesh mesh in this.currentModel.Meshes)
            {
                
                //VertexPositionNormalTexture[] mmp = new VertexPositionNormalTexture[mesh.MeshParts[0].VertexBuffer.VertexCount];
                //mesh.MeshParts[0].VertexBuffer.GetData<VertexPositionNormalTexture>(mmp);
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(this.skeletonTransformHelper.SkinTransforms);
                    
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

                foreach (Matrix boneWorldTrans in this.skeletonTransformHelper.WorldTransforms)
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
        private void HandleKeyBoard()
        {

            Input input = _G.GameInput.GetInput(0);
            // Draw avatar when not detected on/off toggle
            if (input.ButtonJustPressed((int)E_UiButton.V))
                this.drawAvatarOnlyWhenPlayerDetected = !this.drawAvatarOnlyWhenPlayerDetected;
            // Seated and near mode on/off toggle
            if (input.ButtonJustPressed((int)E_UiButton.N))
            {
                this.Chooser.SeatedMode = !this.Chooser.SeatedMode;
                this.skeletonDetected = false;

                // Set near mode to accompany seated mode
                this.Chooser.NearMode = this.Chooser.SeatedMode;
            }


            // Reset the avatar filters (also resets camera)
            if (input.ButtonJustPressed((int)E_UiButton.R))
            {
                this.Reset();
            }

            // Mirror Avatar on/off toggle
            if (input.ButtonJustPressed((int)E_UiButton.M))
            {
                this.mirrorView = !this.mirrorView;
            }

            // Local Axes on/off toggle
            if (input.ButtonJustPressed((int)E_UiButton.G))
            {
                this.drawLocalAxes = !this.drawLocalAxes;
            }

            // Avateering on/off toggle
            if (input.ButtonJustPressed((int)E_UiButton.K))
            {
                this.useKinectAvateering = !this.useKinectAvateering;
            }

            // Tilt Compensation on/off toggle
            if (input.ButtonJustPressed((int)E_UiButton.T))
            {
                this.tiltCompensate = !this.tiltCompensate;
            }

            // Compensate for sensor height, move skeleton to floor on/off toggle
            if (input.ButtonJustPressed((int)E_UiButton.O))
            {
                this.floorOffsetCompensate = !this.floorOffsetCompensate;
            }

            // Torso self intersection constraints on/off toggle
            if (input.ButtonJustPressed((int)E_UiButton.I))
            {
                this.selfIntersectionConstraints = !this.selfIntersectionConstraints;
            }

            // Filter Joint Orientation on/off toggle
            if (input.ButtonJustPressed((int)E_UiButton.F))
            {
                this.filterBoneOrientations = !this.filterBoneOrientations;
            }

            // Constrain Bone orientations on/off toggle
            if (input.ButtonJustPressed((int)E_UiButton.C))
            {
                this.boneConstraints = !this.boneConstraints;
            }

            // Draw Bones in bone orientation constraints on/off toggle
            if (input.ButtonJustPressed((int)E_UiButton.B))
            {
                this.drawBoneConstraintsSkeleton = !this.drawBoneConstraintsSkeleton;
            }
        }


        protected override void LoadContent()
        {
            Model avatar = null;
            avatar = this.Game.Content.Load<Model>("yifu_bone2");

             
            
            // Add the model to the avatar animator
            this.Avatar = avatar;

            // Set the Nui joint to model mapping for this avatar
            
            this.skeletonTransformHelper.BuildJointHierarchy();
            base.LoadContent();
        }

    }
}
