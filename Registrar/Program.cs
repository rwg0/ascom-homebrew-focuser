﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrar
{
    class Program
    {
        static void Main(string[] args)
        {
            Process.Start(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe", "/codebase Homebrew.Skywatcher.dll");
            Process.Start(@"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe", "/codebase Homebrew.Skywatcher.dll");
        }
    }
}
