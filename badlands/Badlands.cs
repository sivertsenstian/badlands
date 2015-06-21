using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SSGL.Core;
using SSGL.Entity.Actor;
using SSGL.Entity.Skybox;
using SSGL.Entity.Tile;
using SSGL.Entity.UI;
using SSGL.Helper;
using SSGL.Helper.Enum;
using SSGL.Voxel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Badlands.Core
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Badlands : Game
    {
        GraphicsDeviceManager _graphics;

        //TEST CUBE
        Chunk chunk1, chunk2, chunk3, chunk4;

        public Badlands()
            : base()
        {
            _graphics = new GraphicsDeviceManager(this);
            IsFixedTimeStep = false;
            _graphics.SynchronizeWithVerticalRetrace = false;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            //Init game
            GameDirector.Game = this;

            //Init camera
            GameDirector.Camera = new Camera(this, new Vector3(15, 20, 15), Vector3.Zero, Vector3.Up, 1.0f, 1000.0f);
            Components.Add(GameDirector.Camera);

            //Init graphicsdevice
            GameDirector.Device = _graphics.GraphicsDevice;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {   ///////////////////////////////////////////// Load and Define GameDirector Content
            // Create a new SpriteBatch, which can be used to draw textures.
            GameDirector.SpriteBatch = new SpriteBatch(GraphicsDevice);

            //Load Textures
            //Misc
            GameDirector.Assets.Textures.Add(Misc.DEBUG, Content.Load<Texture2D>("Texture/Debug"));
            //Terrain
            GameDirector.Assets.Textures.Add(Terrain.TEXTURE, Content.Load<Texture2D>("Texture/Terrain/Terrain"));
            //UI
            GameDirector.Assets.Textures.Add(UI.CURSOR, Content.Load<Texture2D>("Texture/UI/Cursor"));

            //Load Colors
            //Misc
            GameDirector.Assets.Colors.Add(Misc.DEBUG, Color.Magenta);
            //UI
            GameDirector.Assets.Colors.Add(UI.CURSOR, Color.DarkOrange);

            //Load Fonts
            GameDirector.Assets.Fonts.Add(Misc.PRIMARY, Content.Load<SpriteFont>("Font/Core"));

            //Load Skybox
            string skyboxType = "Mars"; //Mars, Grass, Mianmar
            GameDirector.Assets.Skybox = new List<Texture2D> {
                Content.Load<Texture2D>("Texture/Skybox/" + skyboxType + "/Front"),
                Content.Load<Texture2D>("Texture/Skybox/" + skyboxType + "/Back"),
                Content.Load<Texture2D>("Texture/Skybox/" + skyboxType + "/Left"),
                Content.Load<Texture2D>("Texture/Skybox/" + skyboxType + "/Right"),
                Content.Load<Texture2D>("Texture/Skybox/" + skyboxType + "/Top"),
                Content.Load<Texture2D>("Texture/Skybox/" + skyboxType + "/Bottom")
            };

            //Load Actors
            //Skybox skybox = new Skybox(GameDirector.Assets.Skybox);
            //GameDirector.Actors.Add(skybox.Id, skybox);

            //Load UI
            FPSCounter counter = new FPSCounter() { Font =  GameDirector.Assets.Fonts[Misc.PRIMARY] };
            Cursor cursor = new Cursor() { Texture = GameDirector.Assets.Textures[UI.CURSOR], Color = GameDirector.Assets.Colors[UI.CURSOR] };

            GameDirector.UI.Add(cursor);
            GameDirector.UI.Add(counter);

            //TEST CUBE
            chunk1 = new Chunk(new Vector3(0, 0, 0));
            chunk1.CreateMesh();

            chunk2 = new Chunk(new Vector3(0, 0, 8));
            chunk2.CreateMesh();

            chunk3 = new Chunk(new Vector3(8, 0, 0));
            chunk3.CreateMesh();

            chunk4 = new Chunk(new Vector3(8, 0, 8));
            chunk4.CreateMesh();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
           
            //Update Actors
            foreach (KeyValuePair<Guid, BaseActor> actor in GameDirector.Actors)
            {
                actor.Value.Update(gameTime);
            }

            //Update UI
            for (var u = 0; u < GameDirector.UI.Count; u++)
            {
                GameDirector.UI[u].Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Default.ClearColor);         
            
            //Draw Actors
            //foreach (KeyValuePair<Guid, BaseActor> actor in GameDirector.Actors)
            //{
            //    actor.Value.Draw(gameTime);
            //}

            //Draw UI
            GameDirector.SpriteBatch.Begin();
            for (var u = 0; u < GameDirector.UI.Count; u++)
            {
                GameDirector.UI[u].Draw(gameTime);
            }
            GameDirector.SpriteBatch.End();

            //Debug
            string camerapos = string.Format("camera position: {0}", GameDirector.Camera.Position.ToString());
            GameDirector.SpriteBatch.Begin();

            GameDirector.SpriteBatch.DrawString(GameDirector.Assets.Fonts[Misc.PRIMARY], camerapos, new Vector2(1, 15), Color.Black);
            GameDirector.SpriteBatch.DrawString(GameDirector.Assets.Fonts[Misc.PRIMARY], camerapos, new Vector2(0, 15), Color.Red);

            GameDirector.SpriteBatch.End();

            chunk1.Render();
            chunk2.Render();
            chunk3.Render();
            chunk4.Render();

            base.Draw(gameTime);
        }

        //private void renderBlock(Block block)
        //{
        //    DepthStencilState _depthState = new DepthStencilState();
        //    _depthState.DepthBufferEnable = true; /* Enable the depth buffer */
        //    _depthState.DepthBufferWriteEnable = true; /* When drawing to the screen, write to the depth buffer */

        //    // Set the World matrix which defines the position of the cube
        //    cubeEffect.World = Matrix.Identity * Matrix.Identity * Matrix.Identity;

        //    // Set the View matrix which defines the camera and what it's looking at
        //    cubeEffect.View = GameDirector.Camera.View;

        //    // Set the Projection matrix which defines how we see the scene (Field of view)
        //    cubeEffect.Projection = GameDirector.Camera.Projection;

        //    // Enable textures on the Cube Effect. this is necessary to texture the model
        //    //cubeEffect.TextureEnabled = true;
        //    //cubeEffect.Texture = cubeTexture;
        //    cubeEffect.Texture = GameDirector.Assets.Textures[block.Type];
        //    cubeEffect.TextureEnabled = true;

        //    // Enable some pretty lights
        //    //cubeEffect.EnableDefaultLighting();

        //    // apply the effect and render the cube
        //    //RasterizerState rasterizerState = new RasterizerState();
        //    //rasterizerState.FillMode = FillMode.WireFrame;
        //    //GameDirector.Device.RasterizerState = rasterizerState;
        //    foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
        //    {
        //        pass.Apply();
        //        using (VertexBuffer VertexBuffer = new VertexBuffer(GameDirector.Device, typeof(VertexPositionNormalTexture), Block.NUM_VERTICES, BufferUsage.WriteOnly))
        //        {
        //            using (IndexBuffer IndexBuffer = new IndexBuffer(GameDirector.Device, typeof(int), Block.NUM_INDICES, BufferUsage.WriteOnly))
        //            {
        //                //Load Vertices into buffer
        //                VertexBuffer.SetData<VertexPositionNormalTexture>(block.Vertices);

        //                //Load Indices into buffer
        //                IndexBuffer.SetData(block.Indices);

        //                //Send IndexBuffer to device
        //                GameDirector.Device.Indices = IndexBuffer;
        //                //Send Vertexbuffer to device
        //                GameDirector.Device.SetVertexBuffer(VertexBuffer);
        //                //Use depthbuffer when drawing a shape
        //                GameDirector.Device.DepthStencilState = _depthState;

        //                // Draw the primitives from the vertex buffer and index buffer to the device as triangles
        //                GameDirector.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Block.NUM_VERTICES, 0, Block.NUM_INDICES / 3);
        //            }
        //        }
        //    }
        //}
    }
}
