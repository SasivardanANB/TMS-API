using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel = TMS.DataGateway.DataModels;
using Domain = TMS.DomainObjects.Objects;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System.Net;
using NLog;

namespace TMS.DataGateway.Repositories
{
    public class Order : IOrder
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public OrderResponse CreateUpdateOrder(OrderRequest order)
        {
            //OrderResponse orderResponse = new OrderResponse();
            //try
            //{
            //    using (var context = new DataModel.TMSDBContext())
            //    {
            //        var config = new MapperConfiguration(cfg =>
            //        {
            //            cfg.CreateMap<Domain.Order, DataModel.Order>().ReverseMap()
            //            .ForMember(x => x.RoleName, opt => opt.Ignore())
            //            .ForMember(x => x.BusinessAreaName, opt => opt.Ignore())
            //            .ForMember(x => x.ApplicationNames, opt => opt.Ignore());
            //        });

            //        IMapper mapper = config.CreateMapper();
            //        var orders = mapper.Map<List<Domain.Order>, List<DataModel.Order>>(order.Requests);

            //        //Encrypt Password
            //        foreach (var orderData in orders)
            //        {
            //            if (!string.IsNullOrEmpty(orderData.Password))
            //            {
            //                orderData.Password = Encryption.EncryptionLibrary.EncryptPassword(orderData.Password);
            //            }
            //            if (orderData.BusinessAreaID == 0)
            //            {
            //                orderData.BusinessAreaID = null;
            //            }
            //            if (orderData.RoleID == 0)
            //            {
            //                orderData.RoleID = null;
            //            }
            //            if (orderData.ID > 0) //Update Order
            //            {
            //                context.Entry(orderData).State = System.Data.Entity.EntityState.Modified;
            //                context.SaveChanges();
            //                orderResponse.StatusMessage = DomainObjects.Resource.ResourceData.OrdersUpdated;
            //            }
            //            else //Create Order
            //            {
            //                context.Orders.Add(orderData);
            //                context.SaveChanges();
            //                orderResponse.StatusMessage = DomainObjects.Resource.ResourceData.OrdersCreated;
            //            }
            //        }

            //        order.Requests = mapper.Map<List<DataModel.Order>, List<Domain.Order>>(orders);
            //        orderResponse.Data = order.Requests;
            //        orderResponse.Status = DomainObjects.Resource.ResourceData.Success;
            //        orderResponse.StatusCode = (int)HttpStatusCode.OK;

            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.Log(LogLevel.Error, ex);
            //    orderResponse.Status = DomainObjects.Resource.ResourceData.Failure;
            //    orderResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
            //    orderResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            //}
            //return orderResponse;
            throw new NotImplementedException();
        }

        public OrderResponse GetOrders(OrderRequest orderRequest)
        {
            throw new NotImplementedException();
        }
    }
}
