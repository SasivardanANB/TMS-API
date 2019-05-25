namespace DMS.DataGateway.Migrations
{
    using CsvHelper;
    using NLog;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<DMS.DataGateway.DataModels.DMSDBContext>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "DMS.DataGateway.DataModels.DMSDBContext";
        }

        protected override void Seed(DMS.DataGateway.DataModels.DMSDBContext context)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string provinces = "DMS.DataGateway.SeedData.Provinces.csv";
                string cities = "DMS.DataGateway.SeedData.Cities.csv";
                string postalCodes = "DMS.DataGateway.SeedData.PostalCodes.csv";
                string subdistricts = "DMS.DataGateway.SeedData.SubDistricts.csv";


                using (Stream stream = assembly.GetManifestResourceStream(provinces))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var provincesData = csvReader.GetRecords<DMS.DataGateway.DataModels.Province>().ToArray();
                        context.Provinces.AddOrUpdate(c => c.ID, provincesData);
                    }
                }


                context.SaveChanges();

                using (Stream stream = assembly.GetManifestResourceStream(cities))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var citiesData = csvReader.GetRecords<CitySeed>().ToArray();
                        foreach (CitySeed city in citiesData)
                        {
                            context.Cities.AddOrUpdate(c => c.ID, new DMS.DataGateway.DataModels.City
                            {
                                CityCode = city.CityCode,
                                CityDescription = city.CityName,
                                ProvinceID = context.Provinces.Where(p => p.ProvinceCode == city.ProvinceCode).Select(p => p.ID).FirstOrDefault()
                            });
                        }
                    }
                }

                context.SaveChanges();

                using (Stream stream = assembly.GetManifestResourceStream(subdistricts))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var subdistrictsData = csvReader.GetRecords<SubDistrictSeed>().ToArray();
                        foreach (SubDistrictSeed subDistrict in subdistrictsData)
                        {
                            context.SubDistricts.AddOrUpdate(c => c.ID, new DMS.DataGateway.DataModels.SubDistrict
                            {
                                SubDistrictCode = subDistrict.SubdistrictCode,
                                SubDistrictName = subDistrict.SubdistrictName,
                                CityID = context.Cities.Where(p => p.CityDescription == subDistrict.CityName).Select(p => p.ID).FirstOrDefault()
                            });
                        }
                    }
                }

                context.SaveChanges();

                using (Stream stream = assembly.GetManifestResourceStream(postalCodes))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var postalCodesData = csvReader.GetRecords<DMS.DataGateway.DataModels.PostalCode>().ToArray();
                        context.PostalCodes.AddOrUpdate(c => c.ID, postalCodesData);
                    }
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
            }
        }

        public class CitySeed
        {
            public string ProvinceCode { get; set; }
            public string CityCode { get; set; }
            public string CityName { get; set; }
        }

        public class SubDistrictSeed
        {
            public string SubdistrictCode { get; set; }
            public string SubdistrictName { get; set; }
            public string CityName { get; set; }
        }
    }
}
