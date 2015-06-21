using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SSGL.Core;
using SSGL.Helper;
using SSGL.Helper.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Entity.UI
{
    public class Selector : BaseUI
    {
        public bool Active { get; set; }
        public Vector3 Selected { get; set; }

        private VertexPositionNormalTexture[] _vertices;
        private Color _color;

        private VertexPositionColor[] _selecedTile =
        {
            new VertexPositionColor(Vector3.Zero, new Color(GameDirector.Assets.Colors[Misc.DEBUG], 0.002f)),
            new VertexPositionColor(Vector3.Zero, new Color(GameDirector.Assets.Colors[Misc.DEBUG], 0.002f)),
            new VertexPositionColor(Vector3.Zero, new Color(GameDirector.Assets.Colors[Misc.DEBUG], 0.002f)),
            new VertexPositionColor(Vector3.Zero, new Color(GameDirector.Assets.Colors[Misc.DEBUG], 0.002f)),
            new VertexPositionColor(Vector3.Zero, new Color(GameDirector.Assets.Colors[Misc.DEBUG], 0.002f)),
            new VertexPositionColor(Vector3.Zero, new Color(GameDirector.Assets.Colors[Misc.DEBUG], 0.002f))
        };
        // Effect and vertex declaration for drawing the picked triangle.
        private BasicEffect _lineEffect;

        public Selector(VertexPositionNormalTexture[] selectionArea, Color? highlightColor = null)
        {
            Active = false;
            
            _lineEffect = new BasicEffect(GameDirector.Device);
            _lineEffect.VertexColorEnabled = true;
            _vertices = selectionArea;
            _color = highlightColor ?? Color.Red;

        }

        public override void Update(GameTime gameTime)
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                Active = !Active;
            }

            if (Active)
            {
                Highlight(this._vertices);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Active)
            {
                DrawHighlight();
            }
            base.Draw(gameTime);
        }

        private bool Highlight(VertexPositionNormalTexture[] vertices)
        {
            Ray ray = Util.CalculateCursorRay(GameDirector.Device, GameDirector.Camera.Projection, GameDirector.Camera.View);
            // Keep track of the closest triangle we found so far,
            // so we can always return the closest one.
            float? closestIntersection = null;
            for (int i = 0; i < vertices.Length; i += 3)
            {
                // Perform a ray to triangle intersection test.
                float? intersection;

                Util.RayIntersectsTriangle(ref ray,
                                      ref vertices[i].Position,
                                      ref vertices[i + 1].Position,
                                      ref vertices[i + 2].Position,
                                      out intersection);

                // Does the ray intersect this triangle?
                if (intersection != null)
                {
                    // If so, is it closer than any other previous triangle?
                    if ((closestIntersection == null) || (intersection < closestIntersection))
                    {
                        // Store the distance to this triangle.
                        closestIntersection = intersection;

                        this._selecedTile[0] = new VertexPositionColor(vertices[i].Position, this._color);
                        this._selecedTile[1] = new VertexPositionColor(vertices[i + 1].Position, this._color);
                        this._selecedTile[2] = new VertexPositionColor(vertices[i + 2].Position, this._color);
                        if (!Vector3.Equals(vertices[i].Position, vertices[i - 1].Position))
                        {
                            this._selecedTile[3] = new VertexPositionColor(vertices[i + 3].Position, this._color);
                            this._selecedTile[4] = new VertexPositionColor(vertices[i + 4].Position, this._color);
                            this._selecedTile[5] = new VertexPositionColor(vertices[i + 5].Position, this._color);
                        }
                        else
                        {
                            this._selecedTile[3] = new VertexPositionColor(vertices[i - 3].Position, this._color);
                            this._selecedTile[4] = new VertexPositionColor(vertices[i - 2].Position, this._color);
                            this._selecedTile[5] = new VertexPositionColor(vertices[i - 1].Position, this._color);
                        }
                        Selected = vertices[i].Position;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Helper for drawing the outline of the triangle currently under the cursor.
        /// </summary>
        private void DrawHighlight()
        {
            if (this._selecedTile[0].Position != Vector3.Zero)
            {
                // Set line drawing renderstates. We disable backface culling
                // and turn off the depth buffer because we want to be able to
                // see the picked triangle outline regardless of which way it is
                // facing, and even if there is other geometry in front of it.
                GameDirector.Device.DepthStencilState = DepthStencilState.None;

                // Activate the line drawing BasicEffect.
                _lineEffect.Projection = GameDirector.Camera.Projection;
                _lineEffect.View = GameDirector.Camera.View;
                _lineEffect.CurrentTechnique.Passes[0].Apply();

                // Draw the triangle.
                GameDirector.Device.DrawUserPrimitives(PrimitiveType.TriangleList, this._selecedTile, 0, 2);
                // Reset renderstates to their default values.
                GameDirector.Device.RasterizerState = RasterizerState.CullCounterClockwise;
                GameDirector.Device.DepthStencilState = DepthStencilState.Default;
            }
        }
    }
}
