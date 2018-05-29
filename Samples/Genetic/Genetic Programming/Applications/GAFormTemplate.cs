using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace SampleApp
{
    public class GeneticAlgorithmForm:Form
    {

        protected double[] data = null;
        protected double[,] dataToShow = null;
        protected int selectionMethod = 0;
        protected int populationSize = 100;
        protected int iterations = 1000;
        protected CancellationTokenSource cts = new CancellationTokenSource();
        protected bool IsClosed;

        protected virtual void startButton_Click(object sender, System.EventArgs e)
        {

        }


        protected virtual void stopButton_Click(object sender, System.EventArgs e)
        {
     
            cts.Cancel();
           
            
        }

        private void close(object sender, System.EventArgs e)
        {

            cts.Cancel();
            IsClosed = true;

        }


        public GeneticAlgorithmForm()
        {
            InitializeComponent();
        }

        // Delegates to enable async calls for setting controls properties
        private delegate void AddSubItemCallback(System.Windows.Forms.ListView control, int item, string subitemText);
        // Delegates to enable async calls for setting controls properties
        private delegate void SetTextCallback(System.Windows.Forms.Control control, string text);


        // Thread safe adding of subitem to list control
        protected void AddSubItem(System.Windows.Forms.ListView control, int item, string subitemText)
        {

            if (control.InvokeRequired)
            {
                AddSubItemCallback d = new AddSubItemCallback(AddSubItem);
                Invoke(d, new object[] { control, item, subitemText });
            }
            else
            {
                control.Items[item].SubItems.Add(subitemText);
            }
        }


        // Thread safe updating of control's text property
        protected void SetText(System.Windows.Forms.Control control, string text)
        {
            if (control.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                Invoke(d, new object[] { control, text });
            }
            else
            {
                control.Text = text;
            }
        }


        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected  override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }



 

        #region Windows Form Designer generated code


        protected System.Windows.Forms.Button startButton; 
        protected System.Windows.Forms.Button stopButton;





        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            startButton =  new System.Windows.Forms.Button();
            stopButton = new System.Windows.Forms.Button();
            // 
            // startButton
            // 
            this.startButton.Enabled = false;
            this.startButton.Size = new System.Drawing.Size(120, 34);
            this.startButton.Name = "startButton";
            this.startButton.TabIndex = 3;
            this.startButton.Text = "&Start";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Size = new System.Drawing.Size(120, 34);
            this.stopButton.Name = "stopButton";
            this.stopButton.TabIndex = 4;
            this.stopButton.Text = "S&top";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);


            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "GeneticAlgorithmForm";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.close);

            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);


        }

  

        #endregion




    }
}