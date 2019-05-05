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
using TMS.DataGateway.Repositories.Iterfaces;

namespace TMS.DataGateway.Repositories
{
    public class PIC : IPIC
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PICResponse CreateUpdatePIC(PICRequest picRequest)
        {
            PICResponse picResponse = new PICResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Domain.PIC, DataModel.PIC>().ReverseMap();
                    });
                    //    .ForMember(x => x.RoleName, opt => opt.Ignore())
                    //    .ForMember(x => x.BusinessAreaName, opt => opt.Ignore())
                    //    .ForMember(x => x.ApplicationNames, opt => opt.Ignore());
                    //});

                    IMapper mapper = config.CreateMapper();
                    var pics = mapper.Map<List<Domain.PIC>, List<DataModel.PIC>>(picRequest.Requests);
                    foreach (var picData in pics)
                    {
                        picData.IsDeleted = false;
                        if (picData.ID > 0) //Update User
                        {
                            context.Entry(picData).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            picResponse.StatusMessage = DomainObjects.Resource.ResourceData.PicUpdated;
                        }
                        else //Create User
                        {
                            context.Pics.Add(picData);
                            context.SaveChanges();
                            picResponse.StatusMessage = DomainObjects.Resource.ResourceData.PicCreated;
                        }
                    }
                    picRequest.Requests = mapper.Map<List<DataModel.PIC>, List<Domain.PIC>>(pics);
                    picResponse.Data = picRequest.Requests;
                    picResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    picResponse.StatusCode = (int)HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                picResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                picResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                picResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return picResponse;
        }

        public PICResponse DeletePIC(int picId)
        {
            PICResponse picResponse = new PICResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    var picData = context.Pics.Where(pic => pic.ID == picId).FirstOrDefault();
                    if (picData != null)
                    {
                        picData.IsDeleted = true;
                        context.SaveChanges();
                        picResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        picResponse.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        picResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                        picResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                picResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                picResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                picResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return picResponse;
        }

        public PICResponse GetPICs(PICRequest picRequest)
        {
            PICResponse picResponse = new PICResponse();
            List<Domain.PIC> picList = new List<Domain.PIC>();
            try
            {
                using (var context = new TMSDBContext())
                {
                    picList =
                        (from pic in context.Pics
                         select new Domain.PIC
                         {
                             ID = pic.ID,
                             PICName = pic.PICName,
                             PICEmail = pic.PICEmail,
                             PICPhone = pic.PICPhone,
                             IsActive = pic.IsActive,
                             IsDeleted = pic.IsDeleted,
                             PhotoId = pic.PhotoId,
                             PhotoGuId=pic.ImageGuid.ImageGuIdValue
                         }).ToList();
                }
                // Filter
                if (picRequest.Requests.Count > 0)
                {
                    var picFilter = picRequest.Requests[0];

                    if (picFilter.ID > 0)
                    {
                        picList = picList.Where(s => s.ID == picFilter.ID).ToList();
                    }

                    if (!String.IsNullOrEmpty(picFilter.PICName))
                    {
                        picList = picList.Where(s => s.PICName.Contains(picFilter.PICName)).ToList();
                    }

                    if (!String.IsNullOrEmpty(picFilter.PICEmail))
                    {
                        picList = picList.Where(s => s.PICEmail.Contains(picFilter.PICEmail)).ToList();
                    }

                }
                // Sorting
                switch (picRequest.SortOrder.ToLower())
                {
                    case "picname":
                        picList = picList.OrderBy(s => s.PICName).ToList();
                        break;
                    case "picname_desc":
                        picList = picList.OrderByDescending(s => s.PICName).ToList();
                        break;
                    case "picemail":
                        picList = picList.OrderBy(s => s.PICEmail).ToList();
                        break;
                    case "picemail_Desc":
                        picList = picList.OrderByDescending(s => s.PICEmail).ToList();
                        break;
                    default:  // ID Descending 
                        picList = picList.OrderByDescending(s => s.ID).ToList();
                        break;
                }
                // Total NumberOfRecords
                picResponse.NumberOfRecords = picList.Count;
                // Paging
                int pageNumber = (picRequest.PageNumber ?? 1);
                int pageSize = Convert.ToInt32(picRequest.PageSize);
                if (pageSize > 0)
                {
                    picList = picList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }
                if (picList.Count > 0)
                {
                    picResponse.Data = picList;
                    picResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    picResponse.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    picResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                    picResponse.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                picResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                picResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                picResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return picResponse;
        }
    }
}
