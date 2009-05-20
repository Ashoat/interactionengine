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

namespace InteractionEngine.Client.ThreeDimensional {

    /// <summary>
    /// Coolio.
    /// </summary>
    public class UserInterface3D : UserInterface {

        public static User3D user;
        public static GraphicsDevice graphicsDevice;

        // Contain lists of Interactables that had been MOUSEMASK_OVER'd and MOUSEMASK_CLICK'd in the last iteration of input().
        // Used for knowing when to invoke MOUSEMASK_OUT and MOUSEMASK_RELEASE events.
        private System.Collections.Generic.Dictionary<Interactable3D, int> eventsAwaitingReset = new System.Collections.Generic.Dictionary<Interactable3D, int>();
        
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
            return (val & mask) == mask;
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
        private void resetEvents(System.Collections.Generic.List<Event> newEvents, Ray ray) {
            // Check states of mouse input
            Microsoft.Xna.Framework.Input.MouseState mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            bool leftReleased = mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released;
            bool rightReleased = mouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released;
            // Loop through list and test to see if any are ready to receive a RELEASE or OUT event
            System.Collections.Generic.LinkedList<Interactable3D> removals = new System.Collections.Generic.LinkedList<Interactable3D>();
            foreach (Interactable3D interactable in eventsAwaitingReset.Keys) {
                int eveCode = eventsAwaitingReset[interactable];
                if (testMask(eveCode, MOUSEMASK_LEFT_PRESS)) {
                    if (leftReleased) {
                        Event evvie = interactable.getEvent(MOUSEMASK_LEFT_RELEASE, new Vector3());
                        if (evvie != null) newEvents.Add(evvie);
//                        eventsAwaitingReset[interactable] = unsetMask(eveCode, MOUSEMASK_LEFT_PRESS);
                        removals.AddLast(interactable);
                    }
                }
                if (testMask(eveCode, MOUSEMASK_RIGHT_PRESS)) {
                    if (rightReleased) {
                        Event evvie = interactable.getEvent(MOUSEMASK_RIGHT_RELEASE, new Vector3());
                        if (evvie != null) newEvents.Add(evvie);
//                        eventsAwaitingReset[interactable] = unsetMask(eveCode, MOUSEMASK_RIGHT_PRESS);
                        removals.AddLast(interactable);
                    }
                }
                if (testMask(eveCode, MOUSEMASK_OVER)) {
                    Graphics3D graphics = (Graphics3D)interactable.getGraphics();
                    if (graphics.intersects(ray) == -1) {
                        Event evvie = interactable.getEvent(MOUSEMASK_OUT, new Vector3());
                        if (evvie != null) newEvents.Add(evvie);
//                        eventsAwaitingReset[interactable] = unsetMask(eveCode, MOUSEMASK_OVER);
                        removals.AddLast(interactable);
                    }
                }
                if ((eventsAwaitingReset[interactable] & MOUSEMASK_ACTION) == 0) removals.AddLast(interactable);
            }
            // Remove ones that no longer have any mouse events to resetS
            foreach (Interactable3D removal in removals) {
                eventsAwaitingReset.Remove(removal);
            }

        }

        private void checkInteractableForInteraction(System.Collections.Generic.List<Event> newEvents, Ray ray, Interactable3D interaction) {
            Microsoft.Xna.Framework.Input.MouseState mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            Graphics3D graphics = (Graphics3D)interaction.getGraphics();
            // Check to see if the mouse is intersecting the GameObject.
            if (graphics.intersects(ray) >= 0) {
                int alreadyEvented = eventsAwaitingReset.ContainsKey(interaction) ? eventsAwaitingReset[interaction] : 0;
                // MOUSEMASK_OVER?
                if (!testMask(alreadyEvented, MOUSEMASK_OVER)) {
                    eventsAwaitingReset[interaction] = setMask(alreadyEvented, MOUSEMASK_OVER);
                    Event evvie = interaction.getEvent(MOUSEMASK_OVER, graphics.intersectionPoint(ray) ?? new Vector3());
                    if (evvie != null) newEvents.Add(evvie);
                }
                // MOUSEMASK_LEFT_CLICK?
                if (!testMask(alreadyEvented, MOUSEMASK_LEFT_PRESS) && mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) {
                    eventsAwaitingReset[interaction] = setMask(alreadyEvented, MOUSEMASK_LEFT_PRESS);
                    Event evvie = interaction.getEvent(MOUSEMASK_LEFT_PRESS, graphics.intersectionPoint(ray) ?? new Vector3());
                    if (evvie != null) newEvents.Add(evvie);
                }
                // MOUSEMASK_RIGHT_CLICK?
                if (!testMask(alreadyEvented, MOUSEMASK_LEFT_PRESS) && mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) {
                    eventsAwaitingReset[interaction] = setMask(alreadyEvented, MOUSEMASK_LEFT_PRESS);
                    Event evvie = interaction.getEvent(MOUSEMASK_LEFT_PRESS, graphics.intersectionPoint(ray) ?? new Vector3());
                    if (evvie != null) newEvents.Add(evvie);
                }
            }
        }

