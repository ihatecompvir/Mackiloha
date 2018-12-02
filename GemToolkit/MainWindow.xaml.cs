using Microsoft.Win32; // OpenFileDialog
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using GemToolkit.ViewModel;
using Mackiloha;
using Mackiloha.Milo2;
using Mackiloha.IO;

namespace GemToolkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool Debug => App.MainApplication.Debug;
        public Uri Uri => App.MainApplication.BuildUri("Main");

        OpenFileDialog ofd = new OpenFileDialog();
        SaveFileDialog sfd = new SaveFileDialog();

        public MainWindow()
        {
            Initialized += MainWindow_Initialized;
            InitializeComponent();
        }

        private void MainWindow_Initialized(object sender, EventArgs e)
        {
            var model = new HelloViewModel();
            model.PropertyChanged += Model_PropertyChanged;


            DataContext = model;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var model = sender as HelloViewModel;

            switch (e.PropertyName)
            {
                case "State":
                    if (model.State == "open")
                    {
                        model.State = "idle";

                        ofd.Title = "Open MILO file";
                        ofd.Filter = "MILO|*.milo_ps2;*.milo_ps3;*.milo_wii;*.milo_xbox;*.rnd_gc;*.rnd_ps2;*.rnd_xbox;*.gh";

                        if (ofd.ShowDialog() == false) return;

                        OpenMilo(ofd.FileName);
                    }
                    break;
            }
        }

        private void OpenMilo(string path)
        {
            var mf = MiloFile.ReadFromFile(path);
            var serializer = new MiloSerializer(new SystemInfo() { BigEndian = mf.BigEndian });

            var model = DataContext as HelloViewModel;
            model.MiloPath = path;
            
            // TODO: Add try-catch block
            using (var ms = new MemoryStream(mf.Data))
            {
                var milo = serializer.ReadFromStream<MiloObjectDir>(ms);
                model.Milo = milo;
                model.CreateNodes();

                //model.Milo.Entries.ForEach(x => model.Entries.Add(x));

                //Milo_Editor.Serializer = serializer;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.HtmlView.Dispose();
        }
    }
}
