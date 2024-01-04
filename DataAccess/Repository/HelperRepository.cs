using Dapper;
using Dapper.Oracle;
using DataAccess.Context;
using DataAccess.Contracts;
using DataAccess.Dto;
using DataAccess.Dto.Request;
using DataAccess.Dto.Response;
using DataAccess.Entities;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class HelperRepository : IHelperRepository
	{
		private readonly DapperContext _context;
		private DtoWrapper _dto;

		public HelperRepository(DapperContext context, DtoWrapper dto)
		{
			_context = context;
			_dto = dto;
		}

		public async Task<CheckCredentialResDto> EmpPasswordCheck(int empCode, string password)
		{
			var userId = Convert.ToString(empCode);
			var query = "select count(*) from employee_master where emp_code=:userId and password=:password and status_id=1 ";

			using var connection = _context.CreateConnection();
			_dto.checkCredRes.flag = await connection.QuerySingleOrDefaultAsync<int>(query, new { userId, password });

			return _dto.checkCredRes;
		}

		public async Task<CheckCredentialResDto> HostnameCheck(string hostname)
		{
			var query = "select count(1) from BRANCH_SYS_INFO_request t, employee_master em where t.req_by = em.emp_code and (em.grade_id < 7 or (em.branch_id=3531 and em.grade_id<9)) and em.status_id = 1  and t.status = 2  and ((upper(REPLACE(t.mac_id, ':', '')) = upper(replace(:hostname,':', ''))) or (upper(REPLACE(t.mac_id, '-', '')) = upper(replace(:hostname,':', ''))))";

			using var connection = _context.CreateConnection();
			_dto.checkCredRes.flag = await connection.QuerySingleOrDefaultAsync<int>(query, new { hostname });
			
			return _dto.checkCredRes;
		}

		public async Task<CheckCredentialResDto> ResetPasswordProd(int userName, string ifStaff, byte[] data)
		{
			var result = 0;

			var procedureName = "resetPassword";
			var parameters = new OracleDynamicParameters();
			parameters.Add("empId", userName, OracleMappingType.Int32, ParameterDirection.Input);
			parameters.Add("userId", ifStaff, OracleMappingType.NVarchar2, ParameterDirection.Input);
			parameters.Add("newPass", data, OracleMappingType.Raw, ParameterDirection.Input);
			parameters.Add("msgFlg", result, OracleMappingType.Int32, ParameterDirection.Output);

			parameters.BindByName = true;
			using var connection = _context.CreateConnection();
			await connection.QueryFirstOrDefaultAsync
				(procedureName, parameters, commandType: CommandType.StoredProcedure);

			_dto.checkCredRes.flag = parameters.Get<int>("msgFlg");
			return _dto.checkCredRes;
		}

		public async Task<CheckCredentialResDto> ResetPassword(int userName, string ifStaff, string data)
		{
			var result = 0;

			var procedureName = "resetPassword_uat";
			var parameters = new OracleDynamicParameters();
			parameters.Add("empId", userName, OracleMappingType.Int32, ParameterDirection.Input);
			parameters.Add("userId", ifStaff, OracleMappingType.NVarchar2, ParameterDirection.Input);
			parameters.Add("newPass", data, OracleMappingType.NVarchar2, ParameterDirection.Input);
			parameters.Add("msgFlg", result, OracleMappingType.Int32, ParameterDirection.Output);

			parameters.BindByName = true;
			using var connection = _context.CreateConnection();
			await connection.QueryFirstOrDefaultAsync
				(procedureName, parameters, commandType: CommandType.StoredProcedure);

			_dto.checkCredRes.flag = parameters.Get<int>("msgFlg");
			return _dto.checkCredRes;
		}

		public async Task LoginTracker(int userId, int status, string sip, int appId)
		{
			var procedureName = "pro_hrm_log_track";
			var parameters = new OracleDynamicParameters();
			parameters.Add("emcod", userId, OracleMappingType.Int32, ParameterDirection.Input);
			parameters.Add("stat", status, OracleMappingType.Int32, ParameterDirection.Input);
			parameters.Add("sys_ip", userId, OracleMappingType.NVarchar2, ParameterDirection.Input);
			parameters.Add("modid", status, OracleMappingType.Int32, ParameterDirection.Input);
			parameters.BindByName = true;
			using var connection = _context.CreateConnection();
			await connection.QueryFirstOrDefaultAsync
				(procedureName, parameters, commandType: CommandType.StoredProcedure);

			return;
		}

		public async Task<ChangePasswordResDto> ChangePasswordProd(int userName, byte[] password, byte[] oldPassword)
		{
			var result = string.Empty;

			var procedureName = "change_passwd_new";
			var parameters = new OracleDynamicParameters();
			parameters.Add("user_nm", userName, OracleMappingType.Int32, ParameterDirection.Input);
			parameters.Add("oldpass", password, OracleMappingType.Raw, ParameterDirection.Input);
			parameters.Add("newpass", oldPassword, OracleMappingType.Raw, ParameterDirection.Input);
			parameters.Add("msg", result, OracleMappingType.NVarchar2, ParameterDirection.Output);

			parameters.BindByName = true;
			using var connection = _context.CreateConnection();
			await connection.QueryFirstOrDefaultAsync
				(procedureName, parameters, commandType: CommandType.StoredProcedure);

			_dto.changePwdRes.status = Convert.ToString(parameters.Get<int>("msg"));
			return _dto.changePwdRes;
		}

		public async Task<ChangePasswordResDto> ChangePassword(PwdChangeReqDto pwdReq)
		{
			var result = string.Empty;

			var procedureName = "change_passwd_new_uat";
			var parameters = new OracleDynamicParameters();
			parameters.Add("user_nm", pwdReq.empCode, OracleMappingType.Int32, ParameterDirection.Input);
			parameters.Add("oldpass", pwdReq.oldPassword, OracleMappingType.NVarchar2, ParameterDirection.Input);
			parameters.Add("newpass", pwdReq.password, OracleMappingType.NVarchar2, ParameterDirection.Input);
			parameters.Add("msg", result, OracleMappingType.NVarchar2, ParameterDirection.Output);

			parameters.BindByName = true;
			using var connection = _context.CreateConnection();
			await connection.QueryFirstOrDefaultAsync
				(procedureName, parameters, commandType: CommandType.StoredProcedure);

			_dto.changePwdRes.status = parameters.Get<string>("msg");
			return _dto.changePwdRes;
		}

		public async Task<GetRolesResDto> GetRolesProd(int userName, byte[] password)
		{
			var result = 0;
			var accessId = 0;
			var roleId = 0;
			var branchId = 0;

			var procedureName = "get_access_level";
			var parameters = new OracleDynamicParameters();
			parameters.Add("empid", userName, OracleMappingType.Int32, ParameterDirection.Input);
			parameters.Add("passwd", password, OracleMappingType.Raw, ParameterDirection.Input);
			parameters.Add("accessid", accessId, OracleMappingType.Int32, ParameterDirection.Output);
			parameters.Add("roleid", roleId, OracleMappingType.Int32, ParameterDirection.Output);
			parameters.Add("emp_br", branchId, OracleMappingType.Int32, ParameterDirection.Output);
			parameters.Add("passwd_flg", result, OracleMappingType.Int32, ParameterDirection.Output);

			parameters.BindByName = true;
			using var connection = _context.CreateConnection();
			await connection.QueryFirstOrDefaultAsync
				(procedureName, parameters, commandType: CommandType.StoredProcedure);

			_dto.getRolesRes.accessId = parameters.Get<int>("accessid");
			_dto.getRolesRes.roleId = parameters.Get<int>("roleid");
			_dto.getRolesRes.branchId = parameters.Get<int>("emp_br");
			_dto.getRolesRes.flag = parameters.Get<int>("passwd_flg");

			return _dto.getRolesRes;
		}

		public async Task<GetRolesResDto> GetRoles(int userName, string password)
		{
			var result = 0;
			var accessId = 0;
			var roleId = 0;
			var branchId = 0;

			var procedureName = "get_roles";
			var parameters = new OracleDynamicParameters();
			parameters.Add("empid", userName, OracleMappingType.Int32, ParameterDirection.Input);
			parameters.Add("passwd", password, OracleMappingType.NVarchar2, ParameterDirection.Input);
			parameters.Add("accessid", accessId, OracleMappingType.Int32, ParameterDirection.Output);
			parameters.Add("roleid", roleId, OracleMappingType.Int32, ParameterDirection.Output);
			parameters.Add("emp_br", branchId, OracleMappingType.Int32, ParameterDirection.Output);			
			parameters.Add("passwd_flg", result, OracleMappingType.Int32, ParameterDirection.Output);

			parameters.BindByName = true;
			using var connection = _context.CreateConnection();
			await connection.QueryFirstOrDefaultAsync
				(procedureName, parameters, commandType: CommandType.StoredProcedure);

			_dto.getRolesRes.accessId = parameters.Get<int>("accessid");
			_dto.getRolesRes.roleId = parameters.Get<int>("roleid");
			_dto.getRolesRes.branchId = parameters.Get<int>("emp_br");
			_dto.getRolesRes.flag = parameters.Get<int>("passwd_flg");

			return _dto.getRolesRes;
		}

		public async Task<EmployeeStatusResDto> GetEmployeeStatus(long mobile)
		{
			var cellno = Convert.ToString(mobile);
			var query = "select e.emp_code, e.emp_name, upper(s.remark) status, e.branch_id from EMP_GREETING_MASTER t, employee_master e,STATUS_MST s where t.emp_code = e.emp_code and e.status_id=s.status_id and t.mobile_no=:cellno";

			using var connection = _context.CreateConnection();
			var empStatus = await connection.QuerySingleOrDefaultAsync<EmployeeStatusResDto>(query, new { cellno });

			return empStatus;
		}
	}
}
