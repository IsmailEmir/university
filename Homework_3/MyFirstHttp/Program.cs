using MyFirstHttp.Framework.Controllers;
using MyFirstHttp.Framework.Handlers;
using MyFirstHttp.Framework.Http;
using MyFirstHttp.Framework.Registry;

var registry = new ControllerRegistry();
registry.RegisterFactory(typeof(Auth), () => new Auth());
registry.Build();

var notFoundHandler = new NotFoundHandler();
var controllerHandler = new ControllerHandler(registry, notFoundHandler);
var fileHandler = new StaticFileHandler(controllerHandler);

var server = new HttpServer(fileHandler);

server.Run();

SteamController.SendMessage("ismailemirkoc07@gmail.com", "yOoqOVMCcY");