using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace DataValidation
{
    class Record : IComparable<Record>
    {
        public string fullName { get; set; }
        public string cityName { get; set; }
        public string phoneNumber { get; set; }
        public string emailAddress { get; set; }
        public bool phoneValid { get; set; }
        public bool emailValid { get; set; }

        public Record()
        {
            phoneValid = true;
            emailValid = true;
        }

        public int CompareTo(Record other)
        {
            return fullName.CompareTo(other.fullName);
        }
    }

    class City : IComparable<City>
    {
        public string cityName { get; set; }
        public int invalidCount { get; set; }

        public City(string name)
        {
            cityName = name;
            invalidCount = 0;
        }

        public int CompareTo(City other)
        {
            return -1 * invalidCount.CompareTo(other.invalidCount);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Record> records = GetInputData("Contacts.json");
            records.Sort();

            ValidatePhones(records);
            ValidateEmails(records);

            ListRecords(records);
            Console.ReadKey();

            List<City> cities = new List<City>();
            CreateCitiesList(cities, records);
            GetCityInvalidCount(cities, records);
            cities.Sort();
            ListCities(cities);

            Console.ReadKey();
        }

        private static void ListCities(List<City> cities)
        {
            foreach (City c in cities)
            {
                Console.WriteLine($"{c.cityName} has {c.invalidCount} validation errors.");
            };
        }

        private static void GetCityInvalidCount(List<City> cities, List<Record> records)
        {
            foreach (Record r in records)
            {
                int index = cities.IndexOf(cities.Where(item => item.cityName == r.cityName).FirstOrDefault());

                if (!r.phoneValid)
                {
                    cities[index].invalidCount++;
                }

                if (!r.emailValid)
                {
                    cities[index].invalidCount++;
                }
            };
        }

        private static void CreateCitiesList(List<City> cities, List<Record> records)
        {
            foreach (Record r in records)
            {
                City city = new City(r.cityName);
                bool alreadyListed = cities.Any(item => item.cityName == city.cityName);
                if (!alreadyListed)
                {
                    cities.Add(city);
                }
            }
        }

        private static void ListRecords(List<Record> records)
        {
            foreach (Record r in records)
            {
                string message;
                if (r.phoneValid && r.emailValid) message = $"{r.fullName}: Valid";
                else if (!r.phoneValid && !r.emailValid) message = $"{r.fullName}: Email and Phone are invalid.";
                else if (!r.phoneValid) message = $"{r.fullName}: Phone is invalid.";
                else if (!r.emailValid) message = $"{r.fullName}: Email is invalid.";
                else message = $"{r.fullName}: It's all gone wrong!";

                Console.WriteLine(message);
            };
        }

        private static void ValidateEmails(List<Record> records)
        {
            foreach (Record r in records)
            {
                foreach (Char c in r.phoneNumber)
                {
                    if (!Char.IsNumber(c) && c != '-' && c != ' ')
                    {
                        r.phoneValid = false;
                        break;
                    }
                }
            }
        }

        private static void ValidatePhones(List<Record> records)
        {
            foreach (Record r in records)
            {
                if (r.emailAddress.Count(f => f == '@') != 1)
                {
                    r.emailValid = false;
                    continue;
                }

                if (r.emailAddress[0] == '@' || r.emailAddress.Length == r.emailAddress.IndexOf('@'))
                {
                    r.emailValid = false;
                    continue;
                }
            }
        }

        private static List<Record> GetInputData(string path)
        {
            string fileText = File.ReadAllText(@path);
            return JsonConvert.DeserializeObject<List<Record>>(fileText);
        }
    }
}
