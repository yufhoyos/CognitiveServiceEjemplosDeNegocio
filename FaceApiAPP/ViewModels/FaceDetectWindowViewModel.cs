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
using System.Linq;

namespace FaceApiAPP.ViewModels
{
    public class FaceDetectWindowViewModel : BindableBase
    {
        private string _title = "Face Api Demo";
        private string _faceApiVal1 = "";
        private string _faceApiVal2 = "";
        private string _filename1 = "";
        private string _filename2 = "";
        private string _faceApiCompareVal = "";
        private Uri _mediaElementSource1;
        private Uri _mediaElementSource2;
        private FaceApi faceApi;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string FaceApiVal2
        {
            get { return _faceApiVal2; }
            set { SetProperty(ref _faceApiVal2, value); }
        }

        public string FaceApiVal1
        {
            get { return _faceApiVal1; }
            set { SetProperty(ref _faceApiVal1, value); }
        }

        public string FaceApiCompareVal
        {
            get { return _faceApiCompareVal; }
            set { SetProperty(ref _faceApiCompareVal, value); }
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
        public DelegateCommand Comparar { get; private set; }
        public DelegateCommand SubirAlGrupo { get; private set; }
        public DelegateCommand BuscarEnGrupo { get; private set; }

        public FaceDetectWindowViewModel()
        {
            AbrirFoto1 = new DelegateCommand(AbrirFoto1Async);
            AbrirFoto2 = new DelegateCommand(AbrirFoto2Async);
            Comparar = new DelegateCommand(CompararAsync);
            SubirAlGrupo = new DelegateCommand(SubirAlGrupoAsync);
            BuscarEnGrupo = new DelegateCommand(BuscarEnGrupoAsync);
            faceApi = new FaceApi();
        }

        private async void BuscarEnGrupoAsync()
        {
            FaceApiCompareVal = "";
            if (!string.IsNullOrEmpty(_filename1))
            {
                try
                {
                    var resultado=await faceApi.BuscarRostroEnGrupo("grupomdenet", _filename1);
                    FaceApiCompareVal = resultado;
                }
                catch (Exception ex)
                {
                    FaceApiCompareVal = $"Error {ex.Message}";
                }
            }
        }

        private async void SubirAlGrupoAsync()
        {
            if (!string.IsNullOrEmpty(_filename1))
            {
                try
                {
                    await faceApi.SubirRostroAGrupo("grupomdenet",_filename1);
                }
                catch (Exception ex)
                {
                    FaceApiCompareVal = $"Error {ex.Message}";
                }
            }
        }
        
        private async void CompararAsync()
        {
            FaceApiCompareVal = "";
            if (!string.IsNullOrEmpty(_filename1) && !string.IsNullOrEmpty(_filename2))
            {
                try
                {
                    var comparacion = await faceApi.ComprarRostros(_filename1, _filename2);
                    FaceApiCompareVal = comparacion;
                }
                catch (Exception ex)
                {
                    FaceApiCompareVal = $"Error {ex.Message}";
                }
            }
        }

        private async void AbrirFoto2Async()
        {
            _filename2 = AbrirFoto();
            FaceApiVal2 = "";
            if (!string.IsNullOrEmpty(_filename2))
            {
                MediaElementSource2 = new Uri(_filename2);
                try
                {
                    var rostros = await faceApi.AnalizarRostros(_filename2, true);
                    FaceApiVal2 = rostros.FirstOrDefault().FaceObj;
                }
                catch (Exception ex)
                {
                    FaceApiVal2 = $"Error {ex.Message}";
                }
            }
        }

        private async void AbrirFoto1Async()
        {
            _filename1 = AbrirFoto();
            FaceApiVal1 = "";
            if (!string.IsNullOrEmpty(_filename1))
            {
                MediaElementSource1 = new Uri(_filename1);
                try
                {
                    var rostros = await faceApi.AnalizarRostros(_filename1, true);
                    FaceApiVal1 = rostros.FirstOrDefault().FaceObj;
                }
                catch (Exception ex)
                {
                    FaceApiVal1 = $"Error {ex.Message}";
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
