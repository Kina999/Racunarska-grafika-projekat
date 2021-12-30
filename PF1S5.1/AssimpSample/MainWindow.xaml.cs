using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;
using System.ComponentModel;

namespace AssimpSample 
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;
        Animation animation = null;
        private bool animationNotActive = true;

        public bool AnimationNotActive
        {
            get
            {
                return animationNotActive;
            }
            set
            {
                animationNotActive = value;
                OnPropertyChanged("AnimationNotActive");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();
            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Truck"),
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Barrel"),
                    "kamaz.3ds", "barrel.3ds", (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);
                float[] tackaBoja = m_world.ambijentalnaKomponentaReflektujucegIzvora;
                slColorR.Value = tackaBoja[0] * 255f;
                slColorG.Value = tackaBoja[1] * 255f;
                slColorB.Value = tackaBoja[2] * 255f;
                DataContext = this;
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.AnimationNotActive)
            {
                switch (e.Key)
                {
                    case Key.Subtract: m_world.m_sceneDistance -= 0.5f; break;
                    case Key.OemPlus: m_world.m_sceneDistance += 0.5f; break;

                    case Key.L: m_world.m_yRotation -= 5f; break;
                    case Key.J: m_world.m_yRotation += 5f; break;
                    
                    case Key.I:
                        if (m_world.m_xRotation + 5f > 40f)
                        {
                            m_world.m_xRotation = 40f;
                        }
                        else
                        {
                            m_world.m_xRotation += 5f;
                        }
                        break;
                    case Key.K:
                        if (m_world.m_xRotation - 5f < -40f)
                        {
                            m_world.m_xRotation = -40f;
                        }
                        else
                        {
                            m_world.m_xRotation -= 5f;
                        }
                        break;

                    case Key.P: animation = new Animation(m_world, this); break;
                    case Key.Q: Application.Current.Shutdown(); break;
                }
            }
        }

        private void slColorR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_world.rampHeight = (float)slColorRamp.Value / 50;
        }

        private void slColorZ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_world.wallHeight = (float)slColorWall.Value / 50;
        }

        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_world.ambijentalnaKomponentaReflektujucegIzvora = new float[] { (float)slColorR.Value / 255f, (float)slColorG.Value / 255f, (float)slColorB.Value / 255f, 1f };
        }
    }
}
