using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using KomodoDesktop.Services;

namespace KomodoDesktop.Views;

public partial class SignUpWindow : Window
{
    public SignUpWindow()
    {
        InitializeComponent();
    }

    private void OnCreate(object? sender, RoutedEventArgs e)
    {
        var username = TxtUsername.Text?.Trim() ?? "";
        var password = TxtPassword.Text ?? "";
        var role     = (CmbRole.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "cashier";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Please fill in all fields.", error: true);
            return;
        }

        if (password.Length < 6)
        {
            ShowMessage("Password must be at least 6 characters.", error: true);
            return;
        }

        var (success, message) = DatabaseService.CreateUser(username, password, role);
        ShowMessage(message, !success);

        if (success)
        {
            TxtUsername.Text = "";
            TxtPassword.Text = "";
        }
    }

    private void OnCancel(object? sender, RoutedEventArgs e) => Close();

    private void ShowMessage(string message, bool error)
    {
        LblMessage.Foreground = error
            ? new SolidColorBrush(Color.Parse("#FF5252"))
            : new SolidColorBrush(Color.Parse("#80CBC4"));
        LblMessage.Text = message;
    }
}
