namespace TMS.DataGateway.Migrations
{
    using CsvHelper;
    using EntityFramework.Seeder;
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
    using DataModel = TMS.DataGateway.DataModels;

    internal sealed class Configuration : DbMigrationsConfiguration<TMS.DataGateway.DataModels.TMSDBContext>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "TMS.DataGateway.DataModels.TMSDBContext";
            SetSqlGenerator("System.Data.SqlClient", new CustomSqlServerMigrationSqlGenerator());
        }

        protected override void Seed(TMS.DataGateway.DataModels.TMSDBContext context)
        {
            //// Uncomment below code to debug while running Update-Database Command
            //if (!System.Diagnostics.Debugger.IsAttached)
            //    System.Diagnostics.Debugger.Launch();

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string applications = "TMS.DataGateway.SeedData.applications.csv";
                string menus = "TMS.DataGateway.SeedData.Menus.csv";
                string activities = "TMS.DataGateway.SeedData.Activities.csv";
                string menuActivities = "TMS.DataGateway.SeedData.MenuActivities.csv";
                string provinces = "TMS.DataGateway.SeedData.Provinces.csv";
                string companies = "TMS.DataGateway.SeedData.Companies.csv";
                string cities = "TMS.DataGateway.SeedData.Cities.csv";
                string postalCodes = "TMS.DataGateway.SeedData.PostalCodes.csv";
                string subdistricts = "TMS.DataGateway.SeedData.SubDistricts.csv";
                string businessAreas = "TMS.DataGateway.SeedData.BusinessAreas.csv";
                string orderStatuses = "TMS.DataGateway.SeedData.OrderStatus.csv";
                string fleetTypes = "TMS.DataGateway.SeedData.FleetTypes.csv";
                string partnerTypes = "TMS.DataGateway.SeedData.PartnerTypes.csv";
                string vehicleTypes = "TMS.DataGateway.SeedData.VehicleTypes.csv";

                using (Stream stream = assembly.GetManifestResourceStream(applications))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var applicationData = csvReader.GetRecords<DataModel.Application>().ToArray();
                        context.Applications.AddOrUpdate(c => c.ID, applicationData);
                    }
                }

                #region "menus, activities & menuactivities"

                using (Stream stream = assembly.GetManifestResourceStream(menus))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var menuData = csvReader.GetRecords<DataModel.Menu>().ToArray();
                        context.Menus.AddOrUpdate(c => c.ID, menuData);
                    }
                }

                using (Stream stream = assembly.GetManifestResourceStream(activities))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var activitiesData = csvReader.GetRecords<DataModel.Activity>().ToArray();
                        context.Activities.AddOrUpdate(c => c.ID, activitiesData);
                    }
                }

                context.SaveChanges();

                using (Stream stream = assembly.GetManifestResourceStream(menuActivities))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var menuActivitiesData = csvReader.GetRecords<DataModel.MenuActivity>().ToArray();
                        foreach (DataModel.MenuActivity menuActivity in menuActivitiesData)
                        {
                            context.MenuActivities.AddOrUpdate(c => c.ID, new DataModel.MenuActivity
                            {
                                MenuID = menuActivity.MenuID,
                                ActivityID = menuActivity.ActivityID
                            });
                        }
                    }
                }

                #endregion

                #region "provinces, cities, subdistricts & postalcode"

                using (Stream stream = assembly.GetManifestResourceStream(provinces))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var provincesData = csvReader.GetRecords<DataModel.Province>().ToArray();
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
                            context.Cities.AddOrUpdate(c => c.ID, new DataModel.City
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
                            context.SubDistricts.AddOrUpdate(c => c.ID, new DataModel.SubDistrict
                            {
                                SubdistrictCode = subDistrict.SubdistrictCode,
                                SubdistrictName = subDistrict.SubdistrictName,
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
                        var postalCodesData = csvReader.GetRecords<DataModel.PostalCode>().ToArray();
                        foreach (DataModel.PostalCode postalCode in postalCodesData)
                        {
                            context.PostalCodes.AddOrUpdate(c => c.ID, new DataModel.PostalCode
                            {
                                PostalCodeNo = postalCode.PostalCodeNo,
                                SubDistrictID = postalCode.SubDistrictID
                            });
                        }
                    }
                }

                #endregion

                #region "companies & businessareas"

                using (Stream stream = assembly.GetManifestResourceStream(companies))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var companiesData = csvReader.GetRecords<DataModel.CompanyCode>().ToArray();
                        context.CompanyCodes.AddOrUpdate(c => c.ID, companiesData);
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
                            context.BusinessAreas.AddOrUpdate(c => c.ID, new DataModel.BusinessArea
                            {
                                BusinessAreaCode = businessArea.BusinessAreaCode,
                                BusinessAreaDescription = businessArea.BusinessAreaDescription,
                                CompanyCodeID = context.CompanyCodes.Where(p => p.CompanyCodeCode == businessArea.CompanyCodeCode).Select(p => p.ID).FirstOrDefault()
                            });
                        }
                    }
                }

                #endregion

                using (Stream stream = assembly.GetManifestResourceStream(orderStatuses))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var orderStatusData = csvReader.GetRecords<DataModel.OrderStatus>().ToArray();
                        context.OrderStatuses.AddOrUpdate(c => c.ID, orderStatusData);
                    }
                }

                using (Stream stream = assembly.GetManifestResourceStream(fleetTypes))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var fleetTypeData = csvReader.GetRecords<DataModel.FleetType>().ToArray();
                        context.FleetTypes.AddOrUpdate(c => c.ID, fleetTypeData);
                    }
                }

                using (Stream stream = assembly.GetManifestResourceStream(partnerTypes))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var partnerTypeData = csvReader.GetRecords<DataModel.PartnerType>().ToArray();
                        context.PartnerTypes.AddOrUpdate(c => c.ID, partnerTypeData);
                    }
                }

                using (Stream stream = assembly.GetManifestResourceStream(vehicleTypes))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        CsvReader csvReader = new CsvReader(reader);
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        var vehicleTypeData = csvReader.GetRecords<DataModel.VehicleType>().ToArray();
                        context.VehicleTypes.AddOrUpdate(c => c.ID, vehicleTypeData);
                    }
                }

                // Uncomment below line while debugging seed method
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
            public string CompanyCodeCode { get; set; }
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
            }
        }
    }
}
