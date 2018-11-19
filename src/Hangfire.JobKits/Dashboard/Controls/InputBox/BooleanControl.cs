using System.Reflection;
using Hangfire.Dashboard;

namespace Hangfire.JobKits.Dashboard.Controls.InputBox
{
    internal sealed class BooleanControl : RazorPage
    {
        public ParameterInfo Parameter { get; }

        public BooleanControl(ParameterInfo parameter)
        {
            Parameter = parameter;
        }

        public override void Execute()
        {
            WriteLiteral("<div class=\"checkbox\">");
            WriteLiteral($"<input type=\"hidden\" value=\"false\" name=\"{Parameter.Name}\">");
            WriteLiteral($"<label><input type=\"checkbox\" name=\"{Parameter.Name}\" value=\"true\">{Parameter.Name}</input></label>");
            WriteLiteral("</div>");
        }
    }
}
