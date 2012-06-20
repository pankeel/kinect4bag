using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Windows.Controls;
using ModelViewer.OnScreenMessages;

namespace ModelViewer
{
    public enum ProjectionType { Perspective, Orthagonal }
    public enum ShadingType { Solid, Wireframe }

    public class ModelViewControl : GraphicsDeviceControl
    {
        public static Vector2 ScreenSize;

        private Model model;
        private WorldAxes worldAxes;
        private Input input;
        private Grid gridXY;

        internal ArcBallCamera camera;

        private Color ambientColor = Color.White;

        private SpriteBatch spriteBatch;

        private ShadingType shadingType = ShadingType.Solid;
        public ShadingType ShadingType
        {
            get { return shadingType; }
            set
            {
                shadingType = value;
                ViewModeLabel.Instance.ShadingType = value;
            }
        }

        RasterizerState rsWireframe, rsSolid;
        private Vector3[][] diffuseColors;

        private bool isLoading = false;
        private float radius = 0;

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        private ModelMesh selectedMesh = null;

        public BoundingSphere boundingSphere;

        // added to display errors in the textBoxErrors of the UI
        private TextBox errorTextBox;
        public TextBox ErrorTextBox
        {
            get { return errorTextBox; }
            set { errorTextBox = value; }
        }

        /// <summary>
        /// Enable this to generate a divide-by-zero exception error in the Draw() method
        /// for the purposes of test how exceptions are getting handled.
        /// </summary>
        public bool ExceptionGeneratorEnabled;

        private Border modelViewBorder;

        public Border ModelViewBorder
        {
            get { return modelViewBorder; }
            set { modelViewBorder = value; }
        }

        protected override void Initialize ()
        {
            input = new Input();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ContentManager Content = new ContentManager(Services, "Content");

            ViewModeLabel.Instance.Initialize(Content);
            Message.Instance.Initialize(Content);
            ReferenceAxis.Instance.Initialize(GraphicsDevice);

            camera = new ArcBallCamera();
            camera.FieldOfView = MathHelper.Pi / 3;
            ResetView();

            worldAxes = new WorldAxes();
            worldAxes.Size = 20;

            gridXY = new Grid(this);

            rsWireframe = new RasterizerState();
            rsWireframe.FillMode = FillMode.WireFrame;
            rsWireframe.CullMode = CullMode.None;
            rsSolid = new RasterizerState();
            rsSolid.FillMode = FillMode.Solid;
        }

        protected override void Draw ()
        {
            //try
            //{
            // Use this to generate a dummy divide by zero exception error so that we can figure out
            // how to step through the code when an exception is thrown. Right now we just get a message
            // box using the try/catch approach, or just a red X, without the try/catch.
            if (ExceptionGeneratorEnabled)
            {
                int a = 0;
                int b = 0;
                int c = a / b;
            }

            // some lame reason our width and height arent set during our initialize so we have to load it here...
            if (camera.AspectRatio == 0)
            {
                camera.AspectRatio = (float)Width / (float)Height;
            }

            // always clear to dark gray
            GraphicsDevice.Clear(Color.DimGray);

            input.Update();

            if (Focused)
            {
                // show focus Indicator
                modelViewBorder.BorderBrush = System.Windows.Media.Brushes.Goldenrod;

                // make sure our mouse is within our bounds
                if (input.X > 0 && input.X < Width && input.Y > 0 && input.Y < Height)
                {

                    if (input.LeftDown)
                    {
                        Vector2 mouse = input.MouseDelta;
                        if (mouse != Vector2.Zero)
                        {
                            camera.Yaw -= mouse.X * 0.02f;
                            camera.Pitch -= mouse.Y * 0.02f;

                            // be sure to stay focused if we click
                            //if (!Focused)
                            //    Focus();
                        }
                    }

                    // might not work if where not focused
                    if (input.MouseWheelDelta != 0)
                    {
                        float delta = input.MouseWheelDelta / 120; // 120 is 1 mouse wheel 
                        camera.Distance -= delta * radius * 0.1f;
                    }
                }
            }
            else
                modelViewBorder.BorderBrush = System.Windows.Media.Brushes.DimGray;

            camera.Update();

            //drawing 2D graphics modifies render state, reset that to default
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            // draw the world axes first
            worldAxes.Draw(GraphicsDevice, camera);

            // then draw the grid
            gridXY.Draw();

            if (model != null)
            {
                if (shadingType == ShadingType.Wireframe)
                    GraphicsDevice.RasterizerState = rsWireframe;
                else GraphicsDevice.RasterizerState = rsSolid;

                // Draw the model.
                int i = 0, n = 0;
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        if (shadingType == ShadingType.Wireframe)
                        {
                            //by default wireframe sets to diffuse color which is hard to see
                            //so I set it to white
                            effect.TextureEnabled = false;

                            if (selectedMesh != null && selectedMesh == mesh)
                                effect.DiffuseColor = Color.Yellow.ToVector3();
                            else
                                effect.DiffuseColor = new Vector3(1, 1, 1);
                        }
                        else
                        {
                            //restore original colors
                            effect.TextureEnabled = true;

                            if (selectedMesh != null && selectedMesh == mesh)
                                effect.DiffuseColor = Color.Yellow.ToVector3();
                            else
                                effect.DiffuseColor = diffuseColors[i][n++];
                        }

                        effect.World = mesh.ParentBone.Transform * Matrix.Identity;
                        effect.View = camera.View;
                        effect.Projection = camera.Projection;

                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;

                        if (selectedMesh != null && selectedMesh == mesh)
                            effect.AmbientLightColor = Color.Yellow.ToVector3();
                        else
                            effect.AmbientLightColor = ambientColor.ToVector3();
                    }

                    mesh.Draw();

                    i++;
                    n = 0;
                }
                BoundingBoxRenderer.Instance.Draw(camera);

