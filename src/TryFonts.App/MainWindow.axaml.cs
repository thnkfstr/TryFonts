using Avalonia.Controls;
using Avalonia.Input;

namespace TryFonts.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        SetupKeyboardShortcuts();
    }

    private void SetupKeyboardShortcuts()
    {
        // / or Ctrl+F → focus search box
        KeyDown += (_, e) =>
        {
            if (e.Key == Key.Slash && e.KeyModifiers == KeyModifiers.None)
            {
                FocusControl("SearchBox");
                e.Handled = true;
            }
            else if (e.Key == Key.F && e.KeyModifiers == KeyModifiers.Control)
            {
                FocusControl("SearchBox");
                e.Handled = true;
            }
            else if (e.Key == Key.L && e.KeyModifiers == KeyModifiers.Control)
            {
                FocusControl("PreviewTextBox");
                e.Handled = true;
            }
        };
    }

    private void FocusControl(string name)
    {
        if (this.FindControl<Control>(name) is { } control)
        {
            control.Focus();
            // Select all text in TextBoxes so the user can type immediately
            if (control is TextBox tb)
                tb.SelectAll();
        }
    }
}
