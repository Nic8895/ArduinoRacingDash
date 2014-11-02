﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using iRSDKSharp;
using iRacingSdkWrapper;
using iRacingSdkWrapper.Bitfields;

namespace iRacingSLI
{
    public partial class iRacingSLI : Form
    {
        private SdkWrapper wrapper;
        private connectionHelper connection;

        public iRacingSLI()
        {
            InitializeComponent();
            console("Start iRacingSDK Wrapper");
            wrapper = new SdkWrapper();
            wrapper.EventRaiseType = SdkWrapper.EventRaiseTypes.CurrentThread;
            wrapper.TelemetryUpdateFrequency = 20;

            wrapper.Connected += wrapper_Connected;
            wrapper.Disconnected += wrapper_Disconnected;
            wrapper.SessionInfoUpdated += wrapper_SessionInfoUpdated;
            wrapper.TelemetryUpdated += wrapper_TelemetryUpdated;

            connection = new connectionHelper(console);
            connection.setupConnection(startConnection, cboPorts);

            wrapper.Start();
        }

        private void StatusChanged()
        {
            if (wrapper.IsConnected)
            {
                if (wrapper.IsRunning)
                {
                    statusLabel.Text = "Status: connected!";
                }
                else
                {
                    statusLabel.Text = "Status: disconnected.";
                }
            }
            else
            {
                if (wrapper.IsRunning)
                {
                    statusLabel.Text = "Status: disconnected, waiting for sim...";
                }
                else
                {
                    statusLabel.Text = "Status: disconnected";
                }
            }
        }

        private void wrapper_Connected(object sender, EventArgs e)
        {
            this.StatusChanged();
        }

        private void wrapper_Disconnected(object sender, EventArgs e)
        {
            this.StatusChanged();
        }

        // Event handler called when the session info is updated
        private void wrapper_SessionInfoUpdated(object sender, SdkWrapper.SessionInfoUpdatedEventArgs e)
        {
            console("sessionInfoUpdated");
        }

        // Event handler called when the telemetry is updated
        private void wrapper_TelemetryUpdated(object sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            dataPacket data = new dataPacket(console);
            data.fetch(e.TelemetryInfo, wrapper.Sdk, false);
            connection.send(data.compile(false, 0));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            wrapper.Stop();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (startButton.Text == "Stop")
                stopConnection();
            else
                startConnection(Regex.Match(cboPorts.Text, @"\(([^)]*)\)").Groups[1].Value);

            this.StatusChanged();
        }

        public void startConnection(String port)
        {
            //wrapper.Start();
            startButton.Text = "Stop";
            connection.openSerial(port);
        }

        public void stopConnection()
        {
            //wrapper.Stop();
            startButton.Text = "Start";
            connection.closeSerial();
        }

        public void console(String str)
        {
            if (consoleTextBox.Text.Length > 0)
                consoleTextBox.AppendText("\r\n" + str);
            else
                consoleTextBox.AppendText(str);
        }
    }
}