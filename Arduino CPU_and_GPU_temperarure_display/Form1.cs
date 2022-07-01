using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using OpenHardwareMonitor.Hardware;


namespace Arduino_CPU_and_GPU_temperarure_display
{
    public partial class Form1 : Form
    {
        
        SerialPort port = new SerialPort();
        Computer c = new Computer()
        {
            GPUEnabled = true,
            CPUEnabled = true
        };

        int value1, value2, value3, value4, tick;
        bool flag = true;
        

        public Form1()
        {
            InitializeComponent();
            Init();
            c.Open();
            button1_Click(null,null);
            
        }


        private void Init()
        {
            comboBox2.SelectedIndex = 4;
            try
            {
                notifyIcon1.Visible = false;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.DataBits = 8;
                port.Handshake = Handshake.None;
                port.RtsEnable = true;
                string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    comboBox1.Items.Add(port);
                }
                port.BaudRate = 9600;
                comboBox1.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Status();
            tick++;
            if (flag)
            {
                notifyIcon1.Visible = true;
                Hide();
            }
                
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!port.IsOpen)
                {
                    port.PortName = comboBox1.Text;
                    port.Open();
                    timer1.Interval = Convert.ToInt32(comboBox2.Text);
                    timer1.Enabled = true;
                    label3.Text = "Connected";
                }
            }


            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                port.Write("DIS*");
                port.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            label3.Text = "Disconnected";
            timer1.Enabled = false;
        }

       

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            flag = false;
            Show();
            ShowInTaskbar = true;
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }

      
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;   
                Hide();
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (port.IsOpen == true)
            {
                port.Write("DIS*");
                port.Close();
            }
            
        }

        private void Status()
        {
            foreach (var hardwadre in c.Hardware)
            {
                if (hardwadre.HardwareType == HardwareType.GpuNvidia)
                {
                    hardwadre.Update();
                    foreach (var sensor in hardwadre.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                            value1 = (int)sensor.Value.GetValueOrDefault();

                        if (sensor.SensorType == SensorType.Load)
                            value3 = (int)sensor.Value.GetValueOrDefault();
                    }
                        
                        
                }
                if (hardwadre.HardwareType == HardwareType.CPU)
                {
                    hardwadre.Update();
                    foreach (var sensor in hardwadre.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                            value2 = (int)sensor.Value.GetValueOrDefault();

                        if (sensor.SensorType == SensorType.Load)
                            value4 = (int)sensor.Value.GetValueOrDefault();

                    }
                        
                }

                
            }
            try
            {
                if (tick >= 20 && tick <= 25)
                {
                    if (tick == 25) tick = 0;
                    port.Write(DateTime.Now.ToString("HH:mm:ss") + "t" + DateTime.Now.ToString("dd-MM-yyyy") + "d");
                }

                else
                {
                    port.Write(value1 + "*" + value2 + "#" + value3 + "$" + value4 + "%");
                }
                
            }
            catch (Exception ex)
            {
                timer1.Stop();
                MessageBox.Show(ex.Message);
                label3.Text = "Arduino's not responding...";
            }
        }
    }
}
