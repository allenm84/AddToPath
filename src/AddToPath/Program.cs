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
      AppLogger.WriteLine("Received: {0}", string.Join(", ", args));

      string path = "";
      try
      {
        path = args[0];
        var attr = File.GetAttributes(path);
        if (!attr.HasFlag(FileAttributes.Directory))
        {
          AppLogger.WriteLine("The path provided is not a directory");
          Environment.Exit(267);
          return;
        }
      }
      catch (Exception ex)
      {
        AppLogger.WriteLine("Error retrieving attributes: {0}", ex.Message);
        Environment.Exit(1);
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
        AppLogger.WriteLine("The directory is already in the PATH variable");
      }
      else
      {
        paths.Add(path);
        try
        {
          Environment.SetEnvironmentVariable(Name, string.Join(";", paths), EnvironmentVariableTarget.Machine);
          AppLogger.WriteLine("The directory was added to the PATH variable");
        }
        catch (Exception ex)
        {
          AppLogger.WriteLine("Error setting variable name: ", ex.Message);
          Environment.Exit(1);
          return;
        }
      }

      Environment.Exit(0);
      return;
    }
  }
}
