﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Cornerstone.GUI.Dialogs;

namespace MediaPortal.Plugins.MovingPictures.ConfigScreen.Popups {
    public partial class LoadingPopup : Form {
        delegate void VoidDelegate();
        
        public LoadingPopup() {
            InitializeComponent();
            splashPane1.ShowProgressComponents = true;
            MovingPicturesCore.InitializeProgress += new ProgressDelegate(MovingPicturesCore_InitializeProgress);

        }

        private void LoadingPopup_Load(object sender, EventArgs e) {
            centerDialog();

            //Thread workThread = new Thread(new ThreadStart(worker));
            //workThread.Start();
        }

        void MovingPicturesCore_InitializeProgress(string actionName, int percentDone) {
            splashPane1.Status = actionName;
            splashPane1.Progress = percentDone;

            if (percentDone == 100) {
                Thread.Sleep(500);
                CloseThreadSafe();
            }
        }

        private void centerDialog() {
            Rectangle screen = Screen.GetWorkingArea(this);

            this.Location = new Point(((screen.Width - this.Width) / 2) + screen.X,
                                      ((screen.Height - this.Height) / 2) + screen.Y);
        }

        private void worker() {
            for (int i = 0; i < 5; i++) {
                splashPane1.Status = "Loading item #" + i;
                Thread.Sleep(1000);
                splashPane1.Progress = (i+1) * 20;
            }

            CloseThreadSafe();
        }

        private void CloseThreadSafe() {
            if (InvokeRequired) {
                Invoke(new VoidDelegate(CloseThreadSafe));
                return;
            }

            Close();
        }

    }
}
