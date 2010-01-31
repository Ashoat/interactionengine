using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using InteractionEngine.Constructs.Datatypes;
using System;

namespace GlobalGameJam.GameObjects {

    public abstract class NPC : Character {
        public bool running;
        private int runSquaresLeft;
        private Direction moveDirection;
        public override void update() {
            base.update();
            if (this.busyPerformingAction.value > 0 || Health < 0) return;
            List<Character> chars = Map.getVisibleCharacters(this.position, 1);
            foreach (Character character in chars) {
                //if (characterType.getAttitudeToward(character.characterType) < 0) {
                int relation = characterType.getAttitudeToward(character);
                Console.Write(this.classHash + " sees " + character.classHash);
                Console.WriteLine("; Attitude: " + relation.ToString());
                
                if (relation > 0) {
                    if (MathHelper.PointDistance(this.position, character.position) <= 1.0f) {
                        // Attack
                        this.attack(character);
                    } else {
                        //Begin attack run
                        moveDirection = getDirection(this.position,character.position);
                        running = true;
                        runSquaresLeft = 2;
                    }
                } else if (relation < 0) {
                    // Begin defense run
                    running = true;
                    runSquaresLeft = 2;
                    moveDirection = getDirection(character.position,this.position);
                } else {
                    // Don't update motion pattern
                }
            }
            if (running) {
                runSquaresLeft--;
                this.move(moveDirection);
                if (runSquaresLeft == 0) {
                    running = false;
                }
            }
        }

        private Direction getDirection(Point start, Point end) {
            if (end.X - start.X > 0)
                return Direction.EAST;
            if (end.X - start.X < 0)
                return Direction.WEST;
            if (end.Y - start.Y > 0)
                return Direction.SOUTH;
            if (end.Y - start.Y < 0)
                return Direction.NORTH;
            throw new InvalidProgramException("The two points aren't above, below or to the side of each other!");
        }

    }

}