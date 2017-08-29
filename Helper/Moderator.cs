using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class Moderator
    {
        private string apiKey = "3f0912d2459d4d80bfbde004a1d86739";
        private string endpointManagement = "https://eastus2.api.cognitive.microsoft.com/contentmoderator/lists/v1.0";
        private string endpointModerate = "https://eastus2.api.cognitive.microsoft.com/contentmoderator/moderate/v1.0";

        private RequestHelper requestHelperManagement;
        private RequestHelper requestHelperModerate;
        public Moderator()
        {
            requestHelperManagement = new RequestHelper(apiKey, endpointManagement);
            requestHelperModerate = new RequestHelper(apiKey, endpointModerate);
        }

        public async Task<CrearListaObj> CrearLista(string NombreLista)
        {
            CrearListaObj crear = new CrearListaObj
            {
                Name = NombreLista,
                Description = NombreLista,
                Metadata = new Dictionary<string, string> { { "typem", "groserias" } }
            };
            return await requestHelperManagement.PostAsync<CrearListaObj, CrearListaObj>("/termlists", crear);
        }

        public async Task<ListaTerminos> IngresarTermino(string termmino, int ListaCodigo, string NombreLista)
        {
            CrearListaObj existe = null;
            try
            {
                existe = await requestHelperManagement.GetAsync<CrearListaObj>($"/termlists/{ListaCodigo}");
            }
            catch (HttpRequestException hex)
            {

            }
            if (existe == null)
            {
                existe = await CrearLista(NombreLista);
                ListaCodigo = existe.Id;
            }
            await requestHelperManagement.PostAsync<string>($"/termlists/{ListaCodigo}/terms/{termmino}?language=spa");
            await RefrescarLista(ListaCodigo);
            ListaTerminos listaact = await requestHelperManagement.GetAsync<ListaTerminos>($"/termlists/{ListaCodigo}/terms?language=spa");
            return listaact;
        }

        public async Task<bool> RefrescarLista(int ListaCodigo)
        {
            await requestHelperManagement.PostAsync<ListaRefresh>($"/termlists/{ListaCodigo}/RefreshIndex?language=spa");
            return true;
        }

        public async Task<string> Moderar(string Contenido, int ListaCodigo)
        {
            var res = await requestHelperModerate.PostTextAsync<ModerarResult, string>($"/ProcessText/Screen/?language=spa&listId={ListaCodigo}", Contenido);
            return JsonConvert.SerializeObject(res, Formatting.Indented);
        }
    }

    public class ModerarResult
    {
        public string OriginalText { get; set; }
        public string NormalizedText { get; set; }

        public string Language { get; set; }
        public List<TerminoMod> Terms { get; set; }

        public StatusMod Status { get; set; }

    }

    public class StatusMod
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public string Exception { get; set; }
    }

    public class TerminoMod
    {
        public int Index { get; set; }
        public int OriginalIndex { get; set; }
        public int ListId { get; set; }
        public string Term { get; set; }
    }

    public class CrearListaObj
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class ListaTerminos
    {
        public Datos Data { get; set; }
    }

    public class Datos
    {
        public string Language { get; set; }
        public List<Termino> Terms { get; set; }
    }

    public class Termino
    {
        public string Term { get; set; }
    }

    public class ListaRefresh
    {
        public string ContentSourceId { get; set; }
        public string IsUpdateSuccess { get; set; }
    }
}
