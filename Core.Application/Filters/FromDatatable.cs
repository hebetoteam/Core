using Core.Utilities.Dtos.Datatables;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Core.Application.Filters
{
    public class FromDatatable : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var page = bindingContext.ValueProvider.GetValue("pagination[page]");
            var perpage = bindingContext.ValueProvider.GetValue("pagination[perpage]");

            var field = bindingContext.ValueProvider.GetValue("sort[field]");
            var sort = bindingContext.ValueProvider.GetValue("sort[sort]");

            var query = bindingContext.ValueProvider.GetValue("query");

            var result = new KTDataTablePagination();
            if (page.Length > 0)
            {
                result.Page = Int32.Parse((string)page);
            }

            if (perpage.Length > 0)
            {
                result.Perpage = Int32.Parse((string)perpage);
            }

            if (field.Length > 0)
            {
                result.Field = (string)field;
            }

            if (sort.Length > 0)
            {
                result.Sort = (string)sort;
            }

            if (query.Length > 0)
            {
                result.Query = (string)query;
            }

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}
