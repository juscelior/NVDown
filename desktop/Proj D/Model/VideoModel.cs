using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HDSDownload;

namespace Proj_D.Model
{

    public delegate void VideoModelProgress(object sender, ProgressEventArgs e);
    public class VideoModel
    {
        public string Titulo
        {
            get
            {
                return string.Format("{0} - {1}", this.Curso, this.Aula);
            }
        }
        public string Curso { get; set; }
        public string Aula { get; set; }
        public string PathM3u8 { get; set; }
        public string PathFile { get; set; }
        public int Progresso { get; set; }

        public event VideoModelProgress VideoProgresso;

        private HDSAgent _agentHDS;

        public VideoModel()
        {
        }

        public bool Download()
        {
            if (this._agentHDS == null)
            {
                this._agentHDS = new HDSAgent(this.Curso, this.Aula, this.PathM3u8, this.PathFile);

                this._agentHDS.Progress += new HDSAgentProgress(_agentHDS_Progress);
            }

            return this._agentHDS.Download();
        }

        public void Convert()
        {
            if (this._agentHDS == null)
            {
                this._agentHDS = new HDSAgent(this.Curso, this.Aula, this.PathM3u8, this.PathFile);
            }

            this._agentHDS.ReprocessConvert();
        }

        void _agentHDS_Progress(object sender, ProgressEventArgs e)
        {
            this.Progresso = e.Progress;

            this.VideoProgresso(this, e);
        }
    }
}
