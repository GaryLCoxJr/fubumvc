using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuFastPack.Domain;
using FubuFastPack.Persistence;
using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    public abstract class RepositoryGrid<TEntity> : Grid<TEntity, IRepository> where TEntity : DomainEntity
    {
        public override IGridDataSource<TEntity> BuildSource(IRepository repository)
        {
            return new RepositoryDataSource(this, repository);
        }

        protected virtual IQueryable<TEntity> query(IRepository repository)
        {
            return repository.Query<TEntity>();
        }

        #region Nested type: RepositoryDataSource

        public class RepositoryDataSource : IGridDataSource<TEntity>
        {
            private readonly RepositoryGrid<TEntity> _grid;
            private readonly IRepository _repository;
            private readonly IList<Expression<Func<TEntity, bool>>> _wheres = new List<Expression<Func<TEntity, bool>>>();

            public RepositoryDataSource(RepositoryGrid<TEntity> grid, IRepository repository)
            {
                _grid = grid;
                _repository = repository;
            }

            // TODO -- test this
            public int TotalCount()
            {
                var queryable = query();
                _wheres.Each(w =>
                {
                    queryable = queryable.Where(w);
                });
                return queryable.Count();
            }

            public IGridData Fetch(GridDataRequest options)
            {
                var queryable = query();
                _wheres.Each(w =>
                {
                    queryable = queryable.Where(w);
                });

                queryable = sort(queryable, options);
                queryable = applyPaging(queryable, options);



                return new EntityGridData<TEntity>(queryable);
            }

            public void ApplyRestrictions(Action<IDataSourceFilter<TEntity>> configure)
            {
                var filter = new QueryableDataSourceFilter<TEntity>();
                configure(filter);

                _wheres.AddRange(filter.Wheres);
            }

            public void ApplyCriteria(FilterRequest<TEntity> request, IQueryService queryService)
            {
                var where = queryService.WhereFilterFor(request);
                _wheres.Add(where);
            }

            private IQueryable<TEntity> query()
            {
                return _grid.query(_repository);
            }

            private IQueryable<TEntity> applyPaging(IQueryable<TEntity> queryable, GridDataRequest options)
            {
                return queryable.Skip(options.ResultsToSkip()).Take(options.ResultsPerPage);
            }

            // TODO -- default sorting?
            private IQueryable<TEntity> sort(IQueryable<TEntity> queryable, GridDataRequest options)
            {
                if (options.SortColumn.IsNotEmpty())
                {
                    var accessor = _grid.Definition.SelectedAccessors.First(x => x.Name == options.SortColumn);

                    var expression = accessor.ToExpression<TEntity>();
                    return options.SortAscending
                               ? queryable.OrderBy(expression)
                               : queryable.OrderByDescending(expression);
                }

                return queryable;
            }
        }

        #endregion
    }
}