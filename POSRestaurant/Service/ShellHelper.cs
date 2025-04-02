using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Service
{
    public static class ShellHelper
    {
        public static void HideAllTabs(this Shell shell)
        {
            // This method configures tab visibility based on the user's permissions
            if (shell == null)
                return;

            var tabBar = shell.Items.FirstOrDefault() as TabBar;
            if (tabBar == null)
                return;

            foreach(var tab in tabBar.Items)
            {
                if (tab.Title.Contains("table", StringComparison.OrdinalIgnoreCase))
                {
                    tab.Title = "Login";
                }
                else
                {
                    tab.IsVisible = false;
                }
            }
        }

        public static void ConfigureTabVisibility(this Shell shell, IAuthService authService)
        {
            // This method configures tab visibility based on the user's permissions
            if (shell == null || authService == null)
                return;

            var tabBar = shell.Items.FirstOrDefault() as TabBar;
            if (tabBar == null)
                return;

            var tableTab = tabBar.Items.FirstOrDefault(t =>
                    t.Route != null && t.Route.Contains("TablePage", StringComparison.OrdinalIgnoreCase));

            if (tableTab != null)
            {
                tableTab.Title = "Tables";
                tableTab.IsVisible = true;
            }
                

            // Configure Reports tab visibility
            if (authService.HasPermission("ViewOrders"))
            {
                var ordersTab = tabBar.Items.FirstOrDefault(t =>
                    t.Route != null && t.Route.Contains("OrdersPage", StringComparison.OrdinalIgnoreCase));

                if (ordersTab != null)
                    ordersTab.IsVisible = true;
            }

            // Configure Reports tab visibility
            if (authService.HasPermission("EditOrders"))
            {
                var manageKOT = tabBar.Items.FirstOrDefault(t =>
                    t.Route != null && t.Route.Contains("ManageKOT", StringComparison.OrdinalIgnoreCase));

                if (manageKOT != null)
                    manageKOT.IsVisible = true;
            }

            // Configure Settings tab visibility
            if (authService.HasPermission("EditMenu"))
            {
                var manageMenuTab = tabBar.Items.FirstOrDefault(t =>
                    t.Route != null && t.Route.Contains("ManageMenuItemPage", StringComparison.OrdinalIgnoreCase));

                if (manageMenuTab != null)
                    manageMenuTab.IsVisible = true;
            }

            // Configure Settings tab visibility
            if (authService.HasPermission("EditStaff"))
            {
                var manageStaffTab = tabBar.Items.FirstOrDefault(t =>
                    t.Route != null && t.Route.Contains("StaffManagementPage", StringComparison.OrdinalIgnoreCase));

                if (manageStaffTab != null)
                    manageStaffTab.IsVisible = true;
            }

            // Configure Settings tab visibility
            if (authService.HasPermission("ViewReport"))
            {
                var report = tabBar.Items.FirstOrDefault(t =>
                    t.Route != null && t.Route.Contains("ItemReportPage", StringComparison.OrdinalIgnoreCase));

                if (report != null)
                    report.IsVisible = true;

                report = tabBar.Items.FirstOrDefault(t =>
                    t.Route != null && t.Route.Contains("SalesReportPage", StringComparison.OrdinalIgnoreCase));

                if (report != null)
                    report.IsVisible = true;

                report = tabBar.Items.FirstOrDefault(t =>
                    t.Route != null && t.Route.Contains("InventoryReport", StringComparison.OrdinalIgnoreCase));

                if (report != null)
                    report.IsVisible = true;
            }

            // Configure Settings tab visibility
            if (authService.HasPermission("AddInventory"))
            {
                var inventoryAddTab = tabBar.Items.FirstOrDefault(t =>
                    t.Route != null && t.Route.Contains("InventoryPage", StringComparison.OrdinalIgnoreCase));

                if (inventoryAddTab != null)
                    inventoryAddTab.IsVisible = true;
            }

            if (authService.HasPermission("EditInventory"))
            {
                var inventoryEditTab = tabBar.Items.FirstOrDefault(t =>
                    t.Route != null && t.Route.Contains("InventoryEdit", StringComparison.OrdinalIgnoreCase));

                if (inventoryEditTab != null)
                    inventoryEditTab.IsVisible = true;
            }

            // Configure Settings tab visibility
            if (authService.HasPermission("EditRoles"))
            {
                var roleTab = tabBar.Items.FirstOrDefault(t =>
                    t.Route != null && t.Route.Contains("RoleManagementPage", StringComparison.OrdinalIgnoreCase));

                if (roleTab != null)
                    roleTab.IsVisible = true;
            }

            // Configure Settings tab visibility
            if (authService.HasPermission("EditUser"))
            {
                var userTab = tabBar.Items.FirstOrDefault(t =>
                    t.Route != null && t.Route.Contains("UserManagementPage", StringComparison.OrdinalIgnoreCase));

                if (userTab != null)
                    userTab.IsVisible = true;
            }
        }
    }
}
