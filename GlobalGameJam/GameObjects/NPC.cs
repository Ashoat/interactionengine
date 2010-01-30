using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using System.Collections.Generic;

namespace GlobalGameJam.GameObjects {

    public abstract class NPC : Character {

        public override void update() {
            base.update();
            if (this.busyPerformingAction.value > 0) return;
            List<Character> chars = Map.getVisibleCharacters(this.position, 1);
            foreach (Character character in chars) {
                //if (characterType.getAttitudeToward(character.characterType) < 0) {
                if (character is Player) {
                    this.move(this.position.X - character.position.X, this.position.Y - character.position.Y);
                }
            }
        }

    }

}