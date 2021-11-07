using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSServiceApp
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string[] AllowedOrigins { get; set; }

        public string CamundaRestApiUri { get; set; }
    }
}
