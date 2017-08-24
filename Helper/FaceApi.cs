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
        private static string apiKey = "db66727920e74e409d080bb240b22755";
        private static string endpoint = "https://eastus2.api.cognitive.microsoft.com/face/v1.0";
        public static async Task<string> AnalizarRostros(string filename)
        {
            FaceServiceClient faceServiceClient = new FaceServiceClient(apiKey, endpoint);
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
            var result = "";
            using (Stream s = File.OpenRead(filename))
            {
                var faces = await faceServiceClient.DetectAsync(s, true, true, atributos);
                result = JsonConvert.SerializeObject(faces, Formatting.Indented);
                foreach (var face in faces)
                {
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
    }
}
