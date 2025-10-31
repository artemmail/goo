namespace Goo
{
	partial class Form1
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
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this._EffectList = new System.Windows.Forms.ListBox();
			this._ChkFilter = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox
			// 
			this.pictureBox.Location = new System.Drawing.Point(12, 12);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(600, 450);
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
			this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
			// 
			// propertyGrid
			// 
			this.propertyGrid.Location = new System.Drawing.Point(618, 224);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(170, 213);
			this.propertyGrid.TabIndex = 1;
			// 
			// _EffectList
			// 
			this._EffectList.FormattingEnabled = true;
			this._EffectList.Location = new System.Drawing.Point(618, 12);
			this._EffectList.Name = "_EffectList";
			this._EffectList.Size = new System.Drawing.Size(170, 199);
			this._EffectList.TabIndex = 2;
			// 
			// _ChkFilter
			// 
			this._ChkFilter.AutoSize = true;
			this._ChkFilter.Location = new System.Drawing.Point(618, 445);
			this._ChkFilter.Name = "_ChkFilter";
			this._ChkFilter.Size = new System.Drawing.Size(77, 17);
			this._ChkFilter.TabIndex = 3;
			this._ChkFilter.Text = "Linear filter";
			this._ChkFilter.UseVisualStyleBackColor = true;
			this._ChkFilter.CheckedChanged += new System.EventHandler(this._ChkFilter_CheckedChanged);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 472);
			this.Controls.Add(this._ChkFilter);
			this.Controls.Add(this._EffectList);
			this.Controls.Add(this.propertyGrid);
			this.Controls.Add(this.pictureBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "Form1";
			this.Text = "CLICK THE IMAGE AND DRAG";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.ListBox _EffectList;
		private System.Windows.Forms.CheckBox _ChkFilter;
	}
}

