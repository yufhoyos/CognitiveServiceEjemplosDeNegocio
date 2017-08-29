using Helper;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FaceApiAPP.ViewModels
{
    public class ContentModeratorWindowViewModel : BindableBase
    {
        private string _Contenido = "";
        private string _ContenidoModerado = "";
        private string _Termino = "";
        private ObservableCollection<string> _TerminosModerar = new ObservableCollection<string>();
        private Moderator moderator;

        public string Contenido
        {
            get { return _Contenido; }
            set { SetProperty(ref _Contenido, value); }
        }

        public string ContenidoModerado
        {
            get { return _ContenidoModerado; }
            set { SetProperty(ref _ContenidoModerado, value); }
        }
        public string Termino
        {
            get { return _Termino; }
            set { SetProperty(ref _Termino, value); }
        }
        public ObservableCollection<string> TerminosModerar
        {
            get { return _TerminosModerar; }
            set { SetProperty(ref _TerminosModerar, value); }
        }

        public DelegateCommand Moderar { get; private set; }
        public DelegateCommand IngresarTermino { get; private set; }
        public ContentModeratorWindowViewModel()
        {
            Moderar = new DelegateCommand(ModerarAsync);
            IngresarTermino = new DelegateCommand(IngresarTerminoAsync);
            moderator = new Moderator();
        }

        private async void IngresarTerminoAsync()
        {
            if (!string.IsNullOrEmpty(Termino))
            {
                try
                {
                    var resultado = await moderator.IngresarTermino(Termino, 20, "terminosgroseros");
                    TerminosModerar = new ObservableCollection<string>(resultado.Data.Terms.Select(t => t.Term));
                }
                catch (Exception ex)
                {
                    //FaceApiCompareVal = $"Error {ex.Message}";
                }
            }
        }

        private void ModerarAsync()
        {

        }
    }
}
