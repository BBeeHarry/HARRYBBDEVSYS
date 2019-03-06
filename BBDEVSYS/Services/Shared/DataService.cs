
using BBDEVSYS.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBDEVSYS.Services.Shared
{
	public static class DataService
	{
		public static List<DataPoint> GetRandomDataForNumericAxis(int count)
		{
			decimal y = 50;
			_dataPoints = new List<DataPoint>();


			for (int i = 0; i < count; i++)
			{
				y = y + (random.Next(0, 20) - 10);

				_dataPoints.Add(new DataPoint(i, y));
			}

			return _dataPoints;
		}

		public static List<DataPoint> GetRandomDataForCategoryAxis(int count)
		{
			decimal y = 50;
			DateTime dateTime = new DateTime(2006, 01, 1, 0, 0, 0);
			string label = "";

			_dataPoints = new List<DataPoint>();


			for (int i = 0; i < count; i++)
			{
				y = y + (random.Next(0, 20) - 10);
				label = dateTime.ToString("dd MMM");

				_dataPoints.Add(new DataPoint(y, label));
				dateTime = dateTime.AddDays(1);
			}

			return _dataPoints;
		}

		public static List<DataPoint> GetRandomDataForDateTimeAxis(int count)
		{
            decimal x = 0;
			decimal y = 50;

			var dateTime = new DateTime(2006, 01, 10, 0, 0, 0, DateTimeKind.Local);
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			_dataPoints = new List<DataPoint>();


			for (int i = 0; i < count; i++)
			{
				y = y + (random.Next(0, 20) - 10);

				x =Convert.ToDecimal( dateTime.ToUniversalTime().Subtract(epoch).TotalMilliseconds );


				_dataPoints.Add(new DataPoint(x, y));
				dateTime = dateTime.AddDays(1);
				//dateTime = dateTime.AddHours(1);
			}

			return _dataPoints;
		}

		private static Random random = new Random(DateTime.Now.Millisecond);
		private static List<DataPoint> _dataPoints;
	}
}