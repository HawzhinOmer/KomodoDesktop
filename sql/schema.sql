-- ─────────────────────────────────────────────────────────────
--  Komodo Desktop — Database Schema
--  Run: mysql -u root -p < sql/schema.sql
-- ─────────────────────────────────────────────────────────────

CREATE DATABASE IF NOT EXISTS komodo_desktop
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE komodo_desktop;

-- ── Users ──────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS users (
  id         INT UNSIGNED    NOT NULL AUTO_INCREMENT,
  username   VARCHAR(50)     NOT NULL,
  password   VARCHAR(64)     NOT NULL,   -- SHA-256 hex hash
  role       ENUM('admin','cashier') NOT NULL DEFAULT 'cashier',
  created_at TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,

  PRIMARY KEY (id),
  UNIQUE KEY uk_username (username)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ── Food Menu ──────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS food (
  id         INT UNSIGNED    NOT NULL AUTO_INCREMENT,
  foodname   VARCHAR(100)    NOT NULL,
  price      DECIMAL(10,2)   NOT NULL DEFAULT 0.00,
  category   ENUM('fastfood','eastfood','westfood','drink') NOT NULL,
  created_at TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,

  PRIMARY KEY (id),
  UNIQUE KEY uk_food_category (foodname, category)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ─────────────────────────────────────────────────────────────
--  After running this, create your first account via the
--  Sign Up button in the application (choose role: admin).
-- ─────────────────────────────────────────────────────────────
