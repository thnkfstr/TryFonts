namespace Try_Fonts
{
    partial class TryFontsForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TryFontsForm));
      tryTextLabel = new Label();
      tryTextBox = new TextBox();
      fontSizeLabel = new Label();
      fontSizeBox = new NumericUpDown();
      boldCheckBox = new CheckBox();
      italicsCheckBox = new CheckBox();
      searchLabel = new Label();
      searchTextBox = new TextBox();
      containsOrStartsWithComboBox = new ComboBox();
      scrollablePanel = new Panel();
      fontViewingContainer = new FlowLayoutPanel();
      ((System.ComponentModel.ISupportInitialize)fontSizeBox).BeginInit();
      scrollablePanel.SuspendLayout();
      SuspendLayout();
      // 
      // tryTextLabel
      // 
      tryTextLabel.AutoSize = true;
      tryTextLabel.Font = new Font("Segoe UI", 10F);
      tryTextLabel.Location = new Point(22, 16);
      tryTextLabel.Name = "tryTextLabel";
      tryTextLabel.Size = new Size(299, 19);
      tryTextLabel.TabIndex = 11;
      tryTextLabel.Text = "Enter the text you want to see in different fonts";
      // 
      // tryTextBox
      // 
      tryTextBox.Font = new Font("Arial", 16F);
      tryTextBox.Location = new Point(23, 46);
      tryTextBox.Multiline = true;
      tryTextBox.Name = "tryTextBox";
      tryTextBox.Size = new Size(1344, 57);
      tryTextBox.TabIndex = 1;
      tryTextBox.Text = "*The quick brown fox jumps over 10 of the 2,345 lazy dogs @ the farm - starting with #6 && costing $7 (plus $0.89 tax?)!";
      tryTextBox.TextChanged += tryTextBox_TextChanged;
      // 
      // fontSizeLabel
      // 
      fontSizeLabel.AutoSize = true;
      fontSizeLabel.Font = new Font("Segoe UI", 10F);
      fontSizeLabel.Location = new Point(25, 120);
      fontSizeLabel.Name = "fontSizeLabel";
      fontSizeLabel.Size = new Size(32, 19);
      fontSizeLabel.TabIndex = 2;
      fontSizeLabel.Text = "Size";
      fontSizeLabel.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // fontSizeBox
      // 
      fontSizeBox.Font = new Font("Segoe UI", 10F);
      fontSizeBox.Location = new Point(59, 117);
      fontSizeBox.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
      fontSizeBox.Name = "fontSizeBox";
      fontSizeBox.Size = new Size(57, 25);
      fontSizeBox.TabIndex = 3;
      fontSizeBox.TextAlign = HorizontalAlignment.Center;
      fontSizeBox.Value = new decimal(new int[] { 16, 0, 0, 0 });
      fontSizeBox.ValueChanged += fontSizeBox_ValueChanged;
      // 
      // boldCheckBox
      // 
      boldCheckBox.AutoSize = true;
      boldCheckBox.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
      boldCheckBox.Location = new Point(135, 119);
      boldCheckBox.Name = "boldCheckBox";
      boldCheckBox.Size = new Size(59, 23);
      boldCheckBox.TabIndex = 4;
      boldCheckBox.Text = "Bold";
      boldCheckBox.UseVisualStyleBackColor = true;
      boldCheckBox.CheckedChanged += boldCheckBox_CheckedChanged;
      // 
      // italicsCheckBox
      // 
      italicsCheckBox.AutoSize = true;
      italicsCheckBox.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
      italicsCheckBox.Location = new Point(205, 119);
      italicsCheckBox.Name = "italicsCheckBox";
      italicsCheckBox.Size = new Size(65, 23);
      italicsCheckBox.TabIndex = 5;
      italicsCheckBox.Text = "Italics";
      italicsCheckBox.UseVisualStyleBackColor = true;
      italicsCheckBox.CheckedChanged += italicsCheckBox_CheckedChanged;
      // 
      // searchLabel
      // 
      searchLabel.AutoSize = true;
      searchLabel.Font = new Font("Segoe UI", 10F);
      searchLabel.Location = new Point(290, 120);
      searchLabel.Name = "searchLabel";
      searchLabel.Size = new Size(49, 19);
      searchLabel.TabIndex = 6;
      searchLabel.Text = "Search";
      searchLabel.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // searchTextBox
      // 
      searchTextBox.Font = new Font("Segoe UI", 10F);
      searchTextBox.Location = new Point(343, 117);
      searchTextBox.Name = "searchTextBox";
      searchTextBox.Size = new Size(200, 25);
      searchTextBox.TabIndex = 7;
      searchTextBox.TextChanged += searchTextBox_TextChanged;
      // 
      // containsOrStartsWithComboBox
      // 
      containsOrStartsWithComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      containsOrStartsWithComboBox.Font = new Font("Segoe UI", 10F);
      containsOrStartsWithComboBox.FormattingEnabled = true;
      containsOrStartsWithComboBox.Items.AddRange(new object[] { "Contains", "Starts with" });
      containsOrStartsWithComboBox.Location = new Point(560, 117);
      containsOrStartsWithComboBox.MaxDropDownItems = 2;
      containsOrStartsWithComboBox.Name = "containsOrStartsWithComboBox";
      containsOrStartsWithComboBox.Size = new Size(140, 25);
      containsOrStartsWithComboBox.TabIndex = 8;
      containsOrStartsWithComboBox.SelectedIndexChanged += containsOrStartsWithComboBox_SelectedIndexChanged;
      // 
      // scrollablePanel
      // 
      scrollablePanel.AutoScroll = true;
      scrollablePanel.Controls.Add(fontViewingContainer);
      scrollablePanel.Location = new Point(25, 160);
      scrollablePanel.Name = "scrollablePanel";
      scrollablePanel.Size = new Size(1342, 951);
      scrollablePanel.TabIndex = 9;
      // 
      // fontViewingContainer
      // 
      fontViewingContainer.AutoSize = true;
      fontViewingContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      fontViewingContainer.Dock = DockStyle.Top;
      fontViewingContainer.FlowDirection = FlowDirection.TopDown;
      fontViewingContainer.Location = new Point(0, 0);
      fontViewingContainer.Name = "fontViewingContainer";
      fontViewingContainer.Size = new Size(1342, 0);
      fontViewingContainer.TabIndex = 10;
      fontViewingContainer.WrapContents = false;
      // 
      // TryFontsForm
      // 
      AutoScaleDimensions = new SizeF(7F, 17F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1384, 1132);
      Controls.Add(tryTextLabel);
      Controls.Add(tryTextBox);
      Controls.Add(fontSizeLabel);
      Controls.Add(fontSizeBox);
      Controls.Add(boldCheckBox);
      Controls.Add(italicsCheckBox);
      Controls.Add(searchLabel);
      Controls.Add(searchTextBox);
      Controls.Add(containsOrStartsWithComboBox);
      Controls.Add(scrollablePanel);
      Font = new Font("Segoe UI", 10F);
      FormBorderStyle = FormBorderStyle.FixedDialog;
      Icon = (Icon)resources.GetObject("$this.Icon");
      MaximizeBox = false;
      MaximumSize = new Size(1400, 1171);
      Name = "TryFontsForm";
      StartPosition = FormStartPosition.CenterScreen;
      Text = "Try Fonts";
      ((System.ComponentModel.ISupportInitialize)fontSizeBox).EndInit();
      scrollablePanel.ResumeLayout(false);
      scrollablePanel.PerformLayout();
      ResumeLayout(false);
      PerformLayout();
    }

    private void TryFonts_Resize(object sender, EventArgs e)
    {
      scrollablePanel.Width = ClientSize.Width - 60;
      scrollablePanel.Height = ClientSize.Height - 200;
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
    private Label tryTextLabel;
    private TextBox tryTextBox;
    private Label fontSizeLabel;
    private NumericUpDown fontSizeBox;
    private CheckBox boldCheckBox;
    private CheckBox italicsCheckBox;
    private Label searchLabel;
    private TextBox searchTextBox;
    private ComboBox containsOrStartsWithComboBox;
    private Panel scrollablePanel;
    private FlowLayoutPanel fontViewingContainer;
  }
}
