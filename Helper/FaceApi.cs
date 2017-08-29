using Microsoft.ProjectOxford.Face;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class FaceApi
    {
        private string apiKey = "db66727920e74e409d080bb240b22755";
        private string endpoint = "https://eastus2.api.cognitive.microsoft.com/face/v1.0";
        private FaceServiceClient faceServiceClient;
        public FaceApi()
        {
            faceServiceClient = new FaceServiceClient(apiKey, endpoint);
        }
        public async Task<List<Rostro>> AnalizarRostros(string filename, bool ConAtributos)
        {
            List<FaceAttributeType> atributos = new List<FaceAttributeType>
            {
                FaceAttributeType.Accessories,
               FaceAttributeType.Age,
               FaceAttributeType.Blur,
               FaceAttributeType.Emotion,
               FaceAttributeType.Exposure,
               FaceAttributeType.FacialHair,
               FaceAttributeType.Gender,
               FaceAttributeType.Glasses    ,
               FaceAttributeType.Hair,
               FaceAttributeType.HeadPose,
               FaceAttributeType.Makeup,
               FaceAttributeType.Noise,
               FaceAttributeType.Occlusion,
               FaceAttributeType.Smile
            };
            List<Rostro> result = new List<Rostro>();
            using (Stream s = File.OpenRead(filename))
            {
                Microsoft.ProjectOxford.Face.Contract.Face[] faces;
                if (ConAtributos)
                {
                    faces = await faceServiceClient.DetectAsync(s, true, true, atributos);
                }
                else
                {
                    faces = await faceServiceClient.DetectAsync(s, true, true);
                }
                foreach (var face in faces)
                {
                    result.Add(new Rostro
                    {
                        FaceId = face.FaceId,
                        FaceObj = JsonConvert.SerializeObject(faces, Formatting.Indented)
                    });
                    var rect = face.FaceRectangle;
                    var landmarks = face.FaceLandmarks;
                    //Propiedades
                    var id = face.FaceId;
                    var attributes = face.FaceAttributes;
                    if (attributes != null)
                    {
                        var age = attributes.Age;
                        var gender = attributes.Gender;
                        var smile = attributes.Smile;
                        var facialHair = attributes.FacialHair;
                        var headPose = attributes.HeadPose;
                        var glasses = attributes.Glasses;
                    }
                }
            }
            return result;
        }

        public async Task<string> ComprarRostros(string filename1, string filename2)
        {
            var rostros1 = await AnalizarRostros(filename1, false);
            var rostros2 = await AnalizarRostros(filename2, false);

            var result = await faceServiceClient.VerifyAsync(rostros1.FirstOrDefault().FaceId, rostros2.FirstOrDefault().FaceId);

            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }

        public async Task SubirRostroAGrupo(string IdGrupo, string filename)
        {
            bool ExisteGrupo;
            try
            {
                await faceServiceClient.GetPersonGroupAsync(IdGrupo);
                ExisteGrupo = true;
            }
            catch
            {
                ExisteGrupo = false;
            }

            if (!ExisteGrupo)
            {
                await faceServiceClient.CreatePersonGroupAsync(IdGrupo, IdGrupo);
            }

            var tag = System.IO.Path.GetFileName(filename.Replace("$", ""));
            var persona = await faceServiceClient.CreatePersonAsync(IdGrupo, tag);
            using (var fiStream = File.OpenRead(filename))
            {
                var persistFace = await faceServiceClient.AddPersonFaceAsync(IdGrupo, persona.PersonId, fiStream, tag);
            }
            await faceServiceClient.TrainPersonGroupAsync(IdGrupo);
        }

        public async Task<string> BuscarRostroEnGrupo(string IdGrupo, string filename)
        {
            var rostros1 = await AnalizarRostros(filename, false);
            Guid[] buscarRostros = rostros1.Select(r => r.FaceId).ToArray();
            var identifyResult = await faceServiceClient.IdentifyAsync(IdGrupo, buscarRostros);
            return JsonConvert.SerializeObject(identifyResult, Formatting.Indented);
        }
    }
}
