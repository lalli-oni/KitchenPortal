using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFServiceWebRole1.Model;

namespace WCFServiceWebRole1.DBInterface
{
    interface DBInterface
    {
        Task<bool> InsertData(DataModel data);

        Task<bool> CheckTemperatureReminder(int temperature);
        DataModel RetrieveLastData();
    }
}
