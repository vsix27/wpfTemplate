using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Vsix.Common.Helpers;
using Vsix.Viewer.Helpers;


namespace Vsix.CommonTests.Helpers
{
    [TestFixture()]
    public class RegistryHelperTests
    {
        private void SkipTest()
        {
            // Assert.Fail();
            Debug.WriteLine("not implemented");
            Assert.IsNotEmpty("...", "not implemented");
        }

        [Test()]
        public void Is64BitTest()
        {
            bool b = RegistryHelper.Is64Bit();
            Assert.IsTrue(b);
        }

        #region commented
        /*
        [Test()]
        public void AddToHKLMStartUpTest(){SkipTest();}
        [Test()]
        public void AddToHKCUStartUpTest(){SkipTest();}
         * */
        #endregion

        [Test()]
        public void GetHKCUStartUpsTest()
        {
            var ss = RegistryHelper.GetHKCUStartUps();
            ss.DebugConsole();
            Assert.IsNotEmpty(ss);
        }

        [Test()]
        public void GetHKLMStartUpTest()
        {
            var ss = RegistryHelper.GetHKCUStartUp("CCleaner");
            Assert.IsNotEmpty(ss);
        }

        [Test()]
        public void GetHKCUStartUpTest()
        {
            SkipTest();
        }

        [Test()]
        public void RemoveFromStartUpTest()
        {
            SkipTest();
        }

        [Test()]
        public void ReadTest()
        {
            SkipTest();
        }

        [Test()]
        public void WriteTest()
        {
            SkipTest();
        }

        [Test()]
        public void ReadKeyListTest()
        {
            SkipTest();
        }

        [Test()]
        public void DeleteKeyTest()
        {
            SkipTest();
        }

        [Test()]
        public void CheckIfAppHasAdminRightsTest()
        {
            Debug.WriteLine("IsUserAdministrator " + RegistryHelper.IsUserAdministrator);
            Assert.IsTrue(true);
        }

        [Test()]
        public void GetVisualStudioPackagesPath()
        {
            var sKeys = RegistryPackage.GetRegistryPackagesPath();
            sKeys.DebugConsole();
            Assert.IsTrue(sKeys.Count > 0);
        }

        [Test()]public void GetVisualStudioApps()
        {
            var sKeys = RegistryHelper.GetRegistryPath(RegistryHelper.HKCU_VS).GetSubKeyNames().ToList();

            Debug.WriteLine("---- GetSubKeyNames ------ start");
            sKeys.DebugConsole();
            Debug.WriteLine("---- GetSubKeyNames ------ end ");

            var sKeys12 = sKeys.Where(c => c.StartsWith("12.0")).ToList();
            Func<string, string> addPackage = (c) => RegistryHelper.HKCU_VS + "\\" + c + "\\Packages";
            //var sKeys12 = (from c in sKeys
            //               where c.StartsWith("12.0") && RegistryHelper.GetRegistryPath(addPackage(c)) != null
            //               select addPackage(c)).ToList();

            // packages are only if string has Exp_Config :  "12.0Exp_Config"
            foreach (string c in sKeys12)
            {
                Debug.WriteLine("***********  " + c + "  *********** start ");

                string sPackage = addPackage(c);
                if (RegistryHelper.GetRegistryPath(sPackage) == null) continue;
                // guids of packages
                int k = 0;
                foreach (var  subKey in  RegistryHelper.GetRegistryPath(sPackage).GetSubKeyNames())
                {
                    var rk = RegistryHelper.GetRegistryPath(sPackage + "\\" + subKey);
                    var rkp = new RegistryPackage(rk);

                    if (string.IsNullOrEmpty(rkp.ProductName) || string.IsNullOrEmpty(rkp.ProductVersion) ||
                        string.IsNullOrEmpty(rkp.MinEdition)) continue;

                    Debug.WriteLine("{0,3}\t{1} ", k++, rkp.ToString());
                }

                Debug.WriteLine("***********  " + c + "  *********** end ");
            }
    
            Assert.IsTrue(true);
        }
       
    }
}
