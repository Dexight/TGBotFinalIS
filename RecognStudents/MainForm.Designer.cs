namespace AForge.WindowsForms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbVideoSource = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.StartButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.originalImageBox = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.processedImgBox = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tresholdTrackBar = new System.Windows.Forms.TrackBar();
            this.statusLabel = new System.Windows.Forms.Label();
            this.ticksLabel = new System.Windows.Forms.Label();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.resolutionsBox = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.originalImageBox)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.processedImgBox)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tresholdTrackBar)).BeginInit();
            this.controlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbVideoSource
            // 
            this.cmbVideoSource.FormattingEnabled = true;
            this.cmbVideoSource.Location = new System.Drawing.Point(17, 711);
            this.cmbVideoSource.Margin = new System.Windows.Forms.Padding(4);
            this.cmbVideoSource.Name = "cmbVideoSource";
            this.cmbVideoSource.Size = new System.Drawing.Size(291, 24);
            this.cmbVideoSource.TabIndex = 1;
            this.cmbVideoSource.SelectionChangeCommitted += new System.EventHandler(this.cmbVideoSource_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 692);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Выбор камеры";
            // 
            // StartButton
            // 
            this.StartButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.StartButton.Location = new System.Drawing.Point(317, 734);
            this.StartButton.Margin = new System.Windows.Forms.Padding(4);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(167, 37);
            this.StartButton.TabIndex = 3;
            this.StartButton.Text = "Старт";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.originalImageBox);
            this.groupBox1.Location = new System.Drawing.Point(1, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(683, 639);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Камера";
            // 
            // originalImageBox
            // 
            this.originalImageBox.Location = new System.Drawing.Point(8, 15);
            this.originalImageBox.Margin = new System.Windows.Forms.Padding(4);
            this.originalImageBox.Name = "originalImageBox";
            this.originalImageBox.Size = new System.Drawing.Size(667, 615);
            this.originalImageBox.TabIndex = 1;
            this.originalImageBox.TabStop = false;
            this.originalImageBox.Click += new System.EventHandler(this.originalImageBox_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.processedImgBox);
            this.panel1.Location = new System.Drawing.Point(845, 15);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 547);
            this.panel1.TabIndex = 12;
            // 
            // processedImgBox
            // 
            this.processedImgBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processedImgBox.Location = new System.Drawing.Point(0, 0);
            this.processedImgBox.Margin = new System.Windows.Forms.Padding(4);
            this.processedImgBox.Name = "processedImgBox";
            this.processedImgBox.Size = new System.Drawing.Size(296, 543);
            this.processedImgBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.processedImgBox.TabIndex = 0;
            this.processedImgBox.TabStop = false;
            this.processedImgBox.Click += new System.EventHandler(this.processedImgBox_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.checkBox1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.tresholdTrackBar);
            this.panel2.Location = new System.Drawing.Point(692, 570);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(589, 121);
            this.panel2.TabIndex = 18;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(397, 62);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(108, 20);
            this.checkBox1.TabIndex = 24;
            this.checkBox1.Text = "Обработать";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(57, 66);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 16);
            this.label2.TabIndex = 23;
            this.label2.Text = "Порог";
            // 
            // tresholdTrackBar
            // 
            this.tresholdTrackBar.LargeChange = 1;
            this.tresholdTrackBar.Location = new System.Drawing.Point(112, 52);
            this.tresholdTrackBar.Margin = new System.Windows.Forms.Padding(4);
            this.tresholdTrackBar.Maximum = 255;
            this.tresholdTrackBar.Name = "tresholdTrackBar";
            this.tresholdTrackBar.Size = new System.Drawing.Size(187, 56);
            this.tresholdTrackBar.TabIndex = 22;
            this.tresholdTrackBar.TickFrequency = 25;
            this.tresholdTrackBar.Value = 120;
            this.tresholdTrackBar.ValueChanged += new System.EventHandler(this.tresholdTrackBar_ValueChanged);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.statusLabel.Location = new System.Drawing.Point(13, 650);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(96, 29);
            this.statusLabel.TabIndex = 24;
            this.statusLabel.Text = "Статус:";
            // 
            // ticksLabel
            // 
            this.ticksLabel.AutoSize = true;
            this.ticksLabel.Location = new System.Drawing.Point(501, 716);
            this.ticksLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ticksLabel.Name = "ticksLabel";
            this.ticksLabel.Size = new System.Drawing.Size(165, 16);
            this.ticksLabel.TabIndex = 30;
            this.ticksLabel.Text = "Ticks for frame processing";
            // 
            // controlPanel
            // 
            this.controlPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.controlPanel.Controls.Add(this.label5);
            this.controlPanel.Controls.Add(this.saveButton);
            this.controlPanel.Controls.Add(this.comboBox1);
            this.controlPanel.Enabled = false;
            this.controlPanel.Location = new System.Drawing.Point(694, 711);
            this.controlPanel.Margin = new System.Windows.Forms.Padding(4);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(591, 72);
            this.controlPanel.TabIndex = 33;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(55, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 16);
            this.label5.TabIndex = 36;
            this.label5.Text = "Выберите класс";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // saveButton
            // 
            this.saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveButton.Location = new System.Drawing.Point(331, 18);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(255, 39);
            this.saveButton.TabIndex = 38;
            this.saveButton.Text = "Сохранить в датасет";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9});
            this.comboBox1.Location = new System.Drawing.Point(18, 27);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(289, 24);
            this.comboBox1.TabIndex = 35;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // resolutionsBox
            // 
            this.resolutionsBox.AllowDrop = true;
            this.resolutionsBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.resolutionsBox.FormattingEnabled = true;
            this.resolutionsBox.Location = new System.Drawing.Point(19, 745);
            this.resolutionsBox.Margin = new System.Windows.Forms.Padding(4);
            this.resolutionsBox.Name = "resolutionsBox";
            this.resolutionsBox.Size = new System.Drawing.Size(289, 24);
            this.resolutionsBox.TabIndex = 34;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(1030, 862);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(255, 39);
            this.button1.TabIndex = 39;
            this.button1.Text = "Проверить нейросеть";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label3.Location = new System.Drawing.Point(1002, 864);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 29);
            this.label3.TabIndex = 40;
            this.label3.Text = "-";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1297, 913);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.resolutionsBox);
            this.Controls.Add(this.controlPanel);
            this.Controls.Add(this.ticksLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbVideoSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Распознавалка";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.originalImageBox)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.processedImgBox)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tresholdTrackBar)).EndInit();
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cmbVideoSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox originalImageBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label ticksLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar tresholdTrackBar;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.PictureBox processedImgBox;
        private System.Windows.Forms.ComboBox resolutionsBox;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
    }
}

