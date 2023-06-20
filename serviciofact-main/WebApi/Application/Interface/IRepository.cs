using System.Collections.Generic;

namespace WebApi.Application.Interface
{
  public interface IRepository<TEntity>
  {
    List<TEntity> ConsultAnys();
    TEntity GetById(int id);
    void Add(TEntity entity);
    void Update(string id, TEntity entity);
    void Delete(string id);
  }
}