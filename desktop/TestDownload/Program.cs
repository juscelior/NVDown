using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using HDSDownload;

namespace TestDownload
{
    class Program
    {
        static void Main(string[] args)
        {
            HDSAgent granAgent = new HDSAgent("Conhecimentos Pedagógicos Específicos Orientador", "Aula 1", @"C:\Users\Juscélio\AppData\Local\Microsoft\Windows\Temporary Internet Files\Content.IE5\GRRA4T10\index_0_av[1].m3u8", null);
            granAgent.Download();

            Console.ReadKey();
        }
    }
}
