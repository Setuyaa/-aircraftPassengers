using System;
namespace AircraftPassengers
{
	public class Passenger
	{
		public string nameSurname = "";
		public string dateOfBirth;
		public string cityTo;
		public int luggage;
		public long password;
		public string food;
		public Passenger()
		{
            Random rnd = new Random();
            var randomName = rnd.Next(0, Enum.GetNames(typeof(ENames)).Length);
            var randomSurname = rnd.Next(0, Enum.GetNames(typeof(ESurnames)).Length);
			string name_ = ((ENames)randomName).ToString();
			string surname_ = ((ESurnames)randomSurname).ToString();
			nameSurname = nameSurname.Insert(0,name_ + " " + surname_);
			int day = rnd.Next(1,29);
            int month = rnd.Next(1, 13);
            int year = rnd.Next(1999, 2017);
            string dayS = day.ToString();
			if (dayS.Length == 1)
				dayS = dayS.Insert(0,"0");
            string monthS = month.ToString();
            if (monthS.Length == 1)
                monthS = monthS.Insert(0, "0");
			string yearS = year.ToString();
			dateOfBirth = dayS + "/" + monthS + "/" + yearS;
			var randomTo = rnd.Next(Enum.GetNames(typeof(EAirportCode)).Length);
			cityTo = ((EAirportCode)randomTo).ToString();
			luggage = rnd.Next(2);
			password = rnd.NextInt64(1000000000, 10000000000);
			int tfood = rnd.Next(2);
			if (tfood == 0) food = "fish";
			else food = "meat";
        }
    }
}

