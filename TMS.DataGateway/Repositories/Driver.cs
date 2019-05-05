﻿using AutoMapper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TMS.DataGateway.DataModels;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using Domain = TMS.DomainObjects.Objects;
using DataModel = TMS.DataGateway.DataModels;


namespace TMS.DataGateway.Repositories
{
    public class Driver : IDriver
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public DriverResponse CreateUpdateDriver(DriverRequest driverRequest)
        {
            DriverResponse driverResponse = new DriverResponse();
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Domain.Driver, DataModel.Driver>().ReverseMap();
                    });

                    IMapper mapper = config.CreateMapper();
                    var drivers = mapper.Map<List<Domain.Driver>, List<DataModel.Driver>>(driverRequest.Requests);
                    foreach (var driverData in drivers)
                    {
                        //For encrypt password
                        if (!string.IsNullOrEmpty(driverData.Password))
                        {
                            driverData.Password = Encryption.EncryptionLibrary.EncryptPassword(driverData.Password);
                        }
                        //For making IsDelete column false
                        driverData.IsDelete = false;
                        //For update driver
                        if (driverData.ID > 0)
                        {
                            tMSDBContext.Entry(driverData).State = System.Data.Entity.EntityState.Modified;
                            tMSDBContext.SaveChanges();
                            driverResponse.StatusMessage = DomainObjects.Resource.ResourceData.DriversUpdated;
                        }
                        //For create driver
                        else
                        {
                            tMSDBContext.Drivers.Add(driverData);
                            tMSDBContext.SaveChanges();
                            driverResponse.StatusMessage = DomainObjects.Resource.ResourceData.DriversCreated;
                        }
                    }
                    driverRequest.Requests = mapper.Map<List<DataModel.Driver>, List<Domain.Driver>>(drivers);
                    driverResponse.Data = driverRequest.Requests;
                    driverResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    driverResponse.StatusCode = (int)HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                driverResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                driverResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                driverResponse.StatusMessage = ex.Message;
            }
            return driverResponse;
        }

        public DriverResponse DeleteDriver(int driverID)
        {
            DriverResponse driverResponse = new DriverResponse();
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    var driverDetails = tMSDBContext.Drivers.Where(i => i.ID == driverID).FirstOrDefault();
                    if (driverDetails != null)
                    {
                        //For deleting driver (soft delete)
                        driverDetails.IsDelete = true;
                        tMSDBContext.SaveChanges();
                    }
                    driverResponse.StatusCode = (int)HttpStatusCode.OK;
                    driverResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    driverResponse.StatusMessage = DomainObjects.Resource.ResourceData.DriverDeleted;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                driverResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                driverResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                driverResponse.StatusMessage = ex.Message;
            }
            return driverResponse;
        }

        public DriverResponse GetDrivers(DriverRequest driverRequest)
        {
            DriverResponse driverResponse = new DriverResponse();
            List<Domain.Driver> driversList;
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    driversList = tMSDBContext.Drivers.Where(d => !d.IsDelete).Select(driver => new Domain.Driver
                    {
                        ID = driver.ID,
                        DriverAddress = driver.DriverAddress,
                        FirstName = driver.FirstName,
                        LastName = driver.LastName,
                        DriverNo = driver.DriverNo,
                        DriverPhone = driver.DriverPhone,
                        Email = driver.Email,
                        IsActive = driver.IsActive,
                        IdentityNo = driver.IdentityNo,
                        DrivingLicenseExpiredDate = driver.DrivingLicenseExpiredDate,
                        DriverImageId = driver.DriverImageId,
                        DrivingLicenseNo = driver.DrivingLicenseNo,
                        DrivingLicenceImageId = driver.DrivingLicenceImageId,
                        IdentityImageId = driver.IdentityImageId,
                        DriverImageGuId = tMSDBContext.ImageGuids.Where(d => d.ID == driver.DriverImageId).Select(g => g.ImageGuIdValue).FirstOrDefault(),
                        DrivingLicenceImageGuId = tMSDBContext.ImageGuids.Where(d => d.ID == driver.DrivingLicenceImageId).Select(g => g.ImageGuIdValue).FirstOrDefault(),
                        IdentityImageGuId = tMSDBContext.ImageGuids.Where(d => d.ID == driver.IdentityImageId).Select(g => g.ImageGuIdValue).FirstOrDefault(),
                    }).ToList();
                }

                // Filter
                if (driverRequest.Requests.Count > 0)
                {
                    var driverFilter = driverRequest.Requests[0];

                    if (driverFilter.ID > 0)
                    {
                        driversList = driversList.Where(s => s.ID == driverFilter.ID).ToList();
                    }

                    if (!String.IsNullOrEmpty(driverFilter.DriverNo))
                    {
                        driversList = driversList.Where(s => s.DriverNo.Contains(driverFilter.DriverNo)).ToList();
                    }

                    if (!String.IsNullOrEmpty(driverFilter.FirstName))
                    {
                        driversList = driversList.Where(s => s.FirstName.Contains(driverFilter.FirstName)).ToList();
                    }

                    if (!String.IsNullOrEmpty(driverFilter.LastName))
                    {
                        driversList = driversList.Where(s => s.LastName.Contains(driverFilter.LastName)).ToList();
                    }

                    if (!String.IsNullOrEmpty(driverFilter.Email))
                    {
                        driversList = driversList.Where(s => s.Email.Contains(driverFilter.Email)).ToList();
                    }

                    if (!String.IsNullOrEmpty(driverFilter.DriverPhone))
                    {
                        driversList = driversList.Where(s => s.DriverPhone.Contains(driverFilter.DriverPhone)).ToList();
                    }

                    if (!String.IsNullOrEmpty(driverFilter.DriverAddress))
                    {
                        driversList = driversList.Where(s => s.DriverAddress.Contains(driverFilter.DriverAddress)).ToList();
                    }

                    if (!String.IsNullOrEmpty(driverFilter.IdentityNo))
                    {
                        driversList = driversList.Where(s => s.IdentityNo.Contains(driverFilter.IdentityNo)).ToList();
                    }

                    if (!String.IsNullOrEmpty(driverFilter.DrivingLicenseNo))
                    {
                        driversList = driversList.Where(s => s.DrivingLicenseNo.Contains(driverFilter.DrivingLicenseNo)).ToList();
                    }

                    if (!String.IsNullOrEmpty(driverFilter.DrivingLicenseExpiredDate.ToString()))
                    {
                        driversList = driversList.Where(s => s.DrivingLicenseExpiredDate.ToString().Contains(driverFilter.DrivingLicenseExpiredDate.ToString())).ToList();
                    }

                    if (driverFilter.IsActive != null)
                    {
                        driversList = driversList.Where(s => s.IsActive == driverFilter.IsActive).ToList();
                    }
                }

                // Sorting
                switch (driverRequest.SortOrder.ToLower())
                {
                    case "firstname":
                        driversList = driversList.OrderBy(s => s.FirstName).ToList();
                        break;
                    case "firstname_desc":
                        driversList = driversList.OrderByDescending(s => s.FirstName).ToList();
                        break;
                    case "lastname":
                        driversList = driversList.OrderBy(s => s.LastName).ToList();
                        break;
                    case "lastname_desc":
                        driversList = driversList.OrderByDescending(s => s.LastName).ToList();
                        break;
                    case "phone":
                        driversList = driversList.OrderBy(s => s.DriverPhone).ToList();
                        break;
                    case "phone_desc":
                        driversList = driversList.OrderByDescending(s => s.DriverPhone).ToList();
                        break;
                    case "email":
                        driversList = driversList.OrderBy(s => s.Email).ToList();
                        break;
                    case "email_desc":
                        driversList = driversList.OrderByDescending(s => s.Email).ToList();
                        break;
                    case "isactive":
                        driversList = driversList.OrderBy(s => s.IsActive).ToList();
                        break;
                    case "isactive_desc":
                        driversList = driversList.OrderByDescending(s => s.IsActive).ToList();
                        break;
                    case "drivernumber":
                        driversList = driversList.OrderBy(s => s.DriverNo).ToList();
                        break;
                    case "drivernumber_desc":
                        driversList = driversList.OrderByDescending(s => s.DriverNo).ToList();
                        break;
                    case "address":
                        driversList = driversList.OrderBy(s => s.DriverAddress).ToList();
                        break;
                    case "address_desc":
                        driversList = driversList.OrderByDescending(s => s.DriverAddress).ToList();
                        break;
                    case "identitynumber":
                        driversList = driversList.OrderBy(s => s.IdentityNo).ToList();
                        break;
                    case "identitynumber_desc":
                        driversList = driversList.OrderByDescending(s => s.IdentityNo).ToList();
                        break;
                    case "drivinglicensenumber":
                        driversList = driversList.OrderBy(s => s.DrivingLicenseNo).ToList();
                        break;
                    case "drivinglicensenumber_desc":
                        driversList = driversList.OrderByDescending(s => s.DrivingLicenseNo).ToList();
                        break;
                    case "drivinglicenseexpiredate":
                        driversList = driversList.OrderBy(s => s.DrivingLicenseExpiredDate).ToList();
                        break;
                    case "drivinglicenseexpiredate_desc":
                        driversList = driversList.OrderByDescending(s => s.DrivingLicenseExpiredDate).ToList();
                        break;
                    default:  // ID Descending 
                        driversList = driversList.OrderByDescending(s => s.ID).ToList();
                        break;
                }

                // Paging
                int pageSize = driverRequest.PageSize.Value;
                int pageNumber = (driverRequest.PageNumber ?? 1);
                driversList = driversList.Skip(pageNumber - 1 * pageSize).Take(pageSize).ToList();

                if (driversList.Count > 0)
                {
                    driverResponse.Data = driversList;
                    driverResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    driverResponse.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    driverResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                    driverResponse.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                driverResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                driverResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                driverResponse.StatusMessage = ex.Message;
            }
            return driverResponse;
        }
    }
}
