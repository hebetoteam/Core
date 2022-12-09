using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.Report
{
    public class TrafficViewModel
    {
        public TrafficData data { get;set;}

        public string errors { get;set;}
    }

    public class TrafficData
    {
        public TrafficViewer viewer { get;set;}
    }

    public class TrafficViewer
    {
        public List<TrafficZone> zones { get;set;} = new List<TrafficZone>();
    }

    public class TrafficZone
    {
        public List<TrafficResponse> httpRequests1dGroups { get;set;} = new List<TrafficResponse>();
    }

    public class TrafficResponse
    {
        public TrafficDataDate date { get;set;} = new TrafficDataDate();

        public TrafficSum sum { get; set; }
    }

    public class TrafficDataDate{
        public string date { get;set;}

        
    }

    public class TrafficSum
    {
        public List<TrafficCountryMap> countryMap = new List<TrafficCountryMap>();

    }

    public class TrafficCountryMap
    {
        public string clientCountryName { get;set;}

        public int requests { get;set;}

        public string countryName { get
            {
                try
                {
                    if (!string.IsNullOrEmpty(clientCountryName))
                    {
                        RegionInfo cultureInfo = new RegionInfo(clientCountryName);
                        return cultureInfo.EnglishName;
                    }
                }
                catch (Exception)
                {

                }
                

                return clientCountryName;
            } 
        }
    }
}
