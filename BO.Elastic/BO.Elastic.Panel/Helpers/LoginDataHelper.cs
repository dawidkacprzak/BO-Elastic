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
        public static string FilePath = Path.Combine(System.IO.Path.GetTempPath(), "boElasticLoginData.dat");

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
                        using (FileStream fs = new FileStream(FilePath, FileMode.Create))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(fs, loginData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static LoginData GetCachedLoginData()
        {
            LoginData loginData = new LoginData();
            try
            {
                using (FileStream fs = new FileStream(FilePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    loginData = (LoginData)formatter.Deserialize(fs);
                }
            }
            catch (SerializationException)
            {
                MessageBox.Show("Błąd podczas wczytywania danych logowania.");
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

        public static bool ClearCachedLoginData()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
