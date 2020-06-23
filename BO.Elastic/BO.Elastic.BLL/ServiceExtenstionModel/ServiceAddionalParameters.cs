using BO.Elastic.BLL.ElasticCore;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.ServiceExtenstionModel
{
    public class ServiceAddionalParameters
    {
        public NextWrap NextWrap { get; set; }
        public Service Service { get; set; }
        public EServiceStatus ServiceStatus { get; set; }
        public string IP { get; set; }
        public string Port { get; set; }
        private List<EServiceAction> actionList;
        public List<EServiceAction> ActionList
        {
            get
            {
                if (actionList == null)
                {
                    return new List<EServiceAction>();
                }
                return actionList;
            }
            set
            {
                actionList = value;
            }
        }
        public EServiceType ServiceType;
    }
}
