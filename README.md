# Water Tanker Manager 🚛💧

A modern, robust web application designed to streamline operations for Water Tanker businesses. Built with **ASP.NET Core MVC** and **Entity Framework Core**, this system manages daily rounds, tracks expenses, and provides detailed financial insights through an interactive dashboard.

## 🚀 Key Features

### 📊 Interactive Dashboard
- **Real-time KPIs**: Track Total Rounds, Revenue, Expenses, and Net Profit/Loss at a glance.
- **Dynamic Visuals**: 
  - **Net Profit/Loss Card**: Automatically changes color (Green/Red) and title based on financial status.
  - **Smart Pie Chart**: visualizes distribution. In a **Loss Scenario**, it intelligently shifts to show "Revenue vs Net Loss" (with a 100% Expense view for critical losses).
- **Pivot Tables**: Analyze monthly expense trends and daily round frequencies.

### 🔐 Role-Based Security
- **Admin Role**: Full control. Manage users, add/edit/delete records, view hidden/archived rounds, and manage business settings.
- **User Role**: Read-only access to the dashboard and lists. ideal for staff viewing.
- **Hidden Mode**: Admins can "Hide" specific rounds (soft delete) to keep the active view clean, with a toggle to view archives.

### 💼 Business Operations
- **Rounds Management**: Log daily tanker rounds with date, count, and amount.
- **Expense Tracking**: Categorize and record daily operational costs.
- **Monthly Reports**: Generate comprehensive monthly breakdowns with Grand Totals for easy accounting.

### 🎨 Modern UI/UX
- **Responsive Design**: Fully functional on Desktop and Mobile.
- **Polished Controls**: "Pill-style" buttons, shadow effects, and intuitive navigation.
- **SweetAlert2 Integration**: Beautiful, user-friendly popups for confirmations and alerts replaces standard browser dialogs.

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQLite (Entity Framework Core)
- **Frontend**: Bootstrap 5, Vanilla CSS, FontAwesome 6
- **Visuals**: Chart.js (Data Visualization), SweetAlert2 (Popups)
- **Auth**: ASP.NET Core Identity

## 📦 Getting Started

1.  **Clone the repository**:
    ```bash
    git clone https://github.com/YOUR_USERNAME/WaterTankerManager.git
    ```
2.  **Navigate to project directory**:
    ```bash
    cd WaterTankerManager
    ```
3.  **Run the application**:
    ```bash
    dotnet run
    ```
4.  **Login**:
    - **Default Admin**: `admin@water.com`
    - **Password**: `Admin123!`

## 📸 Screenshots

*(Add screenshots of your Dashboard, Rounds Page, and Login Screen here)*
