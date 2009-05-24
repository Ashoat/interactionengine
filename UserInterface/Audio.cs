/*••••••••••••••••••••••••••••••••••••••••*\
| Interaction Engine                       |
| (C) Copyright Bluestone Coding 2009      |
|••••••••••••••••••••••••••••••••••••••••••|
|           __    ___ ___  ___             |
|          /++\  | _ ) __|/ __|            |
|          \++/  | _ \__ \ (__             |
|           \/   |___/___/\___|            |
|                                          |
|••••••••••••••••••••••••••••••••••••••••••|
| CLIENT                                   |
| * Audio                        Class     |
| * Audible                      Interface |
\*••••••••••••••••••••••••••••••••••••••••*/

namespace InteractionEngine.UserInterface.Audio {

    /**
     * Holds all information and methods regarding the sounds a GameObject makes.
     */
    public class Audio {

        // Contains a reference to XNA's audio engine.
        // Used for processing and actually outputting sounds.
        private static Microsoft.Xna.Framework.Audio.AudioEngine audioEngine;
        
        /// <summary>
        /// Load the audio settings and instantiate the AudioEngine.
        /// </summary>
        /// <param name="xgs">The XACT settings file path.</param>
        public static void loadAudioSettings(string xgs) {
            audioEngine = new Microsoft.Xna.Framework.Audio.AudioEngine(xgs);
        }

        // Contains the GameObject this module is associated with.
        // Used for referencing and interfacing with the GameObject.
        public readonly Audible gameObject;
        // Contains a reference to an XNA wave bank for this GameObject.
        // Used as a central location for wave files, and is mostly abstracted by the soundBank.
        protected Microsoft.Xna.Framework.Audio.WaveBank waveBank;
        // Contains a references to an XNA sound bank for this GameObject.
        // Used as a central location for all possible sounds that this GameObject can omit. 
        protected Microsoft.Xna.Framework.Audio.SoundBank soundBank;
        
        /// <summary>
        /// Construct the Audio module.
        /// </summary>
        /// <param name="gameObject">The GameObject this module represents.</param>
        public Audio(Audible gameObject, string xwb, string xsb) {
            this.gameObject = gameObject;
            waveBank = new Microsoft.Xna.Framework.Audio.WaveBank(audioEngine, xwb);
            soundBank = new Microsoft.Xna.Framework.Audio.SoundBank(audioEngine, xsb);
        }

        /// <summary>
        /// Start playing a sound.
        /// </summary>
        /// <param name="sound">The string identifying the sound in one the SoundBanks that is being executed.</param>
        public virtual void playSound(string soundIdentifier) {
            soundBank.GetCue(soundIdentifier).Play();
        }

    }

    /**
     * Implemented by GameObjects that can make sounds.
     */
    public interface Audible : InteractionEngine.Constructs.Locatable {

        /// <summary>
        /// Gets the Audio class from GameObject.
        /// </summary>
        /// <returns>The Audio class.</returns>
        Audio getAudio();

    }

    /**
     * Holds all information and methods regarding the sounds a GameObject in a 3D GameWorld makes.
     */
    public class Audio3D : Audio {

        // Contains a reference to the Camera for this 3D game.
        // Used for calculating the volume and direction this sound is coming from in relation to the camera.
        private InteractionEngine.UserInterface.ThreeDimensional.Camera camera;
        // Contains a list of all the actively playing sounds.
        // Used so that we know which sounds need to be calculated.
        private System.Collections.Generic.List<Microsoft.Xna.Framework.Audio.Cue> activeSounds = new System.Collections.Generic.List<Microsoft.Xna.Framework.Audio.Cue>();

        /// <summary>
        /// Construct the Audio3D module.
        /// </summary>
        /// <param name="gameObject">The GameObject this module represents.</param>
        /// <param name="xsb">The path to the XACT WaveBank for this GameObject.</param>
        /// <param name="xwb">The path to the XACT SoundBank for this GameObject.</param>
        public Audio3D(Audible gameObject, string xwb, string xsb) : base(gameObject, xwb, xsb) {
        }

        /// <summary>
        /// The whole point of 3D Audio is so we can figure out volume and stuff based on how far this object is from the camera.
        /// Hence... we need a camera. Otherwise, all sounds output equally loud.
        /// </summary>
        /// <param name="camera">The camera.</param>
        public void setCamera(InteractionEngine.UserInterface.ThreeDimensional.Camera camera) {
            this.camera = camera;
        }

        /// <summary>
        /// Start playing a sound.
        /// </summary>
        /// <param name="sound">The string identifying the sound in one the SoundBanks that is being executed.</param>
        public virtual void playSound(string soundIdentifier) {
            Microsoft.Xna.Framework.Audio.Cue sound = soundBank.GetCue(soundIdentifier);
            activeSounds.Add(sound);
            sound.Play();
        }

        /// <summary>
        /// Update the volume and direction of the sound. Should we called on each Audible3D object on each iteration of the output() method in UserInterface3D.
        /// </summary>
        internal void output() {
            System.Collections.Generic.List<Microsoft.Xna.Framework.Audio.Cue> currentSounds = new System.Collections.Generic.List<Microsoft.Xna.Framework.Audio.Cue>(activeSounds);
            foreach (Microsoft.Xna.Framework.Audio.Cue sound in currentSounds) {
                if (sound.IsStopped) activeSounds.Remove(sound);
                else {
                    InteractionEngine.Constructs.Location location = gameObject.getLocation();
                    Microsoft.Xna.Framework.Audio.AudioEmitter emitter = new Microsoft.Xna.Framework.Audio.AudioEmitter();
                    emitter.Position = location.Position;
                    emitter.Forward = location.Forward;
                    emitter.Up = location.Up;
                    InteractionEngine.Constructs.Location cameraLocation = camera.getLocation();
                    Microsoft.Xna.Framework.Audio.AudioListener listener = new Microsoft.Xna.Framework.Audio.AudioListener();
                    listener.Position = cameraLocation.Position;
                    listener.Forward = cameraLocation.Forward;
                    listener.Up = cameraLocation.Up;
                    sound.Apply3D(listener, emitter);
                }
            }
        }

    }

    /**
     * Implemented by 3D GameObjects that can make sounds.
     */
    public interface Audible3D : InteractionEngine.Constructs.Locatable {

        /// <summary>
        /// Gets the Audio3D class from GameObject.
        /// </summary>
        /// <returns>The Audio3D class.</returns>
        Audio3D getAudio3D();

    }

}