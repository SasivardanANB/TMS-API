﻿using AutoMapper;
using NLog;
using DMS.DataGateway.DataModels;
using System;
using System.Linq;
using DataModel = DMS.DataGateway.DataModels;
using Domain = DMS.DomainObjects.Objects;
using DMS.DataGateway.Encryption;
using DMS.DataGateway.Repositories.Iterfaces;

namespace DMS.DataGateway.Repositories
{
    public class Authenticate : IAuthenticate
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public int InsertToken(Domain.Authenticate token)
        {
            try
            {
                using (var context = new DMSDBContext())
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Domain.Authenticate, DataModel.TokenManager>().ReverseMap();
                    });

                    IMapper mapper = config.CreateMapper();
                    var users = mapper.Map<Domain.Authenticate, DataModel.TokenManager>(token);
                    context.TokenManagers.Add(users);
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
                Convert.ToString(IssuedOn.Ticks)
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
            try
            {
                var key = EncryptionLibrary.DecryptText(token);
                string[] parts = key.Split(new char[] { ':' });
                var userId = Convert.ToInt32(parts[0]);       // UserID
                using (var context = new DMSDBContext())
                {
                    // Validating Time
                    var ExpiresOn = context.TokenManagers.Where(t => t.TokenKey == token && t.DriverId == userId).Select(t => t.ExpiresOn).FirstOrDefault();
                    if ((DateTime.Now > ExpiresOn))
                    {
                        validFlag = false;
                    }
                    else
                    {
                        validFlag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
            }
            return validFlag;
        }
    }
}
