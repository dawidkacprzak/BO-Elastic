using System.Collections.Generic;
using BO.Elastic.BLL.ElasticCore;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.Types;

namespace BO.Elastic.BLL.ServiceExtenstionModel
{
    public class ServiceAddionalParameters
    {
        private List<EServiceAction> actionList;
        public EServiceType ServiceType;
        public NextWrap NextWrap { get; set; }
        public Service Service { get; set; }
        public EServiceStatus ServiceStatus { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }

        public List<EServiceAction> ActionList
        {
            get
            {
                if (actionList == null) return new List<EServiceAction>();
                return actionList;
            }
            set => actionList = value;
        }
    }
}