using System;
using InteractionEngine.UserInterface.TwoDimensional;
using InteractionEngine.Constructs;
using InteractionEngine;

namespace GlobalGameJam {

    static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args) {
            // Set up the UI.
            Engine.userInterface = new UserInterface2D();
            ((UserInterface2D)Engine.userInterface).setWindowSize(430, 430);

            // Set up the engine
            Engine.status = Engine.Status.SINGLE_PLAYER;

            // Set up the local LoadRegion with its buttons.
            LoadRegion localRegion = LoadRegion.createLoadRegion();

            // Annoyance: Static constructors aren't called unless I bother the class first. Ugh.

            // Go!
            Engine.run();
        }

    }


}