using DataAccess.Context;
using DataAccess.Contracts;
using DataAccess.Dto;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Repository
{
    public class RepositoryWrapper : IRepositoryWrapper 
    { 
        private DapperContext _repoContext;
        private IUserRepository _user;
        private IHelperRepository _helper;
        private DtoWrapper _dto;
        private readonly IConfiguration _config;


        public RepositoryWrapper(DapperContext repoContext, DtoWrapper dto, IConfiguration config)
        {
            _repoContext = repoContext;
            _dto = dto;
            _config = config;
        }
        public IHelperRepository Helper 
        { 
            get 
            { 
                if (_helper == null) 
                {
                    _helper = new HelperRepository(_repoContext, _dto); 
                } 
                return _helper; 
            } 
        }

        public IUserRepository User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserRepository(_repoContext, _config);
                }
                return _user;
            }
        }

        public RepositoryWrapper(DapperContext dapperContext) 
        { 
            _repoContext = dapperContext; 
        } 
        
        public void Save() 
        {
            _repoContext.SaveChanges();
        } 
    }
}
