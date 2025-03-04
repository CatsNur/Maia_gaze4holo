using RealityCollective.ServiceFramework.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace maia.Services
{
    public interface IYoloProcessor : IService
    {
        Task<List<YoloItem>> RecognizeObjects(Texture2D texture);
    }
}