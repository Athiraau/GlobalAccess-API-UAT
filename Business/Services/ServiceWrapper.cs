using DataAccess.Contracts;
using Business.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using DataAccess.Dto;

namespace Business.Services
{
    public class ServiceWrapper: IServiceWrapper
    {
        private IHelperService _helper;
        private IUserService _user;
        private IJwtUtils _jwtUtils;

        private readonly DtoWrapper _dto;        
        private readonly ServiceHelper _service;
        private readonly IRepositoryWrapper _repository;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly ILoggerService _logger;
        
        public ServiceWrapper(IRepositoryWrapper repository, ServiceHelper service, IConfiguration config, 
            IWebHostEnvironment env, ILoggerService logger, DtoWrapper dto)
        {            
            _repository = repository;
            _service = service;
            _config = config;
            _env = env;
            _logger = logger;
            _dto = dto;
        }

        public IUserService User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserService(_config, _repository);
                }
                return _user;
            }
        }

        public IHelperService Helper
        {
            get
            {
                if (_helper == null)
                {
                    _helper = new HelperService(_repository, _service, _env, _dto, _config);
                }
                return _helper;
            }
        }

        public IJwtUtils JwtUtils
        {
            get
            {
                if (_jwtUtils == null)
                {
                    _jwtUtils = new JwtUtils(_config, _logger);
                }
                return _jwtUtils;
            }
        }
    }
}
