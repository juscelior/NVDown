using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Proj_D.Model;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;
using MahApps.Metro.Controls;

namespace Proj_D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ObservableCollection<VideoModel> videos;

        public MainWindow()
        {
            InitializeComponent();
            videos = new ObservableCollection<VideoModel>();
            lsvAulasDownload.ItemsSource = videos;
        }

        private void btnAddFileM3u8_Click(object sender, RoutedEventArgs e)
        {

            if (chkConvert.IsChecked.Value)
            {
                VideoModel videoModel = new VideoModel()
                {
                    PathM3u8 = null,
                    Curso = txtNomeCurso.Text.Trim(),
                    Aula = txtNomeAula.Text.Trim(),
                    PathFile = txtUrlArquivo.Text.Trim(),
                    Progresso = 0
                };

                videoModel.VideoProgresso += new VideoModelProgress(videoModel_VideoProgresso);
                videos.Add(videoModel);

                LogModel.Log(videoModel);

                videoModel.Convert();
            }
            else if (string.IsNullOrWhiteSpace(txtUrlArquivo.Text))
            {
                //OpenFileDialog ofdFileM3u8 = new OpenFileDialog();
                //ofdFileM3u8.Title = "Selecione o arquivo m3u8";
                //ofdFileM3u8.Filter = "Arquivos m3u8 (*.m3u8)|*.m3u8";



                //if (ofdFileM3u8.ShowDialog() == true)
                //{
                //    CreateVideoModel(ofdFileM3u8.FileName);
                //}

                CreateVideoModel(txtUrlM3u8.Text.Trim());
            }
            else
            {
                CreateVideoModel(null);
            }
        }

        private void CreateVideoModel(string fileNameM3u8)
        {
            VideoModel videoModel = new VideoModel()
            {
                PathM3u8 = fileNameM3u8.Trim(),
                Curso = txtNomeCurso.Text.Trim(),
                Aula = txtNomeAula.Text.Trim(),
                PathFile = txtUrlArquivo.Text.Trim(),
                Progresso = 0
            };

            videoModel.VideoProgresso += new VideoModelProgress(videoModel_VideoProgresso);
            videos.Add(videoModel);

            LogModel.Log(videoModel);

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoWork), new AsyncModel() { Main = this, Video = videoModel });
        }

        void videoModel_VideoProgresso(object sender, HDSDownload.ProgressEventArgs e)
        {
            this.lsvAulasDownload.Items.Refresh();
        }

        private static void DoWork(object state)
        {
            AsyncModel model = (AsyncModel)state;

            model.Video.Download();

            model.Video.Progresso = 100;

            model.Main.Dispatcher.Invoke(new Action<ListView>((l) => l.Items.Refresh()), model.Main.lsvAulasDownload);

        }

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            VideoModel video = button.DataContext as VideoModel;

            Process.Start(string.Format(@"{0}", System.IO.Path.Combine(Directory.GetCurrentDirectory(), video.Curso, video.Aula)));
        }

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            VideoModel video = button.DataContext as VideoModel;

            video.Convert();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // This demo runs on .Net 4.0, but we're using the Microsoft.Bcl.Async package so we have async/await support
            // The package is only used by the demo and not a dependency of the library!
            MetroDialogOptions.ColorScheme = MahApps.Metro.Controls.Dialogs.MetroDialogColorScheme.Theme;

            var mySettings = new MahApps.Metro.Controls.Dialogs.MetroDialogSettings()
            {
                AffirmativeButtonText = "Hi",
                NegativeButtonText = "Go away!",
                FirstAuxiliaryButtonText = "Cancel",
                ColorScheme = MahApps.Metro.Controls.Dialogs.MetroDialogColorScheme.Theme
            };

            MahApps.Metro.Controls.Dialogs.MessageDialogResult result = await MahApps.Metro.Controls.Dialogs.DialogManager.ShowMessageAsync(this, "Hello!", "Welcome to the world of metro! ",
                MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

            if (result != MahApps.Metro.Controls.Dialogs.MessageDialogResult.FirstAuxiliary)
                await MahApps.Metro.Controls.Dialogs.DialogManager.ShowMessageAsync(this, "Result", "You said: " + (result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative ? mySettings.AffirmativeButtonText : mySettings.NegativeButtonText +
                    Environment.NewLine + Environment.NewLine + "This dialog will follow the Use Accent setting."));
        }
    }
}
