using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Depository.Core;
using Depository.Core.Models;
using Depository.Core.Models.DTO;
using Depository.DAL;
using Depository.DAL.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TransactionsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;


        public TransactionsController(ILogger<TransactionsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
        }


        private async Task WriteToLogEntityOperationResult(EntityOperationResult<transactions> entityOperationResult, string action)
        {
            if (entityOperationResult.IsSuccess)
                _logger.LogInformation(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
            else _logger.LogError(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
        }

        private async Task WriteToLogException(Exception exception, string action)
        {
            _logger.LogError(action + " : " + exception.ToString());
        }

        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            List<transactionsDTO> transactionsDTOList = new List<transactionsDTO>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                   
                    var accounts = unitOfWork.accounts.GetList();
                    var transactions = unitOfWork.transactions.GetList(1000);
                    foreach (var transaction in transactions)
                    {
                        transactionsDTO transactionsDTO = new transactionsDTO();
                        transactionsDTO.id = transaction.id;
                        var account = accounts.Where(x => x.id == transaction.account).FirstOrDefault();
                        if (account!=null) transactionsDTO.account = accounts.Where(x => x.id == transaction.account).FirstOrDefault().accnumber;
                        transactionsDTO.date = transaction.date;
                        transactionsDTO.instruction = transaction.instruction;
                        transactionsDTO.stop = transaction.stop;
                        transactionsDTO.trans_type = transaction.trans_type;
                        transactionsDTOList.Add(transactionsDTO);
                    }
                    return Ok(transactionsDTOList);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetTransactions");
                return Ok(ex);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetListByPage(int page, int size)
        {
            var skip = (page - 1) * size;

            try
            {
                if ((page == null) || (page <= 0))
                {
                    page = 1;
                }
                if ((size == null) || (size <= 0))
                {
                    size = 2000;
                }
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var transactions = unitOfWork.transactions.GetListByPage(skip, size);

                    return Ok(transactions);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetInstructions");
                return Ok(EntityOperationResult<transactions>
                                    .Failure()
                                    .AddError(ex.ToString()));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var transaction = unitOfWork.transactions.Get(id);
                    return Ok(transaction);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetTransaction");
                return BadRequest();
            }
        }
    }
}
