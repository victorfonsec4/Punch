using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;
using System.Diagnostics;

namespace Punch
{
    /// <summary>
    /// This is the main type for your game
    ///claudinho cabaço
    /// </summary>
    public class Punch : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D inimigoText;
        List<Inimigo> inimigos;
        List<Inimigo> removeList;
        Random rnd;
        bool criou;
        int height = 480;
        int width = 720;
        KinectSensor kinect;
        Joint rightHand;
        Joint leftHand;
        Skeleton[] skeletonData;
        Skeleton skeleton;
        Player player;

        public Punch()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = height;
            graphics.PreferredBackBufferWidth = width;
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
            // TODO: Add your initialization logic here
            inimigos = new List<Inimigo>();
            removeList = new List<Inimigo>();
            rnd = new Random();
            criou = false;
            kinect = KinectSensor.KinectSensors[0];
            kinect.Start();
            kinect.SkeletonStream.Enable();
            kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(skeletonReady);
            player = new Player(100);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            inimigoText = Content.Load<Texture2D>("Inimigo");

            // TODO: use this.Content to load your game content here
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
            Vector2 impactPoint;
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            
            impactPoint = player.Update(new Vector3(rightHand.Position.X, rightHand.Position.Y, rightHand.Position.Z), new Vector3 (leftHand.Position.X, leftHand.Position.Y, leftHand.Position.Z) );
            Debug.WriteLine("ImpactPoint(x,y): "+ impactPoint.X + ", " + impactPoint.Y+")");
            // TODO: Add your update logic here
            if ((int)gameTime.TotalGameTime.Milliseconds % 2000 == 0 && !criou)
            {
                criou = true;
                inimigos.Add(new Inimigo(rnd.Next(3 * inimigoText.Width / 2, width - 3 * inimigoText.Width / 2), rnd.Next(3 * inimigoText.Height / 2, height - 3 * inimigoText.Height / 2), inimigoText));
            }
            if ((int)gameTime.TotalGameTime.Milliseconds % 2000 != 0 && criou)
                criou = false;
            foreach (Inimigo i in inimigos)
            {

                i.Update();
                if (i.Scale > 2)
                {
                    player.health--;
                    removeList.Add(i);
                }
                if (i.Hit(impactPoint))
                {
                    removeList.Add(i);
                }
            }
            foreach (Inimigo i in removeList)
                inimigos.Remove(i);
            removeList.Clear();
            Debug.WriteLine(player.health);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            foreach (Inimigo i in inimigos)
            {
                i.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        void skeletonReady(object sender, AllFramesReadyEventArgs imageFrames)
        {
            SkeletonFrame skeletonFrame = imageFrames.OpenSkeletonFrame();
            if (skeletonFrame != null)
            {
                if ((skeletonData == null) || (skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                {
                    skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                }
                skeletonFrame.CopySkeletonDataTo(skeletonData);
            }

            if (skeletonData != null)
            {
                foreach (Skeleton skel in skeletonData)
                {
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        rightHand = skel.Joints[JointType.HandRight];
                        leftHand = skel.Joints[JointType.HandLeft];
                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            skeleton = skel;
                        }
                    }
                }
            }

        }
    }
}
