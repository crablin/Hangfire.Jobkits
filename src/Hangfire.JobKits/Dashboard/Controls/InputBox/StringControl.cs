using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Hangfire.Dashboard;

namespace Hangfire.JobKits.Dashboard.Controls.InputBox
{
    internal sealed class StringControl : RazorPage
    {
        public ParameterInfo Parameter { get; }

        public StringControl(ParameterInfo parameter)
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
            WriteLiteral($"<input type=\"text\" name=\"{Parameter.Name}\" placeholder=\"{description}\" class=\"form-control\" value=\"{defaultValue}\" />");
            WriteLiteral("</div>");
        }
    }
}
