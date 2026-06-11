using System;
using System.Windows.Forms;

namespace TryFontsApp
{
  partial class TryFonts
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
      this.tryTextBox = new System.Windows.Forms.TextBox();
      this.tryTextLabel = new System.Windows.Forms.Label();
      this.fontSizeLabel = new System.Windows.Forms.Label();
      this.fontSizeBox = new System.Windows.Forms.NumericUpDown();
      this.boldCheckBox = new System.Windows.Forms.CheckBox();
      this.italicsCheckBox = new System.Windows.Forms.CheckBox();
      this.fontViewingContainer = new System.Windows.Forms.FlowLayoutPanel();
      this.scrollablePanel = new System.Windows.Forms.Panel();
      this.searchLabel = new System.Windows.Forms.Label();
      this.searchTextBox = new System.Windows.Forms.TextBox();
      this.containsOrStartsWithComboBox = new System.Windows.Forms.ComboBox();
      ((System.ComponentModel.ISupportInitialize)(this.fontSizeBox)).BeginInit();
      this.scrollablePanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // tryTextBox
      // 
      this.tryTextBox.Location = new System.Drawing.Point(30, 39);
      this.tryTextBox.Name = "tryTextBox";
      this.tryTextBox.Size = new System.Drawing.Size(1140, 20);
      this.tryTextBox.TabIndex = 0;
      this.tryTextBox.Text = "*The quick brown fox @ the farm jumps over 10 of the 2,345.6 lazy dogs - " +
                             "starting with #7 && costing $8 (plus 9 cents tax)!?!";
      this.tryTextBox.TextChanged += new System.EventHandler(this.tryTextBox_TextChanged);
      // 
      // tryTextLabel
      // 
      this.tryTextLabel.AutoSize = true;
      this.tryTextLabel.Location = new System.Drawing.Point(27, 16);
      this.tryTextLabel.Name = "tryTextLabel";
      this.tryTextLabel.Size = new System.Drawing.Size(226, 13);
      this.tryTextLabel.TabIndex = 1;
      this.tryTextLabel.Text = "Enter the text you want to see in different fonts";
      // 
      // fontSizeLabel
      // 
      this.fontSizeLabel.AutoSize = true;
      this.fontSizeLabel.Location = new System.Drawing.Point(30, 75);
      this.fontSizeLabel.Name = "fontSizeLabel";
      this.fontSizeLabel.Size = new System.Drawing.Size(27, 13);
      this.fontSizeLabel.TabIndex = 3;
      this.fontSizeLabel.Text = "Size";
      // 
      // fontSizeBox
      // 
      this.fontSizeBox.Location = new System.Drawing.Point(60, 73);
      this.fontSizeBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.fontSizeBox.Name = "fontSizeBox";
      this.fontSizeBox.Size = new System.Drawing.Size(46, 20);
      this.fontSizeBox.TabIndex = 4;
      this.fontSizeBox.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
      this.fontSizeBox.ValueChanged += new System.EventHandler(this.fontSizeBox_ValueChanged);
      // 
      // boldCheckBox
      // 
      this.boldCheckBox.AutoSize = true;
      this.boldCheckBox.Location = new System.Drawing.Point(132, 75);
      this.boldCheckBox.Name = "boldCheckBox";
      this.boldCheckBox.Size = new System.Drawing.Size(47, 17);
      this.boldCheckBox.TabIndex = 5;
      this.boldCheckBox.Text = "Bold";
      this.boldCheckBox.UseVisualStyleBackColor = true;
      this.boldCheckBox.CheckedChanged += new System.EventHandler(this.boldCheckBox_CheckedChanged);
      // 
      // italicsCheckBox
      // 
      this.italicsCheckBox.AutoSize = true;
      this.italicsCheckBox.Location = new System.Drawing.Point(185, 75);
      this.italicsCheckBox.Name = "italicsCheckBox";
      this.italicsCheckBox.Size = new System.Drawing.Size(53, 17);
      this.italicsCheckBox.TabIndex = 6;
      this.italicsCheckBox.Text = "Italics";
      this.italicsCheckBox.UseVisualStyleBackColor = true;
      this.italicsCheckBox.CheckedChanged += new System.EventHandler(this.italicsCheckBox_CheckedChanged);
      // 
      // fontViewingContainer
      // 
      this.fontViewingContainer.AutoSize = true;
      this.fontViewingContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.fontViewingContainer.Dock = System.Windows.Forms.DockStyle.Top;
      this.fontViewingContainer.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.fontViewingContainer.Location = new System.Drawing.Point(0, 0);
      this.fontViewingContainer.Name = "fontViewingContainer";
      this.fontViewingContainer.Size = new System.Drawing.Size(1140, 0);
      this.fontViewingContainer.TabIndex = 7;
      this.fontViewingContainer.WrapContents = false;
      // 
      // scrollablePanel
      // 
      this.scrollablePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.scrollablePanel.AutoScroll = true;
      this.scrollablePanel.Controls.Add(this.fontViewingContainer);
      this.scrollablePanel.Location = new System.Drawing.Point(30, 110);
      this.scrollablePanel.Name = "scrollablePanel";
      this.scrollablePanel.Size = new System.Drawing.Size(1140, 730);
      this.scrollablePanel.TabIndex = 8;
      // 
      // searchLabel
      // 
      this.searchLabel.AutoSize = true;
      this.searchLabel.Location = new System.Drawing.Point(265, 75);
      this.searchLabel.Name = "searchLabel";
      this.searchLabel.Size = new System.Drawing.Size(86, 13);
      this.searchLabel.TabIndex = 9;
      this.searchLabel.Text = "Search for a font";
      // 
      // searchTextBox
      // 
      this.searchTextBox.Location = new System.Drawing.Point(354, 74);
      this.searchTextBox.Name = "searchTextBox";
      this.searchTextBox.Size = new System.Drawing.Size(250, 20);
      this.searchTextBox.TabIndex = 10;
      this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
      // 
      // containsOrStartsWithComboBox
      // 
      this.containsOrStartsWithComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.containsOrStartsWithComboBox.FormattingEnabled = true;
      this.containsOrStartsWithComboBox.Items.AddRange(new object[] {
            "Contains",
            "Starts with"});
      this.containsOrStartsWithComboBox.Location = new System.Drawing.Point(621, 74);
      this.containsOrStartsWithComboBox.MaxDropDownItems = 2;
      this.containsOrStartsWithComboBox.Name = "containsOrStartsWithComboBox";
      this.containsOrStartsWithComboBox.Size = new System.Drawing.Size(121, 21);
      this.containsOrStartsWithComboBox.TabIndex = 11;
      this.containsOrStartsWithComboBox.SelectedIndexChanged += new System.EventHandler(this.containsOrStartsWithComboBox_SelectedIndexChanged);
      // 
      // TryFonts
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1200, 900);
      this.Controls.Add(this.containsOrStartsWithComboBox);
      this.Controls.Add(this.searchTextBox);
      this.Controls.Add(this.searchLabel);
      this.Controls.Add(this.scrollablePanel);
      this.Controls.Add(this.italicsCheckBox);
      this.Controls.Add(this.boldCheckBox);
      this.Controls.Add(this.fontSizeBox);
      this.Controls.Add(this.fontSizeLabel);
      this.Controls.Add(this.tryTextLabel);
      this.Controls.Add(this.tryTextBox);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "TryFonts";
      this.Text = "Try Fonts";
      this.Resize += new System.EventHandler(this.TryFonts_Resize);
      ((System.ComponentModel.ISupportInitialize)(this.fontSizeBox)).EndInit();
      this.scrollablePanel.ResumeLayout(false);
      this.scrollablePanel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private void TryFonts_Resize(object sender, EventArgs e)
    {
      scrollablePanel.Width = this.ClientSize.Width - 60;
      scrollablePanel.Height = this.ClientSize.Height - 200;
      fontViewingContainer.Width = scrollablePanel.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
      foreach (Control control in fontViewingContainer.Controls)
      {
        if (control is TableLayoutPanel rowPanel)
        {
          rowPanel.Dock = DockStyle.Top;
          rowPanel.Width = fontViewingContainer.ClientSize.Width;
          if (rowPanel.ColumnStyles.Count == 2)
          {
            rowPanel.ColumnStyles[0].Width = 70F;
            rowPanel.ColumnStyles[1].Width = 30F;
          }
        }
      }
    }

    #endregion

    private System.Windows.Forms.TextBox tryTextBox;
    private System.Windows.Forms.Label tryTextLabel;
    private System.Windows.Forms.Label fontSizeLabel;
    private System.Windows.Forms.NumericUpDown fontSizeBox;
    private System.Windows.Forms.CheckBox boldCheckBox;
    private System.Windows.Forms.CheckBox italicsCheckBox;
    private System.Windows.Forms.FlowLayoutPanel fontViewingContainer;
    private System.Windows.Forms.Panel scrollablePanel;
    private Label searchLabel;
    private TextBox searchTextBox;
    private ComboBox containsOrStartsWithComboBox;
  }
}
