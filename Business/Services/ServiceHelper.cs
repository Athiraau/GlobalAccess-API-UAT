using Business.Helpers;
using DataAccess.Contracts;
using DataAccess.Entities;
using Microsoft.Extensions.Configuration;

namespace Business.Services
{
    public class ServiceHelper
    {
        private HelperClass _pHelper;
        private ValidationHelper _vHelper;

        private readonly IRepositoryWrapper _repository;
        private readonly ErrorResponse _error;
        private readonly IConfiguration _configuration;

        public ServiceHelper(IRepositoryWrapper repository, ErrorResponse error, IConfiguration configuration)
        {
            _repository = repository;
            _error = error;
            _configuration = configuration;
        }

        public HelperClass PHelper
        {
            get
            {
                if (_pHelper == null)
                {
                    _pHelper = new HelperClass(_repository);
                }
                return _pHelper;
            }
        }

        public ValidationHelper VHelper
        {
            get
            {
                if (_vHelper == null)
                {
                    _vHelper = new ValidationHelper(_error);
                }
                return _vHelper;
            }
        }
    }
}
