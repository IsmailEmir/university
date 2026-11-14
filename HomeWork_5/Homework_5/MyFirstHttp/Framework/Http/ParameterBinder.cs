using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Web;

namespace MyFirstHttp.Framework.Http;

public static class ParameterBinder
{
    public static async Task<object?[]> BindAsync(HttpListenerRequest request, ParameterInfo[] parameters)
    {
        var values = HttpUtility.ParseQueryString(request.Url?.Query ?? string.Empty);

        if (request.HasEntityBody)
        {
            using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
            string rawData = await reader.ReadToEndAsync();
            var bodyValues = HttpUtility.ParseQueryString(rawData);

            foreach (string? key in bodyValues.AllKeys)
            {
                if (key is not null)
                {
                    values.Set(key, bodyValues.Get(key));
                }
            }
        }

        object?[] args = new object?[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            string? value = values.Get(parameter.Name ?? string.Empty) ?? values.Get(parameter.Name?.ToLower() ?? string.Empty);

            if (!string.IsNullOrEmpty(value))
            {
                args[i] = Convert.ChangeType(value, parameter.ParameterType);
            }
            else if (parameter.HasDefaultValue)
            {
                args[i] = parameter.DefaultValue;
            }
            else
            {
                args[i] = null;
            }
        }

        return args;
    }
}
