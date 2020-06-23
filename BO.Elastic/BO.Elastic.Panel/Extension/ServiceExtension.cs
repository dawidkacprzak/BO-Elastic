using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.ServiceExtenstionModel;
using BO.Elastic.BLL.Types;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace BO.Elastic.Panel.ClassExtensions
{
    public static class ServiceExtension
    {
        public static Action GetActionParameters(this ServiceAddionalParameters parameters, EServiceAction eServiceAction)
        {

            switch (eServiceAction)
            {
                case EServiceAction.ConnectBySSH:

                    try
                    {
                        
                    }
                    catch (Exception)
                    {
                        throw new Exception("");
                    }
                    break;

                case EServiceAction.Start:

                    try
                    {

                    }
                    catch (Exception)
                    {
                        throw new Exception("");
                    }
                    break;

                case EServiceAction.Stop:

                    try
                    {

                    }
                    catch (Exception)
                    {
                        throw new Exception("");
                    }
                    break;

                case EServiceAction.Information:

                    try
                    {
                        return new Action(() => {
                            Window window = new Window();
                            window.Show(); });
                        
                    }
                    catch (Exception)
                    {
                        throw new Exception("");
                    }

                default: throw new NotImplementedException();
            }
            return new Action(() => { MessageBox.Show("Oj oj, coś poszło nie tak :/"); });
        }
    }
}
