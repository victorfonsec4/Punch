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
        bool red;
        Vector2 redBallPos, blueBallPos;
        int height = 480;
        int width = 720;
        KinectSensor kinect;
        Joint rightHand;
        Joint leftHand;
        Skeleton[] skeletonData;
        Skeleton skeleton;
        int health;
        Texture2D redball;
        Texture2D blueball;
        Texture2D redEnemyTexture;
        Texture2D blueEnemyTexture;
        Texture2D purpleEnemyTexture;
        Texture2D[] enemyTextures;
        Texture2D fundo;
        float maxX, minX, maxY, minY;
        int score;
        SpriteFont fonte;
        int spawnTime;
        int lastScore;
        int gameOverCounter;
        bool dead;

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
            health = 20;
            rnd = new Random();
            criou = false;
            kinect = KinectSensor.KinectSensors[0];
            kinect.Start();
            kinect.SkeletonStream.Enable();
            kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(skeletonReady);
            maxX = -10;
            maxY = -10;
            minY = 100;
            minX = 100;
            score = 0;
            spawnTime = 5000;
            lastScore = 0;
            gameOverCounter = 0;
            enemyTextures = new Texture2D[3];
            dead = false;
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
            redball = Content.Load<Texture2D>("redball");
            blueball = Content.Load<Texture2D>("blueball");
            fonte = Content.Load<SpriteFont>("fonte");
            blueEnemyTexture = Content.Load <Texture2D>("blue");
            purpleEnemyTexture = Content.Load<Texture2D>("purple");
            redEnemyTexture = Content.Load<Texture2D>("red");
            fundo = Content.Load<Texture2D>("fundo");
            enemyTextures[0] = redEnemyTexture;
            enemyTextures[1] = blueEnemyTexture;
            enemyTextures[2] = purpleEnemyTexture;

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (dead == false)
            {
                red = false;
                redBallPos = new Vector2(leftHand.Position.X, leftHand.Position.Y);
                redBallPos.X += (float)0.5997291;
                redBallPos.X /= (float)0.7665651;
                redBallPos.X *= GraphicsDevice.Viewport.Width;
                redBallPos.Y *= -1;
                redBallPos.Y += (float)0.8524219;
                redBallPos.Y /= (float)0.8524219;
                redBallPos.Y *= GraphicsDevice.Viewport.Height;
                blueBallPos = new Vector2(rightHand.Position.X, rightHand.Position.Y * -1);
                blueBallPos.X += (float)0.5997291;
                blueBallPos.X /= (float)0.7665651;
                blueBallPos.X *= GraphicsDevice.Viewport.Width;
                blueBallPos.Y += (float)0.8524219;
                blueBallPos.Y /= (float)0.8524219;
                blueBallPos.Y *= GraphicsDevice.Viewport.Height;
                if (rightHand.Position.X > maxX)
                    maxX = rightHand.Position.X;
                if (rightHand.Position.X < minX)
                    minX = rightHand.Position.X;
                if (rightHand.Position.Y > maxY)
                    maxY = rightHand.Position.Y;
                if (rightHand.Position.Y < minY)
                    minY = rightHand.Position.Y;
                Debug.WriteLine("ImpactPoint(x,y): " + redBallPos.X + ", " + redBallPos.Y + ")");
                //Debug.WriteLine("RightHandPos: " + "(" + rightHand.Position.X + ", " + rightHand.Position.Y + ")" );
                Debug.WriteLine("Max X, minX, maxY, minY: " + maxX + ", " + minX + ", " + maxY + ", " + minY);
                // TODO: Add your update logic here
                if ((int)gameTime.TotalGameTime.Milliseconds % spawnTime == 0 && !criou)
                {
                    criou = true;
                    inimigos.Add(new Inimigo(rnd.Next(3 * inimigoText.Width / 2, width - 3 * inimigoText.Width / 2), rnd.Next(3 * inimigoText.Height / 2, height - 3 * inimigoText.Height / 2), inimigoText, rnd.Next(0, 3)));
                }
                if ((int)gameTime.TotalGameTime.Milliseconds % spawnTime != 0 && criou)
                    criou = false;
                foreach (Inimigo i in inimigos)
                {

                    i.Update();
                    if (i.Scale > 1.5)
                    {
                        i.texture = enemyTextures[i.enemyType];
                    }
                    if (i.Scale > 2.5)
                    {
                        health -= 1;
                        if (health <= 0)
                            dead = true;
                        red = true;
                        removeList.Add(i);
                    }
                    if ((i.Hit(redBallPos) && (i.enemyType == 0 || i.enemyType == 2)) || (i.Hit(blueBallPos) && (i.enemyType == 1 || i.enemyType == 2)) && i.Scale > 1.6)
                    {
                        removeList.Add(i);
                        score += 20;
                        if ((score - lastScore) > 100000 / spawnTime)
                        {
                            lastScore = score;
                            if (spawnTime - 500 > 0)
                            {
                                spawnTime -= 500;
                            }
                        }
                    }
                }
                foreach (Inimigo i in removeList)
                    inimigos.Remove(i);
                removeList.Clear();
                Debug.WriteLine(health);
            }
            else
            {
                gameOverCounter += gameTime.ElapsedGameTime.Milliseconds;
                if(gameOverCounter > 4000)
                    this.Exit();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (dead == false)
            {
                spriteBatch.Draw(fundo, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                foreach (Inimigo i in inimigos)
                {
                    i.Draw(spriteBatch);

                }
                spriteBatch.Draw(redball, new Rectangle((int)redBallPos.X - redball.Width / 2, (int)redBallPos.Y - redball.Height / 2, redball.Width, redball.Height), Color.White);
                spriteBatch.Draw(blueball, new Rectangle((int)blueBallPos.X - blueball.Width / 2, (int)blueBallPos.Y - blueball.Height / 2, blueball.Width, blueball.Height), Color.White);
                spriteBatch.DrawString(fonte, "Score:" + score, new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(fonte, "Health:" + health, new Vector2(0, 40), Color.White);
                spriteBatch.DrawString(fonte, "Level:" + (5500 - spawnTime) / 500, new Vector2(0, 80), Color.White);
            }
            else
            {
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.DrawString(fonte, "Game Over", new Vector2(GraphicsDevice.Viewport.Width / 2 - 40, GraphicsDevice.Viewport.Height / 2 - 40), Color.Red);
                spriteBatch.DrawString(fonte, "Score:" + score, new Vector2(GraphicsDevice.Viewport.Width / 2 - 40, GraphicsDevice.Viewport.Height / 2 ), Color.Red);
            }
            spriteBatch.End();

            if(red)
                GraphicsDevice.Clear(Color.Red);
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
