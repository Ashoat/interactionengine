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
| * UserInterface3D                  Class |
| * Graphics3D              Abstract Class |
\*••••••••••••••••••••••••••••••••••••••••*/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using InteractionEngine.EventHandling;
using InteractionEngine.Constructs;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace InteractionEngine.UserInterface.ThreeDimensional {

    /// <summary>
    /// Coolio.
    /// </summary>
    public class UserInterface3D : UserInterface {

        public static User3D user;
        public static GraphicsDevice graphicsDevice;
        public static ContentManager content;
        public SpriteBatch spriteBatch;

        // Contain lists of Interactables that had been MOUSEMASK_OVER'd and MOUSEMASK_CLICK'd in the last iteration of input().
        // Used for knowing when to invoke MOUSEMASK_OUT and MOUSEMASK_RELEASE events.
        private System.Collections.Generic.Dictionary<Interactable, int> eventsAwaitingReset = new System.Collections.Generic.Dictionary<Interactable, int>();

        // This specifies the bits that represent unique positive mouse buttons or actions (right-click, mouse-over, etc)
        public const int MOUSEMASK_ACTION = (1 << 4) - 1;
        // This specifies the bit that represents a reversal of the positive mouse actions (right-release, mouse-out, etc)
        // It might also represent the release of a modifier key, although that probably won't be necessary.
        public const int MOUSEMASK_RESET = 1 << 4;

        // These next three bits represent modifier keys. If a key is pressed while a positive mouse action is
        // performed, the respective bit should be masked on top of the action mask.
        // These might possibly find some usage in the mouse-action-resetting portion of the code...
        // say if you wanted to detect when the CTRL key was pressed of released during a mouseover...
        // though I doubt it so I recommend we not implement that until we find it necessary.
        // However, if we do implement that, we would need to apply the RESET mask for key releases.
        public const int MOUSEMASK_CTRL = 1 << 5;
        public const int MOUSEMASK_ALT = 1 << 6;
        public const int MOUSEMASK_SHIFT = 1 << 7;

        // Positive mouse actions (except the first one, to be used when sending modifier-key-changed-events, if we want to do that)
        public const int MOUSEMASK_NULL = 0;
        public const int MOUSEMASK_RIGHT_PRESS = 1 << 0;
        public const int MOUSEMASK_LEFT_PRESS = 1 << 1;
        public const int MOUSEMASK_OVER = 1 << 2;
        // Currently unused
        public const int MOUSEMASK_OTHER = 1 << 3;

        // Negative mouse actions!
        public const int MOUSEMASK_RIGHT_RELEASE = MOUSEMASK_RIGHT_PRESS + MOUSEMASK_RESET;
        public const int MOUSEMASK_LEFT_RELEASE = MOUSEMASK_LEFT_PRESS + MOUSEMASK_RESET;
        public const int MOUSEMASK_OUT = MOUSEMASK_OVER + MOUSEMASK_RESET;

        public static bool testMask(int val, int mask) {
            return (val & mask) != 0;
        }

        public static int toggleMask(int val, int mask) {
            return val ^ mask;
        }

        public static int unsetMask(int val, int mask) {
            return val & (~mask);
        }

        public static int setMask(int val, int mask) {
            return val | mask;
        }

        /// <summary>
        /// Goes through old "held" events that need resetting (such as a "mouseOver" awaiting a "mouseOut")
        /// and checks to see if they are ready to reset, and if so, sends their corresponding release events
        /// and removes from the list of held events.
        /// </summary>
        /// <param name="newEvents">The list into which newly detected events are to be inserted.</param>
        private void resetEvents(System.Collections.Generic.List<InteractionEngine.EventHandling.Event> newEvents, Ray ray) {
            // Check states of mouse input
            Microsoft.Xna.Framework.Input.MouseState mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            // Loop through list and test to see if any are ready to receive a RELEASE or OUT event
            System.Collections.Generic.Dictionary<Interactable, int> removals = new Dictionary<Interactable, int>();
            foreach (Interactable interaction in eventsAwaitingReset.Keys) {
                int alreadyEvented = eventsAwaitingReset[interaction];
                foreach (MouseMaskTest maskTest in this.maskTests) {
                    maskTest.testAndRetrieveNegativeEvent(removals, newEvents, interaction, ray, mouse, alreadyEvented);
                }
            }
            // Remove ones that no longer have any mouse events to reset
            foreach (KeyValuePair<Interactable, int> removal in removals) {
                if (!testMask(removal.Value, MOUSEMASK_ACTION)) eventsAwaitingReset.Remove(removal.Key);
                else eventsAwaitingReset[removal.Key] = removal.Value;
            }

        }

        private class MouseMaskTest {
            public delegate bool IsActivated(MouseState mouse);
            int mask;
            IsActivated isActivated;
            public MouseMaskTest(int mask, IsActivated isActivated) {
                this.mask = mask;
                this.isActivated = isActivated;
            }
            public bool isStillMousedOver(Interactable interaction, Ray ray, MouseState mouse) {
                if (interaction is Interactable3D) return ((Interactable3D)interaction).getGraphics3D().intersects(ray).HasValue;
                if (interaction is Interactable2D) return ((Interactable2D)interaction).getGraphics2D().intersectionPoint(mouse.X, mouse.Y).HasValue;
                return false;
            }
            public void testAndRetrievePositiveEvent(Dictionary<Interactable, int> eventsAwaitingReset, List<Event> newEvents, Interactable interaction, MouseState mouse, Ray ray, Vector3 point) {
                int alreadyEvented = eventsAwaitingReset.ContainsKey(interaction) ? eventsAwaitingReset[interaction] : 0; 
                if (!testMask(alreadyEvented, mask) && isActivated(mouse)) {
                    eventsAwaitingReset[interaction] = setMask(alreadyEvented, mask);
                    Event evvie = interaction.getEvent(mask, point);
                    if (evvie != null) newEvents.Add(evvie);
                }
            }
            public void testAndRetrieveNegativeEvent(Dictionary<Interactable, int> removals, List<Event> newEvents, Interactable interaction, Ray ray, MouseState mouse, int alreadyEvented) {
                if (testMask(alreadyEvented, mask) && (!isActivated(mouse) || !isStillMousedOver(interaction, ray, mouse))) {
                    Event evvie = interaction.getEvent(setMask(mask, MOUSEMASK_RESET), Vector3.Zero);
                    if (evvie != null) newEvents.Add(evvie);
                    //if (!removals.ContainsKey(interaction)) removals.Add(interaction, unsetMask(alreadyEvented, mask));
                    removals[interaction] = unsetMask(alreadyEvented, mask);
                }
            }
        }

        private MouseMaskTest[] maskTests = new MouseMaskTest[] {
            new MouseMaskTest(MOUSEMASK_OVER, (MouseState mouse) => true),
            new MouseMaskTest(MOUSEMASK_LEFT_PRESS, (MouseState mouse) => mouse.LeftButton == ButtonState.Pressed),
            new MouseMaskTest(MOUSEMASK_RIGHT_PRESS, (MouseState mouse) => mouse.RightButton == ButtonState.Pressed),
        };

        #region Keyboard Stuff

        private KeyboardFocus kf;
        private const int repeatDelay = 20;
        private Dictionary<Microsoft.Xna.Framework.Input.Keys, double> repeatTimes = new Dictionary<Microsoft.Xna.Framework.Input.Keys, double>();
        public void registerKeyboardFocus(KeyboardFocus kf) {
            this.kf = kf;
        }
        private void checkKeyboard(System.Collections.Generic.List<InteractionEngine.EventHandling.Event> newEvents) {
            if (this.kf == null) return;
            Microsoft.Xna.Framework.Input.KeyboardState keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            foreach (Microsoft.Xna.Framework.Input.Keys key in keyboard.GetPressedKeys()) {
                if (!repeatTimes.ContainsKey(key) || repeatTimes[key] < InteractionEngine.Engine.gameTime.TotalRealTime.TotalMilliseconds) {
                   // newEvents.Add(this.kf.getEvent((int)key));
                    kf.keyPressed(null, (int)key);
                    repeatTimes[key] = InteractionEngine.Engine.gameTime.TotalRealTime.TotalMilliseconds + repeatDelay;
                }
            }
        }

        #endregion

        /// <summary>
        /// Checks state of user input devices to see if an input event should be triggered.
        /// If so, collects the Event objects and inserts them into the given list.
        /// Make it quick!
        /// </summary>
        /// <param name="newEventList">The list into which newly detected events are to be inserted.</param>
        protected override void retrieveInput(System.Collections.Generic.List<InteractionEngine.EventHandling.Event> newEvents) {
            // Get mouse state
            Microsoft.Xna.Framework.Input.MouseState mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            Ray ray = this.calculateMouseRay(mouse);
            // Reset old events
            resetEvents(newEvents, ray);

            Vector3 point;
            Interactable interaction = this.findClosestIntersectedInteractable(ray, mouse, out point);
            if (interaction != null) {
                foreach (MouseMaskTest maskTest in this.maskTests) {
                    maskTest.testAndRetrievePositiveEvent(eventsAwaitingReset, newEvents, interaction, mouse, ray, point);
                }
            }

        }

        private Ray calculateMouseRay(MouseState mouse) {
            Vector3 near = new Vector3(mouse.X, mouse.Y, 0f);
            Vector3 far = new Vector3(mouse.X, mouse.Y, 1f);
            Matrix world = user.worldTransform;

            Vector3 nearPt = graphicsDevice.Viewport.Unproject(near, user.camera.Projection, user.camera.View, world);
            Vector3 farPt = graphicsDevice.Viewport.Unproject(far, user.camera.Projection, user.camera.View, world);

            Vector3 dir = farPt - nearPt;
            dir.Normalize();

            return new Ray(user.camera.getLocation().Position, dir);
        }

        private Interactable findClosestIntersectedInteractable(Ray ray, MouseState mouse, out Vector3 intersectionPoint) {
            Interactable3D closest3DInteractable = null;
            float closest3DDistance = float.PositiveInfinity;
            Interactable2D closest2DInteractable = null;
            float closest2DLayerDepth = float.PositiveInfinity;
            Vector3 intersection2D = Vector3.Zero;
            // Loop through all of the User's LoadRegions
            foreach (InteractionEngine.Constructs.LoadRegion loadRegion in InteractionEngine.Engine.getGraphableLoadRegions()) {
                // Loop through all the LoadRegion's GameObjects
                foreach (Constructs.GameObjectable gameObject in loadRegion.getGameObjectArray()) {
                    // See if this GameObject can be interacted with.
                    if (gameObject is Interactable3D) {
                        Interactable3D interaction = (Interactable3D)gameObject;
                        float? distance = interaction.getGraphics3D().intersects(ray);
                        if (distance.HasValue && distance.Value < closest3DDistance) {
                            closest3DInteractable = interaction;
                            closest3DDistance = distance.Value;
                        }
                    }
                    if (gameObject is Interactable2D) {
                        Interactable2D interaction = (Interactable2D)gameObject;
                        if (interaction.getGraphics2D().Visible && interaction.getGraphics2D().LayerDepth < closest2DLayerDepth) {
                            Vector3? pointClicked = interaction.getGraphics2D().intersectionPoint(mouse.X, mouse.Y);
                            if (pointClicked.HasValue) {
                                closest2DInteractable = interaction;
                                closest2DLayerDepth = interaction.getGraphics2D().LayerDepth;
                                intersection2D = pointClicked.Value;
                            }
                        }
                    }
                    // End checking gameObject
                }
            }
            if (closest2DInteractable != null) {
                intersectionPoint = intersection2D;
                return closest2DInteractable;
            } else if (closest3DInteractable != null) {
                intersectionPoint = ray.Position + closest3DDistance * ray.Direction;
                return closest3DInteractable;
            }
            intersectionPoint = Vector3.Zero;
            return null;
        }

        public override List<InteractionEngine.EventHandling.Event> input() {
            // Get mouse events
            List<InteractionEngine.EventHandling.Event> events = base.input();
            // Get keyboard events
            List<InteractionEngine.EventHandling.Event> keyboardEvents = new List<InteractionEngine.EventHandling.Event>();
            checkKeyboard(keyboardEvents);
            events.AddRange(keyboardEvents);
            return events;
        }

        /// <summary>n 
        /// Output stuff.
        /// </summary>
        public override void output() {
            // Loop through the user's LoadRegions
            foreach (Constructs.LoadRegion loadRegion in InteractionEngine.Engine.getGraphableLoadRegions()) {
                // Loop through the GameObjects within those LoadRegions
                foreach (Constructs.GameObjectable gameObject in loadRegion.getGameObjectArray()) {
                    if (gameObject is Audio.Audible3D) ((Audio.Audible3D)gameObject).getAudio3D().output();
                    if (gameObject is Graphable3D) ((Graphable)gameObject).getGraphics().onDraw();
                }
                if (loadRegion == user.localLoadRegion) {
                    ;
                }
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.SaveState);
                // TODO: figure this thing out... stop drawing stuff twice
                foreach (Constructs.GameObjectable gameObject in Engine.getGameObjectArray()) {
                    if (gameObject is Graphable2D) ((Graphable)gameObject).getGraphics().onDraw();
                }
                spriteBatch.End();
            }

        }

        /// <summary>
        /// Initialize stuff.
        /// </summary>
        public override void initialize() {
            graphicsDevice = InteractionEngine.Engine.game.GraphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);
            content = InteractionEngine.Engine.game.Content;
            base.initialize();
        }

    }


    public class ModelEffect {

        private BasicEffect basicEffect;
        private EffectPool effectPool;

        public float Alpha = 1f;
        public Vector3 AmbientLightColor = Vector3.Zero;
        public EffectTechnique CurrentTechnique;
        public Vector3 DiffuseColor = Vector3.One;
        public Vector3 EmissiveColor = Vector3.Zero;
        public Vector3 FogColor = Vector3.Zero;
        public bool FogEnabled = false;
        public float FogEnd = 1.0f;
        public float FogStart = 0f;
        public bool LightingEnabled = false;
        public bool PreferPerPixelLighting = false;
        public Vector3 SpecularColor = Vector3.Zero;
        public float SpecularPower = 1f;
        public Texture2D Texture = null;
        public bool TextureEnabled = false;
        public bool VertexColorEnabled = false;
        public Matrix World = Matrix.Identity;

        private string textureName;
        private bool enableDefaultLighting;

        /// <summary>
        /// NOTE: Any additions to the used properties of this class MUST BE ADDED to HelperClass.CloneModelEffect(...)
        /// </summary>
        //private Camera activeCamera;
        private Camera activeCamera;
        public Camera ActiveCamera {
            set {
                activeCamera = value;
                UpdateFromActiveCamera();
            }
            get {
                return activeCamera;
            }
        }
        public void UpdateFromActiveCamera() {
            if (basicEffect == null) return;
            basicEffect.View = activeCamera.View;
            basicEffect.Projection = activeCamera.Projection;
        }

        public ModelEffect(EffectPool effectPool)
            : this() {
            this.effectPool = effectPool;
        }

        public ModelEffect(EffectPool effectPool, ModelEffect source)
            : this(effectPool) {
            this.CloneFrom(source);
        }

        public ModelEffect() {

        }

        public void Initialize(Camera camera) {
            this.basicEffect = new BasicEffect(InteractionEngine.Engine.game.GraphicsDevice, effectPool);
            this.CommitProperties();
            this.ActiveCamera = camera;
            this.setTextureName(this.textureName);
            if (this.enableDefaultLighting) this.basicEffect.EnableDefaultLighting();
        }

        public void setTextureName(string textureName) {
            this.textureName = textureName;
            if (basicEffect != null && this.textureName != null) {
                this.Texture = InteractionEngine.Engine.game.Content.Load<Texture2D>(this.textureName);
                basicEffect.Texture = this.Texture;
            }
        }

        public void CommitProperties() {
            if (basicEffect == null) return;
            basicEffect.Alpha = this.Alpha;
            basicEffect.AmbientLightColor = this.AmbientLightColor;
            if (this.CurrentTechnique != null) basicEffect.CurrentTechnique = this.CurrentTechnique;
            basicEffect.DiffuseColor = this.DiffuseColor;
            basicEffect.EmissiveColor = this.EmissiveColor;
            basicEffect.FogColor = this.FogColor;
            basicEffect.FogEnabled = this.FogEnabled;
            basicEffect.FogEnd = this.FogEnd;
            basicEffect.FogStart = this.FogStart;
            basicEffect.LightingEnabled = this.LightingEnabled;
            basicEffect.PreferPerPixelLighting = this.PreferPerPixelLighting;
            basicEffect.SpecularColor = this.SpecularColor;
            basicEffect.SpecularPower = this.SpecularPower;
            basicEffect.Texture = this.Texture;
            basicEffect.TextureEnabled = this.TextureEnabled;
            basicEffect.VertexColorEnabled = this.VertexColorEnabled;
            basicEffect.World = this.World;
        }

        public void CloneFrom(ModelEffect source) {
            this.ActiveCamera = source.ActiveCamera;
            this.Alpha = source.Alpha;
            this.AmbientLightColor = source.AmbientLightColor;
            this.CurrentTechnique = source.CurrentTechnique;
            this.DiffuseColor = source.DiffuseColor;
            this.EmissiveColor = source.EmissiveColor;
            this.FogColor = source.FogColor;
            this.FogEnabled = source.FogEnabled;
            this.FogEnd = source.FogEnd;
            this.FogStart = source.FogStart;
            this.LightingEnabled = source.LightingEnabled;
            this.PreferPerPixelLighting = source.PreferPerPixelLighting;
            this.SpecularColor = source.SpecularColor;
            this.SpecularPower = source.SpecularPower;
            this.Texture = source.Texture;
            this.TextureEnabled = source.TextureEnabled;
            this.VertexColorEnabled = source.VertexColorEnabled;
            this.World = source.World;
        }

        public void EnableDefaultLighting() {
            if (this.basicEffect != null) this.basicEffect.EnableDefaultLighting();
            else this.enableDefaultLighting = true;
        }

        public BasicEffect Effect {
            get { return basicEffect; }
        }

    }


    public class Animation {

        List<string> frames = new List<string>();
        List<Model> frameModels;

        int index = 0;
        string id;


        public List<Model> Frames {
            get { if (frameModels == null) throw new Exception("Animation content not loaded... call loadContent()"); return frameModels; }
            set { frameModels = value; }
        }
        public int Index {
            get { return index; }
            set { index = value; }
        }
        public Model CurrentFrame {
            get { if (frameModels == null) throw new Exception("Animation content not loaded... call loadContent()");  return frameModels[index]; }
        }
        public string Name {
            get { return id; }
            set { id = value; }
        }
        public Animation(string name) {
            this.id = name;
        }
        public Animation(string[] frames, string name) {
            this.frames = new List<string>(frames);
            this.id = name;
            if (UserInterface3D.graphicsDevice != null) this.loadContent();
        }
        public Animation(List<string> frames, string name) {
            this.frames = new List<string>(frames);
            this.id = name;
            if (UserInterface3D.graphicsDevice != null) this.loadContent();
        }
        /// <summary>
        /// Incriments the frame index
        /// </summary>
        /// <returns>Returns false if index is looped to 0 after incriment</returns>
        public bool NextFrame() {
            index++;
            if (index >= frames.Count) {
                index = 0;
                return false;
            }
            return true;
        }

        public void loadContent() {
            if (this.frameModels == null) {
                this.frameModels = new List<Model>();
                foreach (string modelName in this.frames) {
                    this.frameModels.Add(UserInterface3D.content.Load<Model>(modelName));
                }
            }
        }

    }

    public interface Graphics3D : Graphics {
        float? intersects(Ray ray);
        Vector3? intersectionPoint(Ray ray);
        ModelEffect Effect { get; }
    }

    public class Graphics3DModel : Graphics3D {

        public enum AnimationState {
            Stopped,
            Started,
            Stopping
        };

        List<Animation> anims;
        AnimationState animState = AnimationState.Stopped;
        int animIndex = 0;
        public List<Animation> Animations {
            get { return anims; }
            set {
                anims = value;
                foreach (Animation animation in this.anims) {
                    animation.loadContent();
                }
            }
        }
        public Animation CurrentAnimation {
            get { return anims[animIndex]; }
        }
        public Model CurrentModel {
            get {
                if (animState != AnimationState.Started) {
                    return model;
                } else {
                    return this.CurrentAnimation.CurrentFrame;
                }
            }
        }
        public void StartAnimation(string name) {
            this.animIndex = this.GetAnimIndexOf(this.GetAnimationByName(name));
            animState = AnimationState.Started;
        }
        public void StopAnimation() {
            if (animState == AnimationState.Started)
                animState = AnimationState.Stopping;
        }
        public void ForceStopAnimation() {
            animState = AnimationState.Stopped;
            this.CurrentAnimation.Index = 0;
        }

        public Animation GetAnimationByName(string name) {
            return anims.Find(a => a.Name == name);
        }
        public int GetAnimIndexOf(Animation a) {
            return anims.IndexOf(a);
        }


        private ModelEffect effect;

        private string modelName;

        private Model model;

        private Matrix worldLocal;

        private float scale = 1f;

        private BoundingSphere baseBoundingSphere;

        // Contains a reference to this Graphics module's GameObject.
        // Used for proper Updatable construction.
        private Graphable3D gameObject;

        /// <summary>
        /// Constructs the Graphics3D
        /// </summary>
        /// <param name="textureFileName">The filename of this GameObject's texture.</param>
        public Graphics3DModel(Graphable3D gameObject, ModelEffect effect, string modelName)
            : this(gameObject, new List<Animation>(), effect, modelName) {

        }

        public Graphics3DModel(Graphable3D gameObject, List<Animation> animations, ModelEffect effect, string modelName) {
            this.gameObject = gameObject;
            this.anims = animations;

            this.modelName = modelName;
            this.worldLocal = Matrix.Identity;
            this.effect = effect;

            if (InteractionEngine.Engine.game.GraphicsDevice != null) loadContent();

        }

        public ModelEffect Effect {
            get { return this.effect; }
        }

        public Matrix World {
            get { return this.effect.World; }
        }

        public BoundingSphere BoundingSphere {
            get { return new BoundingSphere(baseBoundingSphere.Center + this.gameObject.getLocation().Position, baseBoundingSphere.Radius * this.scale); }
        }

        public void calculateBoundingSphere() {
            BoundingSphere welded = this.model.Meshes[0].BoundingSphere;
            foreach (ModelMesh mesh in this.model.Meshes) {
                welded = BoundingSphere.CreateMerged(welded, mesh.BoundingSphere);
            }
            this.baseBoundingSphere = welded;
        }

        private void updateWorld() {
            this.worldLocal = localWorld();
            effect.World = worldContainer(Matrix.Identity); // UserInterface3D.user.worldTransform);
        }

        /// <summary>
        /// ports the local world matrix to the [terrain] container
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        private Matrix worldContainer(Matrix container) {
            return localWorld() * container;
        }

        /// <summary>
        /// scale, rotate, and translation. change this to add features
        /// </summary>
        /// <returns></returns>
        private Matrix localWorld() {
            return Matrix.CreateScale(this.scale) * Matrix.CreateRotationY(this.gameObject.getLocation().yaw) * Matrix.CreateTranslation(this.gameObject.getLocation().Position); //add more later. scale + rotate.
        }

        public void SetScale(float scale) {
            this.scale = scale;
            updateWorld();
        }

        /// <summary>
        /// Draw this Graphics3D onto the screen
        /// </summary>
        public virtual void onDraw() {
            if (animState != AnimationState.Stopped) {
                drawModel(this.CurrentAnimation.CurrentFrame);
                if (!this.CurrentAnimation.NextFrame()) //if NextFrame() causes a loopback to frame index 0
                    if (animState == AnimationState.Stopping)
                        animState = AnimationState.Stopped;
            } else {
                drawModel(this.model);
            }
        }


        private void drawModel(Model m) {
            foreach (ModelMesh mesh in m.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {

                    effect.World = worldContainer(UserInterface3D.user.worldTransform); // so that the scale and stuff changes when the terrain scale changes


                    effect.Projection = this.effect.ActiveCamera.Projection;//terrain.Camera.Projection;
                    effect.View = this.effect.ActiveCamera.View;
                    effect.EnableDefaultLighting();

                    //
                    //Now copy all data from the ModelEffect to this BasicEffect
                    //

                    effect.Alpha = this.effect.Alpha;
                    effect.AmbientLightColor = this.effect.AmbientLightColor;
                    //effect.CurrentTechnique = this.effect.CurrentTechnique;
                    effect.DiffuseColor = this.effect.DiffuseColor;
                    effect.EmissiveColor = this.effect.EmissiveColor;
                    effect.FogColor = this.effect.FogColor;
                    effect.FogEnabled = this.effect.FogEnabled;
                    effect.FogEnd = this.effect.FogEnd;
                    effect.FogStart = this.effect.FogStart;
                    //effect.LightingEnabled = this.effect.LightingEnabled;
                    effect.PreferPerPixelLighting = this.effect.PreferPerPixelLighting;
                    effect.SpecularColor = this.effect.SpecularColor;
                    effect.SpecularPower = this.effect.SpecularPower;
                    effect.Texture = this.effect.Texture;
                    effect.TextureEnabled = this.effect.TextureEnabled;
                    effect.VertexColorEnabled = this.effect.VertexColorEnabled;

                }

                mesh.Draw();
            }
        }

        /// <summary>
        /// If the given ray intersects this GameObject, returns the distance at which it intersects.
        /// Otherwise, returns negative one.
        /// </summary>
        /// <param name="ray">The ray</param>
        /// <returns>The distance at which the ray intersects this GameObject, or -1 if it does not intersect.</returns>
        public virtual float? intersects(Ray ray) {
            float shortestDistance = float.PositiveInfinity;
            foreach (ModelMesh mesh in this.CurrentModel.Meshes) {
                Vector3 center = mesh.BoundingSphere.Center + gameObject.getLocation().Position;
                float radius = mesh.BoundingSphere.Radius * this.scale;
                float? distance = ray.Intersects(new BoundingSphere(center, radius));
                if (distance < shortestDistance) shortestDistance = distance.Value;
            }
            if (float.IsPositiveInfinity(shortestDistance)) return null;
            return shortestDistance;
           // return ray.Intersects(this.BoundingSphere);
        }

        public virtual Vector3? intersectionPoint(Ray ray) {
            float? distance = this.intersects(ray);
            if (distance.HasValue) return ray.Position + ray.Direction * distance;
            return null;
        }

        /// <summary>
        /// Called during InteractionGame's LoadContent loop.
        /// </summary>
        public virtual void loadContent() {
            this.model = InteractionEngine.Engine.game.Content.Load<Model>(modelName);
            effect.Initialize(UserInterface3D.user.camera);
            this.calculateBoundingSphere();
            foreach (Animation animation in this.anims) {
                animation.loadContent();
            }
        }

    }

    public interface Graphable3D : Graphable {

        /// <summary>
        /// Gets the Graphics3D module from this GameObject.
        /// </summary>
        /// <returns>The Graphics3D class.</returns>
        Graphics3D getGraphics3D();

    }

    public class Camera : InteractionEngine.Constructs.GameObject, Locatable {

        #region FACTORY

        /// <summary>
        /// All GameObjects need a parameterless constructor for calling by GameObject.createGameObject() and GameObject.createFromUpdate().
        /// NEVER CALL THIS! This constructor is exclusively for use by the InteractionEngine. If anyone else calls it things will break.
        /// If you want to construct this object, use GameObject.createGameObject(LoadRegion).
        /// </summary>
        public Camera() {
        }

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        public const string realHash = "Camera";
        public override string classHash {
            get { return realHash; }
        }

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Camera() {
            GameObject.factoryList.Add(realHash, new GameObjectFactory(GameObject.createFromUpdate<Camera>));
        }

        #endregion

        float fovy;
        float aspectRatio;
        float nearPlane;
        float farPlane;

        Matrix projectionMatrix;
        Matrix viewMatrix;

        Vector3 target;

        BoundingFrustum frustum;

        private Location location;
        public Location getLocation() {
            return location;
        }

        public Vector3 Target {
            get { return target; }
            set {
                target = value;
                updateCamera();
            }
        }

        public Matrix Projection {
            get {
                return projectionMatrix;
            }
        }
        public Matrix View {
            get {
                return viewMatrix;
            }
        }
        public BoundingFrustum Frustrum {
            get {
                updateCamera();
                return frustum;
            }
        }

        // fovy in degrees
        public void SetPerspectiveFov(float fovy, float aspectRatio, float nearPlane, float farPlane) {
            this.fovy = fovy;
            this.aspectRatio = aspectRatio;
            this.farPlane = farPlane;
            this.nearPlane = nearPlane;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fovy), aspectRatio, nearPlane, farPlane);
            updateCamera();
        }
        public void SetLookAt(Vector3 cameraPos, Vector3 cameraTarget, Vector3 cameraUp) {
            this.location.Position = cameraPos;
            this.target = cameraTarget;
            Vector3 heading = cameraTarget - cameraPos;
            Vector3 strafe = Vector3.Cross(heading, cameraUp);
            updateCamera();
        }
        /// <summary>
        /// Moves the camera + displaces the camera target accordingly
        /// </summary>
        /// <param name="cameraPos"></param>
        public void SetPosition(Vector3 cameraPos) {
            target += cameraPos - this.location.Position;
            this.location.Position = cameraPos;
            updateCamera();
        }
        /// <summary>
        /// Moves the camera, without displacing the target.
        /// </summary>
        /// <param name="cameraPos"></param>
        public void SetPositionLockTarget(Vector3 cameraPos) {
            this.location.Position = cameraPos;
            updateCamera();
        }

        /// <summary>
        /// Zooms in/out.
        /// </summary>
        /// <param name="dist"></param>
        public void Zoom(float dist)
        {
            SetPositionLockTarget(this.location.Position + dist * this.location.Heading);
        }

        public void SetTargetDisplacePosition(Vector3 tar) {
            this.location.Position += tar - target;
            target = tar;
            updateCamera();
        }

        /// <summary>
        /// Rotates camera (orbit) around position (axis: y)
        /// </summary>
        /// <param name="posRot"></param>
        /// <param name="rot"></param>
        public void RotateUponPosition(Vector3 posRot, float rot) {
            this.location.Position = Vector3.Transform((posRot - this.location.Position), Matrix.CreateRotationY(MathHelper.ToRadians(rot))) + this.location.Position;
            updateCamera();
        }

        /// <summary>
        /// DIE!
        /// </summary>
        /// <param name="posRot"></param>
        /// <param name="axis"></param>
        /// <param name="rot"></param>
        public void RotateUponAxis(Vector3 posRot, Vector3 axis, float rot) {
            Vector3 relPosition = this.location.Position - posRot;
            if (relPosition.Equals(Vector3.Zero)) return;
            this.location.Position = Vector3.Transform(relPosition, Matrix.CreateFromAxisAngle(axis, MathHelper.ToRadians(rot))) + posRot;
            updateCamera();
        }
        public void ChangeAzimuth(Vector3 posRot, Vector3 axis, float amount) {
            Vector3 relPosition = this.location.Position - posRot;
            Vector3 azimuthAxis = Vector3.Cross(relPosition, axis);
            if (azimuthAxis.Equals(Vector3.Zero)) return;
            azimuthAxis.Normalize();
            this.location.Position = Vector3.Transform(relPosition, Matrix.CreateFromAxisAngle(azimuthAxis, MathHelper.ToRadians(amount))) + posRot;
            updateCamera();
        }
        private void updateCamera() {
            Vector3 newHeading = (this.target - this.location.Position);
            Vector3 newStrafe = new Vector3(-newHeading.Z, 0, newHeading.X);
            this.location.setHeadingAndStrafe(newHeading, newStrafe);
            viewMatrix = Matrix.CreateLookAt(this.location.Position, target, this.location.Top);
            frustum = new BoundingFrustum(viewMatrix * projectionMatrix);
        }

        public override void construct() {
            this.location = new Location(this);
        }


    }


    /**
     * Implemented by GameObjects that can be interacted with.
     */
    public interface Interactable3D : Interactable, Graphable3D {

    }

    public interface Interactable2D : Interactable, Graphable2D {

    }


    public interface Graphics2D : Graphics {
        Vector3? intersectionPoint(double x, double y);
        float LayerDepth { get; }
        bool Visible { get; set; }
    }

    /**
     * Contains all graphics information and methods neccessary to drawing a GameObject in a 2D environment.
     * Could effectively be called a "Sprite" class.
     */
    public class Graphics2DTexture : Graphics2D {

        // Contains a reference to this Graphics module's GameObject.
        // Used for proper Updatable construction.
        public readonly Graphable2D gameObject;
        // Contains texture information for the sprite.
        // Used for drawing the sprite.
        private Microsoft.Xna.Framework.Graphics.Texture2D texture;
        private string textureName;
        private Color[] pixels;
        private bool visible = true;
        private float scale = 1;
        private float layerDepth = 0;

        /// <summary>
        /// Loads the sprite from XNA's ContentPipeline.
        /// </summary>
        /// <param name="textureFileName">The filename of this GameObject's texture.</param>
        public Graphics2DTexture(Graphable2D gameObject, string textureName) {
            this.gameObject = gameObject;
            this.textureName = textureName;
            if (Engine.game.GraphicsDevice != null) this.loadContent();
        }

        public Graphics2DTexture(Graphable2D gameObject) {
            this.gameObject = gameObject;
        }

        public string TextureName {
            get { return textureName; }
            set {
                this.textureName = value;
                if (Engine.game.GraphicsDevice != null) this.loadContent();
            }
        }

        public bool Visible {
            get { return visible; }
            set { this.visible = value; }
        }

        public float Scale {
            get { return this.scale; }
            set { this.scale = value; }
        }

        public float LayerDepth {
            get { return this.layerDepth; }
            set { this.layerDepth = value; }
        }

        /// <summary>
        /// Draw this Graphics2D onto the SpriteBatch.
        /// </summary>
        public virtual void onDraw() {
            if (this.texture == null || !this.visible) return;
            SpriteBatch spriteBatch = ((UserInterface3D)InteractionEngine.Engine.userInterface).spriteBatch;
            Vector3 position3 = this.gameObject.getLocation().Position;
            Vector2 position = new Vector2(position3.X, position3.Y);
            float rotationRadians = this.gameObject.getLocation().yaw ;
            Vector2 origin = Vector2.Zero;
            spriteBatch.Draw(texture, position, (Rectangle?)null, Color.White, rotationRadians,
                origin, scale, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        /// Returns true if a point is contained within this Graphics2D.
        /// </summary>
        /// <param name="x">The x-coordinate of the point.</param>
        /// <param name="y">The y-coordinate of the point.</param>
        /// <returns>True if the point is within the Graphics2D's boundaries; false otherwise.</returns>
        public Vector3? intersectionPoint(double x, double y) {
            if (this.texture == null || !this.visible) return null;
            Vector3 location3 = this.gameObject.getLocation().Position;
            Vector3 inversePosition = Vector3.Multiply(location3, -1);
            Matrix inverseRotation = Matrix.CreateRotationZ(-this.gameObject.getLocation().yaw * MathHelper.Pi / 180);
            Vector3 vector = new Vector3((float)x, (float)y, 0f);
            vector = Vector3.Transform(vector, inverseRotation) + inversePosition;
            if (vector.X < 0 || vector.Y < 0 || (vector.X >= this.texture.Width) || (vector.Y >= this.texture.Height)) return null;
            int index = (int)vector.X * this.texture.Height + (int)vector.Y;
            const int alphaLimit = 10;
            if (this.pixels[index].A < alphaLimit) return null;
            return vector;
        }

        /// <summary>
        /// Called during InteractionGame's LoadContent loop.
        /// </summary>
        public virtual void loadContent() {
            if (textureName == null || textureName.Length == 0) return;
            texture = InteractionEngine.Engine.game.Content.Load<Texture2D>(textureName);
            pixels = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(pixels);
        }

    }

    public interface Graphable2D : Graphable {
        Graphics2D getGraphics2D();
    }



    public class User3D {

        public readonly Constructs.LoadRegion localLoadRegion;
        public readonly Camera camera;
        public Matrix worldTransform = Matrix.Identity;

        public User3D() {
            this.localLoadRegion = Constructs.LoadRegion.createLoadRegion();
            this.camera = GameObject.createGameObject<Camera>(this.localLoadRegion);
        }


    }


}