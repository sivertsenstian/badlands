using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SSGL.Core;
using SSGL.Entity.Actor;
using SSGL.Helper.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Entity.Skybox
{
    public class Skybox : BaseActor
    {
        private List<Vector3> _position { get; set; }
        private List<List<VertexPositionNormalTexture>> _sides { get; set; }
        private float _size;

        public Skybox(List<Texture2D> textures = null)
        {
            this._effects = new List<BasicEffect>();
            this._sides = new List<List<VertexPositionNormalTexture>>();
            this._size = GameDirector.Camera.Far * 1.15f;

            BasicEffect effect;
            if (textures != null)
            {
                foreach (Texture2D texture in textures)
                {
                    effect = new BasicEffect(GameDirector.Device);
                    effect.Texture = texture;
                    effect.TextureEnabled = true;
                    this._effects.Add(effect);

                }
            }
            else
            {
                effect = new BasicEffect(GameDirector.Device);
                effect.Texture = GameDirector.Assets.Textures[Misc.DEBUG];
                effect.TextureEnabled = true;
                this._effects.Add(effect);
            }

            //Base Positions(centered on origin)
            _position = new List<Vector3>();
            //FRONT
            //top left
            _position.Add(new Vector3(-0.5f, 0.5f, 0.5f));
            //top right
            _position.Add(new Vector3(0.5f, 0.5f, 0.5f));
            //bottom right
            _position.Add(new Vector3(0.5f, -0.5f, 0.5f));
            //bottom left
            _position.Add(new Vector3(-0.5f, -0.5f, 0.5f));

            //BACK
            //top left
            _position.Add(new Vector3(-0.5f, 0.5f, -0.5f));
            //top right
            _position.Add(new Vector3(0.5f, 0.5f, -0.5f));
            //bottom right
            _position.Add(new Vector3(0.5f, -0.5f, -0.5f));
            //bottom left
            _position.Add(new Vector3(-0.5f, -0.5f, -0.5f));

            this.Load();
        }

        public override void Load()
        {
            List<VertexPositionNormalTexture> front = new List<VertexPositionNormalTexture>();
            List<VertexPositionNormalTexture> back = new List<VertexPositionNormalTexture>();
            List<VertexPositionNormalTexture> left = new List<VertexPositionNormalTexture>();
            List<VertexPositionNormalTexture> right = new List<VertexPositionNormalTexture>();
            List<VertexPositionNormalTexture> top = new List<VertexPositionNormalTexture>();
            List<VertexPositionNormalTexture> bottom = new List<VertexPositionNormalTexture>();

            //FRONT
            front.Add(new VertexPositionNormalTexture(_position[1], Vector3.Backward, new Vector2(0, 0)));
            front.Add(new VertexPositionNormalTexture(_position[0], Vector3.Backward, new Vector2(1, 0)));
            front.Add(new VertexPositionNormalTexture(_position[3], Vector3.Backward, new Vector2(1, 1)));

            front.Add(new VertexPositionNormalTexture(_position[3], Vector3.Backward, new Vector2(1, 1)));
            front.Add(new VertexPositionNormalTexture(_position[2], Vector3.Backward, new Vector2(0, 1)));
            front.Add(new VertexPositionNormalTexture(_position[1], Vector3.Backward, new Vector2(0, 0)));

            //BACK
            back.Add(new VertexPositionNormalTexture(_position[4], Vector3.Forward, new Vector2(0, 0)));
            back.Add(new VertexPositionNormalTexture(_position[5], Vector3.Forward, new Vector2(1, 0)));
            back.Add(new VertexPositionNormalTexture(_position[6], Vector3.Forward, new Vector2(1, 1)));

            back.Add(new VertexPositionNormalTexture(_position[6], Vector3.Forward, new Vector2(1, 1)));
            back.Add(new VertexPositionNormalTexture(_position[7], Vector3.Forward, new Vector2(0, 1)));
            back.Add(new VertexPositionNormalTexture(_position[4], Vector3.Forward, new Vector2(0, 0)));

            //LEFT
            left.Add(new VertexPositionNormalTexture(_position[0], Vector3.Left, new Vector2(0, 0)));
            left.Add(new VertexPositionNormalTexture(_position[4], Vector3.Left, new Vector2(1, 0)));
            left.Add(new VertexPositionNormalTexture(_position[7], Vector3.Left, new Vector2(1, 1)));

            left.Add(new VertexPositionNormalTexture(_position[7], Vector3.Left, new Vector2(1, 1)));
            left.Add(new VertexPositionNormalTexture(_position[3], Vector3.Left, new Vector2(0, 1)));
            left.Add(new VertexPositionNormalTexture(_position[0], Vector3.Left, new Vector2(0, 0)));

            //RIGHT
            right.Add(new VertexPositionNormalTexture(_position[5], Vector3.Right, new Vector2(0, 0)));
            right.Add(new VertexPositionNormalTexture(_position[1], Vector3.Right, new Vector2(1, 0)));
            right.Add(new VertexPositionNormalTexture(_position[2], Vector3.Right, new Vector2(1, 1)));

            right.Add(new VertexPositionNormalTexture(_position[2], Vector3.Right, new Vector2(1, 1)));
            right.Add(new VertexPositionNormalTexture(_position[6], Vector3.Right, new Vector2(0, 1)));
            right.Add(new VertexPositionNormalTexture(_position[5], Vector3.Right, new Vector2(0, 0)));

            //TOP
            top.Add(new VertexPositionNormalTexture(_position[5], Vector3.Up, new Vector2(0, 0)));
            top.Add(new VertexPositionNormalTexture(_position[4], Vector3.Up, new Vector2(1, 0)));
            top.Add(new VertexPositionNormalTexture(_position[0], Vector3.Up, new Vector2(1, 1)));

            top.Add(new VertexPositionNormalTexture(_position[0], Vector3.Up, new Vector2(1, 1)));
            top.Add(new VertexPositionNormalTexture(_position[1], Vector3.Up, new Vector2(0, 1)));
            top.Add(new VertexPositionNormalTexture(_position[5], Vector3.Up, new Vector2(0, 0)));

            //BOTTOM
            bottom.Add(new VertexPositionNormalTexture(_position[2], Vector3.Down, new Vector2(0, 0)));
            bottom.Add(new VertexPositionNormalTexture(_position[3], Vector3.Down, new Vector2(1, 0)));
            bottom.Add(new VertexPositionNormalTexture(_position[7], Vector3.Down, new Vector2(1, 1)));

            bottom.Add(new VertexPositionNormalTexture(_position[7], Vector3.Down, new Vector2(1, 1)));
            bottom.Add(new VertexPositionNormalTexture(_position[6], Vector3.Down, new Vector2(0, 1)));
            bottom.Add(new VertexPositionNormalTexture(_position[2], Vector3.Down, new Vector2(0, 0)));

            this._sides.Add(front);
            this._sides.Add(back);
            this._sides.Add(left);
            this._sides.Add(right);
            this._sides.Add(top);
            this._sides.Add(bottom);

            this._worldTranslation = Matrix.CreateTranslation(GameDirector.Camera.Position);
            this._worldScale = Matrix.CreateScale(this._size);

        }

        public override void Update(GameTime gameTime)
        {
            this._worldTranslation = Matrix.CreateTranslation(GameDirector.Camera.Position);
            this._worldScale = Matrix.CreateScale(this._size);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            BasicEffect currentEffect;
            for (int i = 0; i < this._sides.Count; i++)
            {
                currentEffect = this._effects.Count == this._sides.Count ? this._effects[i] : this._effects[0];
                currentEffect.World = this._worldScale * this._worldRotation * this._worldTranslation;
                currentEffect.View = GameDirector.Camera.View;
                currentEffect.Projection = GameDirector.Camera.Projection;
                //Razterization
                GameDirector.Device.RasterizerState = this._rasterizerState;

                foreach (EffectPass pass in currentEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    using (Buffer = new VertexBuffer(GameDirector.Device, typeof(VertexPositionNormalTexture), this._sides[i].Count, BufferUsage.WriteOnly))
                    {
                        //load buffer
                        Buffer.SetData<VertexPositionNormalTexture>(this._sides[i].ToArray());
                        //sendt vertex buffer to device
                        GameDirector.Device.SetVertexBuffer(Buffer);
                        //use depth buffer when drawing a shape
                        GameDirector.Device.DepthStencilState = this._depthState;
                        // Draw the primitives from the vertex buffer to the device as triangles
                        GameDirector.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, this._sides[i].Count / 3);
                    }
                }
            }
        }
    }
}
