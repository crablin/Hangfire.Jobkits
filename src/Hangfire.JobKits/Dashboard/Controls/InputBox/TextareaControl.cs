using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using Hangfire.Dashboard;
using Newtonsoft.Json;

namespace Hangfire.JobKits.Dashboard.Controls.InputBox
{

    internal sealed class TextareaControl : RazorPage
    {
        public ParameterInfo Parameter { get; }

        public TextareaControl(ParameterInfo parameter)
        {
            Parameter = parameter;
        }

        public override void Execute()
        {
            var param = Parameter.GetCustomAttribute<JobParamAttribute>();

            var obj = Activator.CreateInstance(Parameter.ParameterType);

            if (IsList(obj))
            {
                Type genericType = obj.GetType().GetGenericArguments()[0];

                ((IList)obj).Add(Activator.CreateInstance(genericType));
            }
            
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });

            WriteLiteral("<div class=\"input-group\">");
            WriteLiteral($"<div class=\"input-group-addon\">{Parameter.Name}</div>");
            WriteLiteral($"<textarea name=\"{Parameter.Name}\" class=\"form-control json-input\">{json}</textarea>");
            WriteLiteral("</div>");
        }

        public bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public bool IsDictionary(object o)
        {
            if (o == null) return false;
            return o is IDictionary &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }

    }
}
