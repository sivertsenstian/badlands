using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SSGL.Component;
using SSGL.Core;
using SSGL.Helper;
using SSGL.Helper.Enum;
using SSGL.Helper.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Entity.Actor
{
    public class BaseActor : IEntity, IActor
    {
        protected Dictionary<string, BaseComponent> Components { get; set; }
        protected DepthStencilState _depthState;
        protected RasterizerState _rasterizerState;
        protected List<BasicEffect> _effects;
        protected Matrix _worldTranslation = Matrix.Identity;
        protected Matrix _worldRotation = Matrix.Identity;
        protected Matrix _worldScale = Matrix.Identity;
        protected VertexBuffer Buffer { get; set; }
        

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<VertexPositionNormalTexture> Vertices { get; set; }

        public BaseActor(string name = "UnnamedActor")
        {
            Name = name;
            Id = Guid.NewGuid();
            Vertices = new List<VertexPositionNormalTexture>();

            _depthState = new DepthStencilState();
            _depthState.DepthBufferEnable = true; /* Enable the depth buffer */
            _depthState.DepthBufferWriteEnable = true; /* When drawing to the screen, write to the depth buffer */

            _rasterizerState = new RasterizerState();
            //_rasterizerState.CullMode = CullMode.None;

            _effects = new List<BasicEffect>();
        }
        public virtual void Init(){

        }

        public virtual void Load()
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime)
        {
            if (Vertices.Count > 0)
            {
                BasicEffect currentEffect;
                for (int i = 0; i < this._effects.Count; i++)
                {
                    currentEffect = this._effects[i];
                    //Set up the base effect
                    currentEffect.World = this._worldScale * this._worldRotation * this._worldTranslation;
                    currentEffect.View = GameDirector.Camera.View;
                    currentEffect.Projection = GameDirector.Camera.Projection;
                    //Set up the base fog
                    currentEffect.FogEnabled = true;
                    currentEffect.FogStart = GameDirector.Camera.Far / 2;
                    currentEffect.FogEnd = GameDirector.Camera.Far;
                    currentEffect.FogColor = GameDirector.Assets.Colors[Misc.DEBUG].ToVector3();
                    //Razterization
                    GameDirector.Device.RasterizerState = this._rasterizerState;

                    foreach (EffectPass pass in currentEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        using (Buffer = new VertexBuffer(GameDirector.Device, typeof(VertexPositionNormalTexture), Vertices.Count, BufferUsage.WriteOnly))
                        {
                            //Load buffer
                            Buffer.SetData<VertexPositionNormalTexture>(Vertices.ToArray());
                            //Send Vertexbuffer to device
                            GameDirector.Device.SetVertexBuffer(Buffer);
                            //Use depthbuffer when drawing a shape
                            GameDirector.Device.DepthStencilState = this._depthState;
                            // Draw the primitives from the vertex buffer to the device as triangles
                            GameDirector.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, Vertices.Count / 3);
                        }
                    }
                }
            }

        }

        public virtual void Dispose()
        {

        }

        public virtual void Broadcast(Message message)
        {
            foreach (KeyValuePair<string, BaseComponent> component in this.Components)
            {
                component.Value.Send(message);
            }
        }

        public BaseComponent GetComponent(string name)
        {
            throw new NotImplementedException();
        }
    }
}
