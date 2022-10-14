using demo;

using OpenTK.Windowing.Desktop;


var gameWindow =
    new DemoWindow(GameWindowSettings.Default, NativeWindowSettings.Default);
gameWindow.Run();