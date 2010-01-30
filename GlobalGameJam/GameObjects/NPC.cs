using InteractionEngine.Constructs;
using InteractionEngine.UserInterface;
using Microsoft.Xna.Framework.Input;
using GlobalGameJam.Graphics;
using System.Collections.Generic;

namespace GlobalGameJam.GameObjects {

    public abstract class NPC : Character {

        // return negative if flee, positive if chase+kill, and zero if neutral
        public abstract int attitudeToward(Character cohabitant);

        public override void update() {
            if (this.busyPerformingAction.value > 0) return;
            List<Character> chars = map.getVisibleCharacters(this.position, 1);
            foreach (Character character in chars) {
                if (attitudeToward(character) < 0) {
                    this.move(this.position.X - character.position.X, this.position.Y - character.position.Y);
                }
            }
        }

    }

}