using Core.Application.ViewModels.Report;
using Core.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces
{
    public interface IHttpService
    {
        Task<GenericResult> GetAsync(string url);

        Task<GenericResult> PostAsync(string url, object parrams);

        Task<TrafficData> GetTraffics(DateTime startDate);
    }
}
