using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using MyFirstHttp.Framework.Attributes;

namespace MyFirstHttp.Framework.Handlers;

public class ControllerHandler : IRequestHandler
{
    private IRequestHandler _successor;
    public void SetSuccessor(IRequestHandler successor)
    {
        _successor = successor;
    }

    public async Task<bool> HandleAsync(HttpListenerContext context)
    {
        string path = context.Request.Url?.AbsolutePath.Trim('/') ?? "/";
        string httpMethod = context.Request.HttpMethod.ToUpper();
        var response = context.Response;

        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            var pathParts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            string controllerRoute = pathParts.Length > 0 ? pathParts[0] : "";
            string actionName = pathParts.Length > 1 ? pathParts[1] : "";
            
            Type matchedControllerType = assembly.GetTypes()
                .FirstOrDefault(t =>
                {
                    if (t.GetCustomAttributes<ControllerAttribute>() == null) return false;

                    var routeProperty = t.GetProperty("Route");
                    if (routeProperty is null) return false;

                    var instance = Activator.CreateInstance(t);
                    string routeValue = routeProperty.GetValue(instance)?.ToString()?.Trim('/');

                    if (string.IsNullOrEmpty(routeValue)) return string.IsNullOrEmpty(controllerRoute) || path == "/";

                    return path.Equals(controllerRoute, StringComparison.OrdinalIgnoreCase) || path.StartsWith(routeValue + "/", StringComparison.OrdinalIgnoreCase);
                });
            
            if (matchedControllerType is null)
            {
                if (_successor != null)
                {
                    return await _successor.HandleAsync(context);
                }
                
                response.StatusCode = 404;
                byte[] buffer = Encoding.UTF8.GetBytes("404 Controller Not Found");
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer);
                await response.OutputStream.FlushAsync();
                response.Close();
                
                return true;
            }

            MethodInfo matchedMethod = matchedControllerType.GetMethods()
                .FirstOrDefault(m =>
                {
                    bool hasCorrectHttpMethod =
                        (httpMethod == "GET" && m.GetCustomAttributes<HttpGetAttribute>().Any()) ||
                        (httpMethod == "POST" && m.GetCustomAttributes<HttpPostAttribute>().Any());
                    
                    if (!hasCorrectHttpMethod) return false;
                    
                    return m.Name.Equals(actionName, StringComparison.OrdinalIgnoreCase);
                });

            if (matchedMethod is null)
            {
                response.StatusCode = 405;
                byte[] buffer = Encoding.UTF8.GetBytes("405 Not Found");
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer);
                await response.OutputStream.FlushAsync();
                response.Close();
                return true;
            }
            
            string rawData = "";
            if (httpMethod == "POST" && context.Request.HasEntityBody)
            {
                using var reader = new StreamReader(context.Request.InputStream,  context.Request.ContentEncoding);
                rawData = await reader.ReadToEndAsync();
            }
            else if (httpMethod == "GET" && !string.IsNullOrEmpty(context.Request.Url?.Query))
            {
                rawData = context.Request.Url.Query.TrimStart('?');
            }

            var queryParams = HttpUtility.ParseQueryString(rawData);
            ParameterInfo[] parameters = matchedMethod.GetParameters();
            object[] args = new object[parameters.Length];
            
            for (int i = 0; i < parameters.Length; i++)
            {
                var param =  parameters[i];
                string value =  queryParams.Get(param.Name) ?? queryParams.Get(param.Name.ToLower());
                
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        args[i] = Convert.ChangeType(value, param.ParameterType);
                    }
                    catch (InvalidCastException)
                    {
                        response.StatusCode = 404;
                        byte[] buffer = Encoding.UTF8.GetBytes("404 Not Found");
                        response.ContentLength64 = buffer.Length;
                        await response.OutputStream.WriteAsync(buffer);
                        await response.OutputStream.FlushAsync();
                        response.Close();
                        return true;
                    }
                }
                else if (param.HasDefaultValue)
                {
                    args[i] = param.DefaultValue;
                }
                else
                {
                    args[i] = null;
                }
            }

            object controllerInstance = Activator.CreateInstance(matchedControllerType);
            object result = matchedMethod.Invoke(controllerInstance, args);
            
            response.StatusCode = 200;
            response.ContentType = "text/plain; charset=utf-8";
            byte[] responseBytes = Encoding.UTF8.GetBytes(result?.ToString() ?? "OK");
            response.ContentLength64 = responseBytes.Length;
            await response.OutputStream.WriteAsync(responseBytes);
            await response.OutputStream.FlushAsync();
            response.Close();
            
            return true;
        }
        
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR IN ControllerHanlers.cs] {ex}");
            Console.WriteLine(ex.StackTrace);
            
            response.StatusCode = 500;
            response.ContentType = "text/plain; charset=utf-8";
            byte[] buffer = Encoding.UTF8.GetBytes("500 Internal Server Error");
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer);
            await response.OutputStream.FlushAsync();
            response.Close();
            return true;
        }
    }
}