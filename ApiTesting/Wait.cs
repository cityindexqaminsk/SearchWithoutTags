using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace ApiTesting
{
    public class Wait
    {
        public static void UntilTrue(Func<bool> p, string err = "exceeded waiting time", int timeout = 15000, int interval = 1000)
        {
            var timer = new Stopwatch();
            timer.Start();
            while (true)
            {
                if (p()) return;
                if (timer.ElapsedMilliseconds > timeout) throw new TimeoutException("Timeout Error: " + err);
                Sleep(interval);
            }
        }

        public static void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        public static T UntilNoException<T>(Func<T> f, int timeout = 15000, int interval = 1000)
        {
            var timer = new Stopwatch();
            timer.Start();
            while (true)
            {
                try
                {
                    return f();
                }
                catch (Exception ex)
                {
                    if (timer.ElapsedMilliseconds > timeout) throw (ex);
                    Sleep(interval);
                }
            }
        }
        public static void UntilNoException(Action f, int timeout = 15000, int interval = 1000)
        {
            var timer = new Stopwatch();
            timer.Start();
            while (true)
            {
                try
                {
                    f();
                    return;
                }
                catch (Exception ex)
                {
                    if (timer.ElapsedMilliseconds > timeout) throw (ex);
                    Sleep(interval);
                }
            }
        }

        public static void UntilNoExceptionAsync(Action f, int timeout = 15000, int interval = 1000)
        {
            var timer = new Stopwatch();
            timer.Start();
            var result = false;
            var mainThread = new Thread(() =>
            {
                while (true)
                {
                    if (result)
                    {
                        return;
                    }
                    if (timer.ElapsedMilliseconds > timeout)
                    {
                        throw new Exception("Timeout exception");
                    }
                }
            });
            mainThread.Start();

            while (true)
            {
                try
                {
                    f();
                    result = true;
                    return;
                }
                catch (Exception ex)
                {
                    Sleep(interval);
                }
            }
        }

        public static void UntilNumberOfExceptions(Action f, int times = 3, int interval = 100)
        {
            for (var i = 0; i < times; i++)
            {
                try
                {
                    f();
                    return;
                }
                catch (Exception ex)
                {
                    if (i == times - 1) throw ex;
                    Sleep(interval);
                }
            }
        }

        public static T UntilNumberOfExceptions<T>(Func<T> f, int times = 3, int interval = 100)
        {
            for (var i = 0; i < times; i++)
            {
                try
                {
                    return f();
                }
                catch (Exception ex)
                {
                    if (i == times - 1) throw ex;
                    Sleep(interval);
                }
            }
            throw new Exception("UntilNumberOfExceptions failed "+times+" times");
        }
    }
}
