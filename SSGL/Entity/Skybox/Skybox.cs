using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SSGL.Core;
using SSGL.Helper.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Entity.Skybox
{
    public class Skybox
    {
        public VertexBuffer Buffer { get; set; }
        public List<Vector3> Positions { get; set; }

        protected GraphicsDevice _device;
        protected List<BasicEffect> _effects;
        protected List<List<VertexPositionNormalTexture>> _sides { get; set; }
        protected DepthStencilState _depthState;
        protected Matrix _worldTranslation = Matrix.Identity;
        protected Matrix _worldRotation = Matrix.Identity;
        protected Matrix _worldScale = Matrix.Identity;
        protected float _size;
        protected Camera _camera;

        public Skybox(GraphicsDevice device, Camera camera, List<Texture2D> textures = null)
        {
            this._device = device;
            this._effects = new List<BasicEffect>();
            this._sides = new List<List<VertexPositionNormalTexture>>();
            this._size = camera.Far;
            this._camera = camera;

            BasicEffect effect;
            if (textures != null)
            {
                foreach (Texture2D texture in textures)
                {
                    effect = new BasicEffect(device);
                    effect.Texture = texture;
                    effect.TextureEnabled = true;
                    this._effects.Add(effect);

                }
            }
            else
            {
                effect = new BasicEffect(device);
                effect.Texture = GameDirector.Assets.Textures[MapTerrain.DEBUG];
                effect.TextureEnabled = true;
                this._effects.Add(effect);
            }

            Positions = new List<Vector3>();

            _depthState = new DepthStencilState();
            _depthState.DepthBufferEnable = true; /* Enable the depth buffer */
            _depthState.DepthBufferWriteEnable = true; /* When drawing to the screen, write to the depth buffer */

            //Base Positions(centered on origin)
            Positions = new List<Vector3>();
            //FRONT
            //top left
            Positions.Add(new Vector3(-0.5f, 0.5f, 0.5f));
            //top right
            Positions.Add(new Vector3(0.5f, 0.5f, 0.5f));
            //bottom right
            Positions.Add(new Vector3(0.5f, -0.5f, 0.5f));
            //bottom left
            Positions.Add(new Vector3(-0.5f, -0.5f, 0.5f));

            //BACK
            //top left
            Positions.Add(new Vector3(-0.5f, 0.5f, -0.5f));
            //top right
            Positions.Add(new Vector3(0.5f, 0.5f, -0.5f));
            //bottom right
            Positions.Add(new Vector3(0.5f, -0.5f, -0.5f));
            //bottom left
            Positions.Add(new Vector3(-0.5f, -0.5f, -0.5f));

            this.Load();
        }

        public void Load()
        {
            List<VertexPositionNormalTexture> front = new List<VertexPositionNormalTexture>();
            List<VertexPositionNormalTexture> back = new List<VertexPositionNormalTexture>();
            List<VertexPositionNormalTexture> left = new List<VertexPositionNormalTexture>();
            List<VertexPositionNormalTexture> right = new List<VertexPositionNormalTexture>();
            List<VertexPositionNormalTexture> top = new List<VertexPositionNormalTexture>();
            List<VertexPositionNormalTexture> bottom = new List<VertexPositionNormalTexture>();

            //FRONT
            front.Add(new VertexPositionNormalTexture(Positions[1], Vector3.Backward, new Vector2(0, 0)));
            front.Add(new VertexPositionNormalTexture(Positions[0], Vector3.Backward, new Vector2(1, 0)));
            front.Add(new VertexPositionNormalTexture(Positions[3], Vector3.Backward, new Vector2(1, 1)));

            front.Add(new VertexPositionNormalTexture(Positions[3], Vector3.Backward, new Vector2(1, 1)));
            front.Add(new VertexPositionNormalTexture(Positions[2], Vector3.Backward, new Vector2(0, 1)));
            front.Add(new VertexPositionNormalTexture(Positions[1], Vector3.Backward, new Vector2(0, 0)));

            //BACK
            back.Add(new VertexPositionNormalTexture(Positions[4], Vector3.Forward, new Vector2(0, 0)));
            back.Add(new VertexPositionNormalTexture(Positions[5], Vector3.Forward, new Vector2(1, 0)));
            back.Add(new VertexPositionNormalTexture(Positions[6], Vector3.Forward, new Vector2(1, 1)));

            back.Add(new VertexPositionNormalTexture(Positions[6], Vector3.Forward, new Vector2(1, 1)));
            back.Add(new VertexPositionNormalTexture(Positions[7], Vector3.Forward, new Vector2(0, 1)));
            back.Add(new VertexPositionNormalTexture(Positions[4], Vector3.Forward, new Vector2(0, 0)));

            //LEFT
            left.Add(new VertexPositionNormalTexture(Positions[0], Vector3.Left, new Vector2(0, 0)));
            left.Add(new VertexPositionNormalTexture(Positions[4], Vector3.Left, new Vector2(1, 0)));
            left.Add(new VertexPositionNormalTexture(Positions[7], Vector3.Left, new Vector2(1, 1)));

            left.Add(new VertexPositionNormalTexture(Positions[7], Vector3.Left, new Vector2(1, 1)));
            left.Add(new VertexPositionNormalTexture(Positions[3], Vector3.Left, new Vector2(0, 1)));
            left.Add(new VertexPositionNormalTexture(Positions[0], Vector3.Left, new Vector2(0, 0)));

            //RIGHT
            right.Add(new VertexPositionNormalTexture(Positions[5], Vector3.Right, new Vector2(0, 0)));
            right.Add(new VertexPositionNormalTexture(Positions[1], Vector3.Right, new Vector2(1, 0)));
            right.Add(new VertexPositionNormalTexture(Positions[2], Vector3.Right, new Vector2(1, 1)));

            right.Add(new VertexPositionNormalTexture(Positions[2], Vector3.Right, new Vector2(1, 1)));
            right.Add(new VertexPositionNormalTexture(Positions[6], Vector3.Right, new Vector2(0, 1)));
            right.Add(new VertexPositionNormalTexture(Positions[5], Vector3.Right, new Vector2(0, 0)));

            //TOP
            top.Add(new VertexPositionNormalTexture(Positions[5], Vector3.Up, new Vector2(0, 0)));
            top.Add(new VertexPositionNormalTexture(Positions[4], Vector3.Up, new Vector2(1, 0)));
            top.Add(new VertexPositionNormalTexture(Positions[0], Vector3.Up, new Vector2(1, 1)));

            top.Add(new VertexPositionNormalTexture(Positions[0], Vector3.Up, new Vector2(1, 1)));
            top.Add(new VertexPositionNormalTexture(Positions[1], Vector3.Up, new Vector2(0, 1)));
            top.Add(new VertexPositionNormalTexture(Positions[5], Vector3.Up, new Vector2(0, 0)));

            //BOTTOM
            bottom.Add(new VertexPositionNormalTexture(Positions[2], Vector3.Down, new Vector2(0, 0)));
            bottom.Add(new VertexPositionNormalTexture(Positions[3], Vector3.Down, new Vector2(1, 0)));
            bottom.Add(new VertexPositionNormalTexture(Positions[7], Vector3.Down, new Vector2(1, 1)));

            bottom.Add(new VertexPositionNormalTexture(Positions[7], Vector3.Down, new Vector2(1, 1)));
            bottom.Add(new VertexPositionNormalTexture(Positions[6], Vector3.Down, new Vector2(0, 1)));
            bottom.Add(new VertexPositionNormalTexture(Positions[2], Vector3.Down, new Vector2(0, 0)));

            this._sides.Add(front);
            this._sides.Add(back);
            this._sides.Add(left);
            this._sides.Add(right);
            this._sides.Add(top);
            this._sides.Add(bottom);

            this._worldTranslation = Matrix.CreateTranslation(this._camera.Position);
            this._worldScale = Matrix.CreateScale(this._size);
        }

        public void Update(GameTime gameTime, Camera camera)
        {
            this._worldTranslation = Matrix.CreateTranslation(this._camera.Position);
            this._worldScale = Matrix.CreateScale(this._size);
        }

        public void Draw(GameTime gameTime, Camera camera)
        {
            for (int i = 0; i < this._sides.Count; i++)
            {
                this._effects[i].World = this._worldScale * this._worldRotation * this._worldTranslation;
                this._effects[i].View = camera.View;
                this._effects[i].Projection = camera.Projection;

                RasterizerState rasterizerState = new RasterizerState();
                //rasterizerState.CullMode = CullMode.None;
                this._device.RasterizerState = rasterizerState;

                foreach (EffectPass pass in this._effects[i].CurrentTechnique.Passes)
                {
                    pass.Apply();
                    using (Buffer = new VertexBuffer(this._device, typeof(VertexPositionNormalTexture), this._sides[i].Count, BufferUsage.WriteOnly))
                    {
                        //load buffer
                        Buffer.SetData<VertexPositionNormalTexture>(this._sides[i].ToArray());

                        //sendt vertex buffer to device
                        this._device.SetVertexBuffer(Buffer);
                        //use depth buffer when drawing a shape
                        this._device.DepthStencilState = this._depthState;

                        // Draw the primitives from the vertex buffer to the device as triangles
                        this._device.DrawPrimitives(PrimitiveType.TriangleList, 0, this._sides[i].Count / 3);
                    }
                }
            }
        }
    }
}
