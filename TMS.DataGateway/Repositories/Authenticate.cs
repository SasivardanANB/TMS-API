using AutoMapper;
using NLog;
using TMS.DataGateway.DataModels;
using TMS.DomainObjects.Objects;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataModel = TMS.DataGateway.DataModels;
using Domain = TMS.DomainObjects.Objects;
using TMS.DataGateway.Encryption;
using TMS.DataGateway.Repositories.Iterfaces;

namespace TMS.DataGateway.Repositories
{
    public class Authenticate : IAuthenticate
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public int InsertToken(Domain.Authenticate token)
        {
            try
            {
                using (var context = new TMSDBContext())
                {
                    var tokenData = context.Tokens.Where(x => x.UserID == token.UserID).FirstOrDefault();
                    if (tokenData != null)
                    {
                        if (tokenData.ExpiresOn < DateTime.Now)
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
                            cfg.CreateMap<Domain.Authenticate, DataModel.Token>().ReverseMap();
                        });

                        IMapper mapper = config.CreateMapper();
                        var users = mapper.Map<Domain.Authenticate, DataModel.Token>(token);
                        context.Tokens.Add(users);
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
                using (var context = new TMSDBContext())
                {
                    // Validating Time
                    var ExpiresOn = context.Tokens.Where(t => t.TokenKey == token && t.UserID == userId).Select(t=>t.ExpiresOn).FirstOrDefault();
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
