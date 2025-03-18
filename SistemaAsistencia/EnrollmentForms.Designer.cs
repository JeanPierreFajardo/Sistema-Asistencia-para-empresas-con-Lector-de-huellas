
namespace SistemaAsistencia
{
    partial class EnrollmentForms
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
            System.Windows.Forms.Button CloseButton;
            this.GroupEvents = new System.Windows.Forms.GroupBox();
            this.ListEvents = new System.Windows.Forms.ListBox();
            this.EnrollmentControl = new DPFP.Gui.Enrollment.EnrollmentControl();
            CloseButton = new System.Windows.Forms.Button();
            this.GroupEvents.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupEvents
            // 
            this.GroupEvents.Controls.Add(this.ListEvents);
            this.GroupEvents.Location = new System.Drawing.Point(13, 318);
            this.GroupEvents.Name = "GroupEvents";
            this.GroupEvents.Size = new System.Drawing.Size(473, 146);
            this.GroupEvents.TabIndex = 6;
            this.GroupEvents.TabStop = false;
            this.GroupEvents.Text = "Events";
            // 
            // ListEvents
            // 
            this.ListEvents.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ListEvents.FormattingEnabled = true;
            this.ListEvents.Location = new System.Drawing.Point(16, 19);
            this.ListEvents.Name = "ListEvents";
            this.ListEvents.Size = new System.Drawing.Size(440, 108);
            this.ListEvents.TabIndex = 0;
            // 
            // EnrollmentControl
            // 
            this.EnrollmentControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.EnrollmentControl.EnrolledFingerMask = 0;
            this.EnrollmentControl.Location = new System.Drawing.Point(2, 4);
            this.EnrollmentControl.MaxEnrollFingerCount = 10;
            this.EnrollmentControl.Name = "EnrollmentControl";
            this.EnrollmentControl.ReaderSerialNumber = "00000000-0000-0000-0000-000000000000";
            this.EnrollmentControl.Size = new System.Drawing.Size(492, 314);
            this.EnrollmentControl.TabIndex = 5;
            // 
            // CloseButton
            // 
            CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            CloseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            CloseButton.Location = new System.Drawing.Point(411, 481);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new System.Drawing.Size(75, 23);
            CloseButton.TabIndex = 4;
            CloseButton.Text = "Close";
            CloseButton.UseVisualStyleBackColor = true;
            CloseButton.Click += new System.EventHandler(this.CloseButton_Click_1);
            // 
            // EnrollmentForms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 509);
            this.Controls.Add(this.GroupEvents);
            this.Controls.Add(CloseButton);
            this.Controls.Add(this.EnrollmentControl);
            this.Name = "EnrollmentForms";
            this.Text = "EnrollmentForms";
            this.GroupEvents.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GroupEvents;
        private System.Windows.Forms.ListBox ListEvents;
        private DPFP.Gui.Enrollment.EnrollmentControl EnrollmentControl;
    }
}