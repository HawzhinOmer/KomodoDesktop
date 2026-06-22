using Avalonia.Controls;
using Avalonia.Interactivity;
using KomodoDesktop.Models;
using KomodoDesktop.Services;

namespace KomodoDesktop.Views;

public partial class CashierWindow : Window
{
    public CashierWindow(User user)
    {
        InitializeComponent();
        Title = $"Komodo — Cashier ({user.Username})";
    }

    private void OnCategory(object? sender, RoutedEventArgs e)
    {
        var category = (sender as Button)?.Tag?.ToString();
        if (category is null) return;

        var items = DatabaseService.GetFoodByCategory(category);
        FoodGrid.ItemsSource = items;

        var total = items.Sum(f => f.Price);
        TxtTotal.Text = total.ToString("F2");
        LblStatus.Text = $"{items.Count} item(s) in {category}";
    }

    private void OnLogout(object? sender, RoutedEventArgs e)
    {
        new LoginWindow().Show();
        Close();
    }
}
