using System;
using InteractionEngine.UserInterface.TwoDimensional;
using InteractionEngine.Constructs;
using InteractionEngine;
using GlobalGameJam.GameObjects;
using InteractionEngine.UserInterface;
using InteractionEngine.UserInterface.Audio;
using Microsoft.Xna.Framework;

namespace GlobalGameJam {

    static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static Audio audio;

        static void Main(string[] args) {
            // Set up the UI.
            Engine.userInterface = new UserInterface2D();
            ((UserInterface2D)Engine.userInterface).setWindowSize(800, 600);

            // Set up the engine
            Engine.status = Engine.Status.SINGLE_PLAYER;

            // Set up the local LoadRegion with its buttons.
            LoadRegion localRegion = LoadRegion.createLoadRegion();
            LoadRegion syncedRegion = LoadRegion.createLoadRegion();

            GalactazoidsGame game = GameObject.createGameObject<GalactazoidsGame>(localRegion);
            Map map = GameObject.createGameObject<Map>(syncedRegion);
            HUD hud = GameObject.createGameObject<HUD>(localRegion);
            FrameRateCounter counter = GameObject.createGameObject<FrameRateCounter>(localRegion);
            HealthBar health = GameObject.createGameObject<HealthBar>(localRegion);
            health.setLocationAndMap(new Vector3(15, 29, 0), map);
            game.addMap(map);


            Audio.loadAudioSettings("ggj");
            audio = new Audio(null, "Wave Bank", "Sound Bank");

            Menu menu = GameObject.createGameObject<Menu>(localRegion);
            game.setMenu(menu); // so that keyboard commands go to menu
            menu.show();

            
            KeyboardFocus kf = GameObject.createGameObject<KeyboardFocus>(localRegion);
            ((UserInterface2D)Engine.userInterface).registerKeyboardFocus(kf);
            
            kf.setFocus(game);
            
            map.LoadMap("levels/level1.ani");
            
            // Annoyance: Static constructors aren't called unless I bother the class first. Ugh.
            new GalactazoidsGame();
            new HUD();
            new Punk();
            new Skunk();
            new Monk();
            new Player();
            new Wall();
            new Map();
            new KeyboardFocus();

            // Go!
            Engine.run();
        }

    }


}