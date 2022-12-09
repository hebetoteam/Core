using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.ImportExport
{
    public class GoogleGISDto
    {
        public string Name { get; set; }    

        public string Fulladdress { get;set; }

        public string Phone { get; set; }

        public string Website { get; set; }

        public string Cid { get; set; }

        public string PlaceId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }  

        public string Featuredimage { get;set;}

    }
}
