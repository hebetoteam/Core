using Core.Application.ViewModels.Blog;
using Core.Application.ViewModels.Common;
using Core.Utilities.Dtos;
using System.Collections.Generic;

namespace Core.Application.Interfaces
{
    public interface IBlogService
    {
        PagedResult<BlogViewModel> GetAllPaging(string startDate, string endDate, string keyword, int blogCategoryId, int pageIndex, int pageSize);

        BlogViewModel Add(BlogViewModel blog);

        void Update(BlogViewModel blog);

        void Delete(int id);

        List<BlogViewModel> GetLatestBlogs(int top);

        List<BlogViewModel> GetHomeBlogs();

        BlogViewModel GetById(int id);

        void Save();

        void UpdateViewCount(int id);

        TagViewModel GetTagById(string id);

        List<TagViewModel> GetListTagByBlogId(int id);

        List<TagViewModel> GetListTagByType(string tagType);

        PagedResult<BlogViewModel> GetAllByTagId(string tagId, int pageIndex, int pageSize);

        List<BlogViewModel> GetBlogRelatesById(int id);
    }
}
