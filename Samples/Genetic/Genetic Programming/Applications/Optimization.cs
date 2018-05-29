// 1D Optimization using Genetic Algorithms
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;

using AForge;
using Accord.Genetic;
using Accord.Controls;
using Accord;
using SampleApp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleApp
{

    public class Optimization : GeneticAlgorithmForm
    {

        private UserFunction userFunction = new UserFunction();

        private int chromosomeLength = 32;
        private int optimizationMode = 0;
        private bool showOnlyBest = false;

        #region GUI


        private Accord.Controls.Chart chart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox minXBox;
        private System.Windows.Forms.TextBox maxXBox;
        private System.Windows.Forms.Label label2;

        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox populationSizeBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox chromosomeLengthBox;
        private System.Windows.Forms.CheckBox onlyBestCheck;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox iterationsBox;
        private System.Windows.Forms.Label label6;
        //private System.Windows.Forms.Button startButton;
        //private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox selectionBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox modeBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox currentIterationBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox currentValueBox;





        public Optimization()
        {
            populationSize = 40;


            InitializeComponent();

            // add data series to chart
            chart.AddDataSeries("function", Color.Red, Chart.SeriesType.Line, 1);
            chart.AddDataSeries("solution", Color.Blue, Chart.SeriesType.Dots, 5);
            UpdateChart();

            // update controls
            minXBox.Text = userFunction.Range.Min.ToString();
            maxXBox.Text = userFunction.Range.Max.ToString();
            selectionBox.SelectedIndex = selectionMethod;
            modeBox.SelectedIndex = optimizationMode;
            UpdateSettings();
        }



        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chart = new Accord.Controls.Chart();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.minXBox = new System.Windows.Forms.TextBox();
            this.maxXBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.modeBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.selectionBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.iterationsBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.onlyBestCheck = new System.Windows.Forms.CheckBox();
            this.chromosomeLengthBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.populationSizeBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.currentValueBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.currentIterationBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(544, 463);
            this.startButton.Enabled = true;

            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(688, 463);
            // 
            // chart
            // 
            this.chart.Location = new System.Drawing.Point(16, 29);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(448, 395);
            this.chart.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chart);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.minXBox);
            this.groupBox1.Controls.Add(this.maxXBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(480, 482);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Function";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 434);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "Range:";
            // 
            // minXBox
            // 
            this.minXBox.Location = new System.Drawing.Point(96, 431);
            this.minXBox.Name = "minXBox";
            this.minXBox.Size = new System.Drawing.Size(80, 26);
            this.minXBox.TabIndex = 3;
            this.minXBox.TextChanged += new System.EventHandler(this.minXBox_TextChanged);
            // 
            // maxXBox
            // 
            this.maxXBox.Location = new System.Drawing.Point(208, 431);
            this.maxXBox.Name = "maxXBox";
            this.maxXBox.Size = new System.Drawing.Size(80, 26);
            this.maxXBox.TabIndex = 4;
            this.maxXBox.TextChanged += new System.EventHandler(this.maxXBox_TextChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(184, 434);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "-";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.modeBox);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.selectionBox);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.iterationsBox);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.onlyBestCheck);
            this.groupBox2.Controls.Add(this.chromosomeLengthBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.populationSizeBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(512, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(296, 324);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // modeBox
            // 
            this.modeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeBox.Items.AddRange(new object[] {
            "Maximize",
            "Minimize"});
            this.modeBox.Location = new System.Drawing.Point(176, 139);
            this.modeBox.Name = "modeBox";
            this.modeBox.Size = new System.Drawing.Size(104, 28);
            this.modeBox.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(16, 142);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(176, 25);
            this.label8.TabIndex = 6;
            this.label8.Text = "Optimization mode:";
            // 
            // selectionBox
            // 
            this.selectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectionBox.Items.AddRange(new object[] {
            "Elite",
            "Rank",
            "Roulette"});
            this.selectionBox.Location = new System.Drawing.Point(176, 102);
            this.selectionBox.Name = "selectionBox";
            this.selectionBox.Size = new System.Drawing.Size(104, 28);
            this.selectionBox.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(16, 105);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(160, 24);
            this.label7.TabIndex = 4;
            this.label7.Text = "Selection method:";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(200, 256);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 23);
            this.label6.TabIndex = 10;
            this.label6.Text = "( 0 - inifinity )";
            // 
            // iterationsBox
            // 
            this.iterationsBox.Location = new System.Drawing.Point(200, 227);
            this.iterationsBox.Name = "iterationsBox";
            this.iterationsBox.Size = new System.Drawing.Size(80, 26);
            this.iterationsBox.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(16, 229);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 24);
            this.label5.TabIndex = 8;
            this.label5.Text = "Iterations:";
            // 
            // onlyBestCheck
            // 
            this.onlyBestCheck.Location = new System.Drawing.Point(16, 285);
            this.onlyBestCheck.Name = "onlyBestCheck";
            this.onlyBestCheck.Size = new System.Drawing.Size(230, 23);
            this.onlyBestCheck.TabIndex = 11;
            this.onlyBestCheck.Text = "Show only best solution";
            // 
            // chromosomeLengthBox
            // 
            this.chromosomeLengthBox.Location = new System.Drawing.Point(200, 66);
            this.chromosomeLengthBox.Name = "chromosomeLengthBox";
            this.chromosomeLengthBox.Size = new System.Drawing.Size(80, 26);
            this.chromosomeLengthBox.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(176, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "Chromosome length:";
            // 
            // populationSizeBox
            // 
            this.populationSizeBox.Location = new System.Drawing.Point(200, 29);
            this.populationSizeBox.Name = "populationSizeBox";
            this.populationSizeBox.Size = new System.Drawing.Size(80, 26);
            this.populationSizeBox.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 24);
            this.label3.TabIndex = 0;
            this.label3.Text = "Population size:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.currentValueBox);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.currentIterationBox);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Location = new System.Drawing.Point(512, 343);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(296, 110);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Current iteration";
            // 
            // currentValueBox
            // 
            this.currentValueBox.Location = new System.Drawing.Point(200, 66);
            this.currentValueBox.Name = "currentValueBox";
            this.currentValueBox.ReadOnly = true;
            this.currentValueBox.Size = new System.Drawing.Size(80, 26);
            this.currentValueBox.TabIndex = 3;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(16, 69);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(96, 22);
            this.label10.TabIndex = 2;
            this.label10.Text = "Value:";
            // 
            // currentIterationBox
            // 
            this.currentIterationBox.Location = new System.Drawing.Point(200, 29);
            this.currentIterationBox.Name = "currentIterationBox";
            this.currentIterationBox.ReadOnly = true;
            this.currentIterationBox.Size = new System.Drawing.Size(80, 26);
            this.currentIterationBox.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(16, 32);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(96, 24);
            this.label9.TabIndex = 0;
            this.label9.Text = "Iteration:";
            // 
            // Optimization
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.ClientSize = new System.Drawing.Size(844, 538);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Optimization";
            this.Text = "1D Optimization using Genetic Algorithms";
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.startButton, 0);
            this.Controls.SetChildIndex(this.stopButton, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }





        // Update settings controls
        private void UpdateSettings()
        {
            populationSizeBox.Text = populationSize.ToString();
            chromosomeLengthBox.Text = chromosomeLength.ToString();
            iterationsBox.Text = iterations.ToString();
        }

        // Update chart
        private void UpdateChart()
        {
            // update chart range
            chart.RangeX = userFunction.Range;

            double[,] data = null;

            if (chart.RangeX.Length > 0)
            {
                // prepare data
                data = new double[501, 2];

                double minX = userFunction.Range.Min;
                double length = userFunction.Range.Length;

                for (int i = 0; i <= 500; i++)
                {
                    data[i, 0] = minX + length * i / 500;
                    data[i, 1] = userFunction.OptimizationFunction(data[i, 0]);
                }
            }

            // update chart series
            chart.UpdateDataSeries("function", data);
        }

        // Update min value
        private void minXBox_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                userFunction.Range = new Range(float.Parse(minXBox.Text), userFunction.Range.Max);
                UpdateChart();
            }
            catch
            {
            }
        }

        // Update max value
        private void maxXBox_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                userFunction.Range = new Range(userFunction.Range.Min, float.Parse(maxXBox.Text));
                UpdateChart();
            }
            catch
            {
            }
        }

        // Delegates to enable async calls for setting controls properties
        private delegate void EnableCallback(bool enable);

        // Enable/disale controls (safe for threading)
        private void EnableControls(bool enable)
        {
            if (InvokeRequired)
            {
                EnableCallback d = new EnableCallback(EnableControls);
                Invoke(d, new object[] { enable });
            }
            else
            {
                minXBox.Enabled = enable;
                maxXBox.Enabled = enable;

                populationSizeBox.Enabled = enable;
                chromosomeLengthBox.Enabled = enable;
                iterationsBox.Enabled = enable;
                selectionBox.Enabled = enable;
                modeBox.Enabled = enable;
                onlyBestCheck.Enabled = enable;

                startButton.Enabled = enable;
                stopButton.Enabled = !enable;
            }
        }

        #endregion GUI



        protected override void startButton_Click(object sender, System.EventArgs e)
        {

            // update settings controls
            UpdateSettings();
            // disable all settings controls except "Stop" button
            EnableControls(false);

            SampleApp.Optimisation1DWrap wrap = new SampleApp.Optimisation1DWrap(
                populationSize: int.TryParse(populationSizeBox.Text, out int result1) ? Math.Max(10, Math.Min(100, result1)) : 40,
                chromosomeLength:int.TryParse(chromosomeLengthBox.Text, out int result3) ? Math.Max(64, result3) : 32, 
                userFunction:userFunction, 
                selectionMethod: selectionBox.SelectedIndex, 
                optimizationMode: modeBox.SelectedIndex, 
                showOnlyBest: onlyBestCheck.Checked);

            SearchSolution(wrap);
            
            // enable settings controls
            EnableControls(true);
        }


        // Worker thread
        void SearchSolution(Optimisation1DWrap wrap)
        {

            iterations = int.TryParse(iterationsBox.Text, out int result2) ? Math.Max(1, result2) : 100;

            var progressHandler = new Progress<KeyValuePair<int, SampleApp.Result>>(kvp => ProgressUpdate(kvp));

            Task tsk = Task.Run(() => wrap.RunMultipleEpochs(iterations, cts.Token, progressHandler));
            tsk.ContinueWith(
               t =>
               {
                   // faulted with exception
                   if (t.IsFaulted)
                   {

                       Exception ex = t.Exception;
                       while (ex is AggregateException && ex.InnerException != null)
                           ex = ex.InnerException;
                       MessageBox.Show("Error: " + ex.Message);
                   }
                   else if (t.IsCanceled)
                   {
                       //MessageBox.Show("Cancelled");
                   }
                   // completed successfully

                   if(!IsClosed)
                   EnableControls(true);
               });


        }


        public void ProgressUpdate(KeyValuePair<int, SampleApp.Result> kvp)
        {
            // update info
            chart.UpdateDataSeries("solution", kvp.Value.Output);
            SetText(currentIterationBox, kvp.Key.ToString());
            SetText(currentValueBox, kvp.Value.BestSolution);
        }


    }
}



