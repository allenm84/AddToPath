using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddToPath
{
  class Program
  {
    const string Name = "PATH";

    static void Main(string[] args)
    {
      if (args.Length != 1)
      {
        Console.Error.WriteLine("Expected a directory path");
        Environment.Exit(160);
        return;
      }

      string path = args[0];
      try
      {
        var attr = File.GetAttributes(path);
        if (!attr.HasFlag(FileAttributes.Directory))
        {
          Console.Error.WriteLine("The path provided is not a directory");
          Environment.Exit(267);
          return;
        }
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine(ex.Message);
        Environment.Exit(267);
        return;
      }
      
      var paths = Environment
        .GetEnvironmentVariable(Name, EnvironmentVariableTarget.Machine)
        .Split(';')
        .ToList();

      var exists = paths
        .Select(p => Path.GetFullPath(p))
        .Any(p => string.Equals(p, path, StringComparison.OrdinalIgnoreCase));

      if (exists)
      {
        Console.Out.WriteLine("The directory is already in the PATH variable");
      }
      else
      {
        paths.Add(path);
        try
        {
          Environment.SetEnvironmentVariable(Name, string.Join(";", paths), EnvironmentVariableTarget.Machine);
          Console.Out.WriteLine("The directory added to the PATH variable");
        }
        catch (Exception ex)
        {
          Console.Error.WriteLine(ex.Message);
          Environment.Exit(1);
          return;
        }
      }

      Environment.Exit(0);
      return;
    }
  }
}
