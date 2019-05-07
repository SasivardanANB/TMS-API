using AutoMapper;
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
    public class Pool : IPool
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PoolResponse CreateUpdatePool(PoolRequest poolRequest)
        {
            PoolResponse poolResponse = new PoolResponse();
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Domain.Pool, DataModel.Pool>().ReverseMap();
                    });

                    IMapper mapper = config.CreateMapper();
                    var pools = mapper.Map<List<Domain.Pool>, List<DataModel.Pool>>(poolRequest.Requests);
                    int poolObjectCount = 0;
                    foreach (var poolData in pools)
                    {
                        //For making ISDelete initially false
                        poolData.IsDelete = false;
                        poolData.PhotoId = InsertImageGuid(poolRequest.Requests[poolObjectCount].PhotoGuId, poolRequest.CreatedBy);
                        //For update pool
                        if (poolData.ID > 0)
                        {
                            poolData.LastModifiedBy = poolRequest.LastModifiedBy;
                            poolData.LastModifiedTime = DateTime.Now;
                            tMSDBContext.Entry(poolData).State = System.Data.Entity.EntityState.Modified;
                            tMSDBContext.SaveChanges();
                            poolResponse.StatusMessage = DomainObjects.Resource.ResourceData.PoolsUpdated;
                        }
                        //For create pool
                        else
                        {
                            poolData.CreatedBy = poolRequest.CreatedBy;
                            poolData.CreatedTime = DateTime.Now;
                            tMSDBContext.Pools.Add(poolData);
                            tMSDBContext.SaveChanges();
                            poolResponse.StatusMessage = DomainObjects.Resource.ResourceData.PoolsCreated;
                        }
                        poolObjectCount++;
                    }
                    poolRequest.Requests = mapper.Map<List<DataModel.Pool>, List<Domain.Pool>>(pools);
                    poolResponse.Data = poolRequest.Requests;
                    poolResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    poolResponse.StatusCode = (int)HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                poolResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                poolResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                poolResponse.StatusMessage = ex.Message;
            }
            return poolResponse;
        }

        public PoolResponse DeletePool(int poolID)
        {
            PoolResponse poolResponse = new PoolResponse();
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    var poolDetails = tMSDBContext.Pools.Where(i => i.ID == poolID).FirstOrDefault();
                    if (poolDetails != null)
                    {
                        poolDetails.IsDelete = true;
                        tMSDBContext.SaveChanges();
                    }
                    poolResponse.StatusCode = (int)HttpStatusCode.OK;
                    poolResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    poolResponse.StatusMessage = DomainObjects.Resource.ResourceData.PoolDeleted;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                poolResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                poolResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                poolResponse.StatusMessage = ex.Message;
            }
            return poolResponse;
        }

        public PoolResponse GetPools(PoolRequest poolRequest)
        {
            PoolResponse poolResponse = new PoolResponse();
            List<Domain.Pool> poolsList = new List<Domain.Pool>();
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    if (poolRequest.Requests.Count == 0)
                    {
                        poolsList = tMSDBContext.Pools
                        .Where(p => !p.IsDelete)
                        .Select(pool => new Domain.Pool
                        {
                            ID = pool.ID,
                            PoolName = pool.PoolName,
                            PoolCode = pool.PoolCode,
                            CityID = pool.CityID,
                            CityName = pool.City.CityDescription,
                            ContactNumber = pool.ContactNumber,
                            Address = pool.Address,
                            PhotoId = pool.PhotoId,
                            PhotoGuId = pool.ImageGuid.ImageGuIdValue,
                            PoolNo=pool.PoolNo
                        }).ToList();
                    }
                    else if (poolRequest.Requests.Count > 0)
                    {
                        var poolFilter = poolRequest.Requests[0];

                        poolsList = tMSDBContext.Pools
                        .Where(p => !p.IsDelete)
                        .Where(p => poolFilter.ID == 0 || p.ID == poolFilter.ID)
                        .Where(p => String.IsNullOrEmpty(poolFilter.PoolName) || p.PoolName.Contains(poolFilter.PoolName))
                        .Where(p => String.IsNullOrEmpty(poolFilter.PoolCode) || p.PoolCode.Contains(poolFilter.PoolCode))
                        .Where(p => String.IsNullOrEmpty(poolFilter.ContactNumber) || p.ContactNumber.Contains(poolFilter.ContactNumber))
                        .Where(p => String.IsNullOrEmpty(poolFilter.Address) || p.Address.Contains(poolFilter.Address))
                        .Select(pool => new Domain.Pool
                        {
                            ID = pool.ID,
                            PoolName = pool.PoolName,
                            PoolCode = pool.PoolCode,
                            CityID = pool.CityID,
                            CityName = pool.City.CityDescription,
                            ContactNumber = pool.ContactNumber,
                            Address = pool.Address,
                            PhotoId = pool.PhotoId,
                            PhotoGuId = pool.ImageGuid.ImageGuIdValue
                        }).ToList();
                    }
                }

                if (poolsList.Count > 0 && poolRequest.SortOrder!=null)
                {
                    // Sorting
                    switch (poolRequest.SortOrder.ToLower())
                    {
                        case "poolno":
                            poolsList = poolsList.OrderBy(s => s.PoolNo).ToList();
                            break;
                        case "poolno_desc":
                            poolsList = poolsList.OrderByDescending(s => s.PoolNo).ToList();
                            break;
                        case "poolname":
                            poolsList = poolsList.OrderBy(s => s.PoolName).ToList();
                            break;
                        case "poolname_desc":
                            poolsList = poolsList.OrderByDescending(s => s.PoolName).ToList();
                            break;
                        case "poolcode":
                            poolsList = poolsList.OrderBy(s => s.PoolCode).ToList();
                            break;
                        case "poolcode_desc":
                            poolsList = poolsList.OrderByDescending(s => s.PoolCode).ToList();
                            break;
                        case "contactnumber":
                            poolsList = poolsList.OrderBy(s => s.ContactNumber).ToList();
                            break;
                        case "contactnumber_desc":
                            poolsList = poolsList.OrderByDescending(s => s.ContactNumber).ToList();
                            break;
                        case "address":
                            poolsList = poolsList.OrderBy(s => s.Address).ToList();
                            break;
                        case "address_desc":
                            poolsList = poolsList.OrderByDescending(s => s.Address).ToList();
                            break;
                        case "city":
                            poolsList = poolsList.OrderBy(s => s.CityName).ToList();
                            break;
                        case "city_desc":
                            poolsList = poolsList.OrderByDescending(s => s.CityName).ToList();
                            break;
                        default:  // ID Descending 
                            poolsList = poolsList.OrderByDescending(s => s.ID).ToList();
                            break;
                    }
                }

                // Paging
                int pageSize = poolRequest.PageSize.Value;
                int pageNumber = (poolRequest.PageNumber ?? 1);
                if (pageSize > 0)
                {
                    poolsList = poolsList.Skip(pageNumber - 1 * pageSize).Take(pageSize).ToList();
                }

                if (poolsList.Count > 0)
                {
                    poolResponse.Data = poolsList;
                    poolResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    poolResponse.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    poolResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                    poolResponse.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                poolResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                poolResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                poolResponse.StatusMessage = ex.Message;
            }
            return poolResponse;
        }
        public int InsertImageGuid(string imageGuidValue, string createdBy)
        {
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    ImageGuid imageGuidObject = new ImageGuid()
                    {
                        ImageGuIdValue = imageGuidValue,
                        CreatedBy = createdBy
                    };
                    tMSDBContext.ImageGuids.Add(imageGuidObject);
                    tMSDBContext.SaveChanges();
                    return imageGuidObject.ID;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
            }
            return 0;
        }
    }
}
