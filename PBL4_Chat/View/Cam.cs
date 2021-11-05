﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Guna.UI.WinForms;
using Timer = System.Timers.Timer;
using NAudio.Wave;

namespace PBL4_Chat.View
{
    public partial class Cam : Form
    {
        public Cam()
        {
            InitializeComponent();
        }

        public delegate string getUserId();
        public getUserId userId;

        public delegate string getUserReceive();
        public getUserReceive userReceiver;


        private const int BUFFER_SIZE = 1024 * 1000;
        private const int PORT_NUMBER = 9999;

        static ASCIIEncoding encoding = new ASCIIEncoding();
        TcpClient client = new TcpClient();
        NetworkStream ns;

        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;

        MemoryStream ms;

        Thread send;
        Thread receive;
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (videoCaptureDevice.IsRunning == false)
                {
                    videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[cbbCamera.SelectedIndex].MonikerString);
                    videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
                    videoCaptureDevice.Start();
                }
                    send = new Thread(xuLyGui);
                    send.IsBackground = true;
                    send.Start();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
        private static readonly Object objLock = new object();
        void myTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //your code

            bool isNullOrEmpty = pbCamera == null || pbCamera.Image == null;
            if (isNullOrEmpty == true)
            {

            }
            else
            {


                //Image image = (Image)pbCamera.Image.Clone();
                ns = client.GetStream();
                ms = new MemoryStream();
                Invoke((MethodInvoker)(delegate ()
                {
                    var image = pbCamera.Image;
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                }));
                    //pbCamera.Image.Save(ms, pbCamera.Image.RawFormat);
                byte[] buffer = ms.GetBuffer();
                byte[] userId_receive1 = encoding.GetBytes(userId() + " " + userReceiver());
                ns.Write(userId_receive1, 0, userId_receive1.Length);
                ns.Write(buffer, 0, buffer.Length);


                //Thread.Sleep(1000);
                try
                {
                    //Task.Delay(10);
                }
                catch (Exception err)
                {

                }
                //i--;

            }


        }
        System.Timers.Timer myTimer = new Timer(250);
        public void xuLyGui()
        {
            try
            {

                myTimer.Start();
                myTimer.Elapsed += new ElapsedEventHandler(myTimer_Elapsed);
                Task.Delay(10);


                //while (true)
                //{

                //    bool isNullOrEmpty = pbCamera == null || pbCamera.Image == null;
                //    if (isNullOrEmpty == true)
                //    {

                //    }
                //    else
                //    {


                //        //Image image = (Image)pbCamera.Image.Clone();
                //        ns = client.GetStream();
                //        ms = new MemoryStream();
                //        pbCamera.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                //        byte[] buffer = ms.GetBuffer();
                //        byte[] userId_receive1 = encoding.GetBytes(userId() + " " + userReceiver());
                //        ns.Write(userId_receive1, 0, userId_receive1.Length);
                //        ns.Write(buffer, 0, buffer.Length);


                //        Thread.Sleep(500);
                //        try
                //        {
                //            //Task.Delay(10);
                //        }
                //        catch (Exception err)
                //        {

                //        }
                //        //i--;

                //    }
                //}    



            }
            catch (Exception err)
            {

            }
        }
        
        
        //int question = 0;
        byte[] message;
        public void XLNhan()
        {
            try
            {
                while (true)
                {


                    ns = client.GetStream();
                    byte[] byte_choose = new Byte[BUFFER_SIZE];
                    ns.Read(byte_choose, 0, byte_choose.Length);
                    string choose = encoding.GetString(byte_choose);
                    if (userReceiver() == null)
                    {
                        break;
                    }
                    // chat image
                    message = new Byte[BUFFER_SIZE];
                    ns.Read(message, 0, message.Length);
                    // private(sender)
                    //MessageBox.Show("1");
                    //MessageBox.Show(choose);
                    if (choose.Contains("private"))
                    {
                        //MessageBox.Show("2");
                        // kiểm tra người nhận có đang nhắn private không
                        if (userReceiver().Split(' ').Length == 1)
                        {
                            //MessageBox.Show("3");
                            //nameReceiver = BLL_User.instance.BLL_getUserById(userId_receive()).firstName + " " + BLL_User.instance.BLL_getUserById(userId_receive()).lastName;
                            // khi người dùng đang nhắn 1 người khác nhưng 1 người khác gửi tin thì tin nhắn này không hiển thị lên
                            if (string.Compare(choose.Split(' ')[0], userReceiver()) == 0)
                            {
                                //MessageBox.Show("a");
                                msg();
                            }
                            else
                            {

                            }
                        }
                        else
                        {

                        }
                    }

                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }

        }
        // hiển tin nhắn lên textbox
        private void msg()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(msg));
            else
            {

                MemoryStream imagestream1 = new MemoryStream(message);
                Image image2 = Image.FromStream(imagestream1);
                gunaTransfarantPictureBox1.Image = image2;

            }
        }

        public delegate void SetImageCallback(Bitmap bmp);
        private void SetImage(Bitmap image)
        {
            if (pbCamera.InvokeRequired)
            {
                SetImageCallback callback = new SetImageCallback(SetImage);
                this.BeginInvoke(callback, new object[] { image });
            }
            else
            {
                pbCamera.Image = image;
            }
        }
        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            SetImage(bitmap);  
            //pbCamera.Image = bitmap;
        }
        WaveIn wave;
        private void Cam_Load(object sender, EventArgs e)
        {
            // load cbb camera
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo fi in filterInfoCollection)
            {
                cbbCamera.Items.Add(fi.Name);
                cbbCamera.SelectedIndex = 0;
                videoCaptureDevice = new VideoCaptureDevice();
            }
            // load mic
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var device = WaveIn.GetCapabilities(i);
                cbbMic.Items.Add(device.ProductName);
            }
            try
            {

                client.Connect("192.168.1.9", PORT_NUMBER);
                ns = client.GetStream();

                // gửi userId mỗi khi load
                byte[] userId_load = encoding.GetBytes(userId() + " " + "Cam");
                ns.Write(userId_load, 0, userId_load.Length);

                //Thread userThread1 = new Thread(new ThreadStart(() => p.XLNhan()));
                receive = new Thread(XLNhan);
                receive.IsBackground = true;
                receive.Start();
                //Thread receive1 = new Thread(XLNhanVoice);
                //receive1.IsBackground = true;
                //receive1.Start();
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            if (videoCaptureDevice.IsRunning)
            {
                myTimer.Stop();
                videoCaptureDevice.Stop();
            }
        }

        // record
        byte[] buffer;
        int size_buffer;
        private void Wave_DataAvailable(object sender, WaveInEventArgs e)
        {
            //writer.Write(e.Buffer, 0, e.BytesRecorded);
            buffer = e.Buffer;
            size_buffer = e.BytesRecorded;
        }
        private void btn_unMute_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("mmsys.cpl", ",1");
            try
            {
                wave = new WaveIn();
                wave.WaveFormat = new WaveFormat(44100, 1);
                wave.DeviceNumber = cbbMic.SelectedIndex;
                wave.DataAvailable += Wave_DataAvailable;
                wave.StartRecording();

                Thread sendVoice = new Thread(xuLyGuiVoice);
                sendVoice.IsBackground = true;
                sendVoice.Start();
            }
            catch(Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
        private static readonly Object objLock1 = new object();
        void myTimer_ElapsedVoice(object sender, ElapsedEventArgs e)
        {

            ns = client.GetStream();
            byte[] bufferVoice = new byte[1024 * 1024];
            int size_bufferVoice = 0;
            Invoke((MethodInvoker)(delegate ()
            {
                //var image = pbCamera.Image;
                //image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                bufferVoice = buffer;
                size_bufferVoice = size_buffer;
            }));
            byte[] userId_receive1 = encoding.GetBytes(userId() + " " + userReceiver() + " " + "Voice");
            ns.Write(userId_receive1, 0, userId_receive1.Length);
            ns.Write(bufferVoice, 0, size_bufferVoice);

        }
        System.Timers.Timer myTimerVoice = new Timer(200);
        public void xuLyGuiVoice()
        {
            try
            {

                myTimerVoice.Start();
                myTimerVoice.Elapsed += new ElapsedEventHandler(myTimer_ElapsedVoice);
                Task.Delay(10);

            }
            catch (Exception err)
            {

            }
        }

        private void btn_Mute_Click(object sender, EventArgs e)
        {
            wave.StopRecording();
            myTimerVoice.Stop();
        }

    }
}