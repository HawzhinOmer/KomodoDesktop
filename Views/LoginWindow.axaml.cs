using Avalonia.Controls;
using Avalonia.Interactivity;
using KomodoDesktop.Services;

namespace KomodoDesktop.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void OnLoginAdmin(object? sender, RoutedEventArgs e)
        => Authenticate("admin");

    private void OnLoginCashier(object? sender, RoutedEventArgs e)
        => Authenticate("cashier");

    private void Authenticate(string role)
    {
        var username = TxtUsername.Text?.Trim() ?? "";
        var password = TxtPassword.Text ?? "";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            LblError.Text = "Please enter your username and password.";
            return;
        }

        var user = DatabaseService.ValidateUser(username, password, role);
        if (user is null)
        {
            LblError.Text = "Invalid username, password, or role.";
            return;
        }

        Window next = role == "admin"
            ? new AdminWindow(user)
            : new CashierWindow(user);

        next.Show();
        Close();
    }

private void OnClose(object? sender, RoutedEventArgs e)
{
    Close();
}
    private void OnClear(object? sender, RoutedEventArgs e)
    {
        TxtUsername.Text = "";
        TxtPassword.Text = "";
        LblError.Text    = "";
    }
}
