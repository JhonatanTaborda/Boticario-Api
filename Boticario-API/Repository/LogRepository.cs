using Boticario.Api.Context;
using Boticario.Api.Models;
using Boticario.Api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boticario.Api.Repository
{
    public class LogRepository
    {           
        public LogRepository()
        { }               

        /// <summary>
        /// Método que salva os Logs no banco de dados
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool InsertLog(LogModel log)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                using (var context = new AppDbContext(optionsBuilder.Options))
                {
                    context.Log.Add(log);

                    context.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }             
        }
    }
}
