using System.Reflection;
using Hangfire.Dashboard;

namespace Hangfire.JobKits.Dashboard.Controls.InputBox
{

    internal sealed class NumberControl : RazorPage
    {
        public ParameterInfo Parameter { get; }

        public NumberControl(ParameterInfo parameter)
        {
            Parameter = parameter;
        }

        public override void Execute()
        {
            WriteLiteral("<div class=\"input-group\">");
            WriteLiteral($"<div class=\"input-group-addon\">{Parameter.Name}</div>");
            WriteLiteral($"<input type=\"number\" name=\"{Parameter.Name}\" placeholder=\"{Parameter.Name}\" class=\"form-control\" />");
            WriteLiteral("</div>");
        }
    }
}
