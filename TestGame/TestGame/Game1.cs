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
using SharedContent;
using System.IO;

namespace TestGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        KeyboardState kbs;
        String str = "";
        Viewport viewport;
        Map map;
        ObjectInfo objectInfo;
        Texture2D background;

        Player player;
        bool pause = false;

        Texture2D playerTexture;
        int gameSpeed = 100;
        int elapsedTime = 0;
        string time;
        Vector2 gravity;
        Vector2 friction;

        Dictionary<Enum, Texture2D> collectibleTextures = new Dictionary<Enum, Texture2D>();
        Dictionary<Enum, Texture2D> enemyTextures = new Dictionary<Enum, Texture2D>();
        Dictionary<Enum, Texture2D> mapTextures = new Dictionary<Enum, Texture2D>();
        Dictionary<Enum, Sprite> itemSprites = new Dictionary<Enum, Sprite>();
        Dictionary<Enum, Sprite> enemySprites = new Dictionary<Enum, Sprite>();
        Dictionary<Enum, Sprite> mapSprites = new Dictionary<Enum, Sprite>();
        Dictionary<CollectibleEnum, int> itemCounter = new Dictionary<CollectibleEnum, int>();
        List<GameObject> mapItems = new List<GameObject>();
        List<EnemyObject> mapEnemies = new List<EnemyObject>();
        List<MapObject> mapPlatforms = new List<MapObject>();

        Texture2D cursor;

        int width = 1000;
        int height = 540;
        int hudHeight = 60;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = height + hudHeight;
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

            viewport = new Viewport(-200, -200, 400, 400);
            kbs = new KeyboardState();
            friction = new Vector2(0.1f, 0.0f);
            gravity = new Vector2(0.0f, 7.0f);

            player = new Player(PlayerSpriteEnum.Standing, new Vector2(700.0f, 350.0f), Direction.Left, 10.0f, 20.0f, new Vector2(0.1f, 0.0f));

            // Add player itemSprites to dictionary
            player.AddSprite(PlayerSpriteEnum.Standing, new PlayerSprite(PlayerSpriteEnum.Standing, 8, new Rectangle(0, 0, 71, 103), new Vector2(35.0f, 90.0f), 1.0f));
            player.AddSprite(PlayerSpriteEnum.Walking, new PlayerSprite(PlayerSpriteEnum.Walking, 8, new Rectangle(0, 103, 65, 103), new Vector2(32.0f, 103.0f), 5.5f));
            player.AddSprite(PlayerSpriteEnum.Jumping, new PlayerSprite(PlayerSpriteEnum.Jumping, 1, new Rectangle(0, 206, 76, 103), new Vector2(38.0f, 103.0f), 1.0f));

            base.Initialize();

            // Initialize map info
            itemCounter = map.collectibles;

            // Add item sprites to dictionary
            foreach (String e in map.GetItemTypes())
            {
                CollectibleEnum i = (CollectibleEnum)Enum.Parse(typeof(CollectibleEnum), e);
                Rectangle rect = new Rectangle(0, 0, collectibleTextures[i].Width / objectInfo.collectibles[i].sprite.frames, collectibleTextures[i].Height);
                itemSprites.Add(i, new Sprite(i, collectibleTextures[i], objectInfo.collectibles[i].sprite.frames, rect, Vector2.Zero));
            }

            // Add enemy sprites to dictionary
            foreach (String e in map.GetEnemyTypes())
            {
                EnemyEnum i = (EnemyEnum)Enum.Parse(typeof(EnemyEnum), e);
                Rectangle rect = new Rectangle(0, 0, enemyTextures[i].Width / objectInfo.enemies[i].sprite.frames, enemyTextures[i].Height);
                enemySprites.Add(i, new Sprite(i, enemyTextures[i], objectInfo.enemies[i].sprite.frames, rect, Vector2.Zero));
            }

            // Add map sprites to list
            mapSprites.Add(MapEnum.Platform, new Sprite(MapEnum.Platform, mapTextures[MapEnum.Platform], 1, mapTextures[MapEnum.Platform].Bounds, Vector2.Zero));

            // Add items to list
            foreach (Map.Item e in map.items)
                mapItems.Add(new GameObject(itemSprites[e.type], e.pos, objectInfo.collectibles[e.type].updateTime));

            // Add enemies to list
            foreach (Map.Enemy e in map.enemies)
                mapEnemies.Add(new EnemyObject(enemySprites[e.type], e.pos, objectInfo.enemies[e.type].updateTime, objectInfo.enemies[e.type].speed));

            // Add map objects to list
            foreach (Map.Platform e in map.platforms)
            {
                mapPlatforms.Add(new MapObject(mapSprites[e.type], e.pos));
            }

            // Add texture data
            foreach (PlayerSpriteEnum e in Enum.GetValues(typeof(PlayerSpriteEnum)))
            {
                player.sprites[e].SetTextureData(playerTexture);
            }

            // Add texture data to items
            foreach (String e in map.GetItemTypes())
            {
                CollectibleEnum i = (CollectibleEnum)Enum.Parse(typeof(CollectibleEnum), e);
                itemSprites[i].SetTextureData(itemSprites[i].texture);
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            DirectoryInfo dir;
            FileInfo[] files;
            string path;
            string filename;

            // Load XML info
            map = Content.Load<Map>("XML/Map/Map1");
            objectInfo = Content.Load<ObjectInfo>("XML/Object/Object");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load font
            spriteFont = Content.Load<SpriteFont>("Font/Stanberry");

            // Load background
            background = Content.Load<Texture2D>("Texture/Map/Background/Background");

            // Load player textures
            playerTexture = Content.Load<Texture2D>("Texture/Player/Sprites");
            
            // Load map textures
            path = "Texture/Map/Platform/";
            dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
            files = dir.GetFiles();
            foreach (FileInfo e in files)
            {
                filename = e.Name.Split('.')[0];
                if (map.GetPlatformTypes().Contains(filename))
                    mapTextures.Add((MapEnum)Enum.Parse(typeof(MapEnum), filename), Content.Load<Texture2D>(path + filename));
            }

            // Load collectibles textures
            path = "Texture/Collectible/Item/";
            dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
            files = dir.GetFiles();
            foreach (FileInfo e in files)
            {
                filename = e.Name.Split('.')[0];
                if (map.GetItemTypes().Contains(filename))
                    collectibleTextures.Add((CollectibleEnum)Enum.Parse(typeof(CollectibleEnum), filename), Content.Load<Texture2D>(path + filename));
            }

            // Load enemies textures
            path = "Texture/Enemy/";
            dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
            files = dir.GetFiles();
            foreach (FileInfo e in files)
            {
                filename = e.Name.Split('.')[0];
                if (map.GetEnemyTypes().Contains(filename))
                    enemyTextures.Add((EnemyEnum)Enum.Parse(typeof(EnemyEnum), filename), Content.Load<Texture2D>(path + filename));
            }

            cursor = Content.Load<Texture2D>("Texture/cursor");
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

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
                pause = true;
            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
                pause = false;

            if(!pause){
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            //TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 250);
            //IsFixedTimeStep = true;

            // Check collisions
            CheckCollision();

            // Check/Update keyboard state
            kbs = Keyboard.GetState();
            foreach(Keys key in kbs.GetPressedKeys())
            {
                switch (key)
                {
                    case Keys.Left:
                    case Keys.A:
                        if (player.dir == Direction.Right)
                            player.UpdateVelocity(-(player.acceleration + friction));
                        else
                            player.UpdateVelocity(-player.acceleration);
                        break;
                    case Keys.Right:
                    case Keys.D:
                        if (player.dir == Direction.Left)
                            player.UpdateVelocity(player.acceleration + friction);
                        else
                            player.UpdateVelocity(player.acceleration);
                        break;
                    case Keys.Up:
                    case Keys.W:
                        if (!player.isJumping && player.canJump)
                        {
                            player.isJumping = true;
                            player.canJump = false;
                        }
                        break;
                    case Keys.Down:
                    case Keys.S:
                        str = "Down";
                        break;
                    case Keys.Space:
                        player.pos.X = Mouse.GetState().X;
                        player.pos.Y = Mouse.GetState().Y;
                        break;
                }
            }

            // Update position
            player.prevPos = player.pos;

            if (!(kbs.IsKeyDown(Keys.Left) || kbs.IsKeyDown(Keys.Right) || kbs.IsKeyDown(Keys.A) || kbs.IsKeyDown(Keys.D)))
            {
                if (player.dir == Direction.Left)
                {
                    if(player.state == PlayerSpriteEnum.Jumping)
                        player.UpdateVelocity(Vector2.Divide(friction, 2));
                    else
                        player.UpdateVelocity(friction);
                    if (player.velocity.X > 0)
                        player.velocity.X = 0;
                }
                else
                {
                    if (player.state == PlayerSpriteEnum.Jumping)
                        player.UpdateVelocity(Vector2.Divide(-friction, 2));
                    else
                        player.UpdateVelocity(-friction);
                    if (player.velocity.X < 0)
                        player.velocity.X = 0;
                }
            }

            player.pos = Vector2.Add(player.GetVelocity(), player.pos);

            // Update gravity
            if (player.isJumping)
            {
                player.pos = Vector2.Subtract(player.pos, player.GetJump());
                player.UpdateJump(gravity);
            }

            if (player.isFalling)
            {
                player.pos = Vector2.Add(gravity, player.pos);
            }           

            // Update animation
            player.UpdateDirection();
            if (player.pos != player.prevPos)
            {
                if (player.pos.Y != player.prevPos.Y)
                    player.state = PlayerSpriteEnum.Jumping;
                else if (player.pos.X != player.prevPos.X && !player.isJumping)
                    player.state = PlayerSpriteEnum.Walking;
            }
            else if(!(kbs.IsKeyDown(Keys.Left) || kbs.IsKeyDown(Keys.Right)) && !player.isJumping)
            {
                player.state = PlayerSpriteEnum.Standing;
            }
            if (player.state != player.prevState)
            {
                player.frame = 0;
                player.prevState = player.state;
            }

            if (elapsedTime >= gameSpeed)
            {
                player.frame = (player.frame + 1) % player.sprites[player.state].frames;
                player.sprites[player.state].rect.X = player.sprites[player.state].rect.Width * player.frame;
            }

            // Update game items animation
            foreach (GameObject i in mapItems)
            {
                i.UpdateAnimation(gameTime.ElapsedGameTime.Milliseconds);
            }

            // Update game enemies animation / state
            foreach (EnemyObject e in mapEnemies)
            {
                e.UpdateAnimation(gameTime.ElapsedGameTime.Milliseconds);
                e.UpdatePosition(gravity);
            }

            if(elapsedTime >= gameSpeed)
                elapsedTime = 0;      
            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            
            base.Update(gameTime);
        }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            time = String.Format((gameTime.TotalGameTime.Minutes > 0) ? "{0:mm\\:ss\\.ff}" : "{0:ss\\.ff}", gameTime.TotalGameTime);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation((int)(-player.pos.X + (width / 2)), (int)(-player.pos.Y + player.sprites[player.state].rect.Height + (height / 2)), 0.0f));
            spriteBatch.DrawString(spriteFont, str, new Vector2(400.0f, 0.0f), Color.White);

            // Draw background
            spriteBatch.Draw(background, Vector2.Zero, Color.White);

            // Draw platforms
            foreach (MapObject e in mapPlatforms)
            {
                spriteBatch.Draw(e.sprite.texture, e.pos, Color.White * e.alpha);
            }

            // Draw items
            foreach (GameObject e in mapItems)
            {
                e.sprite.rect.X = itemSprites[e.sprite.id].rect.Width * e.frame;
                if(e.alive)
                    spriteBatch.Draw(e.sprite.texture, e.pos, e.sprite.rect, Color.White * e.alpha);
            }

            // Draw enemies
            foreach (GameObject e in mapEnemies)
            {
                e.sprite.rect.X = enemySprites[e.sprite.id].rect.Width * e.frame;
                if (e.alive)
                    spriteBatch.Draw(e.sprite.texture, e.pos, e.sprite.rect, Color.White * e.alpha);
            }

            spriteBatch.Draw(playerTexture, new Vector2((int)player.pos.X, (int)player.pos.Y), player.sprites[player.state].rect, Color.White, 0.0f, player.sprites[player.state].origin, 1.0f, (player.dir == Direction.Left) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0f);
            spriteBatch.End();
            DrawHUD();

            base.Draw(gameTime);
        }

        void DrawHUD()
        {
            int x = 0;
            float scale = 1.0f;
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, time, new Vector2(0.0f, 0.0f), Color.White);
            foreach (KeyValuePair<Enum, Sprite> i in itemSprites)
            {
                string counter = itemCounter[(CollectibleEnum)i.Value.id].ToString();
                scale = hudHeight * 0.75f / i.Value.rect.Height;
                Vector2 position = new Vector2(x, graphics.PreferredBackBufferHeight - hudHeight * 0.9f);
                if (itemCounter[(CollectibleEnum)i.Value.id] <= 0)
                    spriteBatch.Draw(i.Value.texture, position, i.Value.rect, Color.White * 0.5f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
                else
                    spriteBatch.Draw(i.Value.texture, position, i.Value.rect, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
                spriteBatch.DrawString(spriteFont, counter, new Vector2(x - spriteFont.MeasureString(counter).X + i.Value.rect.Width * scale, position.Y), Color.White);
                x += (int)Math.Ceiling(i.Value.rect.Width * scale);
            }
            spriteBatch.Draw(cursor, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Color.White);
            spriteBatch.End();
        }

        void CheckCollision()
        {
            Rectangle playerRect = new Rectangle((int)(player.pos.X - player.sprites[player.state].origin.X), (int)(player.pos.Y - player.sprites[player.state].origin.Y), player.sprites[player.state].rect.Width, player.sprites[player.state].rect.Height);
            
            // Collision with items
            foreach (GameObject e in mapItems)
            {
                Rectangle itemRect = new Rectangle((int)(e.pos.X), (int)(e.pos.Y), e.sprite.rect.Width, e.sprite.rect.Height);
                if (playerRect.Intersects(itemRect))
                {
                    if (IntersectPixels(playerRect, player.sprites[player.state].textureData[player.frame], itemRect, e.sprite.textureData[e.frame]) && e.alive)
                    {
                        e.collision = true;
                        e.alive = false;
                        itemCounter[(CollectibleEnum)e.sprite.id]--;
                    }
                    else
                    {
                        e.collision = false;
                    }
                }
                //e.UpdateCollision();
            }

            // Collision with map platforms
            player.isFalling = true;
            if (player.pos.Y - player.prevPos.Y >= 0)
            {
                foreach (MapObject m in mapPlatforms)
                {
                    Rectangle objectRect = new Rectangle((int)m.pos.X, (int)m.pos.Y, m.sprite.rect.Width, 1/*m.sprite.rect.Height*/);
                    if (player.Collided(objectRect))
                        player.isFalling = false;
                    /*str = player.GetJump() + " " + Math.Abs(player.prevPos.Y - player.pos.Y) +
                        "\nPlatform L:" + objectRect.Left + " R:" + objectRect.Right + " T:" + objectRect.Top + " B:" + objectRect.Bottom +
                        "\nPlayer   " + player.test +
                        "\nPosition" + player.pos;*/
                    if (!player.isFalling)
                    {
                        player.isJumping = false;
                        player.canJump = true;
                        player.pos.Y = objectRect.Bottom;
                        player.ResetJump();
                        break;
                    }
                }
            }
        }

        public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);
 
            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];
 
                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }
 
            // No intersection found
            return false;
        }
    }
}