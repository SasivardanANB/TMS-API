using AutoMapper;
using NLog;
using OMS.DataGateway.DataModels;
using OMS.DomainObjects.Objects;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataModel = OMS.DataGateway.DataModels;
using Domain = OMS.DomainObjects.Objects;
using OMS.DataGateway.Encryption;
using OMS.DataGateway.Repositories.Iterfaces;

namespace OMS.DataGateway.Repositories
{
    public class Authenticate : IAuthenticate
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public int InsertToken(Domain.Authenticate token)
        {
            try
            {
                using (var context = new OMSDBContext())
                {

                    var tokenData = context.TokensManagers.Where(x => x.UserID == token.UserID).FirstOrDefault();
                    if (tokenData != null)
                    {
                        if(tokenData.ExpiresOn < DateTime.Now)
                        {
                            tokenData.TokenKey = token.TokenKey;
                            tokenData.CreatedOn = token.CreatedOn;
                            tokenData.ExpiresOn = token.ExpiresOn;
                            tokenData.LastModifiedTime = DateTime.Now;
                        }
                    }
                    else
                    {
                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<Domain.Authenticate, DataModel.TokensManager>().ReverseMap();
                        });

                        IMapper mapper = config.CreateMapper();
                        var users = mapper.Map<Domain.Authenticate, DataModel.TokensManager>(token);
                        context.TokensManagers.Add(users);

                    }
                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                return 0;
            }
        }

        public string GenerateToken(Domain.User user, DateTime IssuedOn)
        {
            try
            {
                string randomnumber =
                   string.Join(":", new string[]
               {   Convert.ToString(user.ID),
                EncryptionLibrary.KeyGenerator.GetUniqueKey(),
                //Convert.ToString(ClientKeys.CompanyID),
                Convert.ToString(IssuedOn.Ticks)
                //ClientKeys.ClientID
                   });

                return EncryptionLibrary.EncryptText(randomnumber);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                return null;
            }

        }
        public bool ValidateToken(string token)
        {
            bool validFlag = false;
            try {


                var key = EncryptionLibrary.DecryptText(token);
                string[] parts = key.Split(new char[] { ':' });
                var userId = Convert.ToInt32(parts[0]);       // UserID
                using (var context = new OMSDBContext())
                {
                    // Validating Time
                    var ExpiresOn = context.TokensManagers.Where(t => t.TokenKey == token && t.UserID == userId).Select(t=>t.ExpiresOn).FirstOrDefault();
                    if (ExpiresOn != null)
                    {
                        if ((DateTime.Now > ExpiresOn))
                        {
                            validFlag = false;
                        }
                        else
                        {
                            validFlag = true;
                        }
                    }
                    else
                    {
                        validFlag = false;
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
            }
            return validFlag;
        }

    }
}
