using BO.Elastic.BLL.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;

namespace BO.Elastic.Panel.Helpers
{
    public static class LoginDataHelper
    {
        public static readonly object saveLoginDataLock = new object();
        public static void SaveLoginData(LoginData loginData)
        {
            if (!string.IsNullOrWhiteSpace(loginData.Login))
            {
                if (string.IsNullOrEmpty(loginData.Password))
                {
                    loginData.Password = "";
                }
                try
                {
                    lock (saveLoginDataLock)
                    {
                        using (FileStream fs = new FileStream(Path.Combine(Path.GetTempPath(), "boElasticLoginData.dat"), FileMode.Create))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(fs, loginData);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public static LoginData GetCachedLoginData()
        {
            LoginData loginData = new LoginData();
            try
            {
                using (FileStream fs = new FileStream(Path.Combine(Path.GetTempPath(), "boElasticLoginData.dat"), FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    loginData = (LoginData)formatter.Deserialize(fs);
                }
            }
            catch (SerializationException)
            {
                MessageBox.Show("Błąd podczas wczytywania konfiguracji. Pobieram ponownie.");
            }
            catch (FileNotFoundException)
            {
                loginData.Login = string.Empty;
                loginData.Password = string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return loginData;
        }
    }
}
