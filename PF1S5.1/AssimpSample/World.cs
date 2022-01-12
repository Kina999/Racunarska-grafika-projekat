// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using System.Drawing;
using SharpGL.Enumerations;
using System.Drawing.Imaging;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        private enum TextureObjects { Brick = 0, Road, Asphalt, Stone, Barrier};
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;
        public float[] ambijentalnaKomponentaReflektujucegIzvora = { 1f, 1f, 0f, 1f };
        /// <summary>
        ///	 Identifikatori OpenGL tekstura
        /// </summary>
        private uint[] m_textures = null;

        /// <summary>
        ///	 Putanje do slika koje se koriste za teksture
        /// </summary>
        private string[] m_textureFiles = { "..//..//Images//brick.jpg", "..//..//Images//road.jpg", "..//..//Images//asphalt.jpg", "..//..//Images//stone.jpg", "..//..//Images//barrier.jpg" };

        public float x_truck_trans = 25;
        public float y_truck_trans = 0;
        public float z_truck_trans = -47;
        public float x_truck_rot = 90;
        public float y_truck_rot = 0;
        public float z_truck_rot = -90;

        public float x_ramp_trans = -7;
        public float y_ramp_trans = 4;
        public float z_ramp_trans = -10;
        public float x_ramp_rot = 0;
        public float y_ramp_rot = 90;
        public float z_ramp_rot = 0;

        public float x_barrel_trans = 1f;
        public float y_barrel_trans = 2f;
        public float z_barrel_trans = -3f;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        public float m_cameraZ = 40.0f;
        public float m_cameraY = 40.0f;
        public float m_cameraX = -20.0f;
        public float m_pointZ = -15.0f;
        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;
        private AssimpScene m_barrel;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float m_yRotation = 0.0f;

        public float rampHeight = 0.0f;
        public float wallHeight = 0.0f;
        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float m_sceneDistance = 0.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        private float[] groundVerticles = new float[]
        {
                    -1f, -1f,
                    0.6f, -1f,
                    1f, -0.6f,
                    -1f, -0.6f
        };

        private float[] roadVerticles = new float[]
        {
                    -0.9f, -0.6f,
                    -0.3f, -0.6f,
                    0.3f, 0.4f,
                    -0.1f, 0.4f,
                    0f, 0.06f,
                    1f, 0.06f,
                    1f, 0.4f,
                    0f, 0.4f,

        };
        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }
        public AssimpScene Barrel
        {
            get { return m_barrel; }
            set { m_barrel = value; }
        }
        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String scene1Path, String sceneFileName, String scene1FileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_barrel = new AssimpScene(scene1Path, scene1FileName, gl);
            this.m_width = width;
            this.m_height = height;
            m_textures = new uint[m_textureCount];
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        public void SetupLighting(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            float[] ambientColor = { 1f, 1f, 1f, 1.0f };
            float[] diffuseColor = { 1f, 1f, 1f, 1.0f }; //tackasti izvor bijele boje

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, ambientColor);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, diffuseColor);


            gl.Enable(OpenGL.GL_LIGHT0);

            float[] lightPosition = { 10.0f, 10.0f, -50.0f, 1.0f }; //gore desno izvor svjetlosti

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, lightPosition);

            /*float[] light1pos = new float[] { 0.0f, 5.0f, 5.0f, 1.0f };
            float[] light1ambient = ambijentalnaKomponentaReflektujucegIzvora;
            float[] light1diffuse = new float[] { 0.6f, 0.6f, 0.6f, 1.0f };
            float[] light1specular = new float[] { 1f, 1f, 1f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 45.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, light1pos);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light1ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, light1specular);

            gl.Enable(OpenGL.GL_LIGHT1);*/

            gl.ShadeModel(OpenGL.GL_SMOOTH);

            // Ukljuci automatsku normalizaciju nad normalama
            gl.Enable(OpenGL.GL_AUTO_NORMAL);
            gl.Enable(OpenGL.GL_NORMALIZE);

        }

        public void SetupTextures(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            //wraping da je repeat po obema osama
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);
                System.Diagnostics.Debug.WriteLine(m_textures[i]);
                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);

                //filter za teksture je NN
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);

                image.UnlockBits(imageData);
                image.Dispose();
            }

        }
        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
