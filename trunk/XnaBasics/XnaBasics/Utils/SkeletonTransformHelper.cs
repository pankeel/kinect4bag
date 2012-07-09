using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SkinnedModel;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    public class SkeletonTransformHelper : DrawableGameComponent
    {
        private bool isDebug = true;
        private Vector3 debugRotationVector = new Vector3();
        public int ModelType = 0;
        /// <summary>
        /// The avatar relative bone transformation matrices.
        /// </summary>
        private Matrix[] boneTransforms;

        /// <summary>
        /// The avatar "absolute" bone transformation matrices in the world coordinate system.
        /// These are in the warrior ("dude") format
        /// </summary>
        private Matrix[] worldTransforms;
        public Matrix[] WorldTransforms
        {
            get
            {
                return this.worldTransforms;
            }
            
        }

        /// <summary>
        /// The avatar skin transformation matrices.
        /// </summary>
        private Matrix[] skinTransforms;
        public Matrix[] SkinTransforms
        {
            get
            {
                return this.skinTransforms;
            }
        }

        /// <summary>
        /// Back link to the avatar model bind pose and skeleton hierarchy data.
        /// </summary>
        private SkinningData skinningDataValue;
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
        /// Store the mapping between the NuiJoint and the Avatar Bone index.
        /// </summary>
        private Dictionary<JointType, int> nuiJointToAvatarBoneIndex;
        public SkeletonTransformHelper(Game game):base(game)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skinningData"></param>
        public void ResetTransform(SkinningData skinningData)
        {
            // Bone matrices for the "dude" model
            this.boneTransforms = new Matrix[skinningData.BindPose.Count];
            this.worldTransforms = new Matrix[skinningData.BindPose.Count];
            this.skinTransforms = new Matrix[skinningData.BindPose.Count];

            // Initialize bone transforms to the bind pose.
            this.skinningDataValue = skinningData;
            RefreshBindPose();
        }


        public void RefreshBindPose()
        {
            this.skinningDataValue.BindPose.CopyTo(this.boneTransforms, 0);
            this.UpdateWorldTransforms(Matrix.Identity);
            this.UpdateSkinTransforms();
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

        /// <summary>
        /// This function configures the mapping between the Nui Skeleton bones/joints and the Avatar bones/joints
        /// 0 = root node
        /// 1 = pelvis
        /// 2 = spine
        /// 3 = spine1
        /// 4 = spine2
        /// 5 = shoulder Center
        /// 6 = neck
        /// 7 = head
        /// 8 = Left clavicle (joint between spine and shoulder)
        /// 9 = Left upper arm (joint at left shoulder)
        /// 10 = Left forearm
        /// 11 = Left hand
        /// 12 = Right clavicle (joint between spine and shoulder)
        /// 13 = Right upper arm (joint at left shoulder)
        /// 14 = Right forearm
        /// 15 = Right hand
        /// 16 = Left Thigh
        /// 17 = Left Knee
        /// 18 = Left Ankle
        /// 19 = Left Ball
        /// 20 = Right Thigh
        /// 21 = Right Knee
        /// 22 = Right Ankle
        /// 23 = Right Ball
        /// </summary>
        public void BuildJointHierarchy()
        {
            if (null == this.nuiJointToAvatarBoneIndex)
            {
                this.nuiJointToAvatarBoneIndex = new Dictionary<JointType, int>();
            }

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
            // Note: the actual hip center joint in the Avatar mesh has a root node (index 0) as well, which we ignore here for rotation.
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
        public void RetargetMatrixHierarchy(Skeleton skeleton)
        {
            Matrix bindRoot = this.skinningDataValue.BindPose[0];


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

                
                this.SetJointTransformation(bone, skeleton, bindRoot, ref boneTransforms);

            }

            // If seated mode is on, sit the avatar down
            //if (this.Chooser.SeatedMode && this.setSeatedPostureInSeatedMode)
            //{
            //    this.SetSeatedPosture(ref boneTransforms);
            //}

            // Set the world position of the avatar
            this.SetAvatarRootWorldPosition(skeleton, ref boneTransforms);

            // Calculate the Avatar world transforms from the relative bone transforms of Kinect skeleton
            this.UpdateWorldTransforms(Matrix.Identity);

            // Refresh the Avatar SkinTransforms data based on the transforms we just applied
            this.UpdateSkinTransforms();
        }

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
                Vector3 hipRotation = RotationHelper.GetInstance().MatrixToEulerAngleVector3(hipOrientation);
               
                // ensure pure rotation, as we set world translation from the Kinect camera below

                int hipCenterIndex = 0;
                Matrix hipCenter = boneTransforms[hipCenterIndex];

                hipCenter.Translation = Vector3.Zero;
                Matrix invPelvis = Matrix.Invert(hipCenter);

                //Matrix combined = (invBindRoot * hipOrientation) * invPelvis;

                //this.ReplaceBoneMatrix(JointType.HipCenter, Matrix.Identity, true, ref boneTransforms);
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
                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
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
                this.ReplaceBoneMatrix(bone.EndJoint, tempMat, false, ref boneTransforms);
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
            bool leanAdjust = true;
            if (cosAngle < 0 && leanAdjust)
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
            
        }

        /// <summary>
        /// Helper used to get the world translation for the root.
        /// </summary>
        /// <param name="joints">Nui Joint collection.</param>
        /// <param name="seatedMode">Boolean true if seated mode.</param>
        /// <returns>Returns a Matrix containing the translation.</returns>
        private Matrix GetModelWorldTranslation(JointCollection joints, bool seatedMode)
        {
            SkeletonPoint transRootVec3 = new SkeletonPoint(); 

            if (seatedMode && joints[JointType.ShoulderCenter].TrackingState != JointTrackingState.NotTracked)
            {
                transRootVec3 = joints[JointType.ShoulderCenter].Position;
            }
            else
            {
                if (joints[JointType.HipCenter].TrackingState != JointTrackingState.NotTracked)
                {
                    transRootVec3 = joints[JointType.HipCenter].Position;
                }
                else if (joints[JointType.ShoulderCenter].TrackingState != JointTrackingState.NotTracked)
                {
                    // finally try shoulder center if this is tracked while hip center is not
                    transRootVec3 = joints[JointType.ShoulderCenter].Position;
                }
            }

            float modelZvalue = 0.0f;
            #region Calculate Real Length 
            {
                
                if (joints[JointType.HipCenter].TrackingState != JointTrackingState.NotTracked
                    && joints[JointType.ShoulderCenter].TrackingState != JointTrackingState.NotTracked
                    && joints[JointType.Head].TrackingState != JointTrackingState.NotTracked)
                {
                    SkeletonPoint hipCenterPoint = joints[JointType.HipCenter].Position, shoulderCenterPoint =  joints[JointType.ShoulderCenter].Position;
                    SkeletonPoint headPoint = joints[JointType.Head].Position;
                    hipCenterPoint.Z = headPoint.Z;
                    shoulderCenterPoint.Z = headPoint.Z;
                    DepthImagePoint hipCenterDepthPoint = Chooser.Sensor.MapSkeletonPointToDepth(hipCenterPoint, Chooser.Sensor.DepthStream.Format),
                        shoulderCenterDepthPoint = Chooser.Sensor.MapSkeletonPointToDepth(shoulderCenterPoint, Chooser.Sensor.DepthStream.Format);
                    float dLengthDepthHipShoulder = Vector2.Distance(new Vector2(hipCenterDepthPoint.X,hipCenterDepthPoint.Y), new Vector2(shoulderCenterDepthPoint.X,shoulderCenterDepthPoint.Y));

                    float dLengthHipShoulder = Vector3.Distance(new Vector3(hipCenterPoint.X, hipCenterPoint.Y, hipCenterPoint.Z), new Vector3(shoulderCenterPoint.X, shoulderCenterPoint.Y, shoulderCenterPoint.Z));
                    float dRealZvalue = (hipCenterPoint.Z + shoulderCenterPoint.Z) / 2;

                    Vector3 modelEndVec3 = new Vector3();
                    for (int i=1;i<5;i++)
                    {
                        modelEndVec3 += boneTransforms[i].Translation;
                    }
                    float modelLengthHipShoulder = Vector3.Distance(new Vector3(0,0,0), modelEndVec3);

                    modelZvalue = dRealZvalue / dLengthHipShoulder * modelLengthHipShoulder;
                }

            }
            #endregion 
            Vector3 certainVector = KinectHelper.SkeletonPointToVector3(transRootVec3);
            
            DepthImagePoint deptImagePoint = Chooser.Sensor.MapSkeletonPointToDepth(transRootVec3, Chooser.Sensor.DepthStream.Format);
            //
            // (x,y) in kinect world space is equal ratio 
            // to the virtual world for the camera space
            //
            Vector3 test = new Vector3();
            if (Chooser.Sensor.DepthStream.Format == DepthImageFormat.Resolution640x480Fps30)
            {
                 //modelZvalue
                float viewportWidth = 2 * (float)Math.Tan(28.5f/180.0f*Math.PI) * modelZvalue,
                     viewportHeight = 2 * (float)Math.Tan(21.5f/180.0f*Math.PI) * modelZvalue,
                     ratioX = deptImagePoint.X / 640.0f,
                     ratioY = deptImagePoint.Y / 480.0f,
                     posX = viewportWidth / 2 - viewportWidth * ratioX, 
                     posY = viewportHeight * ratioY - viewportHeight / 2;
                test.X = posX;
                test.Y = posY;
            }

            test.Z = -modelZvalue;

            return Matrix.CreateTranslation(test);
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
    }
}
