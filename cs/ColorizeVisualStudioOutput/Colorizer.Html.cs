namespace Tasler.Utility.ColorizeVisualStudioOutput;

internal static partial class Colorizer
{
private const string c_htmlHeader = """
<!DOCTYPE html>

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head>
  <meta charset="utf-8" />
  <title>Output Text from Visual Studio </title>
</head>
<body>
  <style>
    body
    {
      background-color: #1F1F1F;
    }
    pre
    {
      color: dimgray;
      font-family: Consolas, 'Cascadia Mono', 'Lucida Console', 'Lucida Sans Typewriter', Courier New, Courier, monospace;
      font-size: 12pt;
    }
    .module
    {
      color: cadetblue;
    }
    .symbol
    {
      color: rebeccapurple;
    }
    .threadExit
    {
      color: darkolivegreen;
    }
    .processExit
    {
      color: darkkhaki;
    }
    .info
    {
      color: cornflowerblue;
      /*font-weight: bold;*/
    }
    .warning
    {
      color: olive;
      /*font-weight: bold;*/
    }
    .error
    {
      color: #F05050;
      /*font-weight: bold;*/
    }
  </style>
  <pre>
""";

private const string c_htmlTrailer = """
  </pre>
</body>
</html>
""";
}

