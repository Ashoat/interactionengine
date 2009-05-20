using InteractionEngine;
using InteractionEngine.Constructs;
using InteractionEngine.EventHandling;
using InteractionEngine.Networking;
using InteractionEngine.UserInterface.TwoDimensional;
using System;
using System.Collections.Generic;

namespace TestNetworkGame {

    public class main {

        public static void Main() {
            // Set up XNA.
            Engine.game = new InteractionGame();
            // Set up the UI.
            Engine.userInterface = new UserInterface2D();
        }

    }

}