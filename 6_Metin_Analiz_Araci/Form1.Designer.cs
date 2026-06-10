namespace MetinAnalizAraci
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.txtMetin = new System.Windows.Forms.TextBox();
            this.btnAnaliz = new System.Windows.Forms.Button();
            this.lblKarakterBaslik = new System.Windows.Forms.Label();
            this.lblKelimeBaslik = new System.Windows.Forms.Label();
            this.lblCumleBaslik = new System.Windows.Forms.Label();
            this.lblKarakter = new System.Windows.Forms.Label();
            this.lblKelime = new System.Windows.Forms.Label();
            this.lblCumle = new System.Windows.Forms.Label();
            this.lblMetinBaslik = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblMetinBaslik
            // 
            this.lblMetinBaslik.AutoSize = true;
            this.lblMetinBaslik.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblMetinBaslik.Location = new System.Drawing.Point(12, 9);
            this.lblMetinBaslik.Name = "lblMetinBaslik";
            this.lblMetinBaslik.Size = new System.Drawing.Size(44, 19);
            this.lblMetinBaslik.TabIndex = 0;
            this.lblMetinBaslik.Text = "Metin";
            // 
            // txtMetin
            // 
            this.txtMetin.Location = new System.Drawing.Point(12, 31);
            this.txtMetin.Multiline = true;
            this.txtMetin.Name = "txtMetin";
            this.txtMetin.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMetin.Size = new System.Drawing.Size(460, 150);
            this.txtMetin.TabIndex = 1;
            // 
            // btnAnaliz
            // 
            this.btnAnaliz.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnAnaliz.Location = new System.Drawing.Point(12, 193);
            this.btnAnaliz.Name = "btnAnaliz";
            this.btnAnaliz.Size = new System.Drawing.Size(120, 35);
            this.btnAnaliz.TabIndex = 2;
            this.btnAnaliz.Text = "Analiz Et";
            this.btnAnaliz.UseVisualStyleBackColor = true;
            this.btnAnaliz.Click += new System.EventHandler(this.btnAnaliz_Click);
            // 
            // lblKarakterBaslik
            // 
            this.lblKarakterBaslik.AutoSize = true;
            this.lblKarakterBaslik.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblKarakterBaslik.Location = new System.Drawing.Point(12, 245);
            this.lblKarakterBaslik.Name = "lblKarakterBaslik";
            this.lblKarakterBaslik.Size = new System.Drawing.Size(118, 19);
            this.lblKarakterBaslik.TabIndex = 3;
            this.lblKarakterBaslik.Text = "Karakter Sayısı:";
            // 
            // lblKarakter
            // 
            this.lblKarakter.AutoSize = true;
            this.lblKarakter.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblKarakter.Location = new System.Drawing.Point(140, 245);
            this.lblKarakter.Name = "lblKarakter";
            this.lblKarakter.Size = new System.Drawing.Size(16, 19);
            this.lblKarakter.TabIndex = 4;
            this.lblKarakter.Text = "0";
            // 
            // lblKelimeBaslik
            // 
            this.lblKelimeBaslik.AutoSize = true;
            this.lblKelimeBaslik.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblKelimeBaslik.Location = new System.Drawing.Point(12, 275);
            this.lblKelimeBaslik.Name = "lblKelimeBaslik";
            this.lblKelimeBaslik.Size = new System.Drawing.Size(102, 19);
            this.lblKelimeBaslik.TabIndex = 5;
            this.lblKelimeBaslik.Text = "Kelime Sayısı:";
            // 
            // lblKelime
            // 
            this.lblKelime.AutoSize = true;
            this.lblKelime.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblKelime.Location = new System.Drawing.Point(140, 275);
            this.lblKelime.Name = "lblKelime";
            this.lblKelime.Size = new System.Drawing.Size(16, 19);
            this.lblKelime.TabIndex = 6;
            this.lblKelime.Text = "0";
            // 
            // lblCumleBaslik
            // 
            this.lblCumleBaslik.AutoSize = true;
            this.lblCumleBaslik.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblCumleBaslik.Location = new System.Drawing.Point(12, 305);
            this.lblCumleBaslik.Name = "lblCumleBaslik";
            this.lblCumleBaslik.Size = new System.Drawing.Size(99, 19);
            this.lblCumleBaslik.TabIndex = 7;
            this.lblCumleBaslik.Text = "Cümle Sayısı:";
            // 
            // lblCumle
            // 
            this.lblCumle.AutoSize = true;
            this.lblCumle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCumle.Location = new System.Drawing.Point(140, 305);
            this.lblCumle.Name = "lblCumle";
            this.lblCumle.Size = new System.Drawing.Size(16, 19);
            this.lblCumle.TabIndex = 8;
            this.lblCumle.Text = "0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 341);
            this.Controls.Add(this.lblCumle);
            this.Controls.Add(this.lblCumleBaslik);
            this.Controls.Add(this.lblKelime);
            this.Controls.Add(this.lblKelimeBaslik);
            this.Controls.Add(this.lblKarakter);
            this.Controls.Add(this.lblKarakterBaslik);
            this.Controls.Add(this.btnAnaliz);
            this.Controls.Add(this.txtMetin);
            this.Controls.Add(this.lblMetinBaslik);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Metin Analiz Aracı";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtMetin;
        private System.Windows.Forms.Button btnAnaliz;
        private System.Windows.Forms.Label lblMetinBaslik;
        private System.Windows.Forms.Label lblKarakterBaslik;
        private System.Windows.Forms.Label lblKelimeBaslik;
        private System.Windows.Forms.Label lblCumleBaslik;
        private System.Windows.Forms.Label lblKarakter;
        private System.Windows.Forms.Label lblKelime;
        private System.Windows.Forms.Label lblCumle;
    }
}
