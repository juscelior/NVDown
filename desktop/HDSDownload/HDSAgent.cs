using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace HDSDownload
{
    public delegate void HDSAgentProgress(object sender, ProgressEventArgs e);

    public class HDSAgent
    {
        private string _nameCourse;
        private string _namePart;
        private string _pathFile;
        private string[] _videoParts;
        private string _pathM3u8;

        public event HDSAgentProgress Progress;

        public HDSAgent(string nameCourse, string namePart, string pathM3u8, string pathFile)
        {
            this._nameCourse = nameCourse;
            this._namePart = namePart;
            this._pathM3u8 = pathM3u8;
            this._pathFile = pathFile;
        }

        public bool Download()
        {
            bool result = false;

            try
            {
                if (string.IsNullOrWhiteSpace(this._pathFile))
                {
                    Console.WriteLine("Lendo o arquivo m3u8...");
                    this.ReadM3u8();
                    Console.WriteLine("Fim da leitura do arquivo.");
                    Console.WriteLine("Iniciando o download...");
                    this.InitDownload();
                    Console.WriteLine("Fim do download.");
                }
                else
                {
                    Console.WriteLine("Iniciando o download do arquivo...");
                    this.InitFileDownload();
                    Console.WriteLine("Fim do download do arquivo.");
                }
                Console.WriteLine("Criando a lista de arquivos para fazer o merge...");
                this.CreateListTXT();
                Console.WriteLine("Lista criada.");
                Console.WriteLine("Fazendo o merge e a conversão...");
                this.MergeAndConvertToMp4();
                Console.WriteLine("Arquivo criado.");
                Console.WriteLine("Deletando arquivos .ts ...");
                this.DeleteFilesTS();
                Console.WriteLine("Fim do processo.");

                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool ReprocessConvert()
        {
            bool result = false;

            try
            {
                Console.WriteLine("Criando a lista de arquivos para fazer o merge...");
                this.CreateListTXT();
                Console.WriteLine("Lista criada.");
                Console.WriteLine("Fazendo o merge e a conversão...");
                this.MergeAndConvertToMp4();
                Console.WriteLine("Arquivo criado.");
                Console.WriteLine("Deletando arquivos .ts ...");
                this.DeleteFilesTS();
                Console.WriteLine("Fim do processo.");

                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        private void ReadM3u8()
        {
            WebClient clientAgent = new WebClient();

            //using (TextReader tr = TextReader.Synchronized(new StreamReader()))
            //{
            string content = clientAgent.DownloadString(this._pathM3u8);

            this._videoParts = content.Split(',');

            for (int i = 1; i < this._videoParts.Length; i++)
            {
                this._videoParts[i] = this._videoParts[i].Split('#')[0];
                this._videoParts[i] = this._videoParts[i].Replace("#EXTINF:10", "");
                this._videoParts[i] = this._videoParts[i].Replace("#EXT-X-ENDLIST", "");
                this._videoParts[i] = this._videoParts[i].Replace("\n", "");
            }
            //}
        }

        private void InitDownload()
        {
            string urlDomain = this._pathM3u8.Substring(0, this._pathM3u8.LastIndexOf('/'));


            WebClient clientAgent = new WebClient();

            DirectoryInfo di = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), this._nameCourse));
            if (!di.Exists)
            {
                di.Create();
            }
            if (di.GetDirectories().FirstOrDefault(d => d.Name.Equals(this._nameCourse)) == null)
            {
                di.CreateSubdirectory(this._namePart);
            }

            for (int i = 1; i < this._videoParts.Length; i++)
            {
                string tempUrl = string.Format("{0}/{1}", urlDomain, this._videoParts[i]);
                byte[] arrBuffer = clientAgent.DownloadData(tempUrl);

                using (FileStream fs = new FileStream(string.Format(@"{0}\{1}\parte-{2:0000}.ts", this._nameCourse, this._namePart, i), FileMode.Create))
                {
                    fs.Write(arrBuffer, 0, arrBuffer.Length);
                    fs.Flush();
                }
            }
        }

        private void InitFileDownload()
        {
            WebClient clientAgent = new WebClient();

            DirectoryInfo di = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), this._nameCourse));
            if (!di.Exists)
            {
                di.Create();
            }
            if (di.GetDirectories().FirstOrDefault(d => d.Name.Equals(this._nameCourse)) == null)
            {
                di.CreateSubdirectory(this._namePart);
            }

            byte[] arrBuffer = clientAgent.DownloadData(this._pathFile);

            using (FileStream fs = new FileStream(string.Format(@"{0}\{1}\{1}.{2}", this._nameCourse, this._namePart, Path.GetExtension(this._pathFile)), FileMode.Create))
            {
                fs.Write(arrBuffer, 0, arrBuffer.Length);
                fs.Flush();
            }

        }

        private void CreateListTXT()
        {

            DirectoryInfo di = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), this._nameCourse, this._namePart));

            List<string> files = di.GetFiles("*.ts").ToList().ConvertAll(f =>
            {
                return string.Format(@"file '{0}\{1}'", di.FullName, f);
            });


            File.WriteAllLines(Path.Combine(Directory.GetCurrentDirectory(), this._nameCourse, this._namePart, "txt.txt"), files);
        }

        private void MergeAndConvertToMp4()
        {
            Process ffmpeg = new Process();

            ffmpeg.StartInfo.RedirectStandardOutput = true;
            ffmpeg.StartInfo.UseShellExecute = false;

            ffmpeg.StartInfo.FileName = "ffmpeg.exe";
            ffmpeg.StartInfo.Arguments = string.Format(@"-f concat -i ""{0}"" -c copy -bsf:a aac_adtstoasc ""{1}""", Path.Combine(Directory.GetCurrentDirectory(), this._nameCourse, this._namePart, "txt.txt"), Path.Combine(Directory.GetCurrentDirectory(), this._nameCourse, this._namePart, string.Format("{0}.mp4", this._namePart)));

            ffmpeg.Start();
            StreamReader stream = ffmpeg.StandardOutput;

            String output = stream.ReadToEnd();

            ffmpeg.WaitForExit();
        }

        private void DeleteFilesTS()
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), this._nameCourse, this._namePart));
            di.GetFiles("*.ts").ToList().ForEach(f =>
            {
                File.Delete(string.Format(@"{0}\{1}", di.FullName, f));
            });

            di.GetFiles("*.txt").ToList().ForEach(f =>
            {
                File.Delete(string.Format(@"{0}\{1}", di.FullName, f));
            });
        }
    }
}
