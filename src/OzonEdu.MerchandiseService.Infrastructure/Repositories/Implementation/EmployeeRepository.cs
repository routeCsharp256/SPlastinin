using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using OpenTracing;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure.ChangeTracker;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure.DbConnection;

namespace OzonEdu.MerchandiseService.Infrastructure.Repositories.Implementation
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbConnectionFactory<NpgsqlConnection> _dbConnectionFactory;
        private readonly IChangeTracker _changeTracker;
        private readonly ITracer _tracer;
        private const int Timeout = 5;

        public EmployeeRepository(IDbConnectionFactory<NpgsqlConnection> dbConnectionFactory,
            IChangeTracker changeTracker, ITracer tracer)
        {
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
        }

        public async Task<Employee> AddAsync(Employee itemToAdd, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.BuildSpan($"{nameof(EmployeeRepository)}.{nameof(AddAsync)}").StartActive();

            // EmployeeId comes from request - it is an identifier from another microservice (Employees service)
            const string sql = @"
                INSERT INTO employees (id, firstname, lastname, middlename, email)
                VALUES (@Id, @FirstName, @LastName, @MiddleName, @Email);";

            var parameters = new
            {
                FirstName = itemToAdd.PersonName.FirstName,
                LastName = itemToAdd.PersonName.LastName,
                MiddleName = itemToAdd.PersonName.MiddleName,
                Email = itemToAdd.Email.Value,
                Id = itemToAdd.Id
            };

            var commandDefinition = new CommandDefinition(
                sql,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);

            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            await connection.ExecuteAsync(commandDefinition);

            _changeTracker.Track(itemToAdd);

            return itemToAdd;
        }

        public async Task<Employee> UpdateAsync(Employee itemToUpdate, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.BuildSpan($"{nameof(EmployeeRepository)}.{nameof(UpdateAsync)}").StartActive();

            const string sql = @"
                UPDATE employees 
                SET firstname = @FirstName, lastname = @LastName, middlename = @MiddleName, email = @Email
                WHERE id = @Id;";

            var parameters = new
            {
                FirstName = itemToUpdate.PersonName.FirstName,
                LastName = itemToUpdate.PersonName.LastName,
                MiddleName = itemToUpdate.PersonName.MiddleName,
                Email = itemToUpdate.Email.Value,
                Id = itemToUpdate.Id
            };

            var commandDefinition = new CommandDefinition(
                sql,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);

            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            await connection.ExecuteAsync(commandDefinition);

            _changeTracker.Track(itemToUpdate);

            return itemToUpdate;
        }

        public async Task<Employee> FindByIdAsync(int employeeId, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.BuildSpan($"{nameof(EmployeeRepository)}.{nameof(FindByIdAsync)}").StartActive();
        
            const string sql = @"
                SELECT id, firstname, lastname, middlename, email
                FROM employees
                WHERE id = @Id;";
        
            var parameters = new
            {
                Id = employeeId
            };
        
            var commandDefinition = new CommandDefinition(
                sql,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);
        
            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            var employees = await connection.QueryAsync<Employee>(commandDefinition);
        
            var employee = employees.FirstOrDefault();
            if (employee != null) _changeTracker.Track(employee);
        
            return employee;
        }

        public async Task<Employee> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.BuildSpan($"{nameof(EmployeeRepository)}.{nameof(FindByEmailAsync)}").StartActive();

            const string sql = @"
                SELECT id, firstname, lastname, middlename, email
                FROM employees
                WHERE email = @Email;";

            var parameters = new
            {
                Email = email
            };

            var commandDefinition = new CommandDefinition(
                sql,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);

            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            var employees = await connection.QueryAsync<Employee>(commandDefinition);

            var employee = employees.FirstOrDefault();
            if (employee != null) _changeTracker.Track(employee);

            return employee;
        }
        
        public async Task<Employee> FindOrCreateEmployee(int employeeId, string firstName, string lastName,
            string middleName,
            string email, CancellationToken cancellationToken)
        {
            using var span = _tracer.BuildSpan($"{nameof(EmployeeRepository)}.{nameof(FindOrCreateEmployee)}").StartActive();
            
            var employee = await FindByEmailAsync(email, cancellationToken);
            bool employeeOriginallyExisted = (employee != null);

            if (!employeeOriginallyExisted)
            {
                employee = new Employee(employeeId, firstName, lastName, middleName, email);
            }

            var savedEmployee = employeeOriginallyExisted
                ? await UpdateAsync(employee, cancellationToken)
                : await AddAsync(employee, cancellationToken);

            return savedEmployee;
        }
    }
}