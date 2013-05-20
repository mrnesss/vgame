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
        #region Game variables
        GraphicsDeviceManager graphics;
        Song[] songs;
        TimeSpan elapsedGameTime;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        KeyboardState kbs, pkbs;
        String str = "hurr";
        Viewport viewport;
        Map map;
        ObjectInfo objectInfo;
        Texture2D background, startScreen, rainbow;
        LevelMenu levelMenu;
        StartMenu startMenu;

        Random random;
        Player player;
        Chef chef;
        GameStateEnum gameState;

        Point collisionPoint;
        float levelSelectionScale;
        int gameSpeed = (int)Math.Round(1000 / 24.0f);
        int elapsedTime = 0;
        int points;
        string accumStr;
        int introTimer = 0;
        int updateTime = 3500;
        string time;
        int blinkCounter;
        float volume = 0.0f;
        bool blink, mute;
        Vector2 gravity;
        Vector2 friction;

        Dictionary<Enum, Texture2D> collectibleTextures = new Dictionary<Enum, Texture2D>();
        Dictionary<Enum, List<Texture2D>> enemyTextures = new Dictionary<Enum, List<Texture2D>>();
        Dictionary<Enum, Texture2D> mapTextures = new Dictionary<Enum, Texture2D>();
        Dictionary<Enum, Texture2D> sceneryTextures = new Dictionary<Enum, Texture2D>();
        Dictionary<Enum, Texture2D> interactiveTextures = new Dictionary<Enum, Texture2D>();
        Dictionary<Enum, Sprite> itemSprites = new Dictionary<Enum, Sprite>();
        Dictionary<Enum, Sprite> enemySprites = new Dictionary<Enum, Sprite>();
        Dictionary<Enum, Sprite> mapSprites = new Dictionary<Enum, Sprite>();
        Dictionary<CollectibleEnum, int[]> itemCounter = new Dictionary<CollectibleEnum, int[]>();
        Dictionary<String, Texture2D> menuTextures = new Dictionary<string, Texture2D>();
        Dictionary<String, Texture2D> menuHiTextures = new Dictionary<string, Texture2D>();
        Dictionary<String, Texture2D> screenTextures = new Dictionary<string, Texture2D>();
        Dictionary<UIEnum, Texture2D> uiTextures = new Dictionary<UIEnum, Texture2D>();
        List<GameObject> mapItems = new List<GameObject>();
        List<EnemyObject> mapEnemies = new List<EnemyObject>();
        List<MapObject> mapPlatforms = new List<MapObject>();
        List<MapObject> mapScenery = new List<MapObject>();
        List<MapObject> mapInteractives = new List<MapObject>();
        List<Texture2D> introTextures = new List<Texture2D>();

        Texture2D cursor;

        int width, height, hudHeight, totalHeight, defaultWidth;
        float defaultRatio;
        #endregion

        // DEVELOPMENT VARIABLES
        List<Map.Scenery> editor = new List<Map.Scenery>();
        Vector2 editorPos = Vector2.Zero;
        struct Brush
        {
            public MapEnum type;
            public Texture2D texture;
        };
        Brush brush;

        public Game1()
        {
            defaultWidth = 1600;
            width = 1280;
            totalHeight = (int)(width * 0.5625);
            hudHeight = 60;
            height = totalHeight - hudHeight;
            defaultRatio = (float)width / defaultWidth;
            random = new Random((int)DateTime.Now.ToFileTime());

            levelSelectionScale = 0.375f * defaultRatio;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = totalHeight;
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
            gameState = GameStateEnum.StartScreen;
            songs = new Song[3];
            viewport = new Viewport(0, 0, 600, 600);
            kbs = new KeyboardState();
            friction = new Vector2(0.35f, 0.0f);
            gravity = new Vector2(0.0f, 20.0f);

            player = new Player(PlayerSpriteEnum.Standing, Vector2.Zero, Direction.Left, 30.0f, 70.0f, new Vector2(0.4f, 0.0f));
            chef = new Chef(ChefSpriteEnum.Standing, new Vector2(0.0f, -1000.0f), Direction.Left, 8.0f);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            DirectoryInfo dir;
            FileInfo[] files;
            List<Texture2D> textures;
            string path;
            string filename;

            // Load songs
            songs[0] = Content.Load<Song>("Audio/Baron_Knoxburry");
            songs[1] = Content.Load<Song>("Audio/Bit_Shifter");
            songs[2] = Content.Load<Song>("Audio/Ggeir_Tjelta");

            MediaPlayer.Volume = volume;
            MediaPlayer.Play(songs[2]);
            MediaPlayer.IsRepeating = true;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load font
            spriteFont = Content.Load<SpriteFont>("Font/Stanberry");

            // Load intro textures
            path = "Texture/Intro/";
            dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
            files = dir.GetFiles();
            foreach (FileInfo e in files)
            {
                filename = e.Name.Split('.')[0];
                introTextures.Add(Content.Load<Texture2D>(path + filename));
            }

            // Load start screen
            startScreen = Content.Load<Texture2D>("Texture/Menu/Background/Start_Screen");
            rainbow = Content.Load<Texture2D>("Texture/Menu/Background/Rainbow");

            // Load books
            path = "Texture/Menu/Options/";
            dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
            files = dir.GetFiles();
            foreach (FileInfo e in files)
            {
                filename = e.Name.Split('.')[0];
                menuTextures.Add(filename, Content.Load<Texture2D>(path + filename));
            }

            // Load books highlight
            path = "Texture/Menu/Highlight/";
            dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
            files = dir.GetFiles();
            foreach (FileInfo e in files)
            {
                filename = e.Name.Split('.')[0];
                menuHiTextures.Add(filename, Content.Load<Texture2D>(path + filename));
            }

            // Load screens
            path = "Texture/Menu/Screens/";
            dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
            files = dir.GetFiles();
            foreach (FileInfo e in files)
            {
                filename = e.Name.Split('.')[0];
                screenTextures.Add(filename, Content.Load<Texture2D>(path + filename));
            }

            // Load UI textures
            path = "Texture/UI/";
            dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
            files = dir.GetFiles();
            foreach (FileInfo e in files)
            {
                filename = e.Name.Split('.')[0];
                uiTextures.Add((UIEnum)Enum.Parse(typeof(UIEnum), filename), Content.Load<Texture2D>(path + filename));
            }

            // Load map textures
            path = "Texture/Map/Platform/";
            dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
            files = dir.GetFiles();

            // Load player textures
            foreach (String s in Enum.GetNames(typeof(PlayerSpriteEnum)))
            {
                Vector2 origin;
                path = "Texture/Player/" + s + "/";
                dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
                files = dir.GetFiles();
                textures = new List<Texture2D>();
                foreach (FileInfo e in files)
                {
                    filename = e.Name.Split('.')[0];
                    textures.Add(Content.Load<Texture2D>(path + filename));
                }
                origin = new Vector2(textures.ElementAt(0).Width / 2, textures.ElementAt(0).Height);
                switch (s)
                {
                    case "Standing":
                        origin.Y = 630.0f;
                        break;
                    case "Walking":
                        origin.Y = 630.0f;
                        break;
                    case "Handing":
                        origin.X = 195.0f;
                        origin.Y = 630.0f;
                        break;
                    case "Dying":
                        origin.Y = 655.0f;
                        break;
                    case "Jumping":
                        origin.Y = 620.0f;
                        break;
                }
                CharacterSprite sprite = new CharacterSprite((PlayerSpriteEnum)Enum.Parse(typeof(PlayerSpriteEnum), s), textures, origin);
                player.AddSprite((PlayerSpriteEnum)sprite.id, sprite);
                sprite.SetTextureData();
            }

            // Load chef textures
            foreach (String s in Enum.GetNames(typeof(ChefSpriteEnum)))
            {
                path = "Texture/Chef/" + s + "/";
                dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
                files = dir.GetFiles();
                textures = new List<Texture2D>();
                foreach (FileInfo e in files)
                {
                    filename = e.Name.Split('.')[0];
                    textures.Add(Content.Load<Texture2D>(path + filename));
                }
                CharacterSprite sprite = new CharacterSprite((ChefSpriteEnum)Enum.Parse(typeof(ChefSpriteEnum), s), textures, Vector2.Zero);
                chef.AddSprite((ChefSpriteEnum)sprite.id, sprite);
                sprite.SetTextureData();
            }

            // Load interactives textures
            path = "Texture/Map/Interactive/";
            dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
            files = dir.GetFiles();
            foreach (FileInfo e in files)
            {
                filename = e.Name.Split('.')[0];
                interactiveTextures.Add((MapEnum)Enum.Parse(typeof(MapEnum), filename), Content.Load<Texture2D>(path + filename));
            }

            // Load start menu
            startMenu = Content.Load<StartMenu>("XML/Menu/Start_Menu");
            startMenu.background = Content.Load<Texture2D>("Texture/Menu/Background/" + startMenu.bg);

            // Load level selection menu
            levelMenu = Content.Load<LevelMenu>("XML/Menu/Level_Selection");
            levelMenu.background = Content.Load<Texture2D>("Texture/Menu/Background/" + levelMenu.bg);
            for (int i = 0; i < levelMenu.levels.Count; i++)
            {
                bool available = levelMenu.levels.ElementAt(i).isAvailable;
                Vector2 pos = levelMenu.levels.ElementAt(i).pos;
                levelMenu.levels.RemoveAt(i);
                levelMenu.levels.Insert(i, new LevelMenu.Level(i, Vector2.Multiply(pos, defaultRatio), available));
            }

            cursor = Content.Load<Texture2D>("Texture/cursor");

            // Set player starting position
            player.SetPosition(levelMenu.levels.ElementAt(0).pos);
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

            // Check pressed keys
            CheckKeyboardState();
            kbs = Keyboard.GetState();

            // Set volume
            MediaPlayer.Volume = volume;

            //TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 250);
            //IsFixedTimeStep = true;

            if (gameState == GameStateEnum.Playing || gameState == GameStateEnum.LevelSelection)
            {
                // Update position
                UpdatePlayerPosition();

                // Update animation
                UpdatePlayerAnimation();
            }

            // If intro is running
            if (gameState == GameStateEnum.Intro)
            {
                introTimer += gameTime.ElapsedGameTime.Milliseconds;
                if ((int)(introTimer / updateTime) > introTextures.Count - 1)
                {
                    gameState = GameStateEnum.MainMenu;
                }

            }

            // If game is currently unpaused
            if (gameState == GameStateEnum.Playing)
            {
                // Set elapsed time
                time = String.Format((elapsedGameTime.Minutes > 0) ? "{0:mm\\:ss\\.ff}" : "{0:ss\\.ff}", map.time - elapsedGameTime);

                // Check if time ran out
                if (map.time - elapsedGameTime < TimeSpan.Zero)
                    player.UpdateHealth(-100.0f);

                // Check collisions
                CheckPlayerCollisions();
                CheckEnemiesCollisions();

                // Update game items animation
                foreach (GameObject i in mapItems)
                {
                    i.UpdateAnimation(gameTime.ElapsedGameTime.Milliseconds);
                    i.UpdatePosition();
                }

                // Update interactives position
                foreach (MapObject e in mapInteractives)
                {
                    e.UpdatePosition(map.rect.Height);
                }

                // Makes the chef appear randomly
                if (chef.canAppear && random.NextDouble() > 0.995f)
                {
                    chef.isAppearing = true;
                    chef.canAppear = false;
                }

                // Update chef animation and position
                UpdateChefAnimation();
                UpdateChefPosition();

                // Update game enemies animation / state
                foreach (EnemyObject e in mapEnemies)
                {
                    e.UpdateAnimation(gameTime.ElapsedGameTime.Milliseconds);
                    e.UpdatePosition(gravity, player.pos, player.sprites[player.state].rect);
                }

                // Update player invincibility frames
                player.UpdateInvincibility(gameTime.ElapsedGameTime.Milliseconds);

                // Check if all the ingredients were picked up
                if (itemCounter.All(p => p.Value.ElementAt(1) == 0))
                    map.isCompleted = true;

                // Update if player died
                if (player.health <= 0.0f)
                {
                    player.isAlive = false;
                    player.SetVelocity(Vector2.Zero);
                }

                // Remove interactives
                mapInteractives.RemoveAll(p => p.isAlive == false);

                // Wait for animation to end to show game over screen
                if (player.state == PlayerSpriteEnum.Dying && player.frame == player.sprites[player.state].frames - 1)
                {
                    gameState = GameStateEnum.GameOver;
                }

                // Wait for animation to end to show level completed screen
                if (player.state == PlayerSpriteEnum.Handing && player.frame == player.sprites[player.state].frames - 1)
                {
                    points = (int)(Math.Truncate(((map.time - elapsedGameTime > TimeSpan.Zero) ? map.time - elapsedGameTime : TimeSpan.Zero).TotalMilliseconds / 10) * player.GetHealth());
                    player.ChangeLevel(1, levelMenu.levels.Count);
                    gameState = GameStateEnum.LevelCompleted;
                }

                // Check is level is completed
                if (gameState == GameStateEnum.LevelCompleted)
                {
                    accumStr = Math.Truncate(((map.time - elapsedGameTime).TotalMilliseconds / 10)) +
                        " x " + player.GetHealth();
                }

                // Update game time
                elapsedGameTime += gameTime.ElapsedGameTime;
            }

            base.Update(gameTime);

            if (elapsedTime >= gameSpeed)
                elapsedTime = 0;
            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
        }

        void LoadLevel(int level)
        {
            DirectoryInfo dir;
            FileInfo[] files;
            string path;
            string filename;

            MediaPlayer.Play(songs[1]);

            // Clear previous lists and dictionaries
            collectibleTextures.Clear();
            enemyTextures.Clear();
            mapTextures.Clear();
            sceneryTextures.Clear();
            itemSprites.Clear();
            enemySprites.Clear();
            mapSprites.Clear();
            mapItems.Clear();
            mapEnemies.Clear();
            mapPlatforms.Clear();
            mapScenery.Clear();
            mapInteractives.Clear();

            // Load XML info
            map = Content.Load<Map>("XML/Map/Map_" + level);
            objectInfo = Content.Load<ObjectInfo>("XML/Object/Object");

            // Set player starting position
            player.SetStartingPosition(map.startPos);
            player.dir = Direction.Right;

            // Set chef starting position
            chef.SetStartingPosition(new Vector2(0.0f, -chef.sprites[chef.state].rect.Height));

            // Load background
            background = Content.Load<Texture2D>("Texture/Map/Background/" + map.background);

            /*// Load interactives textures
            path = "Texture/Map/Interactive/";
            dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
            files = dir.GetFiles();
            foreach (FileInfo e in files)
            {
                filename = e.Name.Split('.')[0];
                if (map.GetInteractiveTypes().Contains(filename))
                    interactiveTextures.Add((MapEnum)Enum.Parse(typeof(MapEnum), filename), Content.Load<Texture2D>(path + filename));
            }*/

            // Load map scenery textures
            path = "Texture/Map/Scenery/";
            dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
            files = dir.GetFiles();
            foreach (FileInfo e in files)
            {
                filename = e.Name.Split('.')[0];
                if (map.GetSceneryTypes().Contains(filename))
                    sceneryTextures.Add((MapEnum)Enum.Parse(typeof(MapEnum), filename), Content.Load<Texture2D>(path + filename));
            }

            // Load map platform textures
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
            foreach (String s in map.GetEnemyTypes())
            {
                path = "Texture/Enemy/" + s + "/";
                dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
                files = dir.GetFiles();
                List<Texture2D> textures = new List<Texture2D>();
                foreach (FileInfo e in files)
                {
                    filename = e.Name.Split('.')[0];
                    textures.Add(Content.Load<Texture2D>(path + filename));
                }
                enemyTextures.Add((EnemyEnum)Enum.Parse(typeof(EnemyEnum), s), textures);
            }

            /*path = "Texture/Enemy/";
            dir = new DirectoryInfo(Content.RootDirectory + "/" + path);
            files = dir.GetFiles();
            foreach (FileInfo e in files)
            {
                filename = e.Name.Split('.')[0];
                if (map.GetEnemyTypes().Contains(filename))
                    enemyTextures.Add((EnemyEnum)Enum.Parse(typeof(EnemyEnum), filename), Content.Load<Texture2D>(path + filename));
            }*/
        }

        void InitializeLevel()
        {
            brush.texture = mapTextures[MapEnum.Platform];

            // Initialize map info
            itemCounter = map.collectibles;

            // Add item sprites to dictionary
            foreach (String e in map.GetItemTypes())
            {
                CollectibleEnum i = (CollectibleEnum)Enum.Parse(typeof(CollectibleEnum), e);
                Rectangle rect = new Rectangle(0, 0, collectibleTextures[i].Width / objectInfo.collectibles[i].sprite.frames, collectibleTextures[i].Height);
                itemSprites.Add(i, new Sprite(i, collectibleTextures[i], objectInfo.collectibles[i].sprite.frames, rect, Vector2.Zero));
            }

            // Add map interactives sprites to dictionary
            foreach (KeyValuePair<MapEnum, MapObject> e in objectInfo.interactives)
            {
                MapEnum i = e.Key;
                Rectangle rect = new Rectangle(0, 0, interactiveTextures[i].Width / objectInfo.interactives[i].sprite.frames, interactiveTextures[i].Height);
                mapSprites.Add(i, new Sprite(i, interactiveTextures[i], objectInfo.interactives[i].sprite.frames, rect, Vector2.Zero));
            }

            // Add map platform sprites to dictionary
            foreach (String e in map.GetPlatformTypes())
            {
                MapEnum i = (MapEnum)Enum.Parse(typeof(MapEnum), e);
                Rectangle rect = new Rectangle(0, 0, mapTextures[i].Width, mapTextures[i].Height);
                mapSprites.Add(i, new Sprite(i, mapTextures[i], 1, mapTextures[i].Bounds, Vector2.Zero));
            }

            // Add enemy sprites to dictionary
            foreach (String e in map.GetEnemyTypes())
            {
                EnemyEnum i = (EnemyEnum)Enum.Parse(typeof(EnemyEnum), e);
                Rectangle rect = new Rectangle(0, 0, enemyTextures[i].ElementAt(0).Width, enemyTextures[i].ElementAt(0).Height);
                enemySprites.Add(i, new Sprite(i, enemyTextures[i], objectInfo.enemies[i].sprite.frames, rect, Vector2.Zero));
            }

            // Add map scenery to list
            foreach(Map.Scenery e in map.scenery)
                mapScenery.Add(new MapObject(sceneryTextures[e.type], e.pos));

            // Add items to list
            foreach (Map.Item e in map.items)
            {
                GameObject item = new GameObject(itemSprites[e.type], e.pos, objectInfo.collectibles[e.type].updateTime);
                mapItems.Add(item);
                item.Initialize();
            }

            // Add enemies to list and initialize them
            foreach (Map.Enemy e in map.enemies)
            {
                EnemyObject enemy;
                if(objectInfo.enemies[e.type].poisonTimer != 0)
                    enemy = new EnemyObject(enemySprites[e.type], e.pos, objectInfo.enemies[e.type].updateTime, objectInfo.enemies[e.type].speed, objectInfo.enemies[e.type].damage, objectInfo.enemies[e.type].poisonTimer, objectInfo.enemies[e.type].dot);
                else
                    enemy = new EnemyObject(enemySprites[e.type], e.pos, objectInfo.enemies[e.type].updateTime, objectInfo.enemies[e.type].speed, objectInfo.enemies[e.type].damage);
                mapEnemies.Add(enemy);
                enemy.Initialize();
            }

            // Add map platforms to list
            foreach (Map.Platform e in map.platforms)
            {
                mapPlatforms.Add(new MapObject(mapSprites[e.type], e.pos));
            }

            // Add map interactives to list
            foreach (Map.Interactive e in map.interactives)
            {
                mapInteractives.Add(new MapObject(mapSprites[e.type], e.pos));
            }

            // Add texture data to items
            foreach (String e in map.GetItemTypes())
            {
                CollectibleEnum i = (CollectibleEnum)Enum.Parse(typeof(CollectibleEnum), e);
                itemSprites[i].SetTextureData(itemSprites[i].texture);
            }

            // Add texture data to enemies
            foreach (String e in map.GetEnemyTypes())
            {
                EnemyEnum i = (EnemyEnum)Enum.Parse(typeof(EnemyEnum), e);
                enemySprites[i].SetTextureData(enemyTextures[i]);
            }

            // Add texture data to interactives
            foreach (KeyValuePair<MapEnum, MapObject> e in objectInfo.interactives)
            {
                MapEnum i = e.Key;
                mapSprites[i].SetTextureData(mapSprites[i].texture);
            }
        }

        void RestartLevel()
        {
            // Reset map state
            map.isCompleted = false;

            // Reset items counter
            foreach (KeyValuePair<CollectibleEnum, int[]> e in itemCounter)
                e.Value[1] = e.Value[0];

            // Reset game items state
            foreach (GameObject e in mapItems)
                e.Initialize();

            // Reset enemies state
            foreach (EnemyObject e in mapEnemies)
                e.Initialize();

            // Reset player state
            player.Initialize();

            // Reset chef state
            chef.Initialize();

            // Reset interactives
            mapInteractives.Clear();
            // Add interactives to list
            foreach (Map.Interactive e in map.interactives)
                mapInteractives.Add(new MapObject(mapSprites[e.type], e.pos));

            // Reset game state
            gameState = GameStateEnum.Playing;

            // Reset time to zero
            elapsedGameTime = TimeSpan.Zero;
        }

        void CheckKeyboardState()
        {
            // DEVELOPMENT
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.D2))
            {
                MapEnum en = MapEnum.Drawer;
                brush.type = en;
                brush.texture = sceneryTextures[en];
            }
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.D3))
            {
                MapEnum en = MapEnum.Extractor;
                brush.type = en;
                brush.texture = sceneryTextures[en];
            }
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.D4))
            {
                MapEnum en = MapEnum.Fridge;
                brush.type = en;
                brush.texture = sceneryTextures[en];
            }
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.D5))
            {
                MapEnum en = MapEnum.Pan;
                brush.type = en;
                brush.texture = sceneryTextures[en];
            }
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.D6))
            {
                MapEnum en = MapEnum.Picture;
                brush.type = en;
                brush.texture = sceneryTextures[en];
            }
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.D7))
            {
                MapEnum en = MapEnum.Spatula;
                brush.type = en;
                brush.texture = sceneryTextures[en];
            }
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.D0) && !kbs.GetPressedKeys().Contains(Keys.D0) && editor.Count > 0)
                editor.RemoveAt(editor.Count - 1);
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.D9) && !kbs.GetPressedKeys().Contains(Keys.D9))
            {
                Map.Scenery plat = new Map.Scenery(brush.type, editorPos);
                editor.Add(plat);
            }
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.D1) && !kbs.GetPressedKeys().Contains(Keys.D1))
            {
                string str = "";
                foreach (Map.Scenery e in editor)
                {
                    str += "<Item>\n" +
                        "\t<type>" + e.type + "</type>\n" +
                        "\t<pos>" + e.pos.X + " " + e.pos.Y + "</pos>\n" +
                        "</Item>\n";
                }
                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Sweet_Chef.txt", str);
            }

            // Quit to main menu
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.B) && !kbs.GetPressedKeys().Contains(Keys.B) &&
                gameState != GameStateEnum.MainMenu && gameState != GameStateEnum.StartScreen && gameState != GameStateEnum.Intro)
            {
                gameState = GameStateEnum.MainMenu;
            }

            // Pause
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.P) && !kbs.GetPressedKeys().Contains(Keys.P))
            {
                if (gameState == GameStateEnum.Playing)
                    gameState = GameStateEnum.Paused;
                else if (gameState == GameStateEnum.Paused)
                    gameState = GameStateEnum.Playing;
            }
            
            // Mute
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.M) && !kbs.GetPressedKeys().Contains(Keys.M))
            {
                mute ^= true;
                if (mute)
                    MediaPlayer.Volume = 0.0f;
                else
                    MediaPlayer.Volume = volume;
            }

            // Volume
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.Add))
            {
                volume += 0.002f;
                if (volume > 1.0f)
                    volume = 1.0f;
            }
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.Subtract))
            {
                volume -= 0.002f;
                if (volume < 0.0f)
                    volume = 0.0f;
            }

            // Restart level
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.R) && !kbs.GetPressedKeys().Contains(Keys.R) &&
                (gameState == GameStateEnum.Playing || gameState == GameStateEnum.Paused || gameState == GameStateEnum.LevelCompleted || gameState == GameStateEnum.GameOver))
            {
                RestartLevel();
            }

            // Game started, currently playing
            if (gameState == GameStateEnum.Playing && player.isAlive && player.state != PlayerSpriteEnum.Handing)
            {
                foreach (Keys key in kbs.GetPressedKeys())
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
                            break;
                    }
                }
            }

            // Game hasn't started, main menu screen
            else if (gameState == GameStateEnum.MainMenu)
            {
                if ((kbs.GetPressedKeys().Contains(Keys.A) && !pkbs.GetPressedKeys().Contains(Keys.A)) ||
                    (kbs.GetPressedKeys().Contains(Keys.Left) && !pkbs.GetPressedKeys().Contains(Keys.Left)))
                    startMenu.ChangeOption(-1, startMenu.options.Count);
                else if ((kbs.GetPressedKeys().Contains(Keys.D) && !pkbs.GetPressedKeys().Contains(Keys.D)) ||
                    (kbs.GetPressedKeys().Contains(Keys.Right) && !pkbs.GetPressedKeys().Contains(Keys.Right)))
                    startMenu.ChangeOption(1, startMenu.options.Count);
                else if (kbs.GetPressedKeys().Contains(Keys.Enter) && !pkbs.GetPressedKeys().Contains(Keys.Enter))
                {
                    switch (startMenu.options[startMenu.selectedOption])
                    {
                        case "Start":
                            player.Initialize(levelMenu.levels.ElementAt(player.GetLevel()).pos);
                            gameState = GameStateEnum.LevelSelection;
                            break;
                        case "Instructions":
                            gameState = GameStateEnum.Options;
                            break;
                        case "Map":
                            gameState = GameStateEnum.Options;
                            break;
                        case "Minigames":
                            gameState = GameStateEnum.Options;
                            break;
                        case "About":
                            gameState = GameStateEnum.Options;
                            break;
                    }
                }
                    
            }
            // Game started, level selection screen
            else if(gameState == GameStateEnum.LevelSelection)
            {
                Vector2 currentPos = player.GetPosition();
                Vector2 delta = new Vector2(player.prevLevel, Int32.MaxValue);
                Vector2 newPos;
                // Moving up
                if ((kbs.GetPressedKeys().Contains(Keys.W) && !pkbs.GetPressedKeys().Contains(Keys.W)) ||
                    (kbs.GetPressedKeys().Contains(Keys.Up) && !pkbs.GetPressedKeys().Contains(Keys.Up)))
                {
                    for (int i = 0; i < levelMenu.levels.Count; i++)
                    {
                        if (currentPos != levelMenu.levels.ElementAt(player.GetLevel()).pos || player.prevLevel != i)
                        {
                            newPos = levelMenu.levels.ElementAt(i).pos;
                            if (newPos.Y - currentPos.Y < 0)
                            {
                                float diff = Math.Abs(currentPos.X - newPos.X);
                                if (diff < delta.Y)
                                    delta = new Vector2(i, diff);
                            }
                        }
                    }
                    player.SetLevel((int)delta.X, levelMenu.levels.Count);
                }
                // Moving down
                if ((kbs.GetPressedKeys().Contains(Keys.S) && !pkbs.GetPressedKeys().Contains(Keys.S)) ||
                    (kbs.GetPressedKeys().Contains(Keys.Down) && !pkbs.GetPressedKeys().Contains(Keys.Down)))
                {
                    for (int i = 0; i < levelMenu.levels.Count; i++)
                    {
                        if (currentPos != levelMenu.levels.ElementAt(player.GetLevel()).pos || player.prevLevel != i)
                        {
                            newPos = levelMenu.levels.ElementAt(i).pos;
                            if (newPos.Y - currentPos.Y > 0)
                            {
                                float diff = Math.Abs(currentPos.X - newPos.X);
                                if(diff < delta.Y)
                                    delta = new Vector2(i, diff);
                            }
                        }
                    }
                    player.SetLevel((int)delta.X, levelMenu.levels.Count);
                }
                // Moving left
                if ((kbs.GetPressedKeys().Contains(Keys.A) && !pkbs.GetPressedKeys().Contains(Keys.A)) ||
                    (kbs.GetPressedKeys().Contains(Keys.Left) && !pkbs.GetPressedKeys().Contains(Keys.Left)))
                {
                    for (int i = 0; i < levelMenu.levels.Count; i++)
                    {
                        if (currentPos != levelMenu.levels.ElementAt(player.GetLevel()).pos || player.prevLevel != i)
                        {
                            newPos = levelMenu.levels.ElementAt(i).pos;
                            if (newPos.X - currentPos.X < 0)
                            {
                                float diff = Math.Abs(currentPos.Y - newPos.Y);
                                if (diff < delta.Y)
                                    delta = new Vector2(i, diff);
                            }
                        }
                    }
                    player.SetLevel((int)delta.X, levelMenu.levels.Count);
                }
                // Moving right
                if ((kbs.GetPressedKeys().Contains(Keys.D) && !pkbs.GetPressedKeys().Contains(Keys.D)) ||
                    (kbs.GetPressedKeys().Contains(Keys.Right) && !pkbs.GetPressedKeys().Contains(Keys.Right)))
                {
                    for (int i = 0; i < levelMenu.levels.Count; i++)
                    {
                        if (currentPos != levelMenu.levels.ElementAt(player.GetLevel()).pos || player.prevLevel != i)
                        {
                            newPos = levelMenu.levels.ElementAt(i).pos;
                            if (newPos.X - currentPos.X > 0)
                            {
                                float diff = Math.Abs(currentPos.Y - newPos.Y);
                                if (diff < delta.Y)
                                    delta = new Vector2(i, diff);
                            }
                        }
                    }
                    player.SetLevel((int)delta.X, levelMenu.levels.Count);
                }
                // Confirm level
                if (kbs.GetPressedKeys().Contains(Keys.Enter) && !pkbs.GetPressedKeys().Contains(Keys.Enter) && levelMenu.levels.ElementAt(player.GetLevel()).isAvailable)
                {
                    LoadLevel(player.GetLevel() + 1);
                    InitializeLevel();
                    player.Initialize();
                    chef.Initialize();
                    gameState = GameStateEnum.Playing;
                    elapsedGameTime = TimeSpan.Zero;
                }
                // Return to previous menu
                if (kbs.GetPressedKeys().Contains(Keys.Back) && !pkbs.GetPressedKeys().Contains(Keys.Back))
                {
                    gameState = GameStateEnum.MainMenu;
                }
            }
            // Start screen
            else if (gameState == GameStateEnum.StartScreen)
            {
                if (kbs.GetPressedKeys().Contains(Keys.Enter) && !pkbs.GetPressedKeys().Contains(Keys.Enter))
                {
                    gameState = GameStateEnum.Intro;
                }
            }
            // Options
            else if (gameState == GameStateEnum.Options)
            {
                if (kbs.GetPressedKeys().Contains(Keys.Back) && !pkbs.GetPressedKeys().Contains(Keys.Back))
                {
                    gameState = GameStateEnum.MainMenu;
                }
            }
            // Options
            else if (gameState == GameStateEnum.Intro)
            {
                if (kbs.GetPressedKeys().Contains(Keys.Enter) && !pkbs.GetPressedKeys().Contains(Keys.Enter))
                {
                    gameState = GameStateEnum.MainMenu;
                }
            }

            // Level completed
            if (gameState == GameStateEnum.LevelCompleted)
            {
                if (kbs.GetPressedKeys().Contains(Keys.Enter) && !pkbs.GetPressedKeys().Contains(Keys.Enter))
                {
                    LoadLevel(player.GetLevel() + 1);
                    InitializeLevel();
                    player.Initialize();
                    chef.Initialize();
                    gameState = GameStateEnum.Playing;
                    elapsedGameTime = TimeSpan.Zero;
                }
            }
            pkbs = kbs;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (gameState == GameStateEnum.Playing || gameState == GameStateEnum.Paused || gameState == GameStateEnum.GameOver || gameState == GameStateEnum.LevelCompleted)
            {
                DrawPlayingScreen();
                DrawHUD();
            }
            else if (gameState == GameStateEnum.MainMenu)
            {
                DrawMainMenuScreen();
            }
            else if (gameState == GameStateEnum.LevelSelection)
            {
                DrawLevelSelection();
            }
            else if (gameState == GameStateEnum.StartScreen)
            {
                blinkCounter += gameTime.ElapsedGameTime.Milliseconds;
                DrawStartScreen();
            }
            else if (gameState == GameStateEnum.Options)
            {
                DrawScreen();
            }
            else if (gameState == GameStateEnum.Intro)
            {
                DrawIntro();
            }
            if (gameState == GameStateEnum.GameOver || gameState == GameStateEnum.LevelCompleted || gameState == GameStateEnum.Intro)
            {
                blinkCounter += gameTime.ElapsedGameTime.Milliseconds;
            }

            // Draw cursor
            //spriteBatch.Begin();
            //spriteBatch.Draw(cursor, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Color.White);
            //spriteBatch.Draw(cursor, new Vector2(0.0f, (player.GetPosition().Y * 0.25f)), Color.White);
            //spriteBatch.DrawString(spriteFont, gameTime.TotalGameTime.ToString(), new Vector2(100.0f, 100.0f), Color.White);
            //spriteBatch.End();

            base.Draw(gameTime);
        }

        void DrawLevelSelection()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(levelMenu.background, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, new Vector2((float)width / levelMenu.background.Width, (float)totalHeight / levelMenu.background.Height), SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateScale(levelSelectionScale));
            foreach (LevelMenu.Level e in levelMenu.levels)
            {
                if (!e.isAvailable)
                    spriteBatch.Draw(uiTextures[UIEnum.Closed], new Vector2(e.pos.X / levelSelectionScale, e.pos.Y / levelSelectionScale), null, Color.White, 0.0f, new Vector2(uiTextures[UIEnum.Closed].Width / 4, uiTextures[UIEnum.Closed].Height), defaultRatio, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(player.sprites[player.state].textures.ElementAt(player.frame), Vector2.Divide(player.GetPosition(), levelSelectionScale), player.sprites[player.state].rect, Color.White, 0.0f, player.sprites[player.state].origin, 1.0f, (player.dir == Direction.Right) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0f);
            spriteBatch.End();
        }

        void DrawPlayingScreen()
        {
            float scale = 0.375f;
            Matrix translationMatrix = Matrix.CreateTranslation(0.0f, 0.0f, 0.0f);

            // Draw background
            spriteBatch.Begin();
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            spriteBatch.End();

            #region Translation matrix settings
            // X settings
            if (player.GetPosition().X - (width / scale / 2) <= 0)
                translationMatrix.M41 = 0.0f;
            else if (player.GetPosition().X + (width / scale / 2.0f) >= map.rect.Width)
                translationMatrix.M41 = (int)((-map.rect.Width) + (width / scale));
            else
                translationMatrix.M41 = (int)(-player.GetPosition().X + (width / scale / 2.0f));

            // Y settings
            if (player.GetPosition().Y - (totalHeight / scale / 1.25f) <= 0)
                translationMatrix.M42 = 0.0f;
            else if (player.GetPosition().Y + (totalHeight / scale / 5.05f) >= map.rect.Height)
                translationMatrix.M42 = (int)((-map.rect.Height) + (totalHeight / scale));
            else
                translationMatrix.M42 = (int)(-player.GetPosition().Y + (totalHeight / scale / 1.25f));
            #endregion

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, translationMatrix * Matrix.CreateScale(scale));

            // Draw editor DEVELOPMENT
            if (editor.Count > 0)
                foreach (Map.Scenery e in editor)
                    spriteBatch.Draw(sceneryTextures[e.type], e.pos, Color.White);

            // Draw scenery
            foreach (MapObject e in mapScenery)
            {
                spriteBatch.Draw(e.texture, e.pos, Color.White * e.alpha);
            }

            // Draw platforms
            foreach (MapObject e in mapPlatforms)
            {
                spriteBatch.Draw(e.sprite.texture, new Vector2(e.pos.X, e.pos.Y), Color.White * e.alpha);
            }

            // Draw items
            foreach (GameObject e in mapItems)
            {
                e.sprite.rect.X = itemSprites[e.sprite.id].rect.Width * e.frame;
                if (e.isAlive)
                    spriteBatch.Draw(e.sprite.texture, e.pos, Color.White * e.alpha);
            }

            // Draw interactives
            foreach (MapObject e in mapInteractives)
            {
                spriteBatch.Draw(e.sprite.texture, e.pos, Color.White * e.alpha);
            }

            // Draw enemies
            foreach (EnemyObject e in mapEnemies)
            {
                if (e.isAlive)
                    spriteBatch.Draw(e.sprite.textures.ElementAt(e.frame), e.pos, e.sprite.rect, Color.White, 0.0f, Vector2.Zero, 1.0f, (e.dir == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            }

            // Draw chef
            spriteBatch.Draw(chef.sprites[chef.state].textures.ElementAt(chef.frame), new Vector2((width / scale) - translationMatrix.M41 - chef.sprites[chef.state].rect.Width, chef.pos.Y - translationMatrix.M42), chef.sprites[chef.state].rect, Color.White, 0.0f, chef.sprites[chef.state].origin, 1.0f, (chef.dir == Direction.Right) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0f);

            /*for (int j = 0; j < player.sprites[player.state].textures[0].Height; j++)
            {
                for (int i = 0; i < player.sprites[player.state].textures[0].Width; i++)
                {
                    if (player.sprites[player.state].GetTextureData(player.dir)[player.frame][(player.sprites[player.state].textures[0].Width * j) + i].A != 0)
                    {
                        //spriteBatch.Draw(cover, new Vector2(i+player.GetPosition().X-player.sprites[player.state].origin.X, j+player.GetPosition().Y-player.sprites[player.state].origin.Y), Color.White);
                        spriteBatch.Draw(cover, new Vector2(i, j), Color.White);
                    }
                }
            }*/
            if (player.blink)
                spriteBatch.Draw(player.sprites[player.state].textures.ElementAt(player.frame), new Vector2((int)player.GetPosition().X, (int)player.GetPosition().Y), player.sprites[player.state].rect, Color.White * 0.5f, 0.0f, player.sprites[player.state].origin, 1.0f, (player.dir == Direction.Right) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0f);
            else
                spriteBatch.Draw(player.sprites[player.state].textures.ElementAt(player.frame), new Vector2((int)player.GetPosition().X, (int)player.GetPosition().Y), player.sprites[player.state].rect, Color.White, 0.0f, player.sprites[player.state].origin, 1.0f, (player.dir == Direction.Right) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0f);
            
            // DEVELOPMENT
            editorPos.X = (int)(Mouse.GetState().X / scale - translationMatrix.M41);
            editorPos.Y = (int)(Mouse.GetState().Y / scale - translationMatrix.M42);
            //DrawString(spriteBatch, "X: " + (Mouse.GetState().X / scale - translationMatrix.M41) + " Y: " + (Mouse.GetState().Y / scale - translationMatrix.M42) , new Vector2((width / scale) - translationMatrix.M41 - spriteFont.MeasureString(Mouse.GetState().ToString()).X * 2, totalHeight / 2 - translationMatrix.M42), 4.0f);
            spriteBatch.End();
        }

        void DrawIntro()
        {
            string skipStr = "Press ENTER to skip";
            if (blinkCounter > 750)
            {
                blinkCounter = 0;
                blink ^= true;
            }
            Vector2 scale;
            Texture2D screen = introTextures[(int)(introTimer / updateTime)];
            scale.X = (float)width / screen.Width;
            scale.Y = (float)totalHeight / screen.Height;
            spriteBatch.Begin();
            spriteBatch.Draw(screen, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            if (!blink)
                DrawString(spriteBatch, skipStr, Vector2.Zero, defaultRatio);
            spriteBatch.End();
        }

        void DrawScreen()
        {
            Vector2 scale;
            string option = startMenu.options[startMenu.selectedOption];
            Texture2D screen = screenTextures[option];
            scale.X = (float)width / screen.Width;
            scale.Y = (float)totalHeight / screen.Height;
            spriteBatch.Begin();
            spriteBatch.Draw(screen, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        void DrawHUD()
        {
            int x = 0;
            float scale = 1.0f;
            float itemsWidth = 0.0f;
            Texture2D screen;
            Vector2 screenScale;
            foreach (KeyValuePair<Enum, Sprite> e in itemSprites)
            {
                scale = hudHeight * 0.75f / e.Value.rect.Height;
                itemsWidth += e.Value.rect.Width * scale;
            }
            itemsWidth /= 2.0f;
            x = (int)(- itemsWidth + width / 2);
            string pauseStr = "Pause";
            string nextLevelStr = "Press ENTER to continue";
            string healthStr = "Health: " + player.GetHealth() + "%";
            spriteBatch.Begin();
            DrawString(spriteBatch, "Time: " + time, new Vector2(width / 2 - spriteFont.MeasureString("Time: 0.00").X / 2, 0.0f), 0.75f);
            DrawString(spriteBatch, healthStr, Vector2.Zero, 0.75f);

            // Draw items needed
            foreach (KeyValuePair<Enum, Sprite> e in itemSprites)
            {
                string counter = itemCounter[(CollectibleEnum)e.Value.id][1].ToString();
                Vector2 position = new Vector2(x, graphics.PreferredBackBufferHeight - hudHeight * 0.9f);
                scale = hudHeight * 0.75f / e.Value.rect.Height;
                if (itemCounter[(CollectibleEnum)e.Value.id][1] == 0)
                    spriteBatch.Draw(e.Value.texture, position, e.Value.rect, Color.White * 0.5f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
                else
                    spriteBatch.Draw(e.Value.texture, position, e.Value.rect, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
                DrawString(spriteBatch, counter, new Vector2(x - spriteFont.MeasureString(counter).X + e.Value.rect.Width * scale, position.Y), 1.0f);
                x += (int)(Math.Ceiling(e.Value.rect.Width * scale));
            }

            // Game paused
            if (gameState == GameStateEnum.Paused)
            {
                spriteBatch.Draw(uiTextures[UIEnum.Cover], GraphicsDevice.Viewport.Bounds, Color.White * 0.5f);
                DrawString(spriteBatch, pauseStr, new Vector2(graphics.PreferredBackBufferWidth / 2 - spriteFont.MeasureString(pauseStr).X / 2, graphics.PreferredBackBufferHeight / 2 - spriteFont.MeasureString(pauseStr).Y / 2), 1.0f);
            }
            // Game over
            else if (gameState == GameStateEnum.GameOver)
            {
                screen = uiTextures[UIEnum.GameOver];
                string restartStr = "Press R to restart the level";
                screenScale.X = (float)width / screen.Width;
                screenScale.Y = (float)totalHeight / screen.Height;
                if (blinkCounter > 750)
                {
                    blinkCounter = 0;
                    blink ^= true;
                }
                spriteBatch.Draw(screen, GraphicsDevice.Viewport.Bounds, Color.White);
                if (!blink)
                    DrawString(spriteBatch, restartStr, new Vector2(graphics.PreferredBackBufferWidth / 2 - spriteFont.MeasureString(restartStr).X / 2, graphics.PreferredBackBufferHeight / 2 + spriteFont.MeasureString(restartStr).Y), 1.0f);
            }
            // Level completed
            else if (gameState == GameStateEnum.LevelCompleted)
            {
                screen = uiTextures[UIEnum.LevelCompleted];
                screenScale.X = (float)width / screen.Width;
                screenScale.Y = (float)totalHeight / screen.Height;
                if (blinkCounter > 750)
                {
                    blinkCounter = 0;
                    blink ^= true;
                }
                spriteBatch.Draw(uiTextures[UIEnum.LevelCompleted], GraphicsDevice.Viewport.Bounds, Color.White);
                DrawString(spriteBatch, accumStr, Vector2.Multiply(new Vector2(570.0f, 360.0f), defaultRatio), 1.75f * defaultRatio);
                DrawString(spriteBatch, points.ToString(), Vector2.Multiply(new Vector2(1015.0f, 814.0f), defaultRatio), 1.75f * defaultRatio);
                if (!blink)
                    DrawString(spriteBatch, nextLevelStr, new Vector2(graphics.PreferredBackBufferWidth / 2 - spriteFont.MeasureString(nextLevelStr).X / 2, graphics.PreferredBackBufferHeight * 0.75f), 1.0f);
            }
            spriteBatch.Draw(brush.texture, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), null, Color.White, 0.0f, Vector2.Zero, 0.375f, SpriteEffects.None, 0);
            DrawString(spriteBatch, Mouse.GetState().ToString(), new Vector2(0.0f, 300.0f), 1.75f);
            spriteBatch.End();
        }

        void DrawMainMenuScreen()
        {
            Vector2 scale;
            scale.X = (float)width / startMenu.background.Width;
            scale.Y = (float)totalHeight / startMenu.background.Height;
            spriteBatch.Begin();
            spriteBatch.Draw(rainbow, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            spriteBatch.Draw(startMenu.background, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            foreach (KeyValuePair<String, Texture2D> e in menuTextures)
            {
                if (e.Key.Equals(startMenu.options[startMenu.selectedOption]))
                    spriteBatch.Draw(menuHiTextures[e.Key], new Vector2(0.0f, -50.0f), null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
                else
                    spriteBatch.Draw(e.Value, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
            spriteBatch.End();
        }

        void DrawStartScreen()
        {
            string startStr = "Press ENTER to start the game";
            if (blinkCounter > 750)
            {
                blinkCounter = 0;
                blink ^= true;
            }
            Vector2 scale;
            scale.X = (float)width / startScreen.Width;
            scale.Y = (float)totalHeight / startScreen.Height;
            spriteBatch.Begin();
            spriteBatch.Draw(startScreen, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            if (!blink)
                DrawString(spriteBatch, startStr, new Vector2(graphics.PreferredBackBufferWidth / 2 - spriteFont.MeasureString(startStr).X / 2, graphics.PreferredBackBufferHeight / 2), 1.0f);
            spriteBatch.End();
        }

        void UpdatePlayerPosition()
        {
            player.prevPos = player.pos;

            if (gameState == GameStateEnum.Playing)
            {
                if (!(kbs.IsKeyDown(Keys.Left) || kbs.IsKeyDown(Keys.Right) || kbs.IsKeyDown(Keys.A) || kbs.IsKeyDown(Keys.D)))
                {
                    if (player.dir == Direction.Left)
                    {
                        if (player.state == PlayerSpriteEnum.Jumping)
                            player.UpdateVelocity(Vector2.Divide(friction, 1.25f));
                        else
                            player.UpdateVelocity(friction);
                        if (player.velocity.X > 0)
                            player.velocity.X = 0;
                    }
                    else
                    {
                        if (player.state == PlayerSpriteEnum.Jumping)
                            player.UpdateVelocity(Vector2.Divide(-friction, 1.25f));
                        else
                            player.UpdateVelocity(-friction);
                        if (player.velocity.X < 0)
                            player.velocity.X = 0;
                    }
                }

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

                // Player fell off-map
                if (player.GetPosition().Y - player.sprites[player.state].rect.Height > map.rect.Height)
                    player.UpdateHealth(-100.0f);
            }
            else if (gameState == GameStateEnum.LevelSelection)
            {
                Vector2 v1, v2, v3;
                v1 = new Vector2(levelMenu.levels.ElementAt(player.GetLevel()).pos.X - player.GetPosition().X, levelMenu.levels.ElementAt(player.GetLevel()).pos.Y - player.GetPosition().Y);
                v2 = v1;
                v1.Normalize();
                v1 *= player.speed / 2;
                v3 = (v1.Length() < v2.Length()) ? v1 : v2;
                if (player.pos != levelMenu.levels.ElementAt(player.GetLevel()).pos)
                {
                    Vector2 moveTo = new Vector2(v3.X * levelSelectionScale, v3.Y * levelSelectionScale);
                    if (Math.Abs(moveTo.X) < 1)
                        moveTo.X = 1 * Math.Sign(moveTo.X);
                    if (Math.Abs(moveTo.Y) < 1)
                        moveTo.Y = 1 * Math.Sign(moveTo.Y);
                    player.SetVelocity(moveTo);
                }
                else
                {
                    player.SetVelocity(Vector2.Zero);
                    player.prevLevel = player.GetLevel();
                }
            }
            player.SetPosition(Vector2.Add(player.GetVelocity(), player.pos));
            //player.pos = Vector2.Add(player.GetVelocity(), player.pos);
        }

        void UpdatePlayerAnimation()
        {
            player.UpdateDirection();
            if (player.health <= 0)
            {
                player.state = PlayerSpriteEnum.Dying;
            }
            else
            {
                if (player.pos != player.prevPos)
                {
                    if (player.GetPosition().Y != player.prevPos.Y && gameState == GameStateEnum.Playing)
                        player.state = PlayerSpriteEnum.Jumping;
                    else if (player.GetPosition().X != player.prevPos.X && !player.isJumping)
                        player.state = PlayerSpriteEnum.Walking;
                }
                else if (!player.isJumping && player.state != PlayerSpriteEnum.Handing)
                {
                    player.state = PlayerSpriteEnum.Standing;
                }
            }

            if (player.state != player.prevState)
            {
                player.frame = 0;
                player.prevState = player.state;
            }

            if (elapsedTime >= gameSpeed)
            {
                player.frame = (player.frame + 1) % player.sprites[player.state].frames;
                if (player.state == PlayerSpriteEnum.Jumping)
                {
                    if ((player.GetPosition().Y - player.prevPos.Y) < 0 && player.frame > 4)
                        player.frame = 4;
                    if ((player.GetPosition().Y - player.prevPos.Y) > 0 && player.frame > 9 && player.isFalling)
                        player.frame = 9;
                }
                //player.sprites[player.state].rect.X = player.sprites[player.state].rect.Width * player.frame;
            }
        }

        void UpdateChefAnimation()
        {
            if (chef.isAttacking)
                chef.state = ChefSpriteEnum.Activating;
            else
                chef.state = ChefSpriteEnum.Standing;

            if (chef.state != chef.prevState)
            {
                chef.frame = 0;
                chef.prevState = chef.state;
            }

            if (elapsedTime >= gameSpeed)
            {
                chef.frame = (chef.frame + 1) % chef.sprites[chef.state].frames;
                if (chef.state == ChefSpriteEnum.Activating && chef.frame >= chef.sprites[chef.state].frames - 1)
                {
                    chef.state = ChefSpriteEnum.Standing;
                    int rand = random.Next(objectInfo.interactives.Count - 1);
                    MapObject interactive = new MapObject(mapSprites.ElementAt(rand).Value, Vector2.Zero, objectInfo.interactives.ElementAt(rand).Value.speed, objectInfo.interactives.ElementAt(rand).Value.damage);
                    interactive.SetPosition(new Vector2(player.GetPosition().X - interactive.sprite.rect.Width / 2, -interactive.sprite.rect.Height));
                    mapInteractives.Add(interactive);
                    if (random.NextDouble() > 0.7f)
                        chef.isAttacking = true;
                    else
                        chef.isAppearing = false;
                }
            }
        }

        void UpdateChefPosition()
        {
            if (chef.isAppearing)
                chef.pos.Y += chef.speed;
            else
                chef.pos.Y -= chef.speed;

            if (chef.pos.Y >= -200)
            {
                chef.pos.Y = -200;
                chef.isAttacking = true;
            }
            else
                chef.isAttacking = false;

            if (chef.pos.Y <= -chef.sprites[chef.state].rect.Height)
            {
                chef.pos.Y = -chef.sprites[chef.state].rect.Height;
                chef.canAppear = true;
            }
        }

        void CheckPlayerCollisions()
        {
            Rectangle playerRect = new Rectangle((int)(player.GetPosition().X - player.sprites[player.state].origin.X), (int)(player.GetPosition().Y - player.sprites[player.state].origin.Y), player.sprites[player.state].rect.Width, player.sprites[player.state].rect.Height);
            
            // Collision with items
            foreach (GameObject e in mapItems)
            {
                Rectangle itemRect = new Rectangle((int)(e.pos.X), (int)(e.pos.Y), e.sprite.rect.Width, e.sprite.rect.Height);
                if (playerRect.Intersects(itemRect))
                {
                    if (IntersectPixels(playerRect, player.sprites[player.state].GetTextureData(player.dir)[player.frame], itemRect, e.sprite.GetTextureData(1)[e.frame]) && e.isAlive)
                    {
                        e.collision = true;
                        e.isAlive = false;
                        itemCounter[(CollectibleEnum)e.sprite.id][1]--;
                        if (itemCounter[(CollectibleEnum)e.sprite.id][1] < 0)
                            itemCounter[(CollectibleEnum)e.sprite.id][1] = 0;
                    }
                    else
                    {
                        e.collision = false;
                    }
                }
            }
            //mapItems.RemoveAll(p => p.isAlive == false);

            // Collision with enemies
            foreach (EnemyObject e in mapEnemies)
            {
                Rectangle enemyRect = new Rectangle((int)(e.pos.X), (int)(e.pos.Y), e.sprite.rect.Width, e.sprite.rect.Height);
                if (IntersectPixels(playerRect, player.sprites[player.state].GetTextureData(player.dir)[player.frame], enemyRect, e.sprite.GetTextureData(e.dir)[e.frame]) && e.isAlive)
                {
                    e.collision = true;
                    if (collisionPoint.X > player.GetPosition().X - player.sprites[player.state].rect.Width / 4 &&
                        collisionPoint.X < player.GetPosition().X + player.sprites[player.state].rect.Width / 4 &&
                        collisionPoint.Y > player.GetPosition().Y - enemyRect.Height &&
                        collisionPoint.Y < player.GetPosition().Y &&
                        collisionPoint.Y > e.pos.Y &&
                        collisionPoint.Y < e.pos.Y + e.sprite.rect.Height &&
                        player.isFalling)
                        e.isAlive = false;
                    if (!player.isInvincible && e.isAlive && player.state != PlayerSpriteEnum.Handing)
                    {
                        player.UpdateHealth(-e.damage);
                        if (player.GetHealth() > 0)
                            player.isInvincible = true;
                    }
                }
                e.collision = false;
            }
            //mapEnemies.RemoveAll(p => p.isAlive == false);

            // Collision with map platforms
            player.isFalling = true;
            if (player.GetPosition().Y - player.prevPos.Y >= 0)
            {
                foreach (MapObject m in mapPlatforms)
                {
                    Rectangle objectRect = new Rectangle((int)m.pos.X, (int)m.pos.Y, m.sprite.rect.Width, 1);
                    if (player.Collided(objectRect))
                        player.isFalling = false;
                    if (!player.isFalling)
                    {
                        player.isJumping = false;
                        player.canJump = true;
                        player.SetPosition(new Vector2(player.GetPosition().X, objectRect.Bottom));
                        player.ResetJump();
                        break;
                    }
                }
            }

            // Collision with map interactives
            foreach (MapObject e in mapInteractives)
            {
                Rectangle interactiveRect = new Rectangle((int)(e.pos.X), (int)(e.pos.Y), e.sprite.rect.Width, e.sprite.rect.Height);
                if (IntersectPixels(playerRect, player.sprites[player.state].GetTextureData(player.dir)[player.frame], interactiveRect, e.sprite.GetTextureData(1)[e.frame]))
                {
                    if ((MapEnum)e.sprite.id == MapEnum.Oven)
                    {
                        if (map.isCompleted && !player.isFalling)
                        {
                            player.dir = e.dir;
                            player.state = PlayerSpriteEnum.Handing;
                            player.SetPosition(new Vector2(e.pos.X - 120, player.GetPosition().Y));
                            player.SetVelocity(Vector2.Zero);
                        }
                    }
                    else if (e.isAlive)
                    {
                        if (!player.isInvincible && player.state != PlayerSpriteEnum.Handing)
                        {
                            player.UpdateHealth(-e.damage);
                            if (player.GetHealth() > 0)
                                player.isInvincible = true;
                            e.isAlive = false;
                        }
                    }
                }
            }

            // Collision with map bounds
            if (player.GetPosition().X - player.sprites[player.state].rect.Width / 2 <= 0)
            {
                player.SetVelocity(Vector2.Zero);
                player.SetPosition(new Vector2(player.sprites[player.state].rect.Width / 2, player.GetPosition().Y));
                if (player.state == PlayerSpriteEnum.Walking)
                    player.state = PlayerSpriteEnum.Standing;
            }
            else if (player.GetPosition().X + player.sprites[player.state].rect.Width / 2 >= map.rect.Width)
            {
                player.SetVelocity(Vector2.Zero);
                player.SetPosition(new Vector2(map.rect.Width - player.sprites[player.state].rect.Width / 2, player.GetPosition().Y));
                if (player.state == PlayerSpriteEnum.Walking)
                    player.state = PlayerSpriteEnum.Standing;
            }
        }

        void CheckEnemiesCollisions()
        {
            // Collision with map platforms
            foreach (EnemyObject e in mapEnemies)
            {
                e.isFalling = true;
                if (e.pos.Y - e.prevPos.Y >= 0)
                {
                    foreach (MapObject m in mapPlatforms)
                    {
                        Rectangle objectRect = new Rectangle((int)m.pos.X, (int)m.pos.Y, m.sprite.rect.Width, 1);
                        if (e.Collided(objectRect))
                        {
                            e.isFalling = false;
                            e.pos.Y = - e.sprite.rect.Height + m.pos.Y + 1;
                        }
                    }
                }
            }
        }

        public bool IntersectPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
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
                        collisionPoint = new Point(x, y);
                        return true;
                    }
                }
            }
 
            // No intersection found
            return false;
        }

        void DrawString(SpriteBatch spriteBatch, String s, Vector2 pos, float scale)
        {
            spriteBatch.DrawString(spriteFont, s, new Vector2(pos.X - 1, pos.Y), Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(spriteFont, s, new Vector2(pos.X + 1, pos.Y), Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(spriteFont, s, new Vector2(pos.X, pos.Y - 1), Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(spriteFont, s, new Vector2(pos.X, pos.Y + 1), Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(spriteFont, s, pos, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
}
