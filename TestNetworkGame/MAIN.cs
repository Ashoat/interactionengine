using InteractionEngine;
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
            // Set up the LoadRegion.
            LoadRegion loadRegion = LoadRegion.createLoadRegion();
            // Set up the GameField.
            GameField gameField = GameObject.createGameObject<GameField>(loadRegion);
            const int offset = 56;
            const int padding = 108;
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    // Give us an O!
                    O firstO = GameObject.createGameObject<O>(loadRegion);
                    firstO.setPosition(offset + padding * i, offset + padding * j);
                    firstO.getGamePiece().display.value = false;
                    // Give us an X!
                    X firstX = GameObject.createGameObject<X>(loadRegion);
                    firstX.setPosition(offset + padding * i, offset + padding * j);
                    firstX.getGamePiece().display.value = false;
                    // Give us a ClickySpot!
                    ClickySpot firstClicky = GameObject.createGameObject<ClickySpot>(loadRegion);
                    firstClicky.setPosition(offset + padding * i, offset + padding * j);
                    firstClicky.relateToGamePieces(firstO, firstX);
                }
            }
            // Go!
            Engine.game.Run();
        }

    }

}