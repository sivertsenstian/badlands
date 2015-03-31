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
                //this._direction = Vector3.Transform(this._direction, Matrix.CreateFromAxisAngle(this._up, (-MathHelper.PiOver4 / 180) * this._speed / 5));
                this._direction = Vector3.Transform(this._direction, Matrix.CreateFromAxisAngle(this._up, -(this._speed / 1000)));
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                //this._direction = Vector3.Transform(this._direction, Matrix.CreateFromAxisAngle(this._up, (MathHelper.PiOver4 / 180) * this._speed / 5));
                this._direction = Vector3.Transform(this._direction, Matrix.CreateFromAxisAngle(this._up, this._speed / 1000));

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

        private void CreateLookAt()
        {
            View = Matrix.CreateLookAt(Position, Position + this._direction, this._up);
        }
    }
}
