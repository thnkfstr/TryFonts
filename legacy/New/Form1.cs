using System.Timers;

namespace Try_Fonts
{
  public partial class TryFontsForm : Form
  {
    private int loadedFontCount = 0;
    private const int LoadIncrement = 30; // Number of fonts to load at once (page size)
    private System.Timers.Timer inputDelayTimer;
    private System.Timers.Timer fontSizeDelayTimer; // Debouncer for font size
    private Panel? loadingPanel;
    private HashSet<string> loadedFonts; // To prevent duplicate fonts
    private CancellationTokenSource loadingCancellationTokenSource;
    private List<FontFamily> cachedFonts; // Cache for all available fonts
    private bool isFirstTimeLoading = true;
    private Button hiddenButton;

    public TryFontsForm()
    {
      InitializeComponent();
      containsOrStartsWithComboBox.SelectedIndex = 0; // Set default to "Contains"
      InitializeLoadingPanel();
      loadedFonts = new HashSet<string>();
      cachedFonts = FontFamily.Families?.ToList() ?? new List<FontFamily>(); // Cache all available fonts initially
      loadingCancellationTokenSource = new CancellationTokenSource();
      LoadMoreFontsAsync(loadingCancellationTokenSource.Token);
      scrollablePanel.MouseWheel += ScrollablePanel_MouseWheel;
      scrollablePanel.Scroll += ScrollablePanel_Scroll;
      fontViewingContainer.ControlAdded += FontViewingContainer_ControlAdded;
      fontViewingContainer.Visible = false;

      // Initialize timer for input delay
      inputDelayTimer = new System.Timers.Timer(500); // Set delay to 500 milliseconds
      inputDelayTimer.Elapsed += OnInputDelayElapsed;
      inputDelayTimer.AutoReset = false;

      // Initialize timer for font size delay
      fontSizeDelayTimer = new System.Timers.Timer(500); // Set delay to 500 milliseconds
      fontSizeDelayTimer.Elapsed += OnFontSizeDelayElapsed;
      fontSizeDelayTimer.AutoReset = false;

      // Create a hidden button to take the focus at startup
      hiddenButton = new Button
      {
        Visible = false,
        TabStop = false
      };
      this.Controls.Add(hiddenButton); 
    }

    private void InitializeLoadingPanel()
    {
      loadingPanel = new Panel
      {
        Dock = DockStyle.Fill,
        BackColor = Color.White,
        BorderStyle = BorderStyle.None
      };
      Controls.Add(loadingPanel);
      loadingPanel.BringToFront();
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      // Set focus to a hidden button to prevent tryTextBox from being highlighted at startup
      hiddenButton.Focus();
      this.ActiveControl = null; // Ensure no control retains focus after the form is shown
    }
    
    private async void LoadMoreFontsAsync(CancellationToken cancellationToken)
    {
      if (cachedFonts == null || !cachedFonts.Any())
      {
        // No fonts available, wait until fonts are properly loaded.
        return;
      }
      string searchText = searchTextBox.Text.ToLower();
      try
      {
        // Ensure cachedFonts is not null before proceeding
        if (cachedFonts == null)
        {
          throw new InvalidOperationException("Cached fonts are not initialized.");
        }
        
        string? selectedOption = containsOrStartsWithComboBox.SelectedItem?.ToString();

        var filteredFonts = cachedFonts
            .Where(f => string.IsNullOrEmpty(searchText) ||
                (selectedOption == "Starts with" && f.Name.StartsWith(searchText, StringComparison.CurrentCultureIgnoreCase)) ||
                (selectedOption == "Contains" && f.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)))
            .Skip(loadedFontCount)
            .Take(LoadIncrement)
            .ToList();

        await Task.Run(() =>
        {
          foreach (var fontFamily in filteredFonts)
          {
            if (cancellationToken.IsCancellationRequested)
            {
              return;
            }

            if (loadedFonts.Contains(fontFamily.Name))
              continue;

            try
            {
              Font? font = null;
              try
              {
                font = new Font(fontFamily, (float)fontSizeBox.Value, GetSelectedFontStyle());
                if (IsHandleCreated && !IsDisposed)
                {
                  Invoke(new Action(() => AddFontToContainer(font, fontFamily.Name)));
                  loadedFonts.Add(fontFamily.Name);
                  loadedFontCount++;
                }
              }
              catch (ArgumentException)
              {
                // Skip fonts that can't be used with the selected style
              }
              finally
              {
                font?.Dispose(); // Dispose to free GDI resources
              }
            }
            catch (Exception ex)
            {
              // Log or handle other potential exceptions
              Console.WriteLine($"Error loading font {fontFamily.Name}: {ex.Message}");
            }
          }
        }, cancellationToken);

        if (IsHandleCreated && !IsDisposed)
        {
          Invoke(new Action(() =>
          {
            fontViewingContainer.PerformLayout(); // Force layout update after loading fonts
            fontViewingContainer.Refresh(); // Force a redraw of the container to fix alignment issues

            // Hide the loading panel once the initial fonts are loaded
            if (loadingPanel != null && loadingPanel.Visible)
            {
              loadingPanel.Hide();
            }

            // Force it to show the fonts it loaded
            if (isFirstTimeLoading == true)
            {
              // You can add any necessary logic here if required for first-time load handling
            }

            if (scrollablePanel != null && scrollablePanel.VerticalScroll.Maximum > 0)
            {
              scrollablePanel.VerticalScroll.Value = Math.Min(scrollablePanel.VerticalScroll.Value + 1, scrollablePanel.VerticalScroll.Maximum);
              scrollablePanel.PerformLayout();
              fontViewingContainer.Visible = true;
              isFirstTimeLoading = false;
            }
          }));
        }

      }
      catch (OperationCanceledException)
      {
        // Handle cancellation if needed
      }
      catch (Exception ex)
      {
        if (IsHandleCreated && !IsDisposed)
        {
          Invoke(new Action(() => MessageBox.Show("Error loading fonts: " + ex.Message)));
        }
      }
    }

