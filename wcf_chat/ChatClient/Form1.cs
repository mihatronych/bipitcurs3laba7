using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatClient.ServiceChat;

namespace ChatClient
{
    public partial class Form1 : Form , IServiceChatCallback
    {
        bool isConnected = false;
        ServiceChatClient client;
        int ID;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        void ConnectUser()
        {
            if (!isConnected)
            {
                client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
                ID = client.Connect(tbUserName.Text);
                tbUserName.Enabled = false;
                bConnDicon.Text = "Disconnect";
                isConnected = true;
                var str = Assembly.GetExecutingAssembly().Location;
                var ar = str.Split('\\');
                str = "";
                for (int i = 0; i < ar.Length - 4; i++)
                {
                    str += ar[i] + "/";
                }
                str += "/ChatHost/bin/Debug/history.txt";
                try
                {
                    var lines = File.ReadAllLines(str, Encoding.UTF8);
                    foreach (var line in lines)
                    {
                        lbChat.Items.Add(line);
                    }
                    lbChat.SelectedIndex = lbChat.Items.Count - 1;
                }
                catch
                {
                    MessageBox.Show("Нет файла истории ");
                }
            }
        }

        void DisonnectUser()
        {
            if (isConnected)
            {
                client.Disconnect(ID);
                client = null;
                tbUserName.Enabled = true;
                bConnDicon.Text = "Connect";
                isConnected = false;
                lbChat.Items.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isConnected)
            {
                DisonnectUser();
            }
            else
            {
                ConnectUser();
            }
        }

        public void MsgCallback(string msg)
        {
            lbChat.Items.Add(msg);
            lbChat.SelectedIndex = lbChat.Items.Count - 1;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisonnectUser();
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (client != null)
                {
                    client.SendMsg(tbMessage.Text, ID);
                    tbMessage.Text = string.Empty;
                }
            }
        }
    }
}
