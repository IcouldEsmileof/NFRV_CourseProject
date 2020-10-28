namespace NFRV_CourseProject
{
    partial class ImageForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        /*protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }*/

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageForm));
            this.PB_Image_original = new System.Windows.Forms.PictureBox();
            this.PB_Image_dithered = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Image_original)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Image_dithered)).BeginInit();
            this.SuspendLayout();
            // 
            // PB_Image_original
            // 
            this.PB_Image_original.Image = ((System.Drawing.Image)(resources.GetObject("PB_Image_original.Image")));
            this.PB_Image_original.Location = new System.Drawing.Point(12, 37);
            this.PB_Image_original.Name = "PB_Image_original";
            this.PB_Image_original.Size = new System.Drawing.Size(566, 671);
            this.PB_Image_original.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_Image_original.TabIndex = 1;
            this.PB_Image_original.TabStop = false;
            // 
            // PB_Image_dithered
            // 
            this.PB_Image_dithered.Location = new System.Drawing.Point(584, 37);
            this.PB_Image_dithered.Name = "PB_Image_dithered";
            this.PB_Image_dithered.Size = new System.Drawing.Size(684, 671);
            this.PB_Image_dithered.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_Image_dithered.TabIndex = 2;
            this.PB_Image_dithered.TabStop = false;
            // 
            // ImageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.PB_Image_dithered);
            this.Controls.Add(this.PB_Image_original);
            this.Name = "ImageForm";
            this.Text = "ImageForm";
            ((System.ComponentModel.ISupportInitialize)(this.PB_Image_original)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Image_dithered)).EndInit();
            this.ResumeLayout(false);

        }

        

        #endregion

        private System.Windows.Forms.PictureBox PB_Image_original;
        private System.Windows.Forms.PictureBox PB_Image_dithered;
    }
}