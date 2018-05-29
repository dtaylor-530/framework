// Time Series Prediction using Genetic Programming and Gene Expression Programming
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
using System.IO;
using System.Threading;

using AForge;
using Accord.Genetic;
using Accord.Controls;
using Accord;
using System.Threading.Tasks;
using System.Collections.Generic;
using SampleApp;

namespace SampleApp
{



    public class TimeSeries : GeneticAlgorithmForm
    {

        private int windowSize = 5;
        private int predictionSize = 1;

        private int functionsSet = 0;
        private int geneticMethod = 0;



        #region GUI


        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView dataList;
        private System.Windows.Forms.ColumnHeader yColumnHeader;
        private System.Windows.Forms.Button loadDataButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.GroupBox groupBox2;
        private Accord.Controls.Chart chart;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox populationSizeBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox selectionBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox functionsSetBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox geneticMethodBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox windowSizeBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox predictionSizeBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox iterationsBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox currentIterationBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox currentLearningErrorBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox currentPredictionErrorBox;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox solutionBox;
        private System.Windows.Forms.ColumnHeader estimatedYColumnHeader;
        private System.Windows.Forms.Button moreSettingsButton;
        private System.Windows.Forms.ToolTip toolTip;



        private int headLength = 20;

        //private Thread workerThread = null;
        //private volatile bool needToStop = false;

        private double[,] windowDelimiter = new double[2, 2] { { 0, 0 }, { 0, 0 } };
        private double[,] predictionDelimiter = new double[2, 2] { { 0, 0 }, { 0, 0 } };

