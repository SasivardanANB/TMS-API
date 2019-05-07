namespace TMS.DataGateway.Migrations
{
    using CsvHelper;
    using EntityFramework.Seeder;
    using NLog;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
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
                string orderStatus = "OMS.DataGateway.SeedData.OrderStatus.csv";

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
                        var citiesData = csvReader.GetRecords<DataModel.City>().ToArray();
                        foreach (DataModel.City city in citiesData)
                        {
                            context.Cities.AddOrUpdate(c => c.ID, new DataModel.City
                            {
                                CityCode = city.CityCode,
                                CityDescription = city.CityDescription,
                                ProvinceID = city.ProvinceID
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
                        var subdistrictsData = csvReader.GetRecords<DataModel.SubDistrict>().ToArray();
                        foreach (DataModel.SubDistrict subDistrict in subdistrictsData)
                        {
                            context.SubDistricts.AddOrUpdate(c => c.ID, new DataModel.SubDistrict
                            {
                                SubdistrictCode = subDistrict.SubdistrictCode,
                                SubdistrictName = subDistrict.SubdistrictName,
                                CityID = subDistrict.CityID
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
                        var businessAreasData = csvReader.GetRecords<DataModel.BusinessArea>().ToArray();
                        foreach (DataModel.BusinessArea businessArea in businessAreasData)
                        {
                            context.BusinessAreas.AddOrUpdate(c => c.ID, new DataModel.BusinessArea
                            {
                                BusinessAreaCode = businessArea.BusinessAreaCode,
                                BusinessAreaDescription = businessArea.BusinessAreaDescription,
                                CompanyCodeID = businessArea.CompanyCodeID,
                                Address = businessArea.Address,
                                PostalCodeID = businessArea.PostalCodeID
                            });
                        }
                    }
                }

                #endregion

                using (Stream stream = assembly.GetManifestResourceStream(orderStatus))
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
    }
}