        private KeyboardFocus kf;
        private Microsoft.Xna.Framework.Input.KeyboardState previousKeyboard;
        public void registerKeyboardFocus(KeyboardFocus kf) {
            this.kf = kf;
        }
        private void checkKeyboard(System.Collections.Generic.List<Event> newEvents) {
            if (this.kf == null) return;
            Microsoft.Xna.Framework.Input.KeyboardState keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            if (this.previousKeyboard == null) this.previousKeyboard = keyboard;
//            Console.WriteLine(keyboard.GetPressedKeys());
            foreach (Microsoft.Xna.Framework.Input.Keys key in keyboard.GetPressedKeys()) {
                if (previousKeyboard.IsKeyUp(key)) newEvents.Add(this.kf.getEvent((int)key));
            }
            this.previousKeyboard = keyboard;
        }

        /// <summary>
        /// Checks state of user input devices to see if an input event should be triggered.
        /// If so, collects the Event objects and inserts them into the given list.
        /// Make it quick!
        /// </summary>
        /// <param name="newEventList">The list into which newly detected events are to be inserted.</param>
        protected override void retrieveInput(System.Collections.Generic.List<Event> newEvents) {
            this.checkKeyboard(newEvents);

            // Get mouse state
            Microsoft.Xna.Framework.Input.MouseState mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();

//            System.Console.WriteLine(mouse.LeftButton);

            Vector3 near = new Vector3(mouse.X, mouse.Y, 0f);
            Vector3 far = new Vector3(mouse.X, mouse.Y, 1f);
            Matrix world = user.worldTransform;

            Vector3 nearPt = graphicsDevice.Viewport.Unproject(near, user.camera.Projection, user.camera.View, world);
            Vector3 farPt = graphicsDevice.Viewport.Unproject(far, user.camera.Projection, user.camera.View, world);

            Vector3 dir = farPt - nearPt;
            dir.Normalize();

            Ray ray = new Ray(user.camera.Position, dir);
            
            // Reset old events
            resetEvents(newEvents, ray);
            // Loop through all of the User's LoadRegions
            foreach (InteractionEngine.Constructs.LoadRegion loadRegion in GameWorld.GameWorld.user.getLoadRegionList()) {
                // Loop through all the LoadRegion's GameObjects
                for (int i = 0; i < loadRegion.getObjectCount(); i++) {
                    Constructs.GameObjectable gameObject = loadRegion.getObject(i);
                    // See if this GameObject can be interacted with.
                    if (gameObject is Interactable3D) {
                        checkInteractableForInteraction(newEvents, ray, (Interactable3D)gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Output stuff.
        /// </summary>
        public override void output() {
            // Loop through the user's LoadRegions
            foreach (Constructs.LoadRegion loadRegion in GameWorld.GameWorld.user.getLoadRegionList()) {
                // Loop through the GameObjects within those LoadRegions
                for (int i = 0; i < loadRegion.getObjectCount(); i++) {
                    Constructs.GameObjectable gameObject = loadRegion.getObject(i);
                    // Go through every GameObject and see if they have something to output
                    if (gameObject is Graphable)
                        ((Graphable)gameObject).getGraphics().onDraw();
                }
            }

        }

        /// <summary>
        /// Initialize stuff.
        /// </summary>
        public override void initialize() {
            user = (User3D)GameWorld.GameWorld.user;
            graphicsDevice = GameWorld.GameWorld.game.GraphicsDevice;
            user.camera.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GameWorld.GameWorld.game.GraphicsDevice.Viewport.AspectRatio, 1.0f, 1000.0f);
            GameWorld.GameWorld.game.GraphicsDevice.RenderState.CullMode = CullMode.None;
            base.initialize();
        }

    }


    public class ModelEffect {

        private BasicEffect basicEffect;
        private EffectPool effectPool;

        public float Alpha = 1f;
        public Vector3 AmbientLightColor = Vector3.One;
        public EffectTechnique CurrentTechnique;
        public Vector3 DiffuseColor = Vector3.One;
        public Vector3 EmissiveColor;// = Vector3.One;
        public Vector3 FogColor = Vector3.One;
        public bool FogEnabled;
        public float FogEnd;
        public float FogStart;
        public bool LightingEnabled = true;
        public bool PreferPerPixelLighting;
        public Vector3 SpecularColor = Vector3.One;
        public float SpecularPower = 1;
        public Texture2D Texture;
        public bool TextureEnabled;
        public bool VertexColorEnabled;
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

        public ModelEffect(EffectPool effectPool) : this() {
            this.effectPool = effectPool;
        }

        public ModelEffect(EffectPool effectPool, ModelEffect source)
            : this(effectPool) {
            this.CloneFrom(source);
        }

        public ModelEffect() {
        }

        public void Initialize(Camera camera) {
            this.basicEffect = new BasicEffect(GameWorld.GameWorld.game.GraphicsDevice, effectPool);
            this.CommitProperties();
            this.ActiveCamera = camera;
            this.setTextureName(this.textureName);
            if (this.enableDefaultLighting) this.basicEffect.EnableDefaultLighting();
        }

        public void setTextureName(string textureName) {
            this.textureName = textureName;
            if (basicEffect != null && this.textureName != null) {
                this.Texture = GameWorld.GameWorld.game.Content.Load<Texture2D>(this.textureName);
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

        public Effect Effect {
            get { return basicEffect; }
        }

    }

    public class Graphics3D : Graphics {


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
        public Graphics3D(Graphable3D gameObject, ModelEffect effect, string modelName)
        {
            this.gameObject = gameObject;

            this.effect = effect;

            this.modelName = modelName;

            this.worldLocal = Matrix.Identity;

        }

         public ModelEffect Effect
        {
            get { return this.effect; }
        }

        public Matrix World
        {
            get { return this.effect.World; }
        }

        public BoundingSphere BoundingSphere
        {
            get { return new BoundingSphere(baseBoundingSphere.Center + this.gameObject.getLocation().getPoint(), baseBoundingSphere.Radius * this.scale); }
        }

        public void calculateBoundingSphere()
        {
            BoundingSphere welded = this.model.Meshes[0].BoundingSphere;
            foreach (ModelMesh mesh in this.model.Meshes)
            {
                welded = BoundingSphere.CreateMerged(welded, mesh.BoundingSphere);
            }
            this.baseBoundingSphere = welded;
        }

        private void updateWorld()
        {
            this.worldLocal = localWorld();
            effect.World = worldContainer(Matrix.Identity); // UserInterface3D.user.worldTransform);
        }

        /// <summary>
        /// ports the local world matrix to the [terrain] container
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        private Matrix worldContainer(Matrix container)
        {
            return localWorld() * container;
        }

        /// <summary>
        /// scale, rotate, and translation. change this to add features
        /// </summary>
        /// <returns></returns>
        private Matrix localWorld()
        {
            return Matrix.CreateScale(this.scale) * Matrix.CreateRotationY(this.gameObject.getLocation().yaw) * Matrix.CreateTranslation(this.gameObject.getLocation().getPoint()); //add more later. scale + rotate.
        }

        public void SetScale(float scale)
        {
            this.scale = scale;
            updateWorld();
        }

        /// <summary>
        /// Draw this Graphics3D onto the screen
        /// </summary>
        public virtual void onDraw() {
            foreach (ModelMesh mesh in this.model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {

                    effect.World = worldContainer(UserInterface3D.user.worldTransform); // so that the scale and stuff changes when the terrain scale changes


                    effect.Projection = UserInterface3D.user.camera.Projection;
                    effect.View = UserInterface3D.user.camera.View;
                    effect.EnableDefaultLighting();

                    //
                    //Now copy all data from the ModelEffect to this BasicEffect
                    //
                    effect.Alpha = this.effect.Alpha;
                    effect.AmbientLightColor = this.effect.AmbientLightColor;
                    if (this.effect.CurrentTechnique != null) effect.CurrentTechnique = this.effect.CurrentTechnique;
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
        public virtual float? intersects(Ray ray)
        {
            //foreach (ModelMesh mesh in model.Meshes)
            //{
                float? distance = ray.Intersects(this.BoundingSphere);
                if (distance.HasValue) return distance;
            //}
            return null;
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
            this.model = GameWorld.GameWorld.game.Content.Load<Model>(modelName);
            effect.Initialize(UserInterface3D.user.camera);
            this.calculateBoundingSphere();
            // Also, remove "virtual" unless the plan is to subclass this.
        }

    }

    public interface Graphable3D : Graphable {

        /// <summary>
        /// Gets the Graphics3D module from this GameObject.
        /// </summary>
        /// <returns>The Graphics3D class.</returns>
        Graphics3D getGraphics3D();

    }

    public class Camera : InteractionEngine.Constructs.GameObject {

        #region FACTORY

        // The classHash, a unique identifying string for the class. Hmm, wow, that's kind of redundant, isn't that? C# already provides such a function through reflection. Oh well.
        // Used for the factory methods called when the client receives a CREATE_NEW_OBJECT update from the server computer.
        internal const string classHash = "Camera";

        /// <summary>
        /// The static constructor. Adds the class's factory method to the GameObject factoryList when the class is first loaded.
        /// </summary>
        static Camera() {
            InteractionEngine.Constructs.GameObject.factoryList.Add(classHash, new InteractionEngine.Constructs.GameObjectFactory(makeCamera));
        }

        /// <summary>
        /// A factory method that creates and returns a new instance of Camera. Used by the client when the server requests it to make a new GameObject.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        /// <param name="reader">The PacketReader from which we will read the fields of the newly constructed GameObject.</param>
        /// <returns>A new instance of Camera.</returns>
        static Camera makeCamera(InteractionEngine.Constructs.LoadRegion loadRegion, int id, Microsoft.Xna.Framework.Net.PacketReader reader) {
            if (GameWorld.GameWorld.status != GameWorld.GameWorld.Status.MULTIPLAYER_CLIENT)
                throw new System.Exception("You're not a client, so why are you calling the GameObject factory method?");
            Camera camera = new Camera(loadRegion, id);
            // ORDER OF STUFF (where you used the reader to construct datatypes, used factory methods exclusively. also, construct modules and their datatypes here too.)
            return camera;
        }

        /// <summary>
        /// Constructs a GameObject and assigns it an ID.
        /// This is the constructor that should be used if and only if you are a MULTIPLAYER_CLIENT.
        /// Furthermore, it is only called by the GameObjectFactory method.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        /// <param name="id">This GameObject's ID.</param>
        private Camera(InteractionEngine.Constructs.LoadRegion loadRegion, int id)
            : base(loadRegion, id) {
        }

        /// <summary>
        /// Returns the class hash. 
        /// </summary>
        /// <returns>The class hash. Do we really have to tell you everything twice?</returns>
        public override string getClassHash() {
            return classHash;
        }

        #endregion

        float fovy;
        float aspectRatio;
        float nearPlane;
        float farPlane;

        Matrix projectionMatrix;
        Matrix viewMatrix;

        Vector3 position, target;
        Vector3 heading, strafe, up;

        BoundingFrustum frustum;

        public Vector3 Heading
        {
            get
            {
                return heading;
            }
            set
            {
                heading = value;
                heading.Normalize();
                updateCamera();
            }
        }

        public Matrix Projection
        {
            get
            {
                return projectionMatrix;
            }
            set
            {
                projectionMatrix = value;
                updateCamera();
            }
        }
        public Matrix View
        {
            get
            {
                return viewMatrix;
            }
            set
            {
                viewMatrix = value;
                updateCamera();
            }
        }
        public BoundingFrustum Frustrum
        {
            get
            {
                updateCamera();
                return frustum;
            }
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public void SetPerspectiveFov(float fovy, float aspectRatio, float nearPlane, float farPlane)
        {
            this.fovy = fovy;
            this.aspectRatio = aspectRatio;
            this.farPlane = farPlane;
            this.nearPlane = nearPlane;
            updateCamera();
        }
        public void SetLookAt(Vector3 cameraPos, Vector3 cameraTarget, Vector3 cameraUp)
        {
            this.position = cameraPos;
            this.target = cameraTarget;
            this.heading = this.target - this.position;
            heading.Normalize();
            this.up = cameraUp;
            this.strafe = Vector3.Cross(heading, up);
            updateCamera();
        }
        /// <summary>
        /// Moves the camera + displaces the camera target accordingly
        /// </summary>
        /// <param name="cameraPos"></param>
        public void SetPosition(Vector3 cameraPos)
        {
            target += cameraPos - position;
            position = cameraPos;
            updateCamera();
        }
        /// <summary>
        /// Moves the camera, without displacing the target.
        /// </summary>
        /// <param name="cameraPos"></param>
        public void SetPositionLockTarget(Vector3 cameraPos)
        {
            position = cameraPos;
            updateCamera();
        }

        public void SetTargetDisplacePosition(Vector3 tar)
        {
            position += tar - target;
            target = tar;
            updateCamera();
        }

        /// <summary>
        /// Rotates camera (orbit) around position (axis: y)
        /// </summary>
        /// <param name="posRot"></param>
        /// <param name="rot"></param>
        public void RotateUponPosition(Vector3 posRot, float rot)
        {
            position = Vector3.Transform((posRot - position), Matrix.CreateRotationY(MathHelper.ToRadians(rot))) + position;
            updateCamera();
        }

        /// <summary>
        /// DIE!
        /// </summary>
        /// <param name="posRot"></param>
        /// <param name="axis"></param>
        /// <param name="rot"></param>
        public void RotateUponAxis(Vector3 posRot, Vector3 axis, float rot)
        {
            Vector3 relPosition = position - posRot;
            position = Vector3.Transform(relPosition, Matrix.CreateFromAxisAngle(axis, MathHelper.ToRadians(rot))) + posRot;
            updateCamera();
        }
        public void ChangeAzimuth(Vector3 posRot, Vector3 axis, float amount)
        {
            Vector3 relPosition = position - posRot;
            Vector3 azimuthAxis = Vector3.Cross(relPosition, axis);
            azimuthAxis.Normalize();
            position = Vector3.Transform(relPosition, Matrix.CreateFromAxisAngle(azimuthAxis, MathHelper.ToRadians(amount))) + posRot;
            updateCamera();
        }
        private void updateCamera()
        {
            //projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fovy), aspectRatio, nearPlane, farPlane);
            viewMatrix = Matrix.CreateLookAt(position, target, up);
            frustum = new BoundingFrustum(viewMatrix * projectionMatrix);
        }

        /// <summary>
        /// Constructs a new Camera.
        /// </summary>
        /// <param name="loadRegion">The LoadRegion to which this GameObject belongs.</param>
        public Camera(Constructs.LoadRegion loadRegion)
            : base(loadRegion) {
            
        }


    }


    /**
     * Implemented by GameObjects that can be interacted with.
     */
    public interface Interactable3D : Graphable3D {

        /// <summary>
        /// Gets an Event from this Interactable module.
        /// </summary>
        /// <param name="invoker">The invoker of this Event. If you have multiple possible invokers (ie. mouse click and mouse over) then we recommend you define constants for them.</param>
        /// <param name="user">The User that invokes this Event. Needed often for associating User invokers with GameObject invokers.</param>
        /// <param name="position">The position where the interaction happened, if applicable.</param>
        /// <returns>An Event.</returns>
        Event getEvent(int invoker, Vector3 position);

    }

    public class User3D : InteractionEngine.Server.User {

        public readonly Constructs.LoadRegion localLoadRegion;
        public readonly Camera camera;
        public Matrix worldTransform = Matrix.Identity;

        public User3D()
            : base() { // TODO: "User" inheritance and stuff
            this.localLoadRegion = new Constructs.LoadRegion();
            this.camera = new Camera(this.localLoadRegion);
            this.addLoadRegion(this.localLoadRegion);
            this.addPermission(this.camera);
        }


    }


}