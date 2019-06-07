namespace DMS.DataGateway.Migrations
{
    using CsvHelper;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Model;
    using System.Data.Entity.SqlServer;
    using System.Data.Entity.Validation;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<DMS.DataGateway.DataModels.DMSDBContext>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "DMS.DataGateway.DataModels.DMSDBContext";
            SetSqlGenerator("System.Data.SqlClient", new CustomSqlServerMigrationSqlGenerator());
        }

        protected override void Seed(DMS.DataGateway.DataModels.DMSDBContext context)
        {
            //// Uncomment below code to debug while running Update-Database Command
            if (!System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Launch();

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string provinces = "DMS.DataGateway.SeedData.Provinces.csv";
                string cities = "DMS.DataGateway.SeedData.Cities.csv";
                string postalCodes = "DMS.DataGateway.SeedData.PostalCodes.csv";
                string subdistricts = "DMS.DataGateway.SeedData.SubDistricts.csv";
                string imageTypes = "DMS.DataGateway.SeedData.ImageTypes.csv";
                string businessAreas = "DMS.DataGateway.SeedData.BusinessAreas.csv";
                string partnerTypes = "DMS.DataGateway.SeedData.PartnerTypes.csv";
                string tripStatuses = "DMS.DataGateway.SeedData.TripStatus.csv";

                using (Stream stream = assembly.GetManifestResourceStream(provinces))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var provincesData = csvReader.GetRecords<DMS.DataGateway.DataModels.Province>().ToArray();
                        context.Provinces.AddOrUpdate(c => c.ProvinceCode, provincesData);
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
                            context.Cities.AddOrUpdate(c => c.CityCode, new DMS.DataGateway.DataModels.City
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
                            context.SubDistricts.AddOrUpdate(c => c.SubDistrictCode, new DMS.DataGateway.DataModels.SubDistrict
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
                        foreach (DMS.DataGateway.DataModels.PostalCode postalCode in postalCodesData)
                        {
                            context.PostalCodes.AddOrUpdate(c => c.PostalCodeNo, new DMS.DataGateway.DataModels.PostalCode
                            {
                                PostalCodeNo = postalCode.PostalCodeNo,
                                SubDistrictID = postalCode.SubDistrictID
                            });
                        }
                    }
                }
                context.SaveChanges();

                using (Stream stream = assembly.GetManifestResourceStream(imageTypes))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var imageTypesData = csvReader.GetRecords<DMS.DataGateway.DataModels.ImageType>().ToArray();
                        context.ImageTypes.AddOrUpdate(c => c.ImageTypeCode, imageTypesData);
                    }
                }
                context.SaveChanges();

                using (Stream stream = assembly.GetManifestResourceStream(businessAreas))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var businessAreasData = csvReader.GetRecords<BusinessAreaSeed>().ToArray();
                        foreach (BusinessAreaSeed businessArea in businessAreasData)
                        {
                            context.BusinessAreas.AddOrUpdate(c => c.BusinessAreaCode, new DMS.DataGateway.DataModels.BusinessArea
                            {
                                BusinessAreaCode = businessArea.BusinessAreaCode,
                                BusinessAreaDescription = businessArea.BusinessAreaDescription
                            });
                        }
                    }
                }
                context.SaveChanges();

                using (Stream stream = assembly.GetManifestResourceStream(partnerTypes))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var partnerTypesData = csvReader.GetRecords<DMS.DataGateway.DataModels.PartnerType>().ToArray();
                        context.PartnerTypes.AddOrUpdate(c => c.PartnerTypeCode, partnerTypesData);
                    }
                }
                context.SaveChanges();

                using (Stream stream = assembly.GetManifestResourceStream(tripStatuses))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var tripStatusData = csvReader.GetRecords<DMS.DataGateway.DataModels.TripStatus>().ToArray();
                        context.TripStatuses.AddOrUpdate(c => c.StatusCode, tripStatusData);
                    }
                }
                context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
            catch (Exception ex)
            {
                throw ex;
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

        public class BusinessAreaSeed
        {
            public string BusinessAreaCode { get; set; }
            public string BusinessAreaDescription { get; set; }
        }

        internal class CustomSqlServerMigrationSqlGenerator : SqlServerMigrationSqlGenerator
        {
            protected override void Generate(AddColumnOperation addColumnOperation)
            {
                SetCreatedUtcColumn(addColumnOperation.Column);

                base.Generate(addColumnOperation);
            }

            protected override void Generate(CreateTableOperation createTableOperation)
            {
                SetCreatedUtcColumn(createTableOperation.Columns);

                base.Generate(createTableOperation);
            }

            private static void SetCreatedUtcColumn(IEnumerable<ColumnModel> columns)
            {
                foreach (var columnModel in columns)
                {
                    SetCreatedUtcColumn(columnModel);
                }
            }

            private static void SetCreatedUtcColumn(PropertyModel column)
            {
                if (column.Name == "CreatedTime")
                {
                    column.DefaultValueSql = "GETDATE()";
                }
                if (column.Name == "CreatedBy")
                {
                    column.DefaultValue = "SYSTEM";
                }
            }
        }
    }
}
