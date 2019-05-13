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
                    int picObjectCount = 0;
                    foreach (var picData in pics)
                    {
                        picData.IsDeleted = false;

                        //For getting new PhotoGuId (Allows in case of new record creation and modification of already existing record imageGuId)
                        if ((picData.ID == 0 && !String.IsNullOrEmpty(picRequest.Requests[picObjectCount].PhotoGuId)) || (picData.ID > 0 && picRequest.Requests[picObjectCount].PhotoGuId != context.ImageGuids.Where(d => d.ID == picData.PhotoId && d.IsActive).Select(g => g.ImageGuIdValue).FirstOrDefault()))
                        {
                            picData.PhotoId = InsertImageGuid(picRequest.Requests[picObjectCount].PhotoGuId, picRequest.CreatedBy, picData.PhotoId);
                        }

                        if (picData.ID > 0) //Update User
                        {
                            picData.LastModifiedBy = picRequest.LastModifiedBy;
                            picData.LastModifiedTime = DateTime.Now;
                            picData.PICPassword= context.Pics.Where(i => i.ID == picData.ID).Select(p => p.PICPassword).FirstOrDefault();
                            context.Entry(picData).State = System.Data.Entity.EntityState.Modified;
                            context.Entry(picData).Property(p => p.PICPassword).IsModified = false;
                            context.SaveChanges();
                            picResponse.StatusMessage = DomainObjects.Resource.ResourceData.PicUpdated;
                        }
                        else //Create User
                        {
                            picData.CreatedBy = picRequest.CreatedBy;
                            picData.CreatedTime = DateTime.Now;
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
                        //Need to assign lastmodifiedby using session userid
                        picData.LastModifiedTime = DateTime.Now;
                        picData.IsDeleted = true;
                        context.SaveChanges();
                        picResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        picResponse.StatusCode = (int)HttpStatusCode.OK;
                        picResponse.StatusMessage = DomainObjects.Resource.ResourceData.PicDeleted;
                    }
                    else
                    {
                        picResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        picResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        picResponse.StatusMessage = DomainObjects.Resource.ResourceData.InvalidPICID;
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
                         where !pic.IsDeleted
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

                // GLobal Search Filter
                if (!string.IsNullOrEmpty(picRequest.GlobalSearch))
                {
                    string globalSearch = picRequest.GlobalSearch;
                    picList = picList.Where(s => !s.IsDeleted && s.PICName.Contains(globalSearch)
                    || s.PICPhone.Contains(globalSearch)
                    || s.PICEmail.ToString().Contains(globalSearch)
                    ).ToList();
                }

                // Sorting
                if(picList.Count>0 && !string.IsNullOrEmpty(picRequest.SortOrder))
                {
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
                    picResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                else
                {
                    picResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    picResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    picResponse.StatusMessage= DomainObjects.Resource.ResourceData.NoRecords;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                picResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                picResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                picResponse.StatusMessage = ex.Message;
            }
            return picResponse;
        }

        //For inserting new record into ImageGuid table also makes IsActive false for previously inserted records
        public int InsertImageGuid(string imageGuidValue, string createdBy, int? existingImageGuID)
        {
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {

                    // Making IsActive false for existed record 
                    if (existingImageGuID > 0)
                    {
                        var existingImageGuidDetails = tMSDBContext.ImageGuids.Where(i => i.ID == existingImageGuID).FirstOrDefault();
                        existingImageGuidDetails.IsActive = false;
                    }

                    //Inserting new record along with IsActive true
                    ImageGuid imageGuidObject = new ImageGuid()
                    {
                        ImageGuIdValue = imageGuidValue,
                        CreatedBy = createdBy,
                        IsActive = true
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
