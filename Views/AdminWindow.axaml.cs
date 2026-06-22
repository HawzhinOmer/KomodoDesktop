using Avalonia.Controls;
using Avalonia.Interactivity;
using KomodoDesktop.Models;
using KomodoDesktop.Services;

namespace KomodoDesktop.Views;

public partial class AdminWindow : Window
{
    private readonly User _user;

    public AdminWindow(User user)
    {
        InitializeComponent();
        _user = user;
        Title = $"Komodo — Admin ({user.Username})";
    }

    // ── Helpers ───────────────────────────────────────────────────
    private string FoodName => TxtFoodName.Text?.Trim() ?? "";
    private string PriceText => TxtPrice.Text?.Trim() ?? "";

    private string? GetCategory(object? sender)
        => (sender as Button)?.Tag?.ToString();

    private bool TryGetPrice(out decimal price)
    {
        if (decimal.TryParse(PriceText, out price) && price >= 0)
            return true;
        SetStatus("Please enter a valid price.", error: true);
        return false;
    }

    private void SetStatus(string message, bool error = false)
    {
        LblStatus.Foreground = error
            ? new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#FF5252"))
            : new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#80CBC4"));
        LblStatus.Text = message;
    }

    private void RefreshGrid(string category)
    {
        FoodGrid.ItemsSource = DatabaseService.GetFoodByCategory(category);
    }

    // ── Insert ────────────────────────────────────────────────────
    private void OnInsert(object? sender, RoutedEventArgs e)
    {
        var category = GetCategory(sender);
        if (category is null) return;

        if (string.IsNullOrEmpty(FoodName))
        {
            SetStatus("Food name is required.", error: true);
            return;
        }
        if (!TryGetPrice(out var price)) return;

        var (success, message) = DatabaseService.InsertFood(FoodName, price, category);
        SetStatus(message, !success);
        if (success) RefreshGrid(category);
    }

    // ── Update ────────────────────────────────────────────────────
    private void OnUpdate(object? sender, RoutedEventArgs e)
    {
        var category = GetCategory(sender);
        if (category is null) return;

        if (string.IsNullOrEmpty(FoodName))
        {
            SetStatus("Enter the food name to update.", error: true);
            return;
        }
        if (!TryGetPrice(out var price)) return;

        var (success, message) = DatabaseService.UpdateFood(FoodName, price, category);
        SetStatus(message, !success);
        if (success) RefreshGrid(category);
    }

    // ── Delete ────────────────────────────────────────────────────
    private void OnDelete(object? sender, RoutedEventArgs e)
    {
        var category = GetCategory(sender);
        if (category is null) return;

        if (string.IsNullOrEmpty(FoodName))
        {
            SetStatus("Enter the food name to delete.", error: true);
            return;
        }

        var (success, message) = DatabaseService.DeleteFood(FoodName, category);
        SetStatus(message, !success);
        if (success) RefreshGrid(category);
    }

    // ── Search ────────────────────────────────────────────────────
    private void OnSearch(object? sender, RoutedEventArgs e)
    {
        var category = GetCategory(sender);
        if (category is null) return;

        FoodGrid.ItemsSource = DatabaseService.SearchFood(FoodName, category);
        SetStatus($"Search results for category: {category}");
    }

    // ── View all by category ──────────────────────────────────────
    private void OnViewCategory(object? sender, RoutedEventArgs e)
    {
        var category = GetCategory(sender);
        if (category is null) return;

        RefreshGrid(category);
        SetStatus($"Showing all items in: {category}");
    }

    // ── Logout ────────────────────────────────────────────────────
    private void OnLogout(object? sender, RoutedEventArgs e)
    {
        new LoginWindow().Show();
        Close();
    }
private void OnManageUsers(object? sender, RoutedEventArgs e)
{
    var win = new SignUpWindow();
    win.ShowDialog(this);
}
}
