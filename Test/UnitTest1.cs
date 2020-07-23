using Microsoft.VisualStudio.TestTools.UnitTesting;
using OriginsRx.Business.Tools;
using System.Diagnostics;
using OriginsRx.Business.Services;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var cSheet = new Sheets();
            var file = cSheet.ReadSheetFromDisk(@"C:\Core\OriginsRx\Business\Sample\sample sales file.xlsx");
            var retVal = cSheet.ParseHeaders(file);

            foreach (string header in retVal)
            {
                Debug.WriteLine(header);
            }
        }

        [TestMethod]
        public void TestMethod1DB()
        {
            var cSheet = new Sheets();

            cSheet.CreateMapDataTable(1);
        }

        [TestMethod]
        public void TestGetMasterMap()
        {
            var db = OriginsRx.Global.GetDb();

            var service = new MapService
                (OriginsRx.Business.AutoMapper.GetAutoMapper(),db);
                
  //          var result = service.Get(1).GetAwaiter().GetResult();
        }
    }
}
