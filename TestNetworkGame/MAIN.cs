﻿using InteractionEngine;
using InteractionEngine.Constructs;
using InteractionEngine.EventHandling;
using InteractionEngine.Networking;
using InteractionEngine.UserInterface.TwoDimensional;
using System;
using System.Collections.Generic;
using TestNetworkGame.Logic;

namespace TestNetworkGame {

    public class main {

        public static void Main() {
            // Set up XNA.
            Engine.game = new InteractionGame();
            // Set up the UI.
            Engine.userInterface = new UserInterface2D();
            ((UserInterface2D)Engine.userInterface).setWindowSize(430, 430);
            // Set up the local LoadRegion with its buttons.
            LoadRegion localRegion = LoadRegion.createLoadRegion();
            Host hostButton = GameObject.createGameObject<Host>(localRegion);
            hostButton.setPosition(50, 400);
            Join joinButton = GameObject.createGameObject<Join>(localRegion);
            joinButton.setPosition(350, 400);
            hostButton.associateJoin(joinButton);
            joinButton.associateHost(hostButton);
            // Annoyance: Static constructors aren't called unless I bother the class first. Ugh.
            new GameField();
            new ClickySpot();
            new O();
            new X();
            // Go!
            Engine.game.Run();
        }

    }

}