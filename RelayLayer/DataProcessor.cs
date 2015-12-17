using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelayLayer
{
    static class DataProcessor
    {
        /// <summary>
        /// Averages all temperature and light data for 2 sensors
        /// </summary>
        /// <param name="dataSet">A list of multiple sensor data over 1 second from 2 sensors</param>
        /// <returns></returns>
        public static DataModel[] AverageDataSet(List<DataModel[]> dataSet)
        {
            int tempOvenSum = 0;

            int tempRoomSum = 0;
            int lightRoomSum = 0;

            int nrOfData = dataSet.Count;

            foreach (DataModel[] datas in dataSet)
            {
                tempOvenSum += datas[0].Temperature;

                tempRoomSum += datas[1].Temperature;
                lightRoomSum += datas[1].Light;
            }

            int avgOvenTemp = tempOvenSum / nrOfData;

            int avgRoomTemp = tempRoomSum / nrOfData;
            int avgRoomLight = lightRoomSum / nrOfData;

            DataModel averagedOvenData = new DataModel()
            {
                Light = 0,
                Temperature = avgOvenTemp,
                SensorName = dataSet[0][0].SensorName,
                TimeOfData = dataSet[0][0].TimeOfData
            };
            DataModel averagedRoomData = new DataModel()
            {
                Light = avgRoomTemp,
                Temperature = avgRoomLight,
                SensorName = dataSet[0][1].SensorName,
                TimeOfData = dataSet[0][1].TimeOfData
            };

            DataModel[] averagedData = new DataModel[2] { averagedOvenData, averagedRoomData };

            return averagedData;
        }
    }
}
