using System;
using InteractionEngine.UserInterface.TwoDimensional;
using InteractionEngine.Constructs;
using InteractionEngine;
using GlobalGameJam.GameObjects;

namespace GlobalGameJam {

    static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args) {
            // Set up the UI.
            Engine.userInterface = new UserInterface2D();
            ((UserInterface2D)Engine.userInterface).setWindowSize(800, 600);

            // Set up the engine
            Engine.status = Engine.Status.SINGLE_PLAYER;

            // Set up the local LoadRegion with its buttons.
            LoadRegion localRegion = LoadRegion.createLoadRegion();

            Game game = GameObject.createGameObject<Game>(localRegion);
            HUD hud = GameObject.createGameObject<HUD>(localRegion);
            Map map = GameObject.createGameObject<Map>(localRegion);
            map.LoadMap("levels/level1.txt");
            // Annoyance: Static constructors aren't called unless I bother the class first. Ugh.
            new Game();
            new HUD();
            new Punk();
            new Skunk();
            new Monk();
            new Player();
            new Wall();
            new Map();

            // Go!
            Engine.run();
        }

    }


}