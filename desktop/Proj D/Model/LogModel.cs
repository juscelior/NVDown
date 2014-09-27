using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Proj_D.Model
{
    public class LogModel
    {
        public static void Log(VideoModel video)
        {
            FileInfo fi = new FileInfo("log.txt");
            StreamWriter sw = fi.AppendText();
            sw.WriteLine(string.Format("------------------------------------------------------------------"));
            sw.WriteLine(string.Format("Curso: {0}", video.Curso));
            sw.WriteLine(string.Format("Aula: {0}", video.Aula));
            sw.WriteLine(string.Format("m3u8: {0}", video.PathM3u8));
            sw.WriteLine(string.Format("Arquivo: {0}", video.PathFile));
            sw.WriteLine(string.Format("------------------------------------------------------------------/n"));
            sw.Flush();
        }
    }
}
