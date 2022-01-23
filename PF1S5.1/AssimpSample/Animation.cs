using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AssimpSample
{
    class Animation
    {
        private World m_world = null;
        DispatcherTimer timer;
        MainWindow mw = null;

        public Animation(World world, MainWindow mw)
        {
            this.m_world = world;
            this.mw = mw;
            timer = new DispatcherTimer();
            m_world.x_truck_trans = 25;
            m_world.y_truck_trans = 0;
            m_world.z_truck_trans = -47;
            m_world.x_truck_rot = 90;
            m_world.y_truck_rot = 0;
            m_world.z_truck_rot = -90;

            m_world.x_ramp_trans = -7;
            m_world.y_ramp_trans = 4;
            m_world.z_ramp_trans = -10;
            m_world.x_ramp_rot = 0;
            m_world.y_ramp_rot = 90;
            m_world.z_ramp_rot = 0;

            m_world.x_barrel_trans = 1f;
            m_world.y_barrel_trans = 2f;
            m_world.z_barrel_trans = -3f;

        mw.AnimationNotActive = false;
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += new EventHandler(StartAnimation);
            timer.Start();
        }

        private void StartAnimation(object sender, EventArgs e)
        {
            if (m_world.x_truck_trans - 1 <= -5)
            {
                RotateTruck();
            }
            else
            {
                m_world.y_barrel_trans -= (float)0.1;
                m_world.x_truck_trans -= 1;
            }
        }
        public void RotateTruck()
        {
            if (m_world.z_truck_rot + 1 >= 0)
            {
                WaitForRamp();
            }
            else
            {
                m_world.z_truck_trans -= (float)0.7;
                m_world.z_truck_rot += 5;
            }
        }
        public void WaitForRamp()
        {
            if (m_world.z_truck_trans + 1 >= -25)
            {
                OpenRamp();
            }
            else
            {
                m_world.z_truck_trans += 1;
            }
        }
        public void OpenRamp()
        {
            if (m_world.y_ramp_rot + 1 >= 180)
            {
                GoIntoDepony();
            }
            else
            {
                m_world.y_ramp_rot += 1;
            }
        }

        public void GoIntoDepony()
        {
            if (m_world.z_truck_trans + 1 >= 5)
            {
                ParkTruck();
            }
            else
            {
                m_world.z_truck_trans += 1;
            }
        }
        public void ParkTruck()
        {
            if (m_world.z_truck_rot + 1 >= 90)
            {
                GarbageOut();
            }
            else
            {
                m_world.z_truck_rot += 1;
                m_world.x_truck_trans -= (float)0.1;
            }
        }

        public void GarbageOut()
        {

            if (m_world.y_barrel_trans + (float)0.1 >= 6)
            {
                if(m_world.z_barrel_trans + (float)0.1 >= -1)
                {
                    EndAnimation();
                }
                else
                {
                    m_world.z_barrel_trans += (float)0.1;
                }
                
            }
            else
            {
                m_world.y_barrel_trans += (float)0.1;
            }
            
        }

        public void EndAnimation()
        {
            mw.AnimationNotActive = true;
            timer.Stop();
        }
    }
}
