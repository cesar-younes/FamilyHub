using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace FamilyHub.Services
{
    public class CognitiveServicesFace
    {
        private static string FaceServiceKey = Environment.GetEnvironmentVariable("FACESERVICEKEY");
        private static string FaceServiceRegion = Environment.GetEnvironmentVariable("FACESERVICEREGION");
        private static readonly IFaceClient _faceServiceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(FaceServiceKey),
            new System.Net.Http.DelegatingHandler[] { });

        public static async Task CreatePersonGroupIfNotExistsAsync(string largePersonGroupId, string personGroupName)
        {
            try
            {
                var largePersonGroup = await _faceServiceClient.LargePersonGroup.GetAsync(largePersonGroupId);

                if (largePersonGroup == null)
                {
                    await _faceServiceClient.LargePersonGroup.CreateAsync(largePersonGroupId, name: personGroupName);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("" + e);
                throw;
            }
        }

        public static async Task AddEmptyPersonInGroupAsync(string personName, string largePersonGroupId)
        {
            try
            {
                Person person = await _faceServiceClient.LargePersonGroupPerson.CreateAsync(
                    // Id of the PersonGroup that the person belongs to
                    largePersonGroupId,
                    // Name of the person
                    personName
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("" + e);
                throw;
            }

        }

        public static async Task AddFaceToPersonAsync(Guid personId, string imageUrl, string largePersonGroupId)
        {
            try
            {
                await _faceServiceClient.LargePersonGroupPerson.AddFaceFromUrlAsync(largePersonGroupId, personId, imageUrl);
            }
            catch (Exception e)
            {
                Debug.WriteLine("" + e);
                throw;
            }
        }

        public static async Task TrainPersonGroupAsync(string largePersonGroupId)
        {
            try
            {
                await _faceServiceClient.LargePersonGroup.TrainAsync(largePersonGroupId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("" + e);
                throw;
            }
        }

        public static async Task<bool> ReturnTrainingStatusWhenTrueAsync(string largePersonGroupId)
        {
            TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await _faceServiceClient.LargePersonGroup.GetTrainingStatusAsync(largePersonGroupId);

                if (trainingStatus.Status != TrainingStatusType.Running)
                {
                    break;
                }

                await Task.Delay(1000);
            }

            return true;
        }

        public static async Task<Person> DetectPersonAsync(string imageUrl, string largePersonGroupId)
        {
           
            var faces = await _faceServiceClient.Face.DetectWithUrlAsync(imageUrl);
            var faceIds = faces.Select(face => face.FaceId.Value).ToArray();

            var results = await _faceServiceClient.Face.IdentifyAsync(faceIds, largePersonGroupId:largePersonGroupId);
            foreach (var identifyResult in results)
            {
                //Console.WriteLine("Result of face: {0}", identifyResult.FaceId);
                if (identifyResult.Candidates.Count == 0)
                {
                    Console.WriteLine("No one identified");
                    return null;
                }
                else
                {
                    // Get top 1 among all candidates returned
                    var candidateId = identifyResult.Candidates[0].PersonId;
                    var person = await _faceServiceClient.LargePersonGroupPerson.GetAsync(largePersonGroupId, candidateId);
                    Console.WriteLine("Identified as {0}", person.Name);
                    return person;
                }
            }

            return null;
        }
    }
}
