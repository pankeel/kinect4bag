using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SkinnedModel;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Samples.Kinect.XnaBasics.ModelRender
{
    public class SkinnedObject3D : Object3D
    {
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
        /// Back link to the avatar model bind pose and skeleton hierarchy data.
        /// </summary>
        private SkinningData skinningDataValue;
        /// <summary>
        /// Gets or sets the Avatar 3D model to animate.
        /// </summary>
        private Model currentModel;
        public Model Model3DAvatar
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

        public SkinnedObject3D(Game game)
            : base(game)
        {

        }
    }
}