//            gl.Color(1f, 0f, 0f);
            
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GLU_SMOOTH);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.FrontFace(OpenGL.GL_CW);

            SetupLighting(gl);
            SetupTextures(gl);

            m_scene.LoadScene();
            m_scene.Initialize();

            m_barrel.LoadScene();
            m_barrel.Initialize();

        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LookAt(m_cameraX, m_cameraY, m_cameraZ, 0.0f, 0.0f, m_pointZ, 0.0f, 1.0f, 0.0f);

            gl.Translate(0, 0, m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0, 0);
            gl.Rotate(m_yRotation, 0, 1.0f, 0);

            DrawRoad1(gl);
            DrawGround1(gl);
            DrawTruck1(gl);
            DrawRamp1(gl);
            DrawWalls1(gl);
            DrawText1(gl);

            gl.LoadIdentity();
        }

        #endregion Metode

        #region FirstPartMethods
        public void DrawText1(OpenGL gl)
        {
            gl.Viewport((int)(m_width/4), 0, (int)(m_width/1.4), (int)(m_height/1.1));
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            
            gl.Ortho2D(-10.0, 10.0, -10.0, 10.0);
            gl.Color(1.0f, 1.0f, 0.0f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            gl.Translate(-1.5, -3.5, 0);

            gl.PushMatrix();
            gl.DrawText3D("Helvetica", 14.0f, 1f, 0.1f, "Predmet: Racunarska grafika");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.DrawText3D("Helvetica", 14.0f, 1f, 0f, "_______________________");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0f, -1f, 0f);
            gl.DrawText3D("Helvetica", 14.0f, 1f, 0f, "Sk.god:2021/22");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0f, -1f, 0f);
            gl.DrawText3D("Helvetica", 14.0f, 1f, 0f, "_____________");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0f, -2f, 0f);
            gl.DrawText3D("Helvetica", 14.0f, 1f, 0f, "Ime: Katarina");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0f, -2f, 0f);
            gl.DrawText3D("Helvetica", 14.0f, 1f, 0f, "___________");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0f, -3f, 0f);
            gl.DrawText3D("Helvetica", 14.0f, 1f, 0f, "Prezime: Zerajic");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0f, -3f, 0f);
            gl.DrawText3D("Helvetica", 14.0f, 1f, 0f, "_____________");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0f, -4f, 0f);
            gl.DrawText3D("Helvetica", 14.0f, 1f, 0f, "Sifra zad: 5.1");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0f, -4f, 0f);
            gl.DrawText3D("Helvetica", 14.0f, 1f, 0f, "___________");
            gl.PopMatrix();

            Resize(gl, m_width, m_height);
        }
        public void DrawWalls1(OpenGL gl)
        {
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.PushMatrix();
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.Color(0.4, 0.4, 0.4);
            Cube cube = new Cube();

            gl.Translate(-5.5, 5 + wallHeight, 14);
            gl.Scale(25.5, 5 + wallHeight, 1);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Brick]);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            gl.Scale(1.0f + wallHeight/3, 3.0f, 3.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(19, 5 + wallHeight, 3);
            gl.Scale(1, 5 + wallHeight, 11);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Brick]);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            gl.Scale(1f + wallHeight/3, 1.5f, 1.5f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(-30, 5 + wallHeight, 3);
            gl.Scale(1, 5 + wallHeight, 11);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Brick]);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            gl.Scale(1f + wallHeight/3, 1.5f, 1.5f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(-20, 5 + wallHeight, -7);
            gl.Scale(10, 5 + wallHeight, 1);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Brick]);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            gl.Scale(1f + wallHeight/3, 1f, 1f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(11.5, 5 + wallHeight, -7);
            gl.Scale(8, 5 + wallHeight, 1);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Brick]);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            gl.Scale(1f + wallHeight/3, 1f, 1f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();
        }


        public void DrawRamp1(OpenGL gl)
        {
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.PushMatrix();
            gl.Color(0.1, 0.1, 0.1);
            Cube cube = new Cube();

            gl.Translate(-8.4, 2.5 + rampHeight, -10);
            gl.Scale(1.5, 2.5 + rampHeight, 1.5);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Stone]);
            cube.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(1.5, 2.5 + rampHeight, -10);
            gl.Scale(1.5, 2.5 + rampHeight, 1.5); 
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Stone]);
            cube.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(x_ramp_trans, y_ramp_trans + rampHeight*2, z_ramp_trans);
            gl.Rotate(x_ramp_rot, y_ramp_rot, z_ramp_rot);
            Cylinder cylinder = new Cylinder();
            cylinder.BaseRadius = 1;
            cylinder.TopRadius = 1;
            cylinder.Height = 6;
            cylinder.CreateInContext(gl);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Barrier]);
            cylinder.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.PopMatrix();
        }
        public void DrawGround1(OpenGL gl)
        {
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);
            gl.PushMatrix();
            gl.Color(0.6, 0.6, 0.6);
            gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Road]);
            gl.Begin(OpenGL.GL_QUADS);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            gl.Scale(50, 50, 50);
            
            gl.Normal(0.0f, 1.0f, 0.0f);

            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-30.0f, 0.0f, 15.0f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(20.0f, 0.0f, 15.0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(20.0f, 0.0f, -8.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-30.0f, 0.0f, -8.0f);
            gl.End();

            float[] light1pos = new float[] { 0.0f, 3.0f, 0.0f, 1.0f };
            float[] light1pos1 = new float[] { 0.0f, -10.0f, 0.0f, 1.0f };
            float[] light1diffuse = new float[] { 0.6f, 0.6f, 0.6f, 1.0f };
            float[] light1specular = new float[] { 1f, 1f, 1f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 45.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, light1pos1);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, ambijentalnaKomponentaReflektujucegIzvora);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, light1specular);

            gl.Enable(OpenGL.GL_LIGHT1);

            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Flush();
            gl.PopMatrix();
        }
        public void DrawRoad1(OpenGL gl)
        {
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);
            gl.PushMatrix();
            gl.Color(0.5, 0.5, 0.5);
            gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled); 
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Asphalt]);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            gl.Scale(1f, 1f, 0.1f);
            gl.Begin(OpenGL.GL_QUADS);
            
            gl.Normal(0.0f, 1.0f, 0.0f);

            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-10.0f, 0.0f, -8.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(3.0f, 0.0f, -8.0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(3.0f, 0.0f, -80.0f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(-10.0f, 0.0f, -80.0f);

            gl.End();
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Asphalt]);

            gl.Begin(OpenGL.GL_QUADS);

            gl.Normal(0.0f, 1.0f, 0.0f);

            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(1.5f, 0.0f, -38.0f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(50.0f, 0.0f, -38.0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(50.0f, 0.0f, -50.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(1.5f, 0.0f, -50.0f);

            gl.End();

            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.Flush();
            gl.PopMatrix();
        }

        

        public void DrawTruck1(OpenGL gl)
        {
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.PushMatrix();
            gl.Translate(x_truck_trans, y_truck_trans, z_truck_trans);
            gl.Rotate(x_truck_rot, y_truck_rot, z_truck_rot);
            gl.Scale(2, 2, 2);
            m_scene.Draw();
            gl.Translate(x_barrel_trans, y_barrel_trans, z_barrel_trans);
            gl.Rotate(0, 0, 90);
            gl.Scale(1.5f, 1.5f, 1.5f);
            m_barrel.Draw();
            gl.Flush();
            gl.PopMatrix();
        }
        
        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            gl.Perspective(45f, (double)width / height, 1f, 20000f);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            gl.Viewport(0, 0, width, height);
            
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion FirstPartMethods

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
