using System.Security.Cryptography;
using System.Text;
using KomodoDesktop.Models;
using MySqlConnector;

namespace KomodoDesktop.Services;

public static class DatabaseService
{
    private const string ConnectionString =
        "Server=localhost;Database=komodo_desktop;User=komodo;Password=komodo123;";

    // ── Password Hashing ──────────────────────────────────────────
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLower();
    }

    // ── User Methods ──────────────────────────────────────────────
    public static User? ValidateUser(string username, string password, string role)
    {
        try
        {
            var hash = HashPassword(password);
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "SELECT id, username, role FROM users " +
                "WHERE username=@u AND password=@p AND role=@r LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", hash);
            cmd.Parameters.AddWithValue("@r", role);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return new User
                {
                    Id       = reader.GetInt32("id"),
                    Username = reader.GetString("username"),
                    Role     = reader.GetString("role")
                };
        }
        catch { /* connection or query failure */ }
        return null;
    }

    public static bool UsernameExists(string username)
    {
        using var conn = new MySqlConnection(ConnectionString);
        conn.Open();
        using var cmd = new MySqlCommand(
            "SELECT COUNT(*) FROM users WHERE username=@u", conn);
        cmd.Parameters.AddWithValue("@u", username);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public static (bool Success, string Message) CreateUser(
        string username, string password, string role)
    {
        try
        {
            if (UsernameExists(username))
                return (false, "Username already exists.");

            var hash = HashPassword(password);
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "INSERT INTO users (username, password, role) VALUES (@u, @p, @r)", conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", hash);
            cmd.Parameters.AddWithValue("@r", role);
            cmd.ExecuteNonQuery();
            return (true, "Account created successfully.");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    // ── Food Methods ──────────────────────────────────────────────
    public static List<FoodItem> GetFoodByCategory(string category)
    {
        var items = new List<FoodItem>();
        try
        {
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "SELECT * FROM food WHERE category=@c ORDER BY foodname", conn);
            cmd.Parameters.AddWithValue("@c", category);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                items.Add(ReadFoodItem(reader));
        }
        catch { /* return empty list on failure */ }
        return items;
    }

    public static List<FoodItem> SearchFood(string name, string category)
    {
        var items = new List<FoodItem>();
        try
        {
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            var sql = string.IsNullOrWhiteSpace(name)
                ? "SELECT * FROM food WHERE category=@c ORDER BY foodname"
                : "SELECT * FROM food WHERE foodname LIKE @n AND category=@c ORDER BY foodname";
            using var cmd = new MySqlCommand(sql, conn);
            if (!string.IsNullOrWhiteSpace(name))
                cmd.Parameters.AddWithValue("@n", $"%{name}%");
            cmd.Parameters.AddWithValue("@c", category);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                items.Add(ReadFoodItem(reader));
        }
        catch { /* return empty list */ }
        return items;
    }

    public static (bool Success, string Message) InsertFood(
        string name, decimal price, string category)
    {
        try
        {
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var check = new MySqlCommand(
                "SELECT COUNT(*) FROM food WHERE foodname=@n AND category=@c", conn);
            check.Parameters.AddWithValue("@n", name);
            check.Parameters.AddWithValue("@c", category);
            if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                return (false, $"\"{name}\" already exists in this category.");

            using var cmd = new MySqlCommand(
                "INSERT INTO food (foodname, price, category) VALUES (@n, @p, @c)", conn);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@p", price);
            cmd.Parameters.AddWithValue("@c", category);
            cmd.ExecuteNonQuery();
            return (true, $"\"{name}\" inserted successfully.");
        }
        catch (Exception ex) { return (false, ex.Message); }
    }

    public static (bool Success, string Message) UpdateFood(
        string name, decimal price, string category)
    {
        try
        {
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "UPDATE food SET price=@p WHERE foodname=@n AND category=@c", conn);
            cmd.Parameters.AddWithValue("@p", price);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@c", category);
            int rows = cmd.ExecuteNonQuery();
            return rows > 0
                ? (true,  $"\"{name}\" updated successfully.")
                : (false, $"\"{name}\" not found in this category.");
        }
        catch (Exception ex) { return (false, ex.Message); }
    }

    public static (bool Success, string Message) DeleteFood(
        string name, string category)
    {
        try
        {
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "DELETE FROM food WHERE foodname=@n AND category=@c", conn);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@c", category);
            int rows = cmd.ExecuteNonQuery();
            return rows > 0
                ? (true,  $"\"{name}\" deleted.")
                : (false, $"\"{name}\" not found in this category.");
        }
        catch (Exception ex) { return (false, ex.Message); }
    }

    // ── Helper ────────────────────────────────────────────────────
    private static FoodItem ReadFoodItem(MySqlDataReader reader) => new()
    {
        Id       = reader.GetInt32("id"),
        FoodName = reader.GetString("foodname"),
        Price    = reader.GetDecimal("price"),
        Category = reader.GetString("category")
    };
}