        public TimeSeries()
        {
            InitializeComponent();

            //
            chart.AddDataSeries("data", Color.Red, Chart.SeriesType.Dots, 5);
            chart.AddDataSeries("solution", Color.Blue, Chart.SeriesType.Line, 1);
            chart.AddDataSeries("window", Color.LightGray, Chart.SeriesType.Line, 1, false);
            chart.AddDataSeries("prediction", Color.Gray, Chart.SeriesType.Line, 1, false);

            selectionBox.SelectedIndex = selectionMethod;
            functionsSetBox.SelectedIndex = functionsSet;
            geneticMethodBox.SelectedIndex = geneticMethod;
            UpdateSettings();

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Sample data (time series)");
        }


        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataList = new System.Windows.Forms.ListView();
            this.yColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.estimatedYColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.loadDataButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chart = new Accord.Controls.Chart();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.moreSettingsButton = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.iterationsBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.predictionSizeBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.windowSizeBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.geneticMethodBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.functionsSetBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.selectionBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.populationSizeBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.currentPredictionErrorBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.currentLearningErrorBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.currentIterationBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.solutionBox = new System.Windows.Forms.TextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(840, 537);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(992, 536);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataList);
            this.groupBox1.Controls.Add(this.loadDataButton);
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(288, 555);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data";
            // 
            // dataList
            // 
            this.dataList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.yColumnHeader,
            this.estimatedYColumnHeader});
            this.dataList.FullRowSelect = true;
            this.dataList.GridLines = true;
            this.dataList.Location = new System.Drawing.Point(16, 29);
            this.dataList.Name = "dataList";
            this.dataList.Size = new System.Drawing.Size(256, 461);
            this.dataList.TabIndex = 1;
            this.dataList.UseCompatibleStateImageBehavior = false;
            this.dataList.View = System.Windows.Forms.View.Details;
            // 
            // yColumnHeader
            // 
            this.yColumnHeader.Text = "Y:Real";
            this.yColumnHeader.Width = 70;
            // 
            // estimatedYColumnHeader
            // 
            this.estimatedYColumnHeader.Text = "Y:Estimated";
            this.estimatedYColumnHeader.Width = 70;
            // 
            // loadDataButton
            // 
            this.loadDataButton.Location = new System.Drawing.Point(16, 504);
            this.loadDataButton.Name = "loadDataButton";
            this.loadDataButton.Size = new System.Drawing.Size(120, 34);
            this.loadDataButton.TabIndex = 1;
            this.loadDataButton.Text = "&Load";
            this.loadDataButton.Click += new System.EventHandler(this.loadDataButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
            this.openFileDialog.Title = "Select data file";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chart);
            this.groupBox2.Location = new System.Drawing.Point(320, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(480, 555);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Function";
            // 
            // chart
            // 
            this.chart.Location = new System.Drawing.Point(16, 29);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(448, 512);
            this.chart.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.moreSettingsButton);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.iterationsBox);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.predictionSizeBox);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.windowSizeBox);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.geneticMethodBox);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.functionsSetBox);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.selectionBox);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.populationSizeBox);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(816, 15);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(296, 350);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Settings";
            // 
            // moreSettingsButton
            // 
            this.moreSettingsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.moreSettingsButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.moreSettingsButton.Location = new System.Drawing.Point(16, 322);
            this.moreSettingsButton.Name = "moreSettingsButton";
            this.moreSettingsButton.Size = new System.Drawing.Size(40, 21);
            this.moreSettingsButton.TabIndex = 17;
            this.moreSettingsButton.Text = ">>";
            this.toolTip.SetToolTip(this.moreSettingsButton, "More settings");
            this.moreSettingsButton.Click += new System.EventHandler(this.moreSettingsButton_Click);
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(200, 322);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(93, 20);
            this.label10.TabIndex = 16;
            this.label10.Text = "( 0 - inifinity )";
            // 
            // iterationsBox
            // 
            this.iterationsBox.Location = new System.Drawing.Point(200, 292);
            this.iterationsBox.Name = "iterationsBox";
            this.iterationsBox.Size = new System.Drawing.Size(80, 26);
            this.iterationsBox.TabIndex = 15;

            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(16, 295);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(112, 24);
            this.label9.TabIndex = 14;
            this.label9.Text = "Iterations:";
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label8.Location = new System.Drawing.Point(16, 278);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(264, 3);
            this.label8.TabIndex = 13;
            // 
            // predictionSizeBox
            // 
            this.predictionSizeBox.Location = new System.Drawing.Point(200, 234);
            this.predictionSizeBox.Name = "predictionSizeBox";
            this.predictionSizeBox.Size = new System.Drawing.Size(80, 26);
            this.predictionSizeBox.TabIndex = 12;
            this.predictionSizeBox.TextChanged += new System.EventHandler(this.predictionSizeBox_TextChanged);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(16, 237);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(144, 23);
            this.label7.TabIndex = 11;
            this.label7.Text = "Prediction size:";
            // 
            // windowSizeBox
            // 
            this.windowSizeBox.Location = new System.Drawing.Point(200, 197);
            this.windowSizeBox.Name = "windowSizeBox";
            this.windowSizeBox.Size = new System.Drawing.Size(80, 26);
            this.windowSizeBox.TabIndex = 10;
            this.windowSizeBox.TextChanged += new System.EventHandler(this.windowSizeBox_TextChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(16, 200);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(128, 24);
            this.label6.TabIndex = 9;
            this.label6.Text = "Window size:";
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(16, 183);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(264, 3);
            this.label5.TabIndex = 8;
            // 
            // geneticMethodBox
            // 
            this.geneticMethodBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.geneticMethodBox.Items.AddRange(new object[] {
            "GP",
            "GEP"});
            this.geneticMethodBox.Location = new System.Drawing.Point(176, 139);
            this.geneticMethodBox.Name = "geneticMethodBox";
            this.geneticMethodBox.Size = new System.Drawing.Size(104, 28);
            this.geneticMethodBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 142);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(160, 23);
            this.label4.TabIndex = 6;
            this.label4.Text = "Genetic method:";
            // 
            // functionsSetBox
            // 
            this.functionsSetBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.functionsSetBox.Items.AddRange(new object[] {
            "Simple",
            "Extended"});
            this.functionsSetBox.Location = new System.Drawing.Point(176, 102);
            this.functionsSetBox.Name = "functionsSetBox";
            this.functionsSetBox.Size = new System.Drawing.Size(104, 28);
            this.functionsSetBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(160, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "Function set:";
            // 
            // selectionBox
            // 
            this.selectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectionBox.Items.AddRange(new object[] {
            "Elite",
            "Rank",
            "Roulette"});
            this.selectionBox.Location = new System.Drawing.Point(176, 66);
            this.selectionBox.Name = "selectionBox";
            this.selectionBox.Size = new System.Drawing.Size(104, 28);
            this.selectionBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "Selection method:";
            // 
            // populationSizeBox
            // 
            this.populationSizeBox.Location = new System.Drawing.Point(200, 29);
            this.populationSizeBox.Name = "populationSizeBox";
            this.populationSizeBox.Size = new System.Drawing.Size(80, 26);
            this.populationSizeBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Population size:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.currentPredictionErrorBox);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.currentLearningErrorBox);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.currentIterationBox);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Location = new System.Drawing.Point(816, 373);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(296, 146);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Current iteration:";
            // 
            // currentPredictionErrorBox
            // 
            this.currentPredictionErrorBox.Location = new System.Drawing.Point(200, 102);
            this.currentPredictionErrorBox.Name = "currentPredictionErrorBox";
            this.currentPredictionErrorBox.ReadOnly = true;
            this.currentPredictionErrorBox.Size = new System.Drawing.Size(80, 26);
            this.currentPredictionErrorBox.TabIndex = 5;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(16, 105);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(160, 24);
            this.label13.TabIndex = 4;
            this.label13.Text = "Prediction error:";
            // 
            // currentLearningErrorBox
            // 
            this.currentLearningErrorBox.Location = new System.Drawing.Point(200, 66);
            this.currentLearningErrorBox.Name = "currentLearningErrorBox";
            this.currentLearningErrorBox.ReadOnly = true;
            this.currentLearningErrorBox.Size = new System.Drawing.Size(80, 26);
            this.currentLearningErrorBox.TabIndex = 3;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(16, 69);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(128, 23);
            this.label12.TabIndex = 2;
            this.label12.Text = "Learning error:";
            // 
            // currentIterationBox
            // 
            this.currentIterationBox.Location = new System.Drawing.Point(200, 29);
            this.currentIterationBox.Name = "currentIterationBox";
            this.currentIterationBox.ReadOnly = true;
            this.currentIterationBox.Size = new System.Drawing.Size(80, 26);
            this.currentIterationBox.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(16, 32);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(112, 24);
            this.label11.TabIndex = 0;
            this.label11.Text = "Iteration:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.solutionBox);
            this.groupBox5.Location = new System.Drawing.Point(16, 577);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1096, 73);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Solution";
            // 
            // solutionBox
            // 
            this.solutionBox.Location = new System.Drawing.Point(16, 29);
            this.solutionBox.Name = "solutionBox";
            this.solutionBox.ReadOnly = true;
            this.solutionBox.Size = new System.Drawing.Size(1064, 26);
            this.solutionBox.TabIndex = 0;
            // 
            // TimeSeries
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.ClientSize = new System.Drawing.Size(1182, 698);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "TimeSeries";
            this.Text = "Time Series Prediction using Genetic Programming and Gene Expression Programming";
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            this.Controls.SetChildIndex(this.groupBox5, 0);
            this.Controls.SetChildIndex(this.startButton, 0);
            this.Controls.SetChildIndex(this.stopButton, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
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
            iterationsBox.Text = iterations.ToString();
            windowSizeBox.Text = windowSize.ToString();
            predictionSizeBox.Text = predictionSize.ToString();
        }

        // Load data
        private void loadDataButton_Click(object sender, System.EventArgs e)
        {
            // show file selection dialog
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader reader = null;
                // read maximum 50 points
                double[] tempData = new double[50];

                try
                {
                    // open selected file
                    reader = File.OpenText(openFileDialog.FileName);
                    string str = null;
                    int i = 0;

                    // read the data
                    while ((i < 50) && ((str = reader.ReadLine()) != null))
                    {
                        // parse the value
                        tempData[i] = double.Parse(str);

                        i++;
                    }

                    // allocate and set data
                    data = new double[i];
                    dataToShow = new double[i, 2];
                    Array.Copy(tempData, 0, data, 0, i);
                    for (int j = 0; j < i; j++)
                    {
                        dataToShow[j, 0] = j;
                        dataToShow[j, 1] = data[j];
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed reading the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    // close file
                    if (reader != null)
                        reader.Close();
                }

                // update list and chart
                UpdateDataListView();
                chart.RangeX = new Range(0, data.Length - 1);
                chart.UpdateDataSeries("data", dataToShow);
                chart.UpdateDataSeries("solution", null);
                // set delimiters
                UpdateDelimiters();
                // enable "Start" button
                startButton.Enabled = true;
            }
        }

        // Update delimiters on the chart
        private void UpdateDelimiters()
        {
            // window delimiter
            windowDelimiter[0, 0] = windowDelimiter[1, 0] = windowSize;
            windowDelimiter[0, 1] = chart.RangeY.Min;
            windowDelimiter[1, 1] = chart.RangeY.Max;
            chart.UpdateDataSeries("window", windowDelimiter);
            // prediction delimiter
            predictionDelimiter[0, 0] = predictionDelimiter[1, 0] = data.Length - 1 - predictionSize;
            predictionDelimiter[0, 1] = chart.RangeY.Min;
            predictionDelimiter[1, 1] = chart.RangeY.Max;
            chart.UpdateDataSeries("prediction", predictionDelimiter);
        }

        // Update data in list view
        private void UpdateDataListView()
        {
            // remove all current records
            dataList.Items.Clear();
            // add new records
            for (int i = 0, n = data.GetLength(0); i < n; i++)
            {
                dataList.Items.Add(data[i].ToString());
            }
        }

        // Delegates to enable async calls for setting controls properties
        private delegate void EnableCallback(bool enable);

        // Enable/disale controls (safe for threading)
        private void EnableControls(bool enable)
        {
            if (InvokeRequired & !IsDisposed)
            {
                

                EnableCallback d = new EnableCallback(EnableControls);
                Invoke(d, new object[] { enable });
            }
            else
            {

                loadDataButton.Enabled = enable;
                populationSizeBox.Enabled = enable;
                iterationsBox.Enabled = enable;
                selectionBox.Enabled = enable;
                functionsSetBox.Enabled = enable;
                geneticMethodBox.Enabled = enable;
                windowSizeBox.Enabled = enable;
                predictionSizeBox.Enabled = enable;
                moreSettingsButton.Enabled = enable;

                startButton.Enabled = enable;
                stopButton.Enabled = !enable;
            }
        }

        // On window size changed
        private void windowSizeBox_TextChanged(object sender, System.EventArgs e)
        {
            UpdateWindowSize();
        }

        // On prediction changed
        private void predictionSizeBox_TextChanged(object sender, System.EventArgs e)
        {
            UpdatePredictionSize();
        }

        // Update window size
        private void UpdateWindowSize()
        {
            if (data != null)
            {
                // get new window size value
                windowSize = int.TryParse(windowSizeBox.Text, out int result) ? Math.Max(5, Math.Min(15, result)) : 1;

                // check if we have too few data
                if (windowSize >= data.Length)
                    windowSize = 1;
                // update delimiters
                UpdateDelimiters();
            }
        }

        // Update prediction size
        private void UpdatePredictionSize()
        {
            if (data != null)
            {
                // get new prediction size value

                predictionSize = int.TryParse(predictionSizeBox.Text, out int result) ? Math.Max(1, Math.Min(10, result)) : 1;

                // check if we have too few data
                if (data.Length - predictionSize - 1 < windowSize)
                    predictionSize = 1;
                // update delimiters
                UpdateDelimiters();
            }
        }



        // On "More settings" button click
        private void moreSettingsButton_Click(object sender, System.EventArgs e)
        {
            var settingsDlg = new SettingsDialog();

            // init the dialog
            settingsDlg.MaxInitialTreeLevel = GPTreeChromosome.MaxInitialLevel;
            settingsDlg.MaxTreeLevel = GPTreeChromosome.MaxLevel;
            settingsDlg.HeadLength = headLength;

            // show the dialog
            if (settingsDlg.ShowDialog() == DialogResult.OK)
            {
                GPTreeChromosome.MaxInitialLevel = settingsDlg.MaxInitialTreeLevel;
                GPTreeChromosome.MaxLevel = settingsDlg.MaxTreeLevel;
                headLength = settingsDlg.HeadLength;
            }
        }




        // Clear current solution
        private void ClearSolution()
        {
            // remove solution form chart
            chart.UpdateDataSeries("solution", null);
            // remove it from solution box
            solutionBox.Text = string.Empty;
            // remove it from data list view
            for (int i = 0, n = dataList.Items.Count; i < n; i++)
            {
                if (dataList.Items[i].SubItems.Count > 1)
                    dataList.Items[i].SubItems.RemoveAt(1);
            }
        }

        #endregion GUI



        // On button "Start"
        protected override void startButton_Click(object sender, System.EventArgs e)
        {
            ClearSolution();
            // update settings controls
            UpdateSettings();

            // disable all settings controls except "Stop" button
            EnableControls(false);


            SampleApp.TimeSeriesWrap wrap = new SampleApp.TimeSeriesWrap(
                data: data,
                windowSize: windowSize,
                populationSize: int.TryParse(populationSizeBox.Text, out int result1) ? Math.Max(10, Math.Min(100, result1)) : 40,
                predictionSize: predictionSize,
                headLength: headLength,
                selectionMethod: selectionBox.SelectedIndex,
                functionsSet: functionsSetBox.SelectedIndex,
                geneticMethod: geneticMethodBox.SelectedIndex);


            SearchSolution(wrap);

            // reenable settings controls

        }



        private void SearchSolution(SampleApp.TimeSeriesWrap wrap)
        {
            iterations = int.TryParse(iterationsBox.Text, out int result2) ? Math.Max(1, result2) : 100;

            double[,] output = null;
 

            var progressHandler = new Progress<KeyValuePair<int, SampleApp.Result>>(kvp =>
             {
                 output = kvp.Value.Output;
                 ProgressUpdate(kvp,output,wrap);
             });

            cts = new CancellationTokenSource();

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
                   else
                   {
                       if (!cts.IsCancellationRequested)
                           for (int j = windowSize, k = 0, n = data.Length; j < n; j++, k++)
                           {
                               AddSubItem(dataList, j, output[k, 1].ToString());
                           }
                       // prevents a cross-threading error when closing the window;
                       tsk.Wait();
                   }
                   if(!IsClosed)
                   EnableControls(true);
               });

        }


        public void ProgressUpdate(KeyValuePair<int, SampleApp.Result> kvp,double[,] output, TimeSeriesWrap wrap)
        {
            // update info
            chart.UpdateDataSeries("solution", output);
            var bestChromoSome = kvp.Value.BestSolution;
            SetText(solutionBox, bestChromoSome);

            var error = wrap.EvaluateError();

            // update info
            SetText(currentIterationBox, kvp.Key.ToString());
            SetText(currentLearningErrorBox, error.Learning.ToString("F3"));
            SetText(currentPredictionErrorBox, error.Prediction.ToString("F3"));
        }

    
    }
}
