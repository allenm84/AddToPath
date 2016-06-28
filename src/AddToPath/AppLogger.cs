using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AddToPath
{
  public static class AppLogger
  { 
    static readonly AppLoggerCompleting sCompleting = new AppLoggerCompleting();
    static readonly ManualResetEventSlim sStopped = new ManualResetEventSlim(false);

    static readonly ConcurrentQueue<string> sQueue = new ConcurrentQueue<string>();
    static readonly string sAppLogFilepath;
    static readonly Thread sFlushQueueThread;

    static AppLogger()
    {
      var app = Process.GetCurrentProcess().ProcessName;
      var dir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
      var logs = Path.Combine(dir, ".applogs", app);
      if (!Directory.Exists(logs))
      {
        Directory.CreateDirectory(logs);
      }

      sFlushQueueThread = new Thread(writeProcWork);
      sFlushQueueThread.Name = "Flush Queue";
      sFlushQueueThread.IsBackground = true;
      sFlushQueueThread.Start();

      sAppLogFilepath = Path.Combine(logs, string.Format("{0:yyyyMMdd}.log", DateTime.Today));
      WriteLine("================================================");
      WriteLine("AppLogger starting: {0}", DateTime.Now);
    }

    static void writeProcWork()
    {
      while (!sStopped.Wait(32))
      {
        Flush();
      }
    }

    static void writeProcCompleted()
    {
      sStopped.Set();
      WriteLine("AppLogger copleted!");
      Flush();
    }

    static void Flush()
    {
      File.AppendAllLines(sAppLogFilepath, GetLines());
    }

    static IEnumerable<string> GetLines()
    {
      string data;
      while (sQueue.TryDequeue(out data))
      {
        yield return data;
      }
    }

    public static void WriteLine(string line)
    {
      sQueue.Enqueue(string.Format("[{0}]: {1}",
        new TimeSpan(Stopwatch.GetTimestamp()),
        line));
    }

    public static void WriteLine(string format, params object[] args)
    {
      WriteLine(string.Format(format, args));
    }

    private class AppLoggerCompleting
    {
      ~AppLoggerCompleting() { writeProcCompleted(); }
    }
  }
}
