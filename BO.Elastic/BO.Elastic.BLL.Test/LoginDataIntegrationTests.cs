using System;
using System.IO;
using BO.Elastic.BLL.Model;
using BO.Elastic.Panel.Helpers;
using NUnit.Framework;

namespace BO.Elastic.BLL.Test
{
    public class LoginDataIntegrationTests
    {
        [Test]
        public void InvokeSaveLoginDataWithInvalidLogin()
        {
            LoginData loginData = new LoginData
            {
                Login = string.Empty,
                Password = string.Empty
            };

            Assert.Throws<ArgumentException>(() => { LoginDataHelper.SaveLoginData(loginData); });
        }

        [Test]
        public void InvokeGetLoginDataWithInvalidPath()
        {
            LoginDataHelper.FilePath = Path.Combine(Path.GetTempPath(), "testV1.dat");

            LoginData loginData = LoginDataHelper.GetCachedLoginData();

            Assert.AreEqual(loginData.Login, string.Empty);
            Assert.AreEqual(loginData.Password, string.Empty);
        }

        [Test]
        public void InvokeClearCachedLoginDataWithExistingFile()
        {
            LoginDataHelper.FilePath = Path.Combine(Path.GetTempPath(), "testV2.dat");
            LoginDataHelper.SaveLoginData(new LoginData
            {
                Login = "testoweoponeo",
                Password = "oponeo123"
            });
            Assert.IsTrue(LoginDataHelper.ClearCachedLoginData());
        }

        [Test]
        public void InvokeClearCachedLoginDataWithoutExistingFile()
        {
            LoginDataHelper.FilePath = Path.Combine(Path.GetTempPath(), "testV3.dat");
            Assert.IsFalse(LoginDataHelper.ClearCachedLoginData());
        }

        [Test]
        public void CompareLoginDataAfterSaving()
        {
            LoginDataHelper.FilePath = Path.Combine(Path.GetTempPath(), "testV4.dat");
            LoginData loginData = new LoginData
            {
                Login = "testV4",
                Password = "testV4"
            };
            LoginDataHelper.SaveLoginData(loginData);
            LoginData savedLoginData = LoginDataHelper.GetCachedLoginData();

            Assert.AreEqual(loginData.Login, savedLoginData.Login);
            Assert.AreEqual(loginData.Password, savedLoginData.Password);
        }
    }
}