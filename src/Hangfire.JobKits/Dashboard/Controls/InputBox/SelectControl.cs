using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Hangfire.Dashboard;

namespace Hangfire.JobKits.Dashboard.Controls.InputBox
{

    internal sealed class SelectControl : RazorPage
    {
        public ParameterInfo Parameter { get; }

        public SelectControl(ParameterInfo parameter)
        {
            Parameter = parameter;
        }

        public override void Execute()
        {
            var param = Parameter.GetCustomAttribute<JobParamAttribute>();

            var description = param?.Description ?? Parameter.Name;
            var defaultValue = param?.DefaultValue?.ToString() ?? string.Empty;

            WriteLiteral("<div class=\"input-group\">");
            WriteLiteral($"<div class=\"input-group-addon\">{Parameter.Name}</div>");
            WriteLiteral($"<select class=\"form-control\" name=\"{Parameter.Name}\" value=\"{defaultValue}\">");

            foreach (var item in Enum.GetValues(Parameter.ParameterType))
            {
                var attr = item.ToString() == defaultValue ? " selected" : "";
                WriteLiteral($"<option{attr}>{item}</option>");
            }

            WriteLiteral("</select>");
            WriteLiteral("</div>");
        }
    }
}
