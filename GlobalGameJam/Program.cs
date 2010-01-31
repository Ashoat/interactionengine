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
            GalactazoidsGame game = GameObject.createGameObject<GalactazoidsGame>(localRegion);
            HUD hud = GameObject.createGameObject<HUD>(localRegion);
            FrameRateCounter counter = GameObject.createGameObject<FrameRateCounter>(localRegion);
            Menu menu = GameObject.createGameObject<Menu>(localRegion);
            game.setMenu(menu); // so that keyboard commands go to menu
            menu.game = game;
            menu.show();
            Audio.loadAudioSettings("ggj");
            audio = new Audio(null, "Wave Bank", "Sound Bank");

            
            
            KeyboardFocus kf = GameObject.createGameObject<KeyboardFocus>(localRegion);
            ((UserInterface2D)Engine.userInterface).registerKeyboardFocus(kf);
            
            kf.setFocus(game);
            
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