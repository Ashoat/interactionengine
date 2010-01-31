using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using InteractionEngine.Constructs.Datatypes;
using System;
using System.Diagnostics;

namespace GlobalGameJam.GameObjects {

    public abstract class NPC : Character {


        public int Level {
            get { return level; }
            set { level = value; }
        }

        private int level = 1;
        public bool running;
        private float lineOfSight = 2;
        private int runSquaresLeft;
        private Direction moveDirection;

        public override void construct() {
            base.construct();
            this.attackStrength.value = attackStrength.value * Level;
            this.Health = Health * Level;
        }

        public override void update() {
            base.update();
            if (this.busyPerformingAction.value > 0 || Health <= 0) return;
            List<Character> chars = Map.getVisibleCharacters(this.Position, lineOfSight);
            foreach (Character character in chars) {
                //if (characterType.getAttitudeToward(character.characterType) < 0) {
                int relation = characterType.getAttitudeToward(character);
                //Console.Write(this.classHash + " sees " + character.classHash);
                //Console.Write("Coords: " + this.Position);
                //Console.WriteLine("; Attitude: " + relation.ToString());
                
                if (relation > 0) {
                    if (MathHelper.PointDistance(this.Position, character.Position) <= 1.0f) {
                        // Attack
                        //Debug.WriteLine("Attacking");
                        this.attack(character);
                    } else {
                        //Begin attack run
                        //Debug.WriteLine("Moving to Attack");
                    }
                    moveDirection = Map.getDirection(this.Position, character.Position);
                    running = true;
                    runSquaresLeft = 2;
                } else if (relation < 0) {
                    // Begin defense run
                    //Debug.WriteLine("Running away");
                    running = true;
                    runSquaresLeft = 2;
                    moveDirection = Map.getDirection(character.Position,this.Position);
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

    }

}