using Microsoft.EntityFrameworkCore;
using MyrStore.Common.Configuration;
using MyrStore.Common.Configuration.Attribute;
using MyrStore.Common.Entity;
using MyrStore.Common.Infrastructure.Mapping;
using System.Data;
using System.Data.Common;

namespace MyrStore.Common.Infrastructure.Context
{
    public partial class MyrContext : DbContext, IMyrContext
    {
        #region Ctor
        public MyrContext(DbContextOptions<MyrContext> options) : base(options)
        {
        }

        #endregion

        #region Utilities
        /// <summary>
        /// Further configuration the model
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //dynamically load all entity and query type configurations
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies.Where(a => a.GetName().Name.Contains("API")))
            {
                var typeConfigurations = assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(MappingAttribute), true).Length > 0);
                //var typeConfigurations = assembly.GetTypes().Where(type =>
                //(type.BaseType?.IsGenericType ?? false)
                //    && (type.BaseType.GetGenericTypeDefinition() == typeof(MyrEntityTypeConfiguration<>)));

                foreach (var typeConfiguration in typeConfigurations)
                {
                    Type genericClass = typeof(EntityMap<>);
                    Type constructedClass = genericClass.MakeGenericType(typeConfiguration);
                    var configuration = (IMappingConfiguration)Activator.CreateInstance(constructedClass);
                    configuration.ApplyConfiguration(modelBuilder);
                }
                modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            }
            //var typeConfigurations = Assembly.GetExecutingAssembly().GetTypes().Where(type =>
            //    (type.BaseType?.IsGenericType ?? false)
            //        && (type.BaseType.GetGenericTypeDefinition() == typeof(MyrEntityTypeConfiguration<>)));


            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Modify the input SQL query by adding passed parameters
        /// </summary>
        /// <param name="sql">The raw SQL query</param>
        /// <param name="parameters">The values to be assigned to parameters</param>
        /// <returns>Modified raw SQL query</returns>
        protected virtual string CreateSqlWithParameters(string sql, params object[] parameters)
        {
            //add parameters to sql
            for (var i = 0; i <= (parameters?.Length ?? 0) - 1; i++)
            {
                if (!(parameters[i] is DbParameter parameter))
                    continue;

                sql = $"{sql}{(i > 0 ? "," : string.Empty)} @{parameter.ParameterName}";

                //whether parameter is output
                if (parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output)
                    sql = $"{sql} output";
            }

            return sql;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a DbSet that can be used to query and save instances of entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>A set for the given entity type</returns>
        public virtual new DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public virtual object GetKeyValue<TEntity>(TEntity entity)
        {
            var keyName = Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties
                .Select(x => x.Name).Single();

            return entity.GetType().GetProperty(keyName).GetValue(entity, null);
        }

        public virtual async Task<int> ExecuteSqlCommandAsync(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            //set specific command timeout
            var previousTimeout = Database.GetCommandTimeout();
            Database.SetCommandTimeout(timeout);

            var result = 0;
            if (!doNotEnsureTransaction)
            {
                //use with transaction
                using (var transaction = Database.BeginTransaction())
                {
                    result = await Database.ExecuteSqlRawAsync(sql, parameters);
                    transaction.Commit();
                }
            }
            else
                result = await Database.ExecuteSqlRawAsync(sql, parameters);

            //return previous timeout back
            Database.SetCommandTimeout(previousTimeout);

            return result;
        }

        /// <summary>
        /// Detach an entity from the context
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        public virtual void Detach<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityEntry = Entry(entity);
            if (entityEntry == null)
                return;

            //set the entity is not being tracked by the context
            entityEntry.State = EntityState.Detached;
        }

        #endregion
    }
}
