
namespace GOL_jacobburgo
{
    partial class SeedDialog
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
            this.applySeedButton = new System.Windows.Forms.Button();
            this.cancelSeedButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.seedNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.timerNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.seedNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timerNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // applySeedButton
            // 
            this.applySeedButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.applySeedButton.Location = new System.Drawing.Point(12, 173);
            this.applySeedButton.Name = "applySeedButton";
            this.applySeedButton.Size = new System.Drawing.Size(90, 30);
            this.applySeedButton.TabIndex = 0;
            this.applySeedButton.Text = "Apply";
            this.applySeedButton.UseVisualStyleBackColor = true;
            // 
            // cancelSeedButton
            // 
            this.cancelSeedButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelSeedButton.Location = new System.Drawing.Point(108, 173);
            this.cancelSeedButton.Name = "cancelSeedButton";
            this.cancelSeedButton.Size = new System.Drawing.Size(90, 30);
            this.cancelSeedButton.TabIndex = 1;
            this.cancelSeedButton.Text = "Cancel";
            this.cancelSeedButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.label1.Location = new System.Drawing.Point(106, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 31);
            this.label1.TabIndex = 2;
            this.label1.Text = "Seed/Timer Editor";
            // 
            // seedNumericUpDown
            // 
            this.seedNumericUpDown.Location = new System.Drawing.Point(250, 80);
            this.seedNumericUpDown.Name = "seedNumericUpDown";
            this.seedNumericUpDown.Size = new System.Drawing.Size(134, 20);
            this.seedNumericUpDown.TabIndex = 3;
            // 
            // timerNumericUpDown
            // 
            this.timerNumericUpDown.Location = new System.Drawing.Point(250, 117);
            this.timerNumericUpDown.Name = "timerNumericUpDown";
            this.timerNumericUpDown.Size = new System.Drawing.Size(134, 20);
            this.timerNumericUpDown.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(134, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Seed for New Render";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(89, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Interval for Timer in Miliseconds";
            // 
            // SeedDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 215);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.timerNumericUpDown);
            this.Controls.Add(this.seedNumericUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelSeedButton);
            this.Controls.Add(this.applySeedButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SeedDialog";
            this.Text = "Seed & Interval";
            ((System.ComponentModel.ISupportInitialize)(this.seedNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timerNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button applySeedButton;
        private System.Windows.Forms.Button cancelSeedButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown seedNumericUpDown;
        private System.Windows.Forms.NumericUpDown timerNumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}