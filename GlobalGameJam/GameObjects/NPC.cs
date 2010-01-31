﻿using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using InteractionEngine.Constructs.Datatypes;
using System;
using System.Diagnostics;
using InteractionEngine;
using Microsoft.Xna.Framework.Graphics;

namespace GlobalGameJam.GameObjects {

    public abstract class NPC : Character {


        public int Level {
            get { return level; }
            set { 
                level = value;
                switch (value)
                {
                    case 1:
                        graphics.Tint = Color.White;
                        break;
                    case 2:
                        graphics.Tint = Color.LightSeaGreen;
                        break;
                    case 3:
                        graphics.Tint = Color.Salmon;
                        break;
                }
            
            }
        }

        private int defaultRunSquares = 3;
        private int level = 1;
        private float lineOfSight = 3;
        private int runSquaresLeft;
        private Direction moveDirection;
        private Entity attackTarget;
        private int attackQueued;

        public override void construct() {
            base.construct();
            this.attackStrength.value = attackStrength.value * Level;
            this.Health = Health * Level;
        }

        public override EntityGraphics makeGraphics()
        {
            return new NPCGraphics(this);
        }

        public override void update() {
            /*
            if (start == null) {
                start = Engine.gameTime.TotalRealTime;
            }
            TimeSpan delta = Engine.gameTime.TotalRealTime - start;
            //Console.WriteLine(delta);
            if (delta.Seconds >= 1) {
                start = Engine.gameTime.TotalRealTime;
                //Console.WriteLine("NPC ticks/sec: " + counter);
                counter = 0;
            }
            counter++;
             * */

            base.update();
            
            if (Busy || Health <= 0) return;
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
                    moveDirection = Map.getDirection(this.Position, character.Position, this.Direction);
                    this.move(moveDirection);
                    runSquaresLeft = defaultRunSquares;
                } else if (relation < 0) {
                    // Begin defense run
                    //Debug.WriteLine("Running away");
                    
                    moveDirection = Map.getDirection(character.Position, this.Position, character.Direction);
                    double newDistance = MathHelper.PointDistance(Map.getPointInDirection(this.Position, moveDirection), character.Position);
                    double oldDistance = MathHelper.PointDistance(this.Position, character.Position);
                    if (newDistance > oldDistance) {
                        bool canMove = this.move(moveDirection);
                        runSquaresLeft = defaultRunSquares;
                    } else runSquaresLeft = 0;
                    //bool inRange = MathHelper.PointDistance(this.Position, character.Position) <= 1.0f;
                    //if (!canMove && !Busy && inRange) this.attack(character);
                } else {
                    // Don't update motion pattern
                }
            }

            if (!Busy && !running && runSquaresLeft > 0) {
                this.move(moveDirection);
                runSquaresLeft--;
            }

            if (attackTarget != null) attackQueued -= Engine.gameTime.ElapsedGameTime.Milliseconds;
            if (!Busy && attackTarget != null && attackQueued <= 0) {
                attack(attackTarget);
                attackTarget = null;
            }
                
        }

        public override void wasAttacked(Character attacker, int damage) {
            base.wasAttacked(attacker, damage);
            if (!Busy) {
                this.attackTarget = attacker;
                this.attackQueued = attackDelay / 2;
            }
        }

    }

}