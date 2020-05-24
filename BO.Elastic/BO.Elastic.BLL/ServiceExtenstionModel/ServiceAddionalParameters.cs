using BO.Elastic.BLL.ElasticCore;
using BO.Elastic.BLL.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.ServiceExtenstionModel
{
    public class ServiceAddionalParameters
    {
        public NextWrap NextWrap { get; set; }
        public EServiceStatus ServiceStatus { get; set; }
        public string IP { get; set; }
        public string Port { get; set; }
        public List<KeyValuePair<string,Action>> ActionList { get; set; }
        public EServiceType ServiceType;
    }
}
