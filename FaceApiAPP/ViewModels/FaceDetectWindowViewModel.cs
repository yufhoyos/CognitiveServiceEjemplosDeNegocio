using Helper;
using Microsoft.ProjectOxford.Face;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FaceApiAPP.ViewModels
{
    public class FaceDetectWindowViewModel : BindableBase
    {
        private string _title = "Face Api Demo";
        private string _faceApeVal1 = "";
        private string _faceApeVal2 = "";
        private Uri _mediaElementSource1;
        private Uri _mediaElementSource2;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string FaceApeVal2
        {
            get { return _faceApeVal2; }
            set { SetProperty(ref _faceApeVal2, value); }
        }

        public string FaceApeVal1
        {
            get { return _faceApeVal1; }
            set { SetProperty(ref _faceApeVal1, value); }
        }

        public Uri MediaElementSource1
        {
            get { return _mediaElementSource1; }
            set { SetProperty(ref _mediaElementSource1, value); }
        }


        public Uri MediaElementSource2
        {
            get { return _mediaElementSource2; }
            set { SetProperty(ref _mediaElementSource2, value); }
        }

        public DelegateCommand AbrirFoto1 { get; private set; }
        public DelegateCommand AbrirFoto2 { get; private set; }

        public FaceDetectWindowViewModel()
        {
            AbrirFoto1 = new DelegateCommand(AbrirFoto1Async, CanAbrirFoto1);
            AbrirFoto2 = new DelegateCommand(AbrirFoto2Async, CanAbrirFoto2);
        }

        private bool CanAbrirFoto1()
        {
            return true;
        }

        private bool CanAbrirFoto2()
        {
            return true;
        }

        private async void AbrirFoto2Async()
        {
            var filename = AbrirFoto();
            if (!string.IsNullOrEmpty(filename))
            {
                MediaElementSource2 = new Uri(filename);
                try
                {
                    FaceApeVal2 = await FaceApi.AnalizarRostros(filename);
                }
                catch (Exception ex)
                {

                }
            }
        }

        private async void AbrirFoto1Async()
        {
            var filename = AbrirFoto();
            if (!string.IsNullOrEmpty(filename))
            {
                MediaElementSource1 = new Uri(filename);
                try
                {
                    FaceApeVal1 = await FaceApi.AnalizarRostros(filename);
                }
                catch (Exception ex)
                {

                }
            }
        }

        private string AbrirFoto()
        {
            var dlg = new OpenFileDialog();
            var result = dlg.ShowDialog();
            if (result == true)
            {
                return dlg.FileName;
            }
            else
            {
                return "";
            }
        }

    }
}
