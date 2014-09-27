using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDSDownload
{
    public class ProgressEventArgs : EventArgs
    {
        public int Progress { get; set; }
    }
}
