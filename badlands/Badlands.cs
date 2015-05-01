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
using System;
using System.Collections.Generic;

namespace Badlands.Core
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Badlands : Game
    {
        Model test;
        GraphicsDeviceManager _graphics;
        public Badlands()
            : base()
        {
            _graphics = new GraphicsDeviceManager(this);
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
            GameDirector.Camera = new Camera(this, new Vector3(850, 500, 850), Vector3.Zero, Vector3.Up, 100, 10000);
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
            GameDirector.Assets.Textures.Add(Terrain.SAND, Content.Load<Texture2D>("Texture/Terrain/Sand"));
            GameDirector.Assets.Textures.Add(Terrain.WATER, Content.Load<Texture2D>("Texture/Terrain/Water"));
            GameDirector.Assets.Textures.Add(Terrain.ROCK, Content.Load<Texture2D>("Texture/Terrain/Rocks"));
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
            GameDirector.Assets.Skybox = new List<Texture2D> {
                Content.Load<Texture2D>("Texture/Skybox/Mianmar/Front"),
                Content.Load<Texture2D>("Texture/Skybox/Mianmar/Back"),
                Content.Load<Texture2D>("Texture/Skybox/Mianmar/Left"),
                Content.Load<Texture2D>("Texture/Skybox/Mianmar/Right"),
                Content.Load<Texture2D>("Texture/Skybox/Mianmar/Top"),
                Content.Load<Texture2D>("Texture/Skybox/Mianmar/Bottom")
            };

            test = Content.Load<Model>("Model/torus");
            //Load Actors
            Skybox skybox = new Skybox(GameDirector.Assets.Skybox);
            GameDirector.Actors.Add(skybox.Id, skybox);

            TileMap world = new TileMap(128, 128)
            { 
                MapTerrain = new List<Enum>() { 
                    Terrain.WATER, Terrain.SAND, Terrain.ROCK 
                } 
            };
            world.Generate();

            GameDirector.Actors.Add(world.Id, world);

            //Load UI
            FPSCounter counter = new FPSCounter() { Font =  GameDirector.Assets.Fonts[Misc.PRIMARY] };
            Cursor cursor = new Cursor() { Texture = GameDirector.Assets.Textures[UI.CURSOR], Color = GameDirector.Assets.Colors[UI.CURSOR] };

            List<VertexPositionNormalTexture> selectorArea = new List<VertexPositionNormalTexture>();
            foreach (KeyValuePair<Enum, TileLayer> layer in world.Layers)
            {
                selectorArea.AddRange(layer.Value.Vertices);
            }
            Selector selector = new Selector(selectorArea.ToArray(), new Color(Color.DarkOrange, 0.002f));

            GameDirector.UI.Add(cursor);
            GameDirector.UI.Add(counter);
            GameDirector.UI.Add(selector);
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
            GraphicsDevice.Clear(Color.Black);         
            
            //Draw Actors
            foreach (KeyValuePair<Guid, BaseActor> actor in GameDirector.Actors)
            {
                actor.Value.Draw(gameTime);
            }

            test.Draw(Matrix.Identity, GameDirector.Camera.View, GameDirector.Camera.Projection);
            //DrawModel(test, Matrix.Identity, GameDirector.Camera.View, GameDirector.Camera.Projection);

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

            //GameDirector.SpriteBatch.DrawString(GameDirector.Assets.Fonts[Misc.PRIMARY], this._pickedTile, new Vector2(1, 30), Color.Black);
            //GameDirector.SpriteBatch.DrawString(GameDirector.Assets.Fonts[Misc.PRIMARY], this._pickedTile, new Vector2(0, 30), Color.White);
            GameDirector.SpriteBatch.End();
            

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }
    }
}
