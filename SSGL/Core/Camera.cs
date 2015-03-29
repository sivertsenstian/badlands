using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Core
{
    public class Camera : GameComponent
    {
        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }
        public float Far { get; protected set; }
        public float Near { get; protected set; }
        public Vector3 Position { get; protected set; }
        
        private Vector3 _direction;
        private Vector3 _up;
        private float _speed;
        private MouseState _previousMouseState;

        public Camera(Game game, Vector3 position, Vector3 target, Vector3 up, float near = 1.0f, float far = 1500f)
            : base(game)
        {
            Far = far;
            Near = near;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)(Game.Window.ClientBounds.Width / Game.Window.ClientBounds.Height), Near, Far);
            Position = position;
            this._direction = target - position;
            this._direction.Normalize();
            this._up = up;

            //Pitch camera a bit to get a more top-down view
            this._direction = Vector3.Transform(this._direction, Matrix.CreateFromAxisAngle(Vector3.Cross(this._up, this._direction), (MathHelper.PiOver4 / 100) * 75));
            this._up = Vector3.Transform(this._up, Matrix.CreateFromAxisAngle(Vector3.Cross(this._up, this._direction), (MathHelper.PiOver4 / 100) * 75));

            this._speed = 20.0f;
            this.CreateLookAt();
        }

        public override void Initialize()
        {
            // Set mouse position and do initial get state
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
            this._previousMouseState = Mouse.GetState();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //Keyboard
            // Move side to side
            if (Keyboard.GetState( ).IsKeyDown(Keys.A))
            {
                Vector3 diff = Vector3.Cross(this._up, this._direction) * this._speed;
                diff.Y = 0;
                Position += diff;
            }
            if (Keyboard.GetState( ).IsKeyDown(Keys.D))
            {
                Vector3 diff = Vector3.Cross(this._up, this._direction) * this._speed;
                diff.Y = 0;
                Position -= diff;
            }
            // Move forward/backward
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Vector3 diff = Vector3.Cross(Vector3.Cross(this._up, this._direction), this._up) * this._speed;
                diff.Y = 0;
                Position += diff;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Vector3 diff = Vector3.Cross(Vector3.Cross(this._up, this._direction), this._up) * this._speed;
                diff.Y = 0;
                Position -= diff;
            }

            // Yaw rotation
            if (Keyboard.GetState().IsKeyDown(Keys.E))
                this._direction = Vector3.Transform(this._direction, Matrix.CreateFromAxisAngle(this._up, (-MathHelper.PiOver4 / 180) * this._speed / 5));
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                this._direction = Vector3.Transform(this._direction, Matrix.CreateFromAxisAngle(this._up, (MathHelper.PiOver4 / 180) * this._speed / 5));

            //Mouse
            // Zoom Out/In
            if (Mouse.GetState().ScrollWheelValue > this._previousMouseState.ScrollWheelValue)
                Position += this._direction * this._speed * 5;
            if (Mouse.GetState().ScrollWheelValue < this._previousMouseState.ScrollWheelValue)
                Position -= this._direction * this._speed * 5;
            
            
            if (Mouse.GetState( ).RightButton == ButtonState.Pressed)
            {

            }

            if(Mouse.GetState( ).MiddleButton == ButtonState.Pressed){
                //Pitch
                this._direction = Vector3.Transform(this._direction, Matrix.CreateFromAxisAngle(Vector3.Cross(this._up, this._direction), (MathHelper.PiOver4 / 100) * (Mouse.GetState().Y - this._previousMouseState.Y)));
                this._up = Vector3.Transform(this._up, Matrix.CreateFromAxisAngle(Vector3.Cross(this._up, this._direction), (MathHelper.PiOver4 / 100) * (Mouse.GetState().Y - this._previousMouseState.Y)));

                //Yaw
                this._direction = Vector3.Transform(this._direction,Matrix.CreateFromAxisAngle(this._up, (-MathHelper.PiOver4 / 150) * (Mouse.GetState( ).X - this._previousMouseState.X)));
            }
            else
            {
                //Edge movement
                if (Mouse.GetState().Y < (Game.Window.ClientBounds.Height * 0.05))
                {
                    Vector3 diff = Vector3.Cross(Vector3.Cross(this._up, this._direction), this._up) * this._speed;
                    diff.Y = 0;
                    Position += diff;
                }
                if (Mouse.GetState().Y > (Game.Window.ClientBounds.Height * 0.95))
                {
                    Vector3 diff = Vector3.Cross(Vector3.Cross(this._up, this._direction), this._up) * this._speed;
                    diff.Y = 0;
                    Position -= diff;
                }
                if (Mouse.GetState().X < (Game.Window.ClientBounds.Width * 0.05))
                    Position += Vector3.Cross(this._up, this._direction) * this._speed;
                if (Mouse.GetState().X > (Game.Window.ClientBounds.Width * 0.95))
                    Position -= Vector3.Cross(this._up, this._direction) * this._speed;
            }

            // Reset prevMouseState
            this._previousMouseState = Mouse.GetState( );

            //Recreate camera-view matrix
            this.CreateLookAt();

            base.Update(gameTime);
        }

        //Shoots a ray inwards in the scene from the mouse location
        public Ray CalculateRay(Vector2 mouseLocation, Matrix view, Matrix projection, Viewport viewport)
        {
            Vector3 nearPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                    mouseLocation.Y, 0.0f),
                    projection,
                    view,
                    Matrix.Identity);

            Vector3 farPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                    mouseLocation.Y, 1.0f),
                    projection,
                    view,
                    Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }


        // CalculateCursorRay Calculates a world space ray starting at the camera's
        // "eye" and pointing in the direction of the cursor. Viewport.Unproject is used
        // to accomplish this. see the accompanying documentation for more explanation
        // of the math behind this function.
        public Ray CalculateCursorRay(Matrix projectionMatrix, Matrix viewMatrix)
        {
            int x = Mouse.GetState().X;
            int y = Mouse.GetState().Y;
            // create 2 positions in screenspace using the cursor position. 0 is as
            // close as possible to the camera, 1 is as far away as possible.
            Vector3 nearSource = new Vector3(x, y, 0f);
            Vector3 farSource = new Vector3(x, y, 1f);

            // use Viewport.Unproject to tell what those two screen space positions
            // would be in world space. we'll need the projection matrix and view
            // matrix, which we have saved as member variables. We also need a world
            // matrix, which can just be identity.
            Vector3 nearPoint = Game.GraphicsDevice.Viewport.Unproject(nearSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            Vector3 farPoint = Game.GraphicsDevice.Viewport.Unproject(farSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            // find the direction vector that goes from the nearPoint to the farPoint
            // and normalize it....
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // and then create a new ray using nearPoint as the source.
            return new Ray(nearPoint, direction);
        }

         /// Checks whether a ray intersects a triangle. This uses the algorithm
        /// developed by Tomas Moller and Ben Trumbore, which was published in the
        /// Journal of Graphics Tools, volume 2, "Fast, Minimum Storage Ray-Triangle
        /// Intersection".
        /// 
        /// This method is implemented using the pass-by-reference versions of the
        /// XNA math functions. Using these overloads is generally not recommended,
        /// because they make the code less readable than the normal pass-by-value
        /// versions. This method can be called very frequently in a tight inner loop,
        /// however, so in this particular case the performance benefits from passing
        /// everything by reference outweigh the loss of readability.
        /// </summary>
        public static void RayIntersectsTriangle(ref Ray ray,
                                          ref Vector3 vertex1,
                                          ref Vector3 vertex2,
                                          ref Vector3 vertex3, out float? result)
        {
            // Compute vectors along two edges of the triangle.
            Vector3 edge1, edge2;
            
            Vector3.Subtract(ref vertex2, ref vertex1, out edge1);
            Vector3.Subtract(ref vertex3, ref vertex1, out edge2);

            // Compute the determinant.
            Vector3 directionCrossEdge2;
            Vector3.Cross(ref ray.Direction, ref edge2, out directionCrossEdge2);

            float determinant;
            Vector3.Dot(ref edge1, ref directionCrossEdge2, out determinant);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -float.Epsilon && determinant < float.Epsilon)
            {
                result = null;
                return;
            }

            float inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            Vector3 distanceVector;
            Vector3.Subtract(ref ray.Position, ref vertex1, out distanceVector);

            float triangleU;
            Vector3.Dot(ref distanceVector, ref directionCrossEdge2, out triangleU);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleU < 0 || triangleU > 1)
            {
                result = null;
                return;
            }

            // Calculate the V parameter of the intersection point.
            Vector3 distanceCrossEdge1;
            Vector3.Cross(ref distanceVector, ref edge1, out distanceCrossEdge1);

            float triangleV;
            Vector3.Dot(ref ray.Direction, ref distanceCrossEdge1, out triangleV);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleV < 0 || triangleU + triangleV > 1)
            {
                result = null;
                return;
            }

            // Compute the distance along the ray to the triangle.
            float rayDistance;
            Vector3.Dot(ref edge2, ref distanceCrossEdge1, out rayDistance);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
            {
                result = null;
                return;
            }

            result = rayDistance;
        }

        private void CreateLookAt()
        {
            View = Matrix.CreateLookAt(Position, Position + this._direction, this._up);
        }
    }
}
