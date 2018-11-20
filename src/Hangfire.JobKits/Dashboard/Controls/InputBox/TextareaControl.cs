using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            var obj = GetGenericInstance(Parameter.ParameterType);
            
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });

            WriteLiteral("<div class=\"input-group\">");
            WriteLiteral($"<div class=\"input-group-addon\">{Parameter.Name}</div>");
            WriteLiteral($"<textarea name=\"{Parameter.Name}\" class=\"form-control json-input\">{json}</textarea>");
            WriteLiteral("</div>");
        }

        internal object GetGenericInstance(Type sourceType)
        {
            var obj = Activator.CreateInstance(Parameter.ParameterType);

            if (IsList(obj))
            {
                Type genericType = obj.GetType().GetGenericArguments()[0];

                ((IList)obj).Add(Activator.CreateInstance(genericType));
            }
            else if (IsDictionary(obj))
            {
                Type[] dictType = obj.GetType().GetGenericArguments();

                obj = new[] {
                    new
                    {
                        Key = Activator.CreateInstance(dictType[0]),
                        Value = Activator.CreateInstance(dictType[1])
                    }
                }.ToDictionary(x => x.Key, x => x.Value);
            }

            return obj;
        }

        private bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        private bool IsDictionary(object o)
        {
            if (o == null) return false;
            return o is IDictionary &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }
    }
}