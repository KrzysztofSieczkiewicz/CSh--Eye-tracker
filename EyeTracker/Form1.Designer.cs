namespace EyeTracker
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ExitButton = new Button();
            GetCamerasButton = new Button();
            CamerasList = new TextBox();
            SuspendLayout();
            // 
            // ExitButton
            // 
            ExitButton.Location = new Point(694, 394);
            ExitButton.Name = "ExitButton";
            ExitButton.Size = new Size(94, 44);
            ExitButton.TabIndex = 0;
            ExitButton.Text = "Exit";
            ExitButton.UseVisualStyleBackColor = true;
            // 
            // GetCamerasButton
            // 
            GetCamerasButton.Location = new Point(645, 12);
            GetCamerasButton.Name = "GetCamerasButton";
            GetCamerasButton.Size = new Size(143, 46);
            GetCamerasButton.TabIndex = 1;
            GetCamerasButton.Text = "Get cameras";
            GetCamerasButton.UseVisualStyleBackColor = true;
            // 
            // CamerasList
            // 
            CamerasList.Location = new Point(645, 64);
            CamerasList.Multiline = true;
            CamerasList.Name = "CamerasList";
            CamerasList.ReadOnly = true;
            CamerasList.Size = new Size(143, 128);
            CamerasList.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(CamerasList);
            Controls.Add(GetCamerasButton);
            Controls.Add(ExitButton);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ExitButton;
        private Button GetCamerasButton;
        private TextBox CamerasList;
    }
}