using Domain.Entities;
using Framework;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace V2.Application.Cities
{
    public class CityManager : ICityManager
    {
        private readonly IRepository<City> _cityRepository;

        public CityManager(IRepository<City> cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public void PopulateDatabase()
        {
            var cityList = _cityRepository.Query().ToList();

            using (var sr = new StringReader(Domain.Resources.AppResources.uf_cidade))
            {
                string line;

                while (!string.IsNullOrEmpty(line = sr.ReadLine()))
                {
                    var split = line.Split(',');
                    var name = split[0];
                    var state = split[1];

                    Console.WriteLine($"{name} - {state}");

                    var city = cityList.FirstOrDefault(i => i.Name == name && i.State == state);

                    if (city == null)
                    {
                        city = new City
                        {
                            Name = name,
                            State = state
                        };

                        SetCityLocation(city, split[2], split[3]);

                        _cityRepository.Insert(city);

                        cityList.Add(city);
                    }
                    else if (!city.HasLocation)
                    {
                        SetCityLocation(city, split[2], split[3]);

                        _cityRepository.Update(city);
                    }
                }
            }

            RemoveItems();

            _cityRepository.SaveChanges();
        }

        private void SetCityLocation(City city, string lat, string lng)
        {
            var parsedLat = Convert.ToDouble(lat, CultureInfo.InvariantCulture);
            var parsedLng = Convert.ToDouble(lng, CultureInfo.InvariantCulture);

            city.SetLocation(parsedLat, parsedLng);
        }

        private void RemoveItems()
        {
            var itemsToRemove = _cityRepository.Query()
                .Where(x => x.Name == "São Paulo" && x.State == "PR")
                .ToList();

            _cityRepository.DeleteRange(itemsToRemove);
        }
    }
}