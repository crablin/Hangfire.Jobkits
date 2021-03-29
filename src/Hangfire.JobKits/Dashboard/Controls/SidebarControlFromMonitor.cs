using Hangfire.Dashboard;
using Hangfire.JobKits.Worker;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hangfire.JobKits.Dashboard.Controls
{
    internal sealed class SidebarControlFromMonitor : RazorPage
    {
        public string SelectedCategory { get; }
        public MonitorMap Map { get; }

        public SidebarControlFromMonitor(string selectedCategory, MonitorMap map)
        {
            SelectedCategory = selectedCategory;
            Map = map;
        }

        public override void Execute()
        {
            WriteLiteral("<div id=\"categories\" class=\"list-group\">");

            foreach (var category in Map.JobCategories)
            {
                WriteLiteral("<a class=\"list-group-item");
                if (category.Key == SelectedCategory)
                {
                    WriteLiteral(" active");
                }
                WriteLiteral("\"href=\"");

                WriteLiteral(Url.To($"{JobKitRoute.Monitor.Url}/{category.Key}"));
                WriteLiteral("\">");
                WriteLiteral(category.Key);
                WriteLiteral("<span class=\"pull-right\">");
                WriteLiteral($"<span class=\"metric\">{category.Value}</span>");
                WriteLiteral("</span>");
                WriteLiteral("</a>");
            }

            WriteLiteral("</div>");
        }

        public static NonEscapedString Render(HtmlHelper helper, string selectedCategory, MonitorMap map)
            => helper.RenderPartial(new SidebarControlFromMonitor(selectedCategory, map));
    }
}