                BoundingSphereRenderer.Instance.Render(camera);
            }

            ReferenceAxis.Instance.Draw(camera);
            //draw text

            ViewModeLabel.Instance.Draw(spriteBatch);
            Message.Instance.Draw(spriteBatch);    
            /*}
            catch (Exception exc)
            {
               errorTextBox.Text = exc.Message;
            }*/


            // this is already set before rendering 3d
            //drawing 2D graphics modifies render state, reset that to default
            //GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        public void BeginLoading ()
        {
            Message.Instance.BeginLoading();
            isLoading = true;
        }

        public void LoadModel (Model model)
        {
            this.model = model;
            if (model != null)
            {
                GetModelDiffuseColors(model);
                BoundingBox bb = GetBoundingBox(model);
                radius = 0;
                if (Math.Abs(bb.Min.X) > radius) radius = Math.Abs(bb.Min.X);
                if (Math.Abs(bb.Min.Y) > radius) radius = Math.Abs(bb.Min.Y);
                if (Math.Abs(bb.Min.Z) > radius) radius = Math.Abs(bb.Min.Z);
                if (Math.Abs(bb.Max.X) > radius) radius = Math.Abs(bb.Max.X);
                if (Math.Abs(bb.Max.Y) > radius) radius = Math.Abs(bb.Max.Y);
                if (Math.Abs(bb.Max.Z) > radius) radius = Math.Abs(bb.Max.Z);

                BoundingBoxRenderer.Instance.Initialize(GraphicsDevice, bb);

                // Get Bounding sphere radius from bounding box needs to be transformed by parent bone
                //BoundingSphereRenderer.Instance.Initialize(GraphicsDevice,  new BoundingSphere(new Vector3(0, radius / 2, 0), radius / 2));
                
                // Get Bounding Sphere based on bounding box 
                BoundingSphereRenderer.Instance.Initialize(GraphicsDevice, getBoundingSphere(bb, radius));

                camera.CameraTarget = boundingSphere.Center;

                // sets the grid scale
                gridXY.GridScale = (int)(Math.Sqrt(Math.Pow(radius, 2)
                    - Math.Pow(boundingSphere.Center.Y, 2)) * 1.33);

                Message.Instance.Hide();
            }
            ResetView();
        }

        public void ZoomToModelExtents ()
        {
            if (model == null) return;
            float angle = camera.FieldOfView / 2;
            float opposite = radius + radius * (float)Math.Tan(angle);
            float dist = opposite / (float)Math.Tan(angle);
            camera.Distance = dist;
        }

        public void EndLoading ()
        {
            Message.Instance.EndLoading();
            isLoading = false;
        }

        BoundingBox GetBoundingBox (Model model)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), mesh.ParentBone.Transform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            // Create and return bounding box
            return new BoundingBox(min, max);
        }

        BoundingSphere getBoundingSphere(BoundingBox box, float diameter)
        {
            // Create the bounding sphere from the bounding box and transforms
            boundingSphere = BoundingSphere.CreateFromBoundingBox(box);

            
            // Reset radius to longest length of box / 2
            //sphere.Radius = diameter / 2;

            return boundingSphere;
        }

        //store original model properties
        private void GetModelDiffuseColors (Model model)
        {
            diffuseColors = new Vector3[model.Meshes.Count][];
            for (int i = 0; i < model.Meshes.Count; i++)
            {
                diffuseColors[i] = new Vector3[model.Meshes[i].Effects.Count];
                for (int n = 0; n < model.Meshes[i].Effects.Count; n++)
                {
                    diffuseColors[i][n] = ((BasicEffect)model.Meshes[i].Effects[n]).DiffuseColor;
                }
            }
        }

        public void AmbientPower (float power)
        {
            ambientColor = Color.Lerp(Color.Black, Color.White, power);
        }

        public void SetFov (float fov)
        {
            camera.FieldOfView = fov;
        }

        public void SetSelectedMesh (string name)
        {
            if (name != null)
                selectedMesh = model.Meshes[name];
            else
                selectedMesh = null;
        }

        protected override void OnResize (EventArgs e)
        {
            base.OnResize(e);

            camera.AspectRatio = (float)Width / (float)Height;
        }

        // note: the largest radius doesn't mean the farthest distance from the origin.
        // I would suggest using the center point and radius to determine farthest distance
        // then using that along with the width, height and fov to determine the appropriate distance
        // not sure of any specific calculation to accomplish this but maybe someone has done this before
        public void ResetView ()
        {
            ZoomToModelExtents();
        }
    }
}