    private void AddFontToContainer(Font font, string fontName)
    {
      string sanitizedText = tryTextBox.Text;

      var rowPanel = new TableLayoutPanel
      {
        ColumnCount = 2,
        AutoSize = true,
        Dock = DockStyle.Top,
        BackColor = Color.WhiteSmoke,
        Margin = new Padding(5),
        Width = fontViewingContainer.ClientSize.Width - 20
      };
      rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
      rowPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));

      var textLabel = new Label
      {
        Text = sanitizedText,
        Font = font,
        AutoSize = true,
        MaximumSize = new Size((int)(rowPanel.Width * 0.7), 0),
        Dock = DockStyle.Fill,
        TabStop = false,
        TextAlign = ContentAlignment.MiddleLeft
      };

      var fontLabelFont = new Font("Segoe UI", 12, FontStyle.Regular);
      var fontLabel = new Label
      {
        Text = fontName,
        Font = fontLabelFont,
        AutoSize = false,
        Width = (int)(rowPanel.Width * 0.3),
        Dock = DockStyle.Fill,
        TabStop = false,
        TextAlign = ContentAlignment.MiddleRight
      };

      rowPanel.Controls.Add(textLabel, 0, 0);
      rowPanel.Controls.Add(fontLabel, 1, 0);

      fontViewingContainer.Controls.Add(rowPanel);

      // Dispose fonts after usage to free resources
      textLabel.Disposed += (s, e) => font.Dispose();
      fontLabel.Disposed += (s, e) => fontLabelFont.Dispose();
    }

    private FontStyle GetSelectedFontStyle()
    {
      FontStyle style = FontStyle.Regular;
      if (boldCheckBox.Checked) style |= FontStyle.Bold;
      if (italicsCheckBox.Checked) style |= FontStyle.Italic;
      return style;
    }

    private void OnInputDelayElapsed(object? sender, ElapsedEventArgs e)
    {
      if (IsHandleCreated && !IsDisposed)
      {
        Invoke(new Action(RefreshFonts));
      }
    }

    private void OnFontSizeDelayElapsed(object? sender, ElapsedEventArgs e)
    {
      if (IsHandleCreated && !IsDisposed)
      {
        Invoke(new Action(RefreshFonts));
      }
    }

    private void LoadFonts()
    {
      loadingCancellationTokenSource.Cancel();
      loadingCancellationTokenSource = new CancellationTokenSource();
      LoadMoreFontsAsync(loadingCancellationTokenSource.Token);
    }

    private void RefreshFonts()
    {
      if (loadedFonts == null)
      {
        loadedFonts = new HashSet<string>(); // Initialize if null
      }

      if (loadingCancellationTokenSource == null)
      {
        loadingCancellationTokenSource = new CancellationTokenSource(); // Initialize if null
      }

      fontViewingContainer.Controls.Clear();
      loadedFontCount = 0;
      loadedFonts.Clear(); // Clear the set to prevent stale entries

      loadingCancellationTokenSource.Cancel();
      loadingCancellationTokenSource = new CancellationTokenSource();

      LoadMoreFontsAsync(loadingCancellationTokenSource.Token);
    }

    private void tryTextBox_TextChanged(object sender, EventArgs e)
    {
      inputDelayTimer.Stop();
      inputDelayTimer.Start();
    }

    private void fontSizeBox_ValueChanged(object sender, EventArgs e)
    {
      if (fontSizeBox.Value > fontSizeBox.Maximum)
      {
        fontSizeBox.Value = fontSizeBox.Maximum;
      }
      fontSizeDelayTimer.Stop();
      fontSizeDelayTimer.Start();
    }

    private void boldCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      RefreshFonts();
    }

    private void italicsCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      RefreshFonts();
    }

    private void searchTextBox_TextChanged(object sender, EventArgs e)
    {
      inputDelayTimer.Stop();
      inputDelayTimer.Start();
    }

    private void containsOrStartsWithComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      RefreshFonts();
    }

    private void ScrollablePanel_MouseWheel(object? sender, MouseEventArgs e)
    {
      if (e.Delta < 0) // User scrolled down
      {
        var scrollPosition = scrollablePanel.VerticalScroll.Value + scrollablePanel.ClientSize.Height;
        if (scrollPosition >= scrollablePanel.VerticalScroll.Maximum / 1.5) // Trigger load when two-thirds of the way down
        {
          LoadFonts();
        }
      }
    }

    private void ScrollablePanel_Scroll(object? sender, ScrollEventArgs e)
    {
      if (e.ScrollOrientation == ScrollOrientation.VerticalScroll && e.NewValue > e.OldValue) // User scrolled down
      {
        var scrollPosition = scrollablePanel.VerticalScroll.Value + scrollablePanel.ClientSize.Height;
        if (scrollPosition >= scrollablePanel.VerticalScroll.Maximum / 1.5) // Trigger load when two-thirds of the way down
        {
          LoadFonts();
        }
      }
    }

    private void FontViewingContainer_ControlAdded(object? sender, ControlEventArgs e)
    {
      if (IsHandleCreated && !IsDisposed)
      {
        fontViewingContainer.ScrollControlIntoView(e.Control);
      }
    }
  }
}
