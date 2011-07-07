﻿/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;

namespace MCForge.Gui
{
    public partial class UpdateWindow : Form
    {
        public UpdateWindow()
        {
            InitializeComponent();
        }
        private void UpdateWindow_Load(object sender, EventArgs e)
        {
            UpdLoadProp("properties/update.properties");
            WebClient client = new WebClient();
            client.DownloadFile(ServerSettings.RevisionList, "text/revs.txt");
            listRevisions.Items.Clear();
            FileInfo file = new FileInfo("text/revs.txt");
            StreamReader stRead = file.OpenText();
            if (File.Exists("text/revs.txt"))
            {
                while (!stRead.EndOfStream)
                {
                    listRevisions.Items.Add(stRead.ReadLine());
                }
            }
            stRead.Close();
            stRead.Dispose();
            file.Delete();
            client.Dispose();
        }


        public void UpdSave(string givenPath)
        {
            StreamWriter SW = new StreamWriter(File.Create(givenPath));
            SW.WriteLine("#This file manages the update process");
            SW.WriteLine("#Toggle AutoUpdate to true for the server to automatically update");
            SW.WriteLine("#Notify notifies players in-game of impending restart");
            SW.WriteLine("#Restart Countdown is how long in seconds the server will count before restarting and updating");
            SW.WriteLine();
            SW.WriteLine("autoupdate= " + chkAutoUpdate.Checked.ToString());
            SW.WriteLine("notify = " + chkNotify.Checked.ToString());
            SW.WriteLine("restartcountdown = " + txtCountdown.Text);
            SW.Flush();
            SW.Close();
            SW.Dispose();
            this.Close();
        }


        public void UpdLoadProp(string givenPath)
        {
            if (File.Exists(givenPath))
            {
                string[] lines = File.ReadAllLines(givenPath);

                foreach (string line in lines)
                {
                    if (line != "" && line[0] != '#')
                    {
                        //int index = line.IndexOf('=') + 1; // not needed if we use Split('=')
                        string key = line.Split('=')[0].Trim();
                        string value = line.Split('=')[1].Trim();

                        switch (key.ToLower())
                        {
                            case "autoupdate":
                                chkAutoUpdate.Checked = (value.ToLower() == "true") ? true : false;
                                break;
                            case "notify":
                                chkNotify.Checked = (value.ToLower() == "true") ? true : false;
                                break;
                            case "restartcountdown":
                                txtCountdown.Text = value;
                                break;
                        }
                    }
                }
                //Save(givenPath);
            }
            //else Save(givenPath);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string chkNum = txtCountdown.Text.Trim();
            double Num;
            bool isNum = double.TryParse(chkNum, out Num);
            if (!isNum || txtCountdown.Text == "")
            {
                MessageBox.Show("You must enter a number for the countdown");
            }
            else
            {
                UpdSave("properties/update.properties");
                Server.autoupdate = chkAutoUpdate.Checked;
            }
        }

        private void cmdDiscard_Click(object sender, EventArgs e)
        {
            UpdLoadProp("properties/update.properties");
            this.Close();
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            if (Server.selectedrevision != "")
            {
                MCForge_.Gui.Program.PerformUpdate(true);
                
            }
            else { MCForge_.Gui.Program.PerformUpdate(false); }
      /*      if (!Program.CurrentUpdate)
                Program.UpdateCheck();
            else
            {
                Thread messageThread = new Thread(new ThreadStart(delegate
                {
                    MessageBox.Show("Already checking for updates.");
                })); messageThread.Start();
            } */
        }


        private void listRevisions_SelectedValueChanged(object sender, EventArgs e)
        {
            Server.selectedrevision = listRevisions.SelectedItem.ToString();
            
        }

    }
}
