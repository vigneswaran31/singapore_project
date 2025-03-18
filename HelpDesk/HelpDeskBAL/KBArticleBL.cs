using HelpDeskEntity;
using HelpDeskEntity.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HelpDeskBAL
{
    public class KBArticleBL
    {
        #region Get Methods

        //Get Articles data list in Grid Format.
        public string GetAllArticles(GridSettings grid,int OperatorId = 0)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.vw_KBArticle.AsQueryable();

                    if(OperatorId > 0)
                    {
                        if ((new CommonBL().IsPermissionAllow(OperatorId, ((int)En_Permission.CreateArticle)) && new CommonBL().IsPermissionAllow(OperatorId, ((int)En_Permission.EditOtherArticle))) || new CommonBL().IsPermissionAllow(OperatorId, ((int)En_Permission.EditOtherArticle)))
                            query = ctx.vw_KBArticle.AsQueryable();
                        else
                            query = query.Where(p => p.AuthorId == OperatorId).AsQueryable();
                    }

                    int count;
                    var data = query.GridCommonSettings(grid, out count);

                    var result = new
                    {
                        total = (int)Math.Ceiling((double)count / grid.PageSize),
                        page = grid.PageIndex,
                        records = count,
                        rows = (from c in data
                                select new
                                {
                                    ArticleId = c.ArticleId,
                                    KBCategoryId =  c.KBCategoryId,
                                    CategoryName = c.CategoryName,
                                    Title = c.Title,
                                    Description = c.Description,
                                    CreatedBy = c.CreatedBy,
                                    CreatedOn = c.CreatedOn,
                                    ModifiedBy = c.ModifiedBy,
                                    ModifiedOn = c.ModifiedOn
                                }).ToArray()
                    };
                    return JsonConvert.SerializeObject(result, new IsoDateTimeConverter());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Articles data by CategoryId
        public List<KBArticle> GetByCategoryId(int CategoryId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    List<int> Categories = ctx.KBCategories.Where(p => p.ParentId == CategoryId && p.IsEnable == true).Select(p => p.KBCategoryId).ToList();
                    return ctx.KBArticles.Where(p => p.KBCategoryId == CategoryId || Categories.Contains(p.KBCategoryId)).OrderBy(p => p.Title).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Articles data list by Search text.
        public List<KBArticle> GetBySearchText(string SearchText)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.KBArticles.Where(p => p.Description.Contains(SearchText) || p.Title.Contains(SearchText)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //Get Articles Details by ArticleId.
        public KBArticle GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.KBArticles.Where(p => p.ArticleId == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Articles Details View by ArticleId.
        public vw_KBArticle GetVwById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.vw_KBArticle.Where(p => p.ArticleId == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Operations

        //Create new KBArticle.
        public void Create(KBArticle Article)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    Article.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    Article.ModifiedOn = DateTime.Now;
                    Article.CreatedBy = HttpContext.Current.User.Identity.Name;
                    Article.CreatedOn = DateTime.Now;

                    ctx.KBArticles.Add(Article);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update Existing KBArticle record.
        public void Update(KBArticle Article)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    Article.ModifiedBy =  HttpContext.Current.User.Identity.Name;
                    Article.ModifiedOn = DateTime.Now;
                    ctx.Entry(Article).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete KBArticle record by Id.
        public void Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    new AttachmentsBL().DeleteByLinkId(id,Constants.ArticleImgUploadPath.ToString());
                    KBArticle Article = ctx.KBArticles.Where(p => p.ArticleId == id).FirstOrDefault();
                    ctx.KBArticles.Remove(Article);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
