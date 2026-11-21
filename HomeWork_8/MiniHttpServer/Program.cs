using DotNetEnv;
using MiniHttpServer.Framework.Controllers;
using MiniHttpServer.Framework.Handlers;
using MiniHttpServer.Framework.Http;
using MiniHttpServer.Framework.Registry;

Env.Load();

var registry = new ControllerRegistry();
registry.RegisterFactory(typeof(Auth), () => new Auth());
registry.RegisterFactory(typeof(TestController), () => new TestController());
registry.RegisterFactory(typeof(UserController), () => new UserController());
registry.Build();

var notFoundHandler = new NotFoundHandler();
var controllerHandler = new ControllerHandler(registry, notFoundHandler);
var fileHandler = new StaticFileHandler(controllerHandler);

var server = new HttpServer(fileHandler);

server.Run();
